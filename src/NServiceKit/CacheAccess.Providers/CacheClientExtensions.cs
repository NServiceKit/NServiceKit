using System;
using System.Collections.Generic;
using NServiceKit.ServiceHost;
using NServiceKit.Common.Web;
using NServiceKit.Common.Extensions;
using NServiceKit.WebHost.Endpoints;

namespace NServiceKit.CacheAccess.Providers
{
    /// <summary>A cache client extensions.</summary>
	public static class CacheClientExtensions
	{
        /// <summary>An ICacheClient extension method that sets.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="cacheClient">  Cache client.</param>
        /// <param name="cacheKey">     The cache key.</param>
        /// <param name="value">        The value.</param>
        /// <param name="expireCacheIn">The expire cache in.</param>
		public static void Set<T>(this ICacheClient cacheClient, string cacheKey, T value, TimeSpan? expireCacheIn)
		{
			if (expireCacheIn.HasValue)
				cacheClient.Set(cacheKey, value, expireCacheIn.Value);
			else
				cacheClient.Set(cacheKey, value);
		}

        /// <summary>An ICacheClient extension method that resolve from cache.</summary>
        ///
        /// <param name="cacheClient">Cache client.</param>
        /// <param name="cacheKey">   The cache key.</param>
        /// <param name="context">    The context.</param>
        ///
        /// <returns>An object.</returns>
		public static object ResolveFromCache(this ICacheClient cacheClient, 
			string cacheKey, 
			IRequestContext context)
		{
			string modifiers = null;
            if (!context.ResponseContentType.IsBinary())
            {

                if (context.ResponseContentType == ContentType.Json)
                {
                    string jsonp = context.Get<IHttpRequest>().GetJsonpCallback();
                    if (jsonp != null)
                        modifiers = ".jsonp," + jsonp.SafeVarName();
                }

                var cacheKeySerialized = GetCacheKeyForSerialized(cacheKey, context.ResponseContentType, modifiers);

                bool doCompression = context.CompressionType != null;
                if (doCompression)
                {
                    var cacheKeySerializedZip = GetCacheKeyForCompressed(cacheKeySerialized, context.CompressionType);

                    var compressedResult = cacheClient.Get<byte[]>(cacheKeySerializedZip);
                    if (compressedResult != null)
                    {
                        return new CompressedResult(
                            compressedResult,
                            context.CompressionType,
                            context.ResponseContentType);
                    }
                }
                else
                {
                    var serializedResult = cacheClient.Get<string>(cacheKeySerialized);
                    if (serializedResult != null)
                    {
                        return serializedResult;
                    }
                }
            }
            else
            {
                var cacheKeySerialized = GetCacheKeyForSerialized(cacheKey, context.ResponseContentType, modifiers);
                var serializedResult = cacheClient.Get<byte[]>(cacheKeySerialized);
                if (serializedResult != null)
                {
                    return serializedResult;
                }
            }

		    return null;
		}

        /// <summary>An ICacheClient extension method that caches.</summary>
        ///
        /// <param name="cacheClient">  Cache client.</param>
        /// <param name="cacheKey">     The cache key.</param>
        /// <param name="responseDto">  The response dto.</param>
        /// <param name="context">      The context.</param>
        /// <param name="expireCacheIn">The expire cache in.</param>
        ///
        /// <returns>An object.</returns>
		public static object Cache(this ICacheClient cacheClient, 
			string cacheKey, 
			object responseDto, 
			IRequestContext context, 
			TimeSpan? expireCacheIn = null)
		{
			cacheClient.Set(cacheKey, responseDto, expireCacheIn);

            if (!context.ResponseContentType.IsBinary())
            {
                string serializedDto = EndpointHost.ContentTypeFilter.SerializeToString(context, responseDto);

                string modifiers = null;
                if (context.ResponseContentType.MatchesContentType(ContentType.Json))
                {
                    var jsonp = context.Get<IHttpRequest>().GetJsonpCallback();
                    if (jsonp != null)
                    {
                        modifiers = ".jsonp," + jsonp.SafeVarName();
                        serializedDto = jsonp + "(" + serializedDto + ")";

                        //Add a default expire timespan for jsonp requests,
                        //because they aren't cleared when calling ClearCaches()
                        if (expireCacheIn == null)
                            expireCacheIn = EndpointHost.Config.DefaultJsonpCacheExpiration;
                    }
                }

                var cacheKeySerialized = GetCacheKeyForSerialized(cacheKey, context.ResponseContentType, modifiers);
                cacheClient.Set(cacheKeySerialized, serializedDto, expireCacheIn);

                bool doCompression = context.CompressionType != null;
                if (doCompression)
                {
                    var cacheKeySerializedZip = GetCacheKeyForCompressed(cacheKeySerialized, context.CompressionType);

                    byte[] compressedSerializedDto = Common.StreamExtensions.Compress(serializedDto, context.CompressionType);
                    cacheClient.Set(cacheKeySerializedZip, compressedSerializedDto, expireCacheIn);

                    return (compressedSerializedDto != null)
                        ? new CompressedResult(compressedSerializedDto, context.CompressionType, context.ResponseContentType)
                        : null;
                }

                return serializedDto;
            }
            else
            {
                string modifiers = null;
                byte[] serializedDto = EndpointHost.ContentTypeFilter.SerializeToBytes(context, responseDto);
                var cacheKeySerialized = GetCacheKeyForSerialized(cacheKey, context.ResponseContentType, modifiers);
                cacheClient.Set(cacheKeySerialized, serializedDto, expireCacheIn);
                return serializedDto;
            }
		}

        /// <summary>An ICacheClient extension method that clears the caches.</summary>
        ///
        /// <param name="cacheClient">Cache client.</param>
        /// <param name="cacheKeys">  A variable-length parameters list containing cache keys.</param>
		public static void ClearCaches(this ICacheClient cacheClient, params string[] cacheKeys)
		{
			var allContentTypes = new List<string>(EndpointHost.ContentTypeFilter.ContentTypeFormats.Values)
			{ ContentType.XmlText, ContentType.JsonText, ContentType.JsvText };

			var allCacheKeys = new List<string>();

			foreach (var cacheKey in cacheKeys)
			{
				allCacheKeys.Add(cacheKey);
				foreach (var serializedExt in allContentTypes)
				{
					var serializedCacheKey = GetCacheKeyForSerialized(cacheKey, serializedExt, null);
					allCacheKeys.Add(serializedCacheKey);

					foreach (var compressionType in CompressionTypes.AllCompressionTypes)
					{
						allCacheKeys.Add(GetCacheKeyForCompressed(serializedCacheKey, compressionType));
					}
				}
			}

			cacheClient.RemoveAll(allCacheKeys);
		}

        /// <summary>Gets cache key for serialized.</summary>
        ///
        /// <param name="cacheKey"> The cache key.</param>
        /// <param name="mimeType"> Type of the mime.</param>
        /// <param name="modifiers">The modifiers.</param>
        ///
        /// <returns>The cache key for serialized.</returns>
		public static string GetCacheKeyForSerialized(string cacheKey, string mimeType, string modifiers)
		{
			return cacheKey + MimeTypes.GetExtension(mimeType) + modifiers;
		}

        /// <summary>Gets cache key for compressed.</summary>
        ///
        /// <param name="cacheKeySerialized">The cache key serialized.</param>
        /// <param name="compressionType">   Type of the compression.</param>
        ///
        /// <returns>The cache key for compressed.</returns>
		public static string GetCacheKeyForCompressed(string cacheKeySerialized, string compressionType)
		{
			return cacheKeySerialized + "." + compressionType;
		}

		/// <summary>
		/// Removes items from cache that have keys matching the specified wildcard pattern
		/// </summary>
		/// <param name="cacheClient">Cache client</param>
		/// <param name="pattern">The wildcard, where "*" means any sequence of characters and "?" means any single character.</param>
		public static void RemoveByPattern(this ICacheClient cacheClient, string pattern)
		{
			var canRemoveByPattern = cacheClient as IRemoveByPattern;
			if (canRemoveByPattern == null)
				throw new NotImplementedException("ICacheRemovableByPattern is not implemented by the cache client: " + cacheClient.GetType().FullName);

			canRemoveByPattern.RemoveByPattern(pattern);
		}
		/// <summary>
		/// Removes items from the cache based on the specified regular expression pattern
		/// </summary>
		/// <param name="cacheClient">Cache client</param>
		/// <param name="regex">Regular expression pattern to search cache keys</param>
		public static void RemoveByRegex(this ICacheClient cacheClient, string regex)
		{
			var canRemoveByPattern = cacheClient as IRemoveByPattern;
			if (canRemoveByPattern == null)
				throw new NotImplementedException("ICacheRemovableByPattern is not implemented by the cache client: " + cacheClient.GetType().FullName);

			canRemoveByPattern.RemoveByRegex(regex);
		}
	}
}
