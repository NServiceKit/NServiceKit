using System;
using System.Collections.Generic;
using System.IO;
using NServiceKit.Logging;
using NServiceKit.ServiceModel.Serialization;

namespace NServiceKit.CacheAccess.Providers
{
	/// <summary>
	/// Implements a very limited subset of ICacheClient, i.e.
	/// 
	///		- T Get[T]()
	///		- Set(path, value)
	///		- Remove(path)
	/// 
	/// </summary>
	public class FileSystemXmlCacheClient 
		: ICacheClient
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(FileSystemXmlCacheClient));

		private readonly string baseFilePath;

        /// <summary>Initializes a new instance of the NServiceKit.CacheAccess.Providers.FileSystemXmlCacheClient class.</summary>
        ///
        /// <param name="baseFilePath">Full pathname of the base file.</param>
		public FileSystemXmlCacheClient(string baseFilePath)
		{
			this.baseFilePath = baseFilePath;
		}

		private string GetAbsolutePath(string relativePath)
		{
			return Path.Combine(this.baseFilePath, relativePath);
		}

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
		public void Dispose()
		{
		}

        /// <summary>Removes the specified item from the cache.</summary>
        ///
        /// <param name="relativePath">The identifier for the item to delete.</param>
        ///
        /// <returns>true if the item was successfully removed from the cache; false otherwise.</returns>
		public bool Remove(string relativePath)
		{
			try
			{
				File.Delete(GetAbsolutePath(relativePath));
				return true;
			}
			catch
			{
				return false;
			}
		}

        /// <summary>Removes the cache for all the keys provided.</summary>
        ///
        /// <param name="keys">The keys.</param>
		public void RemoveAll(IEnumerable<string> keys)
		{
			foreach (var key in keys)
			{
				try
				{
					this.Remove(key);
				}
				catch (Exception ex)
				{
					Log.Error(string.Format("Error trying to remove {0} from the FileSystem Cache", key), ex);
				}
			}
		}

        /// <summary>Gets.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="relativePath">Full pathname of the relative file.</param>
        ///
        /// <returns>A T.</returns>
		public T Get<T>(string relativePath)
		{
			var absolutePath = GetAbsolutePath(relativePath);

			if (!File.Exists(absolutePath)) return default(T);

			var xml = File.ReadAllText(absolutePath);
			return (T)DataContractDeserializer.Instance.Parse(xml, typeof(T));
		}

        /// <summary>Increments the value of the specified key by the given amount. The operation is atomic and happens on the server. A non existent value at key starts at 0.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <param name="key">   The identifier for the item to increment.</param>
        /// <param name="amount">The amount by which the client wants to increase the item.</param>
        ///
        /// <returns>The new value of the item or -1 if not found.</returns>
		public long Increment(string key, uint amount)
		{
			throw new NotImplementedException();
		}

        /// <summary>Increments the value of the specified key by the given amount. The operation is atomic and happens on the server. A non existent value at key starts at 0.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <param name="key">   The identifier for the item to increment.</param>
        /// <param name="amount">The amount by which the client wants to decrease the item.</param>
        ///
        /// <returns>The new value of the item or -1 if not found.</returns>
		public long Decrement(string key, uint amount)
		{
			throw new NotImplementedException();
		}

        /// <summary>Adds key.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="key">  The key.</param>
        /// <param name="value">The value.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		public bool Add<T>(string key, T value)
		{
			throw new NotImplementedException();
		}

        /// <summary>Sets.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="relativePath">Full pathname of the relative file.</param>
        /// <param name="value">       The value.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		public bool Set<T>(string relativePath, T value)
		{
			var absolutePath = GetAbsolutePath(relativePath);
			
			Directory.CreateDirectory(Path.GetDirectoryName(absolutePath));

			var xml = DataContractSerializer.Instance.Parse(value);
			File.WriteAllText(absolutePath, xml);

			return true;
		}

        /// <summary>Replaces.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="key">  The key.</param>
        /// <param name="value">The value.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		public bool Replace<T>(string key, T value)
		{
			throw new NotImplementedException();
		}

        /// <summary>Adds key.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="key">      The key.</param>
        /// <param name="value">    The value.</param>
        /// <param name="expiresAt">The expires at Date/Time.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		public bool Add<T>(string key, T value, DateTime expiresAt)
		{
			throw new NotImplementedException();
		}

        /// <summary>Sets.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="key">      The key.</param>
        /// <param name="value">    The value.</param>
        /// <param name="expiresAt">The expires at Date/Time.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		public bool Set<T>(string key, T value, DateTime expiresAt)
		{
			throw new NotImplementedException();
		}

        /// <summary>Replaces.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="key">      The key.</param>
        /// <param name="value">    The value.</param>
        /// <param name="expiresAt">The expires at Date/Time.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		public bool Replace<T>(string key, T value, DateTime expiresAt)
		{
			throw new NotImplementedException();
		}

        /// <summary>Adds key.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="key">      The key.</param>
        /// <param name="value">    The value.</param>
        /// <param name="expiresIn">The expires in.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		public bool Add<T>(string key, T value, TimeSpan expiresIn)
		{
			throw new NotImplementedException();
		}

        /// <summary>Sets.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="key">      The key.</param>
        /// <param name="value">    The value.</param>
        /// <param name="expiresIn">The expires in.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		public bool Set<T>(string key, T value, TimeSpan expiresIn)
		{
			throw new NotImplementedException();
		}

        /// <summary>Replaces.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="key">      The key.</param>
        /// <param name="value">    The value.</param>
        /// <param name="expiresIn">The expires in.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		public bool Replace<T>(string key, T value, TimeSpan expiresIn)
		{
			throw new NotImplementedException();
		}

        /// <summary>Invalidates all data on the cache.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
		public void FlushAll()
		{
			throw new NotImplementedException();
		}

        /// <summary>Gets all.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="keys">The keys.</param>
        ///
        /// <returns>all.</returns>
		public IDictionary<string, T> GetAll<T>(IEnumerable<string> keys)
		{
			throw new NotImplementedException();
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