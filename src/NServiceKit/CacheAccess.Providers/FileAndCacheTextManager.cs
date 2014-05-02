using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NServiceKit.Common;
using NServiceKit.Common.Extensions;
using NServiceKit.Common.Web;

namespace NServiceKit.CacheAccess.Providers
{
	/// <summary>
	/// Stores both 'compressed' and 'text' caches of the dto in the FileSystem and ICacheTextManager provided.
	/// The ContentType is inferred from the ICacheTextManager's ContentType.
	/// </summary>
	public class FileAndCacheTextManager
		: ICompressableCacheTextManager
	{
		private readonly string baseCachePath;
		private readonly ICacheTextManager cacheManager;

        /// <summary>Initializes a new instance of the NServiceKit.CacheAccess.Providers.FileAndCacheTextManager class.</summary>
        ///
        /// <param name="baseCachePath">Full pathname of the base cache file.</param>
        /// <param name="cacheManager"> Manager for cache.</param>
		public FileAndCacheTextManager(string baseCachePath, ICacheTextManager cacheManager)
		{
			this.baseCachePath = baseCachePath;
			this.cacheManager = cacheManager;
		}

        /// <summary>Gets the cache client.</summary>
        ///
        /// <value>The cache client.</value>
		public ICacheClient CacheClient
		{
			get { return this.cacheManager.CacheClient; }
		}

        /// <summary>Gets the type of the content.</summary>
        ///
        /// <value>The type of the content.</value>
		public string ContentType
		{
			get { return cacheManager.ContentType; }
		}

        /// <summary>Resolves.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="compressionType">Type of the compression.</param>
        /// <param name="cacheKey">       The cache key.</param>
        /// <param name="createCacheFn">  The create cache function.</param>
        ///
        /// <returns>An object.</returns>
		public object Resolve<T>(string compressionType, string cacheKey, Func<T> createCacheFn) 
			where T : class
		{
			var contentTypeCacheKey = cacheKey + MimeTypes.GetExtension(this.ContentType);

			var acceptsCompress = !string.IsNullOrEmpty(compressionType);
			var cacheKeyZip = acceptsCompress
              	? contentTypeCacheKey  + CompressionTypes.GetExtension(compressionType) 
              	: null;

			if (acceptsCompress)
			{
				var fromCache = GetCompressedBytesFromCache(compressionType, cacheKeyZip);
				if (fromCache != null) return fromCache;

				var fromFile = GetCompressedBytesFromFile(compressionType, cacheKeyZip);
				if (fromFile != null) return fromFile;
			}
			else
			{
				var result = this.CacheClient.Get<string>(cacheKey);
				if (result != null) return result;

				var fromFile = GetFromFile(contentTypeCacheKey);
				if (fromFile != null) return fromFile;
			}

			var cacheValueString = this.cacheManager.ResolveText(cacheKey, createCacheFn);
			WriteToFile(contentTypeCacheKey, Encoding.UTF8.GetBytes(cacheValueString));

			if (acceptsCompress)
			{
				var cacheValueZip = cacheValueString.Compress(compressionType);

				this.CacheClient.Set(cacheKeyZip, cacheValueZip);
				WriteToFile(cacheKeyZip, cacheValueZip);

				return new CompressedResult(cacheValueZip, compressionType, this.ContentType);
			}

			return cacheValueString;
		}

        /// <summary>Clears this object to its blank/initial state.</summary>
        ///
        /// <param name="cacheKeys">A variable-length parameters list containing cache keys.</param>
		public void Clear(IEnumerable<string> cacheKeys)
		{
			Clear(cacheKeys.ToArray());
		}

        /// <summary>Clears this object to its blank/initial state.</summary>
        ///
        /// <param name="cacheKeys">A variable-length parameters list containing cache keys.</param>
		public void Clear(params string[] cacheKeys)
		{
			this.cacheManager.Clear(cacheKeys);

			var contentTypeCacheKeys = cacheKeys.ToList().ConvertAll(x => x + MimeTypes.GetExtension(this.ContentType));
			foreach (var cacheKey in contentTypeCacheKeys)
			{
				var filePath = Path.Combine(this.baseCachePath, cacheKey);
				var gzipFilePath = Path.Combine(this.baseCachePath, cacheKey + CompressionTypes.GetExtension(CompressionTypes.GZip));
				var deflateFilePath = Path.Combine(this.baseCachePath, cacheKey + CompressionTypes.GetExtension(CompressionTypes.Deflate));

				DeleteFiles(filePath, gzipFilePath, deflateFilePath);
			}
		}

		private static void DeleteFiles(params string[] filePaths)
		{
			foreach (var filePath in filePaths)
			{
				//Catching an exception is quicker if you expect the file to be there.
				try
				{
					File.Delete(filePath);
				}
				catch (Exception) { }
			}
		}

        /// <summary>Gets compressed bytes from cache.</summary>
        ///
        /// <param name="compressionType">Type of the compression.</param>
        /// <param name="cacheKey">       The cache key.</param>
        ///
        /// <returns>The compressed bytes from cache.</returns>
		public CompressedResult GetCompressedBytesFromCache(string compressionType, string cacheKey)
		{
			var result = this.CacheClient.Get<byte[]>(cacheKey);
			return result != null
			       	? new CompressedResult(result, compressionType, this.ContentType)
			       	: null;
		}

        /// <summary>Gets compressed bytes from file.</summary>
        ///
        /// <param name="compressionType">Type of the compression.</param>
        /// <param name="cacheKey">       The cache key.</param>
        ///
        /// <returns>The compressed bytes from file.</returns>
		public CompressedFileResult GetCompressedBytesFromFile(string compressionType, string cacheKey)
		{
			var filePath = Path.Combine(this.baseCachePath, cacheKey);
			if (!File.Exists(filePath)) return null;

			return new CompressedFileResult
			(
				filePath,
				compressionType,
				this.ContentType
			);
		}

        /// <summary>Gets from file.</summary>
        ///
        /// <param name="cacheKey">The cache key.</param>
        ///
        /// <returns>The data that was read from the file.</returns>
		public HttpResult GetFromFile(string cacheKey)
		{
			try
			{
				var filePath = Path.Combine(this.baseCachePath, cacheKey);
				return File.Exists(filePath)
					? new HttpResult(new FileInfo(filePath), this.ContentType)
					: null;
			}
			catch (Exception)
			{
				return null;
			}
		}

        /// <summary>Writes to file.</summary>
        ///
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="bytes">   The bytes.</param>
		public void WriteToFile(string cacheKey, byte[] bytes)
		{
			var filePath = Path.Combine(this.baseCachePath, cacheKey);
			Directory.CreateDirectory(Path.GetDirectoryName(filePath));

			using (var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
			{
				fs.Write(bytes, 0, bytes.Length);
			}
		}
	}
}