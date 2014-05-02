using System;
using System.Net;
using NServiceKit.CacheAccess;
using NServiceKit.CacheAccess.Providers;
using NServiceKit.Common.Web;
using NServiceKit.Redis;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface.Auth;
using NServiceKit.ServiceInterface.Testing;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints;

namespace NServiceKit.ServiceInterface
{
    /// <summary>A service extensions.</summary>
    public static class ServiceExtensions
    {
        /// <summary>A string extension method that adds a query parameter.</summary>
        ///
        /// <param name="url">   URL of the document.</param>
        /// <param name="key">   The key.</param>
        /// <param name="val">   The value.</param>
        /// <param name="encode">true to encode.</param>
        ///
        /// <returns>A string.</returns>
        public static string AddQueryParam(this string url, string key, object val, bool encode = true)
        {
            return url.AddQueryParam(key, val.ToString(), encode);
        }

        /// <summary>A string extension method that adds a query parameter.</summary>
        ///
        /// <param name="url">   URL of the document.</param>
        /// <param name="key">   The key.</param>
        /// <param name="val">   The value.</param>
        /// <param name="encode">true to encode.</param>
        ///
        /// <returns>A string.</returns>
        public static string AddQueryParam(this string url, object key, string val, bool encode = true)
        {
            return AddQueryParam(url, (key ?? "").ToString(), val, encode);
        }

        /// <summary>A string extension method that adds a query parameter.</summary>
        ///
        /// <param name="url">   URL of the document.</param>
        /// <param name="key">   The key.</param>
        /// <param name="val">   The value.</param>
        /// <param name="encode">true to encode.</param>
        ///
        /// <returns>A string.</returns>
        public static string AddQueryParam(this string url, string key, string val, bool encode = true)
        {
            if (string.IsNullOrEmpty(url)) return null;
            var prefix = url.IndexOf('?') == -1 ? "?" : "&";
            return url + prefix + key + "=" + (encode ? val.UrlEncode() : val);
        }

        /// <summary>A string extension method that sets query parameter.</summary>
        ///
        /// <param name="url">URL of the document.</param>
        /// <param name="key">The key.</param>
        /// <param name="val">The value.</param>
        ///
        /// <returns>A string.</returns>
        public static string SetQueryParam(this string url, string key, string val)
        {
            if (string.IsNullOrEmpty(url)) return null;
            var qsPos = url.IndexOf('?');
            if (qsPos != -1)
            {
                var existingKeyPos = url.IndexOf(key, qsPos, StringComparison.InvariantCulture);
                if (existingKeyPos != -1)
                {
                    var endPos = url.IndexOf('&', existingKeyPos);
                    if (endPos == -1) endPos = url.Length;

                    var newUrl = url.Substring(0, existingKeyPos + key.Length + 1)
                        + val.UrlEncode()
                        + url.Substring(endPos);
                    return newUrl;
                }
            }
            var prefix = qsPos == -1 ? "?" : "&";
            return url + prefix + key + "=" + val.UrlEncode();
        }

        /// <summary>A string extension method that adds a hash parameter.</summary>
        ///
        /// <param name="url">URL of the document.</param>
        /// <param name="key">The key.</param>
        /// <param name="val">The value.</param>
        ///
        /// <returns>A string.</returns>
        public static string AddHashParam(this string url, string key, object val)
        {
            return url.AddHashParam(key, val.ToString());
        }

        /// <summary>A string extension method that adds a hash parameter.</summary>
        ///
        /// <param name="url">URL of the document.</param>
        /// <param name="key">The key.</param>
        /// <param name="val">The value.</param>
        ///
        /// <returns>A string.</returns>
        public static string AddHashParam(this string url, string key, string val)
        {
            if (string.IsNullOrEmpty(url)) return null;
            var prefix = url.IndexOf('#') == -1 ? "#" : "/";
            return url + prefix + key + "=" + val.UrlEncode();
        }

        /// <summary>An IServiceBase extension method that redirects.</summary>
        ///
        /// <param name="service">The service to act on.</param>
        /// <param name="url">    URL of the document.</param>
        ///
        /// <returns>An IHttpResult.</returns>
        public static IHttpResult Redirect(this IServiceBase service, string url)
        {
            return service.Redirect(url, "Moved Temporarily");
        }

