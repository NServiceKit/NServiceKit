using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NServiceKit.Common;

namespace NServiceKit.ServiceInterface.Auth
{
    /// <summary>
    /// Thread-safe In memory UserAuth data store so it can be used without a dependency on Redis.
    /// </summary>
    public class InMemoryAuthRepository : RedisAuthRepository
    {
        /// <summary>The instance.</summary>
        public static readonly InMemoryAuthRepository Instance = new InMemoryAuthRepository();

        /// <summary>Gets or sets the sets.</summary>
        ///
        /// <value>The sets.</value>
        protected Dictionary<string, HashSet<string>> Sets { get; set; }

        /// <summary>Gets or sets the hashes.</summary>
        ///
        /// <value>The hashes.</value>
        protected Dictionary<string, Dictionary<string, string>> Hashes { get; set; }
        internal List<IClearable> TrackedTypes = new List<IClearable>();

        class TypedData<T> : IClearable
        {
            internal static TypedData<T> Instance = new TypedData<T>();

            private TypedData()
            {
                lock (InMemoryAuthRepository.Instance.TrackedTypes) 
                    InMemoryAuthRepository.Instance.TrackedTypes.Add(this);
            }

            internal readonly List<T> Items = new List<T>();
            internal int Sequence = 0;
            
            /// <summary>Clears this object to its blank/initial state.</summary>
            public void Clear()
            {
                lock (Items) Items.Clear();
                Interlocked.CompareExchange(ref Sequence, 0, Sequence);
            }
        }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.Auth.InMemoryAuthRepository class.</summary>
        public InMemoryAuthRepository() : base(new InMemoryManagerFacade(Instance))
        {
            this.Sets = new Dictionary<string, HashSet<string>>();
            this.Hashes = new Dictionary<string, Dictionary<string, string>>();
        }

        class InMemoryManagerFacade : IRedisClientManagerFacade
        {
            private readonly InMemoryAuthRepository root;

            /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.Auth.InMemoryAuthRepository.InMemoryManagerFacade class.</summary>
            ///
            /// <param name="root">The root.</param>
            public InMemoryManagerFacade(InMemoryAuthRepository root)
            {
                this.root = root;
            }

            /// <summary>Gets the client.</summary>
            ///
            /// <returns>The client.</returns>
            public IRedisClientFacade GetClient()
            {
                return new InMemoryClientFacade(root);
            }

            /// <summary>Clears this object to its blank/initial state.</summary>
            public void Clear()
            {
                lock (Instance.Sets) Instance.Sets.Clear();
                lock (Instance.Hashes) Instance.Hashes.Clear();
                lock (Instance.TrackedTypes) Instance.TrackedTypes.ForEach(x => x.Clear());
            }
        }

        class InMemoryClientFacade : IRedisClientFacade
        {
            private readonly InMemoryAuthRepository root;

            /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.Auth.InMemoryAuthRepository.InMemoryClientFacade class.</summary>
            ///
            /// <param name="root">The root.</param>
            public InMemoryClientFacade(InMemoryAuthRepository root)
            {
                this.root = root;
            }

            class InMemoryTypedClientFacade<T> : ITypedRedisClientFacade<T>
            {
                private readonly InMemoryAuthRepository root;

                /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.Auth.InMemoryAuthRepository.InMemoryClientFacade.InMemoryTypedClientFacade&lt;T&gt; class.</summary>
                ///
                /// <param name="root">The root.</param>
                public InMemoryTypedClientFacade(InMemoryAuthRepository root)
                {
                    this.root = root;
                }

                /// <summary>Gets the next sequence.</summary>
                ///
                /// <returns>The next sequence.</returns>
                public int GetNextSequence()
                {
                    return Interlocked.Increment(ref TypedData<T>.Instance.Sequence);
                }

                /// <summary>Gets by identifier.</summary>
                ///
                /// <param name="id">The identifier.</param>
                ///
                /// <returns>The by identifier.</returns>
                public T GetById(object id)
                {
                    if (id == null) return default(T);

                    lock (TypedData<T>.Instance.Items)
                    {
                        return TypedData<T>.Instance.Items.FirstOrDefault(x => id.ToString() == x.ToId().ToString());
                    }
                }

