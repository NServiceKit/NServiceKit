// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NServiceKit.Net30.Collections.Concurrent;

namespace NServiceKit.Html
{
    /// <summary>A scope storage.</summary>
    public static class ScopeStorage
    {
        private static readonly IScopeStorageProvider _defaultStorageProvider = new StaticScopeStorageProvider();
        private static IScopeStorageProvider _stateStorageProvider;

        /// <summary>Gets or sets the current provider.</summary>
        ///
        /// <value>The current provider.</value>
        public static IScopeStorageProvider CurrentProvider
        {
            get { return _stateStorageProvider ?? _defaultStorageProvider; }
            set { _stateStorageProvider = value; }
        }

        /// <summary>Gets the current scope.</summary>
        ///
        /// <value>The current scope.</value>
        public static IDictionary<object, object> CurrentScope
        {
            get { return CurrentProvider.CurrentScope; }
        }

        /// <summary>Gets the global scope.</summary>
        ///
        /// <value>The global scope.</value>
        public static IDictionary<object, object> GlobalScope
        {
            get { return CurrentProvider.GlobalScope; }
        }

        /// <summary>Creates transient scope.</summary>
        ///
        /// <param name="context">The context.</param>
        ///
        /// <returns>The new transient scope.</returns>
        public static IDisposable CreateTransientScope(IDictionary<object, object> context)
        {
            var currentContext = CurrentScope;
            CurrentProvider.CurrentScope = context;
            return new DisposableAction(() => CurrentProvider.CurrentScope = currentContext); // Return an IDisposable that pops the item back off
        }

        /// <summary>Creates transient scope.</summary>
        ///
        /// <returns>The new transient scope.</returns>
        public static IDisposable CreateTransientScope()
        {
            return CreateTransientScope(new ScopeStorageDictionary(baseScope: CurrentScope));
        }
    }

    /// <summary>A static scope storage provider.</summary>
    public class StaticScopeStorageProvider : IScopeStorageProvider
    {
        private static readonly IDictionary<object, object> _defaultContext =
            new ScopeStorageDictionary(null, new ConcurrentDictionary<object, object>(ScopeStorageComparer.Instance));

        private IDictionary<object, object> _currentContext;

        /// <summary>Gets or sets the current scope.</summary>
        ///
        /// <value>The current scope.</value>
        public IDictionary<object, object> CurrentScope
        {
            get { return _currentContext ?? _defaultContext; }
            set { _currentContext = value; }
        }

        /// <summary>Gets the global scope.</summary>
        ///
        /// <value>The global scope.</value>
        public IDictionary<object, object> GlobalScope
        {
            get { return _defaultContext; }
        }
    }

    /// <summary>Interface for scope storage provider.</summary>
    public interface IScopeStorageProvider
    {
        /// <summary>Gets or sets the current scope.</summary>
        ///
        /// <value>The current scope.</value>
        IDictionary<object, object> CurrentScope { get; set; }

        /// <summary>Gets the global scope.</summary>
        ///
        /// <value>The global scope.</value>
        IDictionary<object, object> GlobalScope { get; }
    }

    /// <summary>Dictionary of scope storages.</summary>
    public class ScopeStorageDictionary : IDictionary<object, object>
    {
        private static readonly StateStorageKeyValueComparer _keyValueComparer = new StateStorageKeyValueComparer();
        private readonly IDictionary<object, object> _baseScope;
        private readonly IDictionary<object, object> _backingStore;

        /// <summary>Initializes a new instance of the NServiceKit.Html.ScopeStorageDictionary class.</summary>
        public ScopeStorageDictionary()
            : this(baseScope: null)
        {
        }

        /// <summary>Initializes a new instance of the NServiceKit.Html.ScopeStorageDictionary class.</summary>
        ///
        /// <param name="baseScope">The base scope.</param>
        public ScopeStorageDictionary(IDictionary<object, object> baseScope)
            : this(baseScope: baseScope, backingStore: new Dictionary<object, object>(ScopeStorageComparer.Instance))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScopeStorageDictionary"/> class.
        /// </summary>
        /// <param name="baseScope">The base scope.</param>
        /// <param name="backingStore">
        /// The dictionary to use as a storage. Since the dictionary would be used as-is, we expect the implementer to 
        /// use the same key-value comparison logic as we do here.
        /// </param>
        internal ScopeStorageDictionary(IDictionary<object, object> baseScope, IDictionary<object, object> backingStore)
        {
            _baseScope = baseScope;
            _backingStore = backingStore;
        }

        /// <summary>Gets the backing store.</summary>
        ///
        /// <value>The backing store.</value>
        protected IDictionary<object, object> BackingStore
        {
            get { return _backingStore; }
        }

        /// <summary>Gets the base scope.</summary>
        ///
        /// <value>The base scope.</value>
        protected IDictionary<object, object> BaseScope
        {
            get { return _baseScope; }
        }

        /// <summary>Gets the keys.</summary>
        ///
        /// <value>The keys.</value>
        public virtual ICollection<object> Keys
        {
            get { return GetItems().Select(item => item.Key).ToList(); }
        }

        /// <summary>Gets the values.</summary>
        ///
        /// <value>The values.</value>
        public virtual ICollection<object> Values
        {
            get { return GetItems().Select(item => item.Value).ToList(); }
        }

        /// <summary>Gets the number of. </summary>
        ///
        /// <value>The count.</value>
        public virtual int Count
        {
            get { return GetItems().Count(); }
        }

        /// <summary>Gets a value indicating whether this object is read only.</summary>
        ///
        /// <value>true if this object is read only, false if not.</value>
        public virtual bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>Indexer to get or set items within this collection using array index syntax.</summary>
        ///
        /// <param name="key">The key.</param>
        ///
        /// <returns>The indexed item.</returns>
        public object this[object key]
        {
            get
            {
                object value;
                TryGetValue(key, out value);
                return value;
            }
            set { SetValue(key, value); }
        }

        /// <summary>Sets a value.</summary>
        ///
        /// <param name="key">  The key.</param>
        /// <param name="value">The value.</param>
        public virtual void SetValue(object key, object value)
        {
            _backingStore[key] = value;
        }

        /// <summary>Attempts to get value from the given data.</summary>
        ///
        /// <param name="key">  The key.</param>
        /// <param name="value">The value.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        public virtual bool TryGetValue(object key, out object value)
        {
            return _backingStore.TryGetValue(key, out value) || (_baseScope != null && _baseScope.TryGetValue(key, out value));
        }

        /// <summary>Removes the given key.</summary>
        ///
        /// <param name="key">The key to remove.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        public virtual bool Remove(object key)
        {
            return _backingStore.Remove(key);
        }

        /// <summary>Gets the enumerator.</summary>
        ///
        /// <returns>The enumerator.</returns>
        public virtual IEnumerator<KeyValuePair<object, object>> GetEnumerator()
        {
            return GetItems().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>Adds key.</summary>
        ///
        /// <param name="key">  The key.</param>
        /// <param name="value">The value.</param>
        public virtual void Add(object key, object value)
        {
            SetValue(key, value);
        }

        /// <summary>Query if 'key' contains key.</summary>
        ///
        /// <param name="key">The key.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        public virtual bool ContainsKey(object key)
        {
            return _backingStore.ContainsKey(key) || (_baseScope != null && _baseScope.ContainsKey(key));
        }

        /// <summary>Adds item.</summary>
        ///
        /// <param name="item">The item to remove.</param>
        public virtual void Add(KeyValuePair<object, object> item)
        {
            SetValue(item.Key, item.Value);
        }

        /// <summary>Clears this object to its blank/initial state.</summary>
        public virtual void Clear()
        {
            _backingStore.Clear();
        }

        /// <summary>Query if this object contains the given item.</summary>
        ///
        /// <param name="item">The item to remove.</param>
        ///
        /// <returns>true if the object is in this collection, false if not.</returns>
        public virtual bool Contains(KeyValuePair<object, object> item)
        {
            return _backingStore.Contains(item) || (_baseScope != null && _baseScope.Contains(item));
        }

        /// <summary>Copies to.</summary>
        ///
        /// <param name="array">     The array.</param>
        /// <param name="arrayIndex">Zero-based index of the array.</param>
        public virtual void CopyTo(KeyValuePair<object, object>[] array, int arrayIndex)
        {
            GetItems().ToList().CopyTo(array, arrayIndex);
        }

        /// <summary>Removes the given item.</summary>
        ///
        /// <param name="item">The item to remove.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        public virtual bool Remove(KeyValuePair<object, object> item)
        {
            return _backingStore.Remove(item);
        }

        /// <summary>Gets the items in this collection.</summary>
        ///
        /// <returns>An enumerator that allows foreach to be used to process the items in this collection.</returns>
        protected virtual IEnumerable<KeyValuePair<object, object>> GetItems()
        {
            if (_baseScope == null) {
                return _backingStore;
            }
            return Enumerable.Concat(_backingStore, _baseScope).Distinct(_keyValueComparer);
        }

        private class StateStorageKeyValueComparer : IEqualityComparer<KeyValuePair<object, object>>
        {
            private IEqualityComparer<object> _stateStorageComparer = ScopeStorageComparer.Instance;

            /// <summary>Tests if two KeyValuePair&lt;object,object&gt; objects are considered equal.</summary>
            ///
            /// <param name="x">Key value pair&lt;object,object&gt; to be compared.</param>
            /// <param name="y">Key value pair&lt;object,object&gt; to be compared.</param>
            ///
            /// <returns>true if the objects are considered equal, false if they are not.</returns>
            public bool Equals(KeyValuePair<object, object> x, KeyValuePair<object, object> y)
            {
                return _stateStorageComparer.Equals(x.Key, y.Key);
            }

            /// <summary>Returns a hash code for this object.</summary>
            ///
            /// <param name="obj">The object.</param>
            ///
            /// <returns>A hash code for this object.</returns>
            public int GetHashCode(KeyValuePair<object, object> obj)
            {
                return _stateStorageComparer.GetHashCode(obj.Key);
            }
        }
    }

    /// <summary>
    /// Custom comparer for the context dictionaries
    /// The comparer treats strings as a special case, performing case insesitive comparison. 
    /// This guaratees that we remain consistent throughout the chain of contexts since PageData dictionary 
    /// behaves in this manner.
    /// </summary>
    internal class ScopeStorageComparer : IEqualityComparer<object>
    {
        private static IEqualityComparer<object> _instance;
        private readonly IEqualityComparer<object> _defaultComparer = EqualityComparer<object>.Default;
        private readonly IEqualityComparer<string> _stringComparer = StringComparer.OrdinalIgnoreCase;

        private ScopeStorageComparer()
        {
        }

        /// <summary>Gets the instance.</summary>
        ///
        /// <value>The instance.</value>
        public static IEqualityComparer<object> Instance
        {
            get
            {
                if (_instance == null) {
                    _instance = new ScopeStorageComparer();
                }
                return _instance;
            }
        }

        /// <summary>Tests if two object objects are considered equal.</summary>
        ///
        /// <param name="x">Object to be compared.</param>
        /// <param name="y">Object to be compared.</param>
        ///
        /// <returns>true if the objects are considered equal, false if they are not.</returns>
        public new bool Equals(object x, object y)
        {
            string xString = x as string;
            string yString = y as string;

            if ((xString != null) && (yString != null)) {
                return _stringComparer.Equals(xString, yString);
            }

            return _defaultComparer.Equals(x, y);
        }

        /// <summary>Returns a hash code for this object.</summary>
        ///
        /// <param name="obj">The object.</param>
        ///
        /// <returns>A hash code for this object.</returns>
        public int GetHashCode(object obj)
        {
            string objString = obj as string;
            if (objString != null) {
                return _stringComparer.GetHashCode(objString);
            }

            return _defaultComparer.GetHashCode(obj);
        }
    }

    internal class DisposableAction : IDisposable
    {
        private Action _action;
        private bool _hasDisposed;

        /// <summary>Initializes a new instance of the NServiceKit.Html.DisposableAction class.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="action">The action.</param>
        public DisposableAction(Action action)
        {
            if (action == null) {
                throw new ArgumentNullException("action");
            }
            _action = action;
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Releases the unmanaged resources used by the NServiceKit.Html.DisposableAction and optionally releases the managed resources.</summary>
        ///
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            // If we were disposed by the finalizer it's because the user didn't use a "using" block, so don't do anything!
            if (disposing) {
                lock (this) {
                    if (!_hasDisposed) {
                        _hasDisposed = true;
                        _action();
                    }
                }
            }
        }
    }
}