        /// <summary>An IServiceBase extension method that redirects.</summary>
        ///
        /// <param name="service">The service to act on.</param>
        /// <param name="url">    URL of the document.</param>
        /// <param name="message">The message.</param>
        ///
        /// <returns>An IHttpResult.</returns>
        public static IHttpResult Redirect(this IServiceBase service, string url, string message)
        {
            return new HttpResult(HttpStatusCode.Redirect, message)
            {
                ContentType = service.RequestContext.ResponseContentType,
                Headers = {
                    { HttpHeaders.Location, url }
                },
            };
        }

        /// <summary>An IServiceBase extension method that authentication required.</summary>
        ///
        /// <param name="service">The service to act on.</param>
        ///
        /// <returns>An IHttpResult.</returns>
        public static IHttpResult AuthenticationRequired(this IServiceBase service)
        {
            return new HttpResult
            {
                StatusCode = HttpStatusCode.Unauthorized,
                ContentType = service.RequestContext.ResponseContentType,
                Headers = {
                    { HttpHeaders.WwwAuthenticate, AuthService.DefaultOAuthProvider + " realm=\"{0}\"".Fmt(AuthService.DefaultOAuthRealm) }
                },
            };
        }

        /// <summary>An IServiceBase extension method that gets session identifier.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="service">The service to act on.</param>
        ///
        /// <returns>The session identifier.</returns>
        public static string GetSessionId(this IServiceBase service)
        {
            var req = service.RequestContext.Get<IHttpRequest>();
            var id = req.GetSessionId();
            if (id == null)
                throw new ArgumentNullException("Session not set. Is Session being set in RequestFilters?");

            return id;
        }

        /// <summary>
        /// If they don't have an ICacheClient configured use an In Memory one.
        /// </summary>
        private static readonly MemoryCacheClient DefaultCache = new MemoryCacheClient { FlushOnDispose = true };

        /// <summary>An IResolver extension method that gets cache client.</summary>
        ///
        /// <param name="service">The service to act on.</param>
        ///
        /// <returns>The cache client.</returns>
        public static ICacheClient GetCacheClient(this IResolver service)
        {
            return service.TryResolve<ICacheClient>()
                ?? (service.TryResolve<IRedisClientsManager>()!=null ? service.TryResolve<IRedisClientsManager>().GetCacheClient():null)
                ?? DefaultCache;
        }

        /// <summary>An IHttpRequest extension method that saves a session.</summary>
        ///
        /// <param name="service">  The service to act on.</param>
        /// <param name="session">  The session.</param>
        /// <param name="expiresIn">The expires in.</param>
        public static void SaveSession(this IServiceBase service, IAuthSession session, TimeSpan? expiresIn = null)
        {
            if (service == null) return;

            service.RequestContext.Get<IHttpRequest>().SaveSession(session, expiresIn);
        }

        /// <summary>An IHttpRequest extension method that removes the session described by httpReq.</summary>
        ///
        /// <param name="service">The service to act on.</param>
        public static void RemoveSession(this IServiceBase service)
        {
            if (service == null) return;

            service.RequestContext.Get<IHttpRequest>().RemoveSession();
        }

        /// <summary>An IHttpRequest extension method that removes the session described by httpReq.</summary>
        ///
        /// <param name="service">The service to act on.</param>
        public static void RemoveSession(this Service service)
        {
            if (service == null) return;

            service.RequestContext.Get<IHttpRequest>().RemoveSession();
        }

        /// <summary>An ICacheClient extension method that cache set.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="cache">    The cache to act on.</param>
        /// <param name="key">      The key.</param>
        /// <param name="value">    The value.</param>
        /// <param name="expiresIn">The expires in.</param>
        public static void CacheSet<T>(this ICacheClient cache, string key, T value, TimeSpan? expiresIn)
        {
            if (expiresIn.HasValue)
                cache.Set(key, value, expiresIn.Value);
            else
                cache.Set(key, value);
        }

        /// <summary>An IHttpRequest extension method that saves a session.</summary>
        ///
        /// <param name="httpReq">  The httpReq to act on.</param>
        /// <param name="session">  The session.</param>
        /// <param name="expiresIn">The expires in.</param>
        public static void SaveSession(this IHttpRequest httpReq, IAuthSession session, TimeSpan? expiresIn = null)
        {
            if (httpReq == null) return;

            using (var cache = httpReq.GetCacheClient())
            {
                var sessionKey = SessionFeature.GetSessionKey(httpReq.GetSessionId());
                cache.CacheSet(sessionKey, session, expiresIn ?? AuthFeature.GetDefaultSessionExpiry());
            }

            httpReq.Items[RequestItemsSessionKey] = session;
        }

        /// <summary>An IHttpRequest extension method that removes the session described by httpReq.</summary>
        ///
        /// <param name="httpReq">The httpReq to act on.</param>
        public static void RemoveSession(this IHttpRequest httpReq)
        {
            if (httpReq == null) return;

            using (var cache = httpReq.GetCacheClient())
            {
                var sessionKey = SessionFeature.GetSessionKey(httpReq.GetSessionId());
                cache.Remove(sessionKey);
            }

            httpReq.Items.Remove(RequestItemsSessionKey);
        }

        /// <summary>An IHttpRequest extension method that gets a session.</summary>
        ///
        /// <param name="service">The service to act on.</param>
        /// <param name="reload"> true to reload.</param>
        ///
        /// <returns>The session.</returns>
        public static IAuthSession GetSession(this IServiceBase service, bool reload = false)
        {
            return service.RequestContext.Get<IHttpRequest>().GetSession(reload);
        }

        /// <summary>An IHttpRequest extension method that gets a session.</summary>
        ///
        /// <param name="service">The service to act on.</param>
        /// <param name="reload"> true to reload.</param>
        ///
        /// <returns>The session.</returns>
        public static IAuthSession GetSession(this Service service, bool reload = false)
        {
            var req = service.RequestContext.Get<IHttpRequest>();
            if (req.GetSessionId() == null)
                service.RequestContext.Get<IHttpResponse>().CreateSessionIds(req);
            return req.GetSession(reload);
        }

        /// <summary>The request items session key.</summary>
        public const string RequestItemsSessionKey = "__session";

        /// <summary>An IHttpRequest extension method that gets a session.</summary>
        ///
        /// <param name="httpReq">The httpReq to act on.</param>
        /// <param name="reload"> true to reload.</param>
        ///
        /// <returns>The session.</returns>
        public static IAuthSession GetSession(this IHttpRequest httpReq, bool reload = false)
        {
            if (httpReq == null) return null;

            object oSession = null;
            if (!reload)
                httpReq.Items.TryGetValue(RequestItemsSessionKey, out oSession);

            if (oSession != null)
                return (IAuthSession)oSession;

            using (var cache = httpReq.GetCacheClient())
            {
                var sessionId = httpReq.GetSessionId();
                var session = cache.Get<IAuthSession>(SessionFeature.GetSessionKey(sessionId));
                if (session == null)
                {
                    session = AuthService.CurrentSessionFactory();
                    session.Id = sessionId;
                    session.CreatedAt = session.LastModified = DateTime.UtcNow;
                    session.OnCreated(httpReq);
                }

                if (httpReq.Items.ContainsKey(RequestItemsSessionKey))
                    httpReq.Items.Remove(RequestItemsSessionKey);

                httpReq.Items.Add(RequestItemsSessionKey, session);
                return session;
            }
        }

        /// <summary>A TService extension method that executes the action.</summary>
        ///
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <typeparam name="TRequest">Type of the request.</typeparam>
        /// <param name="service">       The service to act on.</param>
        /// <param name="request">       The request.</param>
        /// <param name="invokeAction">  The invoke action.</param>
        /// <param name="requestContext">Context for the request.</param>
        ///
        /// <returns>An object.</returns>
        public static object RunAction<TService, TRequest>(
            this TService service, TRequest request, Func<TService, TRequest, object> invokeAction,
            IRequestContext requestContext = null)
            where TService : IService
        {
            var actionCtx = new ActionContext
            {
                RequestFilters = new IHasRequestFilter[0],
                ResponseFilters = new IHasResponseFilter[0],
                ServiceType = typeof(TService),
                RequestType = typeof(TRequest),                
                ServiceAction = (instance, req) => invokeAction(service, request)
            };

            requestContext = requestContext ?? new MockRequestContext();
            var runner = new ServiceRunner<TRequest>(EndpointHost.AppHost, actionCtx);
            var response = runner.Execute(requestContext, service, request);
            return response;
        }
    }
}
