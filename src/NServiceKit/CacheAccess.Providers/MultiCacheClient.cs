using System;
using System.Collections.Generic;
using NServiceKit.Common;

namespace NServiceKit.CacheAccess.Providers
{
    /// <summary>A multi cache client.</summary>
	public class MultiCacheClient 
		: ICacheClient
	{
		private readonly List<ICacheClient> cacheClients;

        /// <summary>Initializes a new instance of the NServiceKit.CacheAccess.Providers.MultiCacheClient class.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="cacheClients">A variable-length parameters list containing cache clients.</param>
		public MultiCacheClient(params ICacheClient[] cacheClients)
		{
			if (cacheClients.Length == 0)
			{
				throw new ArgumentNullException("cacheClients");
			}
			this.cacheClients = new List<ICacheClient>(cacheClients);
		}

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
		public void Dispose()
		{
			cacheClients.ExecAll(client => client.Dispose());
		}

        /// <summary>Removes the specified item from the cache.</summary>
        ///
        /// <param name="key">The identifier for the item to delete.</param>
        ///
        /// <returns>true if the item was successfully removed from the cache; false otherwise.</returns>
		public bool Remove(string key)
		{
			var firstResult = default(bool);
			cacheClients.ExecAllWithFirstOut(client => client.Remove(key), ref firstResult);
			return firstResult;
		}

        /// <summary>Gets.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="key">The identifier for the item to increment.</param>
        ///
        /// <returns>A T.</returns>
		public T Get<T>(string key)
		{
			return cacheClients.ExecReturnFirstWithResult(client => client.Get<T>(key));
		}

        /// <summary>Increments the value of the specified key by the given amount. The operation is atomic and happens on the server. A non existent value at key starts at 0.</summary>
        ///
        /// <param name="key">   The identifier for the item to increment.</param>
        /// <param name="amount">The amount by which the client wants to increase the item.</param>
        ///
        /// <returns>The new value of the item or -1 if not found.</returns>
		public long Increment(string key, uint amount)
		{
			var firstResult = default(long);
			cacheClients.ExecAllWithFirstOut(client => client.Increment(key, amount), ref firstResult);
			return firstResult;
		}

        /// <summary>Increments the value of the specified key by the given amount. The operation is atomic and happens on the server. A non existent value at key starts at 0.</summary>
        ///
        /// <param name="key">   The identifier for the item to increment.</param>
        /// <param name="amount">The amount by which the client wants to decrease the item.</param>
        ///
        /// <returns>The new value of the item or -1 if not found.</returns>
		public long Decrement(string key, uint amount)
		{
			var firstResult = default(long);
			cacheClients.ExecAllWithFirstOut(client => client.Decrement(key, amount), ref firstResult);
			return firstResult;
		}

        /// <summary>Adds key.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="key">  The key.</param>
        /// <param name="value">The value.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		public bool Add<T>(string key, T value)
		{
			var firstResult = default(bool);
			cacheClients.ExecAllWithFirstOut(client => client.Add(key, value), ref firstResult);
			return firstResult;
		}

        /// <summary>Sets.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="key">  The key.</param>
        /// <param name="value">The value.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		public bool Set<T>(string key, T value)
		{
			var firstResult = default(bool);
			cacheClients.ExecAllWithFirstOut(client => client.Set(key, value), ref firstResult);
			return firstResult;
		}

        /// <summary>Replaces.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="key">  The key.</param>
        /// <param name="value">The value.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		public bool Replace<T>(string key, T value)
		{
			var firstResult = default(bool);
			cacheClients.ExecAllWithFirstOut(client => client.Replace(key, value), ref firstResult);
			return firstResult;
		}

        /// <summary>Adds key.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="key">      The key.</param>
        /// <param name="value">    The value.</param>
        /// <param name="expiresAt">The expires at Date/Time.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		public bool Add<T>(string key, T value, DateTime expiresAt)
		{
			var firstResult = default(bool);
			cacheClients.ExecAllWithFirstOut(client => client.Add(key, value, expiresAt), ref firstResult);
			return firstResult;
		}

        /// <summary>Sets.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="key">      The key.</param>
        /// <param name="value">    The value.</param>
        /// <param name="expiresAt">The expires at Date/Time.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		public bool Set<T>(string key, T value, DateTime expiresAt)
		{
			var firstResult = default(bool);
			cacheClients.ExecAllWithFirstOut(client => client.Set(key, value, expiresAt), ref firstResult);
			return firstResult;
		}

        /// <summary>Replaces.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="key">      The key.</param>
        /// <param name="value">    The value.</param>
        /// <param name="expiresAt">The expires at Date/Time.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		public bool Replace<T>(string key, T value, DateTime expiresAt)
		{
			var firstResult = default(bool);
			cacheClients.ExecAllWithFirstOut(client => client.Replace(key, value, expiresAt), ref firstResult);
			return firstResult;
		}

        /// <summary>Adds key.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="key">      The key.</param>
        /// <param name="value">    The value.</param>
        /// <param name="expiresIn">The expires in.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		public bool Add<T>(string key, T value, TimeSpan expiresIn)
		{
			var firstResult = default(bool);
			cacheClients.ExecAllWithFirstOut(client => client.Add(key, value, expiresIn), ref firstResult);
			return firstResult;
		}

        /// <summary>Sets.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="key">      The key.</param>
        /// <param name="value">    The value.</param>
        /// <param name="expiresIn">The expires in.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		public bool Set<T>(string key, T value, TimeSpan expiresIn)
		{
			var firstResult = default(bool);
			cacheClients.ExecAllWithFirstOut(client => client.Set(key, value, expiresIn), ref firstResult);
			return firstResult;
		}

        /// <summary>Replaces.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="key">      The key.</param>
        /// <param name="value">    The value.</param>
        /// <param name="expiresIn">The expires in.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		public bool Replace<T>(string key, T value, TimeSpan expiresIn)
		{
			var firstResult = default(bool);
			cacheClients.ExecAllWithFirstOut(client => client.Replace(key, value, expiresIn), ref firstResult);
			return firstResult;
		}

        /// <summary>Invalidates all data on the cache.</summary>
		public void FlushAll()
		{
			cacheClients.ExecAll(client => client.FlushAll());
		}

        /// <summary>Gets all.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="keys">The keys.</param>
        ///
        /// <returns>all.</returns>
		public IDictionary<string, T> GetAll<T>(IEnumerable<string> keys)
		{
			foreach (var client in cacheClients)
			{
				try
				{
					var result = client.GetAll<T>(keys);
					if (result != null)
					{
						return result;
					}
				}
				catch (Exception ex)
				{
					ExecExtensions.LogError(client.GetType(), "Get", ex);
				}
			}

			return new Dictionary<string, T>();
		}

        /// <summary>Removes the cache for all the keys provided.</summary>
        ///
        /// <param name="keys">The keys.</param>
		public void RemoveAll(IEnumerable<string> keys)
		{
			foreach (var key in keys)
			{
				this.Remove(key);
			}
		}

        /// <summary>Sets all.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="values">The values.</param>
		public void SetAll<T>(IDictionary<string, T> values)
		{
			foreach (var entry in values)
			{
				Set(entry.Key, entry.Value);
			}
		}
	}

}