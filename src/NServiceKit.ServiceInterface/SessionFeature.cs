using System;
using System.Web;
using NServiceKit.CacheAccess;
using NServiceKit.Common.Utils;
using NServiceKit.Common.Web;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface.Auth;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints;
using NServiceKit.WebHost.Endpoints.Extensions;

namespace NServiceKit.ServiceInterface
{
    /// <summary>A session feature.</summary>
    public class SessionFeature : IPlugin
    {
        /// <summary>The only ASP net.</summary>
        public const string OnlyAspNet = "Only ASP.NET Requests accessible via Singletons are supported";
        /// <summary>Identifier for the session.</summary>
        public const string SessionId = "ss-id";
        /// <summary>Identifier for the permanent session.</summary>
        public const string PermanentSessionId = "ss-pid";
        /// <summary>The session options key.</summary>
        public const string SessionOptionsKey = "ss-opt";
        /// <summary>Identifier for the user authentication.</summary>
        public const string XUserAuthId = HttpHeaders.XUserAuthId;
        /// <summary>2 weeks.</summary>
        public static TimeSpan DefaultSessionExpiry = TimeSpan.FromDays(7 * 2);

        private static bool alreadyConfigured;

        /// <summary>Registers this object.</summary>
        ///
        /// <param name="appHost">The application host.</param>
        public void Register(IAppHost appHost)
        {
            if (alreadyConfigured) return;
            alreadyConfigured = true;

            //Add permanent and session cookies if not already set.
            appHost.RequestFilters.Add(AddSessionIdToRequestFilter);
        }

        /// <summary>Adds a session identifier to request filter.</summary>
        ///
        /// <param name="req">       The request.</param>
        /// <param name="res">       The resource.</param>
        /// <param name="requestDto">The request dto.</param>
        public static void AddSessionIdToRequestFilter(IHttpRequest req, IHttpResponse res, object requestDto)
        {
            if (req.GetItemOrCookie(SessionId) == null)
            {
                res.CreateTemporarySessionId(req);
            }
            if (req.GetItemOrCookie(PermanentSessionId) == null)
            {
                res.CreatePermanentSessionId(req);
            }
        }

        /// <summary>Gets session identifier.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <param name="httpReq">The HTTP request.</param>
        ///
        /// <returns>The session identifier.</returns>
        public static string GetSessionId(IHttpRequest httpReq = null)
        {
            if (httpReq == null && HttpContext.Current == null)
                throw new NotImplementedException(OnlyAspNet);

            httpReq = httpReq ?? HttpContext.Current.Request.ToRequest();

            return httpReq.GetSessionId();
        }

        /// <summary>Creates session identifiers.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <param name="httpReq">The HTTP request.</param>
        /// <param name="httpRes">The HTTP resource.</param>
        public static void CreateSessionIds(IHttpRequest httpReq = null, IHttpResponse httpRes = null)
        {
            if (httpReq == null || httpRes == null)
            {
                if (HttpContext.Current == null)
                    throw new NotImplementedException(OnlyAspNet);
            }

            httpReq = httpReq ?? HttpContext.Current.Request.ToRequest();
            httpRes = httpRes ?? HttpContext.Current.Response.ToResponse();

            httpRes.CreateSessionIds(httpReq);
        }

        /// <summary>Gets session key.</summary>
        ///
        /// <param name="httpReq">The HTTP request.</param>
        ///
        /// <returns>The session key.</returns>
        public static string GetSessionKey(IHttpRequest httpReq = null)
        {
            var sessionId = GetSessionId(httpReq);
            return sessionId == null ? null : GetSessionKey(sessionId);
        }

        /// <summary>Gets session key.</summary>
        ///
        /// <param name="sessionId">Identifier for the session.</param>
        ///
        /// <returns>The session key.</returns>
        public static string GetSessionKey(string sessionId)
        {
            return IdUtils.CreateUrn<IAuthSession>(sessionId);
        }

        /// <summary>Gets or create session.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="cacheClient">The cache client.</param>
        /// <param name="httpReq">    The HTTP request.</param>
        /// <param name="httpRes">    The HTTP resource.</param>
        ///
        /// <returns>The or create session.</returns>
        public static T GetOrCreateSession<T>(ICacheClient cacheClient, IHttpRequest httpReq = null, IHttpResponse httpRes = null) 
            where T : class
        {
            T session = null;
            if (GetSessionKey(httpReq) != null)
                session = cacheClient.Get<T>(GetSessionKey(httpReq));
            else
                CreateSessionIds(httpReq, httpRes);

            return session ?? (T)typeof(T).CreateInstance();
        }
    }
}