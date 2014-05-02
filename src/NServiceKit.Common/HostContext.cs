using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NServiceKit.ServiceHost;

namespace NServiceKit.Common
{
    /// <summary>A host context.</summary>
    public class HostContext
    {
        /// <summary>The instance.</summary>
        public static readonly HostContext Instance = new HostContext();

        [ThreadStatic] 
		private static IDictionary items; //Thread Specific
        
		/// <summary>
		/// Gets a list of items for this request. 
		/// </summary>
		/// <remarks>This list will be cleared on every request and is specific to the original thread that is handling the request.
		/// If a handler uses additional threads, this data will not be available on those threads.
		/// </remarks>
		public virtual IDictionary Items
        {
            get
            {
                return items ?? (HttpContext.Current != null
                    ? HttpContext.Current.Items
                    : items = new Dictionary<object, object>());
            }
            set { items = value; }
        }

        /// <summary>Gets or create.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="createFn">The create function.</param>
        ///
        /// <returns>The or create.</returns>
        public T GetOrCreate<T>(Func<T> createFn)
        {
            if (Items.Contains(typeof(T).Name))
                return (T)Items[typeof(T).Name];

            return (T) (Items[typeof(T).Name] = createFn());
        }

        /// <summary>Ends a request.</summary>
        public void EndRequest()
        {
            items = null;
        }

        /// <summary>
        /// Track any IDisposable's to dispose of at the end of the request in IAppHost.OnEndRequest()
        /// </summary>
        /// <param name="instance"></param>
        public void TrackDisposable(IDisposable instance)
        {
            if (instance == null) return;
            if (instance is IService) return; //IService's are already disposed right after they've been executed

            DispsableTracker dispsableTracker = null;
            if (!Items.Contains(DispsableTracker.HashId))
                Items[DispsableTracker.HashId] = dispsableTracker = new DispsableTracker();
            if (dispsableTracker == null)
                dispsableTracker = (DispsableTracker) Items[DispsableTracker.HashId];
            dispsableTracker.Add(instance);
        }
    }

    /// <summary>A dispsable tracker.</summary>
    public class DispsableTracker : IDisposable
    {
        /// <summary>Identifier for the hash.</summary>
        public const string HashId = "__disposables";

        List<WeakReference> disposables = new List<WeakReference>();

        /// <summary>Adds instance.</summary>
        ///
        /// <param name="instance">The instance to add.</param>
        public void Add(IDisposable instance)
        {
            disposables.Add(new WeakReference(instance));
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            foreach (var wr in disposables)
            {
                var disposable = (IDisposable)wr.Target;
                if (wr.IsAlive)
                    disposable.Dispose();
            }
        }
    }
}