using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NServiceKit.Redis;
using NServiceKit.Redis.Generic;

namespace NServiceKit.ServiceInterface.Auth
{
    /// <summary>Interface for redis client manager facade.</summary>
    public interface IRedisClientManagerFacade : IClearable
    {
        /// <summary>Gets the client.</summary>
        ///
        /// <returns>The client.</returns>
        IRedisClientFacade GetClient();
    }

    /// <summary>Interface for clearable.</summary>
    public interface IClearable
    {
        /// <summary>Clears this object to its blank/initial state.</summary>
        void Clear();		
    }

    /// <summary>Interface for redis client facade.</summary>
    public interface IRedisClientFacade : IDisposable
    {
        /// <summary>Gets all items from set.</summary>
        ///
        /// <param name="setId">Identifier for the set.</param>
        ///
        /// <returns>all items from set.</returns>
        HashSet<string> GetAllItemsFromSet(string setId);

        /// <summary>Stores the given item.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="item">The item.</param>
        void Store<T>(T item) where T : class, new();

        /// <summary>Gets value from hash.</summary>
        ///
        /// <param name="hashId">Identifier for the hash.</param>
        /// <param name="key">   The key.</param>
        ///
        /// <returns>The value from hash.</returns>
        string GetValueFromHash(string hashId, string key);

        /// <summary>Sets entry in hash.</summary>
        ///
        /// <param name="hashId">Identifier for the hash.</param>
        /// <param name="key">   The key.</param>
        /// <param name="value"> The value.</param>
        void SetEntryInHash(string hashId, string key, string value);

        /// <summary>Removes the entry from hash.</summary>
        ///
        /// <param name="hashId">Identifier for the hash.</param>
        /// <param name="key">   The key.</param>
        void RemoveEntryFromHash(string hashId, string key);

        /// <summary>Adds an item to set to 'item'.</summary>
        ///
        /// <param name="setId">Identifier for the set.</param>
        /// <param name="item"> The item.</param>
        void AddItemToSet(string setId, string item);

        /// <summary>Gets as.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        ///
        /// <returns>An ITypedRedisClientFacade&lt;T&gt;</returns>
        ITypedRedisClientFacade<T> As<T>();
    }

    /// <summary>Interface for typed redis client facade.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
    public interface ITypedRedisClientFacade<T>
    {
        /// <summary>Gets the next sequence.</summary>
        ///
        /// <returns>The next sequence.</returns>
        int GetNextSequence();

        /// <summary>Gets by identifier.</summary>
        ///
        /// <param name="id">The identifier.</param>
        ///
        /// <returns>The by identifier.</returns>
        T GetById(object id);

        /// <summary>Gets by identifiers.</summary>
        ///
        /// <param name="ids">The identifiers.</param>
        ///
        /// <returns>The by identifiers.</returns>
        List<T> GetByIds(IEnumerable ids);
    }

    /// <summary>The redis client manager facade.</summary>
    public class RedisClientManagerFacade : IRedisClientManagerFacade
    {
        private readonly IRedisClientsManager redisManager;

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.Auth.RedisClientManagerFacade class.</summary>
        ///
        /// <param name="redisManager">Manager for redis.</param>
        public RedisClientManagerFacade(IRedisClientsManager redisManager)
        {
            this.redisManager = redisManager;
        }

        /// <summary>Gets the client.</summary>
        ///
        /// <returns>The client.</returns>
        public IRedisClientFacade GetClient()
        {
            return new RedisClientFacade(redisManager.GetClient());
        }

        /// <summary>Clears this object to its blank/initial state.</summary>
        public void Clear()
        {
            using (var redis = redisManager.GetClient())
                redis.FlushAll();
        }

        private class RedisClientFacade : IRedisClientFacade
        {
            private readonly IRedisClient redisClient;

            class RedisITypedRedisClientFacade<T> : ITypedRedisClientFacade<T>
            {
                private readonly IRedisTypedClient<T> redisTypedClient;

                /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.Auth.RedisClientManagerFacade.RedisClientFacade.RedisITypedRedisClientFacade&lt;T&gt; class.</summary>
                ///
                /// <param name="redisTypedClient">The redis typed client.</param>
                public RedisITypedRedisClientFacade(IRedisTypedClient<T> redisTypedClient)
                {
                    this.redisTypedClient = redisTypedClient;
                }

                /// <summary>Gets the next sequence.</summary>
                ///
                /// <returns>The next sequence.</returns>
                public int GetNextSequence()
                {
                    return (int) redisTypedClient.GetNextSequence();
                }

                /// <summary>Gets by identifier.</summary>
                ///
                /// <param name="id">The identifier.</param>
                ///
                /// <returns>The by identifier.</returns>
                public T GetById(object id)
                {
                    return redisTypedClient.GetById(id);
                }

                /// <summary>Gets by identifiers.</summary>
                ///
                /// <param name="ids">The identifiers.</param>
                ///
                /// <returns>The by identifiers.</returns>
                public List<T> GetByIds(IEnumerable ids)
                {
                    return redisTypedClient.GetByIds(ids).ToList();
                }
            }

            /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.Auth.RedisClientManagerFacade.RedisClientFacade class.</summary>
            ///
            /// <param name="redisClient">The redis client.</param>
            public RedisClientFacade(IRedisClient redisClient)
            {
                this.redisClient = redisClient;
            }

            /// <summary>Gets all items from set.</summary>
            ///
            /// <param name="setId">Identifier for the set.</param>
            ///
            /// <returns>all items from set.</returns>
            public HashSet<string> GetAllItemsFromSet(string setId)
            {
                return redisClient.GetAllItemsFromSet(setId);
            }

            /// <summary>Stores the given item.</summary>
            ///
            /// <typeparam name="T">Generic type parameter.</typeparam>
            /// <param name="item">The item.</param>
            public void Store<T>(T item) where T : class , new()
            {
                redisClient.Store(item);
            }

            /// <summary>Gets value from hash.</summary>
            ///
            /// <param name="hashId">Identifier for the hash.</param>
            /// <param name="key">   The key.</param>
            ///
            /// <returns>The value from hash.</returns>
            public string GetValueFromHash(string hashId, string key)
            {
                return redisClient.GetValueFromHash(hashId, key);
            }

            /// <summary>Sets entry in hash.</summary>
            ///
            /// <param name="hashId">Identifier for the hash.</param>
            /// <param name="key">   The key.</param>
            /// <param name="value"> The value.</param>
            public void SetEntryInHash(string hashId, string key, string value)
            {
                redisClient.SetEntryInHash(hashId, key, value);
            }

            /// <summary>Removes the entry from hash.</summary>
            ///
            /// <param name="hashId">Identifier for the hash.</param>
            /// <param name="key">   The key.</param>
            public void RemoveEntryFromHash(string hashId, string key)
            {
                redisClient.RemoveEntryFromHash(hashId, key);
            }

            /// <summary>Adds an item to set to 'item'.</summary>
            ///
            /// <param name="setId">Identifier for the set.</param>
            /// <param name="item"> The item.</param>
            public void AddItemToSet(string setId, string item)
            {
                redisClient.AddItemToSet(setId, item);
            }

            /// <summary>Gets as.</summary>
            ///
            /// <typeparam name="T">Generic type parameter.</typeparam>
            ///
            /// <returns>An ITypedRedisClientFacade&lt;T&gt;</returns>
            public ITypedRedisClientFacade<T> As<T>()
            {
                return new RedisITypedRedisClientFacade<T>(redisClient.As<T>());
            }

            /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
            public void Dispose()
            {
                this.redisClient.Dispose();
            }
        }
    }

}