// ConcurrentDictionary.cs
//
// Copyright (c) 2009 Jérémie "Garuma" Laval
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
//

#if !NET_4_0 && !NETFX_CORE

using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace NServiceKit.Net30.Collections.Concurrent
{
    /// <summary>Dictionary of concurrents.</summary>
    /// <typeparam name="TKey">  Type of the key.</typeparam>
    /// <typeparam name="TValue">Type of the value.</typeparam>
    public class ConcurrentDictionary<TKey, TValue> : IDictionary<TKey, TValue>,
      ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>,
      IDictionary, ICollection, IEnumerable
    {
        IEqualityComparer<TKey> comparer;

        SplitOrderedList<TKey, KeyValuePair<TKey, TValue>> internalDictionary;

        /// <summary>Initializes a new instance of the NServiceKit.Net30.Collections.Concurrent.ConcurrentDictionary&lt;TKey, TValue&gt; class.</summary>
        public ConcurrentDictionary () : this (EqualityComparer<TKey>.Default)
        {
        }

        /// <summary>Initializes a new instance of the NServiceKit.Net30.Collections.Concurrent.ConcurrentDictionary&lt;TKey, TValue&gt; class.</summary>
        ///
        /// <param name="collection">The collection.</param>
        public ConcurrentDictionary (IEnumerable<KeyValuePair<TKey, TValue>> collection)
            : this (collection, EqualityComparer<TKey>.Default)
        {
        }

        /// <summary>Initializes a new instance of the NServiceKit.Net30.Collections.Concurrent.ConcurrentDictionary&lt;TKey, TValue&gt; class.</summary>
        ///
        /// <param name="comparer">The comparer.</param>
        public ConcurrentDictionary (IEqualityComparer<TKey> comparer)
        {
            this.comparer = comparer;
            this.internalDictionary = new SplitOrderedList<TKey, KeyValuePair<TKey, TValue>> (comparer);
        }

        /// <summary>Initializes a new instance of the NServiceKit.Net30.Collections.Concurrent.ConcurrentDictionary&lt;TKey, TValue&gt; class.</summary>
        ///
        /// <param name="collection">The collection.</param>
        /// <param name="comparer">  The comparer.</param>
        public ConcurrentDictionary (IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer)
            : this (comparer)
        {
            foreach (KeyValuePair<TKey, TValue> pair in collection)
                Add (pair.Key, pair.Value);
        }

        /// <summary>Initializes a new instance of the NServiceKit.Net30.Collections.Concurrent.ConcurrentDictionary&lt;TKey, TValue&gt; class.</summary>
        ///
        /// <param name="concurrencyLevel">The concurrency level.</param>
        /// <param name="capacity">        The capacity.</param>
        public ConcurrentDictionary (int concurrencyLevel, int capacity)
            : this (EqualityComparer<TKey>.Default)
        {

        }

        /// <summary>Initializes a new instance of the NServiceKit.Net30.Collections.Concurrent.ConcurrentDictionary&lt;TKey, TValue&gt; class.</summary>
        ///
        /// <param name="concurrencyLevel">The concurrency level.</param>
        /// <param name="collection">      The collection.</param>
        /// <param name="comparer">        The comparer.</param>
        public ConcurrentDictionary (int concurrencyLevel,
                                     IEnumerable<KeyValuePair<TKey, TValue>> collection,
                                     IEqualityComparer<TKey> comparer)
            : this (collection, comparer)
        {

        }

        /// <summary>Initializes a new instance of the NServiceKit.Net30.Collections.Concurrent.ConcurrentDictionary&lt;TKey, TValue&gt; class.</summary>
        ///
        /// <param name="concurrencyLevel">The concurrency level.</param>
        /// <param name="capacity">        The capacity.</param>
        /// <param name="comparer">        The comparer.</param>
        public ConcurrentDictionary (int concurrencyLevel, int capacity, IEqualityComparer<TKey> comparer)
            : this (comparer)
        {

        }

        void Add (TKey key, TValue value)
        {
            while (!TryAdd (key, value));
        }

        void IDictionary<TKey, TValue>.Add (TKey key, TValue value)
        {
            Add (key, value);
        }

        /// <summary>Attempts to add from the given data.</summary>
        ///
        /// <param name="key">  The key.</param>
        /// <param name="value">The value.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        public bool TryAdd (TKey key, TValue value)
        {
            return internalDictionary.Insert (Hash (key), key, Make (key, value));
        }

        void ICollection<KeyValuePair<TKey,TValue>>.Add (KeyValuePair<TKey, TValue> pair)
        {
            Add (pair.Key, pair.Value);
        }

        /// <summary>Adds an or update.</summary>
        ///
        /// <param name="key">               The key.</param>
        /// <param name="addValueFactory">   The add value factory.</param>
        /// <param name="updateValueFactory">The update value factory.</param>
        ///
        /// <returns>A TValue.</returns>
        public TValue AddOrUpdate (TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory)
        {
            return internalDictionary.InsertOrUpdate (Hash (key),
                                                      key,
                                                      () => Make (key, addValueFactory (key)),
                                                      (e) => Make (key, updateValueFactory (key, e.Value))).Value;
        }

        /// <summary>Adds an or update.</summary>
        ///
        /// <param name="key">               The key.</param>
        /// <param name="addValue">          The add value.</param>
        /// <param name="updateValueFactory">The update value factory.</param>
        ///
        /// <returns>A TValue.</returns>
        public TValue AddOrUpdate (TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory)
        {
            return AddOrUpdate (key, (_) => addValue, updateValueFactory);
        }

        TValue AddOrUpdate (TKey key, TValue addValue, TValue updateValue)
        {
            return internalDictionary.InsertOrUpdate (Hash (key),
                                                      key,
                                                      Make (key, addValue),
                                                      Make (key, updateValue)).Value;
        }

        TValue GetValue (TKey key)
        {
            TValue temp;
            if (!TryGetValue (key, out temp))
                throw new KeyNotFoundException (key.ToString ());
            return temp;
        }

        /// <summary>Attempts to get value from the given data.</summary>
        ///
        /// <param name="key">  The key.</param>
        /// <param name="value">The value.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        public bool TryGetValue (TKey key, out TValue value)
        {
            KeyValuePair<TKey, TValue> pair;
            bool result = internalDictionary.Find (Hash (key), key, out pair);
            value = pair.Value;

            return result;
        }

        /// <summary>Attempts to update from the given data.</summary>
        ///
        /// <param name="key">            The key.</param>
        /// <param name="newValue">       The new value.</param>
        /// <param name="comparisonValue">The comparison value.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        public bool TryUpdate (TKey key, TValue newValue, TValue comparisonValue)
        {
            return internalDictionary.CompareExchange (Hash (key), key, Make (key, newValue), (e) => e.Value.Equals (comparisonValue));
        }

        /// <summary>Indexer to get or set items within this collection using array index syntax.</summary>
        ///
        /// <param name="key">The key.</param>
        ///
        /// <returns>The indexed item.</returns>
        public TValue this[TKey key] {
            get {
                return GetValue (key);
            }
            set {
                AddOrUpdate (key, value, value);
            }
        }

        /// <summary>Gets or add.</summary>
        ///
        /// <param name="key">         The key.</param>
        /// <param name="valueFactory">The value factory.</param>
        ///
        /// <returns>The or add.</returns>
        public TValue GetOrAdd (TKey key, Func<TKey, TValue> valueFactory)
        {
            return internalDictionary.InsertOrGet (Hash (key), key, Make (key, default(TValue)), () => Make (key, valueFactory (key))).Value;
        }

        /// <summary>Gets or add.</summary>
        ///
        /// <param name="key">  The key.</param>
        /// <param name="value">The value.</param>
        ///
        /// <returns>The or add.</returns>
        public TValue GetOrAdd (TKey key, TValue value)
        {
            return internalDictionary.InsertOrGet (Hash (key), key, Make (key, value), null).Value;
        }

        /// <summary>Attempts to remove from the given data.</summary>
        ///
        /// <param name="key">  The key.</param>
        /// <param name="value">The value.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        public bool TryRemove (TKey key, out TValue value)
        {
            KeyValuePair<TKey, TValue> data;
            bool result = internalDictionary.Delete (Hash (key), key, out data);
            value = data.Value;
            return result;
        }

        bool Remove (TKey key)
        {
            TValue dummy;

            return TryRemove (key, out dummy);
        }

        bool IDictionary<TKey, TValue>.Remove (TKey key)
        {
            return Remove (key);
        }

        bool ICollection<KeyValuePair<TKey,TValue>>.Remove (KeyValuePair<TKey,TValue> pair)
        {
            return Remove (pair.Key);
        }

        /// <summary>Query if 'key' contains key.</summary>
        ///
        /// <param name="key">The key.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        public bool ContainsKey (TKey key)
        {
            KeyValuePair<TKey, TValue> dummy;
            return internalDictionary.Find (Hash (key), key, out dummy);
        }

        bool IDictionary.Contains (object key)
        {
            if (!(key is TKey))
                return false;

            return ContainsKey ((TKey)key);
        }

        void IDictionary.Remove (object key)
        {
            if (!(key is TKey))
                return;

            Remove ((TKey)key);
        }

        object IDictionary.this [object key]
        {
            get {
                if (!(key is TKey))
                    throw new ArgumentException ("key isn't of correct type", "key");

                return this[(TKey)key];
            }
            set {
                if (!(key is TKey) || !(value is TValue))
                    throw new ArgumentException ("key or value aren't of correct type");

                this[(TKey)key] = (TValue)value;
            }
        }

        void IDictionary.Add (object key, object value)
        {
            if (!(key is TKey) || !(value is TValue))
                throw new ArgumentException ("key or value aren't of correct type");

            Add ((TKey)key, (TValue)value);
        }

        bool ICollection<KeyValuePair<TKey,TValue>>.Contains (KeyValuePair<TKey, TValue> pair)
        {
            return ContainsKey (pair.Key);
        }

        /// <summary>Convert this object into an array representation.</summary>
        ///
        /// <returns>An array that represents the data in this object.</returns>
        public KeyValuePair<TKey,TValue>[] ToArray ()
        {
            // This is most certainly not optimum but there is
            // not a lot of possibilities

            return new List<KeyValuePair<TKey,TValue>> (this).ToArray ();
        }

        /// <summary>Clears this object to its blank/initial state.</summary>
        public void Clear()
        {
            // Pronk
            internalDictionary = new SplitOrderedList<TKey, KeyValuePair<TKey, TValue>> (comparer);
        }

        /// <summary>Gets the number of elements contained in the <see cref="T:System.Collections.ICollection" />.</summary>
        ///
        /// <value>The number of elements contained in the <see cref="T:System.Collections.ICollection" />.</value>
        public int Count {
            get {
                return internalDictionary.Count;
            }
        }

        /// <summary>Gets a value indicating whether this object is empty.</summary>
        ///
        /// <value>true if this object is empty, false if not.</value>
        public bool IsEmpty {
            get {
                return Count == 0;
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly {
            get {
                return false;
            }
        }

        bool IDictionary.IsReadOnly {
            get {
                return false;
            }
        }

        /// <summary>Gets an <see cref="T:System.Collections.ICollection" /> object containing the keys of the <see cref="T:System.Collections.IDictionary" /> object.</summary>
        ///
        /// <value>An <see cref="T:System.Collections.ICollection" /> object containing the keys of the <see cref="T:System.Collections.IDictionary" /> object.</value>
        public ICollection<TKey> Keys {
            get {
                return GetPart<TKey> ((kvp) => kvp.Key);
            }
        }

        /// <summary>Gets an <see cref="T:System.Collections.ICollection" /> object containing the values in the <see cref="T:System.Collections.IDictionary" /> object.</summary>
        ///
        /// <value>An <see cref="T:System.Collections.ICollection" /> object containing the values in the <see cref="T:System.Collections.IDictionary" /> object.</value>
        public ICollection<TValue> Values {
            get {
                return GetPart<TValue> ((kvp) => kvp.Value);
            }
        }

        ICollection IDictionary.Keys {
            get {
                return (ICollection)Keys;
            }
        }

        ICollection IDictionary.Values {
            get {
                return (ICollection)Values;
            }
        }

        ICollection<T> GetPart<T> (Func<KeyValuePair<TKey, TValue>, T> extractor)
        {
            List<T> temp = new List<T> ();

            foreach (KeyValuePair<TKey, TValue> kvp in this)
                temp.Add (extractor (kvp));

            return temp.AsReadOnly ();
        }

        void ICollection.CopyTo (Array array, int startIndex)
        {
            KeyValuePair<TKey, TValue>[] arr = array as KeyValuePair<TKey, TValue>[];
            if (arr == null)
                return;

            CopyTo (arr, startIndex, Count);
        }

        void CopyTo (KeyValuePair<TKey, TValue>[] array, int startIndex)
        {
            CopyTo (array, startIndex, Count);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo (KeyValuePair<TKey, TValue>[] array, int startIndex)
        {
            CopyTo (array, startIndex);
        }

        void CopyTo (KeyValuePair<TKey, TValue>[] array, int startIndex, int num)
        {
            foreach (var kvp in this) {
                array [startIndex++] = kvp;

                if (--num <= 0)
                    return;
            }
        }

        /// <summary>Gets the enumerator.</summary>
        ///
        /// <returns>The enumerator.</returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator ()
        {
            return GetEnumeratorInternal ();
        }

        IEnumerator IEnumerable.GetEnumerator ()
        {
            return (IEnumerator)GetEnumeratorInternal ();
        }

        IEnumerator<KeyValuePair<TKey, TValue>> GetEnumeratorInternal ()
        {
            return internalDictionary.GetEnumerator ();
        }

        IDictionaryEnumerator IDictionary.GetEnumerator ()
        {
            return new ConcurrentDictionaryEnumerator (GetEnumeratorInternal ());
        }

        class ConcurrentDictionaryEnumerator : IDictionaryEnumerator
        {
            IEnumerator<KeyValuePair<TKey, TValue>> internalEnum;

            /// <summary>Initializes a new instance of the NServiceKit.Net30.Collections.Concurrent.ConcurrentDictionary&lt;TKey, TValue&gt;.ConcurrentDictionaryEnumerator class.</summary>
            ///
            /// <param name="internalEnum">The internal enum.</param>
            public ConcurrentDictionaryEnumerator (IEnumerator<KeyValuePair<TKey, TValue>> internalEnum)
            {
                this.internalEnum = internalEnum;
            }

            /// <summary>Determines if we can move next.</summary>
            ///
            /// <returns>true if it succeeds, false if it fails.</returns>
            public bool MoveNext ()
            {
                return internalEnum.MoveNext ();
            }

            /// <summary>Resets this object.</summary>
            public void Reset ()
            {
                internalEnum.Reset ();
            }

            /// <summary>Gets the current.</summary>
            ///
            /// <value>The current.</value>
            public object Current {
                get {
                    return Entry;
                }
            }

            /// <summary>Gets both the key and the value of the current dictionary entry.</summary>
            ///
            /// <value>A <see cref="T:System.Collections.DictionaryEntry" /> containing both the key and the value of the current dictionary entry.</value>
            ///
            /// ### <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Collections.IDictionaryEnumerator" /> is positioned before the first entry of the dictionary or after the last
            /// entry.
            /// </exception>
            public DictionaryEntry Entry {
                get {
                    KeyValuePair<TKey, TValue> current = internalEnum.Current;
                    return new DictionaryEntry (current.Key, current.Value);
                }
            }

            /// <summary>Gets the key of the current dictionary entry.</summary>
            ///
            /// <value>The key of the current element of the enumeration.</value>
            ///
            /// ### <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Collections.IDictionaryEnumerator" /> is positioned before the first entry of the dictionary or after the last
            /// entry.
            /// </exception>
            public object Key {
                get {
                    return internalEnum.Current.Key;
                }
            }

            /// <summary>Gets the value of the current dictionary entry.</summary>
            ///
            /// <value>The value of the current element of the enumeration.</value>
            ///
            /// ### <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Collections.IDictionaryEnumerator" /> is positioned before the first entry of the dictionary or after the last
            /// entry.
            /// </exception>
            public object Value {
                get {
                    return internalEnum.Current.Value;
                }
            }
        }

        object ICollection.SyncRoot {
            get {
                return this;
            }
        }

        bool IDictionary.IsFixedSize {
            get {
                return false;
            }
        }

        bool ICollection.IsSynchronized {
            get { return true; }
        }

        static KeyValuePair<U, V> Make<U, V> (U key, V value)
        {
            return new KeyValuePair<U, V> (key, value);
        }

        uint Hash (TKey key)
        {
            return (uint)comparer.GetHashCode (key);
        }
    }
}

#endif