                /// <summary>Gets by identifiers.</summary>
                ///
                /// <param name="ids">The identifiers.</param>
                ///
                /// <returns>The by identifiers.</returns>
                public List<T> GetByIds(IEnumerable ids)
                {
                    var idsSet = new HashSet<object>();
                    foreach (var id in ids) idsSet.Add(id.ToString());

                    lock (TypedData<T>.Instance.Items)
                    {
                        return TypedData<T>.Instance.Items.Where(x => idsSet.Contains(x.ToId().ToString())).ToList();
                    }
                }
            }

            /// <summary>Gets all items from set.</summary>
            ///
            /// <param name="setId">Identifier for the set.</param>
            ///
            /// <returns>all items from set.</returns>
            public HashSet<string> GetAllItemsFromSet(string setId)
            {
                lock (root.Sets)
                {
                    HashSet<string> set;
                    return root.Sets.TryGetValue(setId, out set) ? set : new HashSet<string>();
                }
            }

            /// <summary>Stores the given item.</summary>
            ///
            /// <typeparam name="T">Generic type parameter.</typeparam>
            /// <param name="item">The item.</param>
            public void Store<T>(T item) where T : class , new()
            {
                if (item == null) return;

                lock (TypedData<T>.Instance.Items)
                {
                    for (var i = 0; i < TypedData<T>.Instance.Items.Count; i++)
                    {
                        var o = TypedData<T>.Instance.Items[i];
                        if (o.ToId().ToString() != item.ToId().ToString()) continue;
                        TypedData<T>.Instance.Items[i] = item;
                        return;
                    }
                    TypedData<T>.Instance.Items.Add(item);
                }
            }

            /// <summary>Gets value from hash.</summary>
            ///
            /// <param name="hashId">Identifier for the hash.</param>
            /// <param name="key">   The key.</param>
            ///
            /// <returns>The value from hash.</returns>
            public string GetValueFromHash(string hashId, string key)
            {
                hashId.ThrowIfNull("hashId");
                key.ThrowIfNull("key");

                lock (root.Hashes)
                {
                    Dictionary<string, string> hash;
                    if (!root.Hashes.TryGetValue(hashId, out hash)) return null;

                    string value;
                    hash.TryGetValue(key, out value);
                    return value;
                }
            }

            /// <summary>Sets entry in hash.</summary>
            ///
            /// <param name="hashId">Identifier for the hash.</param>
            /// <param name="key">   The key.</param>
            /// <param name="value"> The value.</param>
            public void SetEntryInHash(string hashId, string key, string value)
            {
                hashId.ThrowIfNull("hashId");
                key.ThrowIfNull("key");

                lock (root.Hashes)
                {
                    Dictionary<string, string> hash;
                    if (!root.Hashes.TryGetValue(hashId, out hash))
                        root.Hashes[hashId] = hash = new Dictionary<string, string>();

                    hash[key] = value;
                }
            }

            /// <summary>Removes the entry from hash.</summary>
            ///
            /// <param name="hashId">Identifier for the hash.</param>
            /// <param name="key">   The key.</param>
            public void RemoveEntryFromHash(string hashId, string key)
            {
                hashId.ThrowIfNull("hashId");
                key.ThrowIfNull("key");

                lock (root.Hashes)
                {
                    Dictionary<string, string> hash;
                    if (!root.Hashes.TryGetValue(hashId, out hash))
                        root.Hashes[hashId] = hash = new Dictionary<string, string>();

                    hash.Remove(key);
                }
            }

            /// <summary>Adds an item to set to 'item'.</summary>
            ///
            /// <param name="setId">Identifier for the set.</param>
            /// <param name="item"> The item.</param>
            public void AddItemToSet(string setId, string item)
            {
                lock (root.Sets)
                {
                    HashSet<string> set;
                    if (!root.Sets.TryGetValue(setId, out set))
                        root.Sets[setId] = set = new HashSet<string>();

                    set.Add(item);
                }
            }

            /// <summary>Gets as.</summary>
            ///
            /// <typeparam name="T">Generic type parameter.</typeparam>
            ///
            /// <returns>An ITypedRedisClientFacade&lt;T&gt;</returns>
            public ITypedRedisClientFacade<T> As<T>()
            {
                return new InMemoryTypedClientFacade<T>(root);
            }

            /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
            public void Dispose()
            {
            }
        }

    }
}