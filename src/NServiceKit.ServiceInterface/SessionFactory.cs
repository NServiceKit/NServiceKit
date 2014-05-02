using System;
using System.Web;
using NServiceKit.CacheAccess;
using NServiceKit.ServiceHost;
using NServiceKit.Text;
using NServiceKit.Text.Common;
using NServiceKit.WebHost.Endpoints.Extensions;

namespace NServiceKit.ServiceInterface
{
    /// <summary>A session factory.</summary>
    public class SessionFactory : ISessionFactory
    {
        private readonly ICacheClient cacheClient;

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.SessionFactory class.</summary>
        ///
        /// <param name="cacheClient">The cache client.</param>
        public SessionFactory(ICacheClient cacheClient)
        {
            this.cacheClient = cacheClient;
        }

        /// <summary>A session cache client.</summary>
        public class SessionCacheClient : ISession
        {
            private readonly ICacheClient cacheClient;
            private string prefixNs;

            /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.SessionFactory.SessionCacheClient class.</summary>
            ///
            /// <param name="cacheClient">The cache client.</param>
            /// <param name="sessionId">  Identifier for the session.</param>
            public SessionCacheClient(ICacheClient cacheClient, string sessionId)
            {
                this.cacheClient = cacheClient;
                this.prefixNs = "sess:" + sessionId + ":";
            }

            /// <summary>Indexer to get or set items within this collection using array index syntax.</summary>
            ///
            /// <param name="key">The key.</param>
            ///
            /// <returns>The indexed item.</returns>
            public object this[string key]
            {
                get
                {
                    return cacheClient.Get<object>(this.prefixNs + key);
                }
                set
                {
                    JsWriter.WriteDynamic(() => 
                        cacheClient.Set(this.prefixNs + key, value));
                }
            }

            /// <summary>Sets.</summary>
            ///
            /// <typeparam name="T">Generic type parameter.</typeparam>
            /// <param name="key">  The key.</param>
            /// <param name="value">The value.</param>
            public void Set<T>(string key, T value)
            {
                cacheClient.Set(this.prefixNs + key, value);
            }

            /// <summary>Gets.</summary>
            ///
            /// <typeparam name="T">Generic type parameter.</typeparam>
            /// <param name="key">The key.</param>
            ///
            /// <returns>A T.</returns>
            public T Get<T>(string key)
            {
                return cacheClient.Get<T>(this.prefixNs + key);
            }
        }

        /// <summary>Gets the session for this request, creates one if it doesn't exist.</summary>
        ///
        /// <param name="httpReq">.</param>
        /// <param name="httpRes">.</param>
        ///
        /// <returns>The or create session.</returns>
        public ISession GetOrCreateSession(IHttpRequest httpReq, IHttpResponse httpRes)
        {
            var sessionId = httpReq.GetSessionId()
                ?? httpRes.CreateSessionIds(httpReq);

            return new SessionCacheClient(cacheClient, sessionId);
        }

        /// <summary>Gets the session for this request, creates one if it doesn't exist. Only for ASP.NET apps. Uses the HttpContext.Current singleton.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <returns>The or create session.</returns>
        public ISession GetOrCreateSession()
        {
            if (HttpContext.Current != null)
                return GetOrCreateSession(
                    HttpContext.Current.Request.ToRequest(),
                    HttpContext.Current.Response.ToResponse());
            
            throw new NotImplementedException("Only ASP.NET Requests can be accessed via Singletons");
        }
    }
}