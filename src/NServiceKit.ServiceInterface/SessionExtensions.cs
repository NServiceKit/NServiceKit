using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Web;
using NServiceKit.CacheAccess;
using NServiceKit.Common;
using NServiceKit.Configuration;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface.Auth;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints;
using NServiceKit.WebHost.Endpoints.Extensions;

namespace NServiceKit.ServiceInterface
{
    /// <summary>A session options.</summary>
    public class SessionOptions
    {
        /// <summary>The temporary.</summary>
        public const string Temporary = "temp";
        /// <summary>The permanent.</summary>
        public const string Permanent = "perm";
    }

    /// <summary>
    /// Configure NServiceKit to have ISession support
    /// </summary>
    public static class SessionExtensions 
    {
        /// <summary>An IHttpRequest extension method that gets session identifier.</summary>
        ///
        /// <param name="httpReq">The HTTP request.</param>
        ///
        /// <returns>The session identifier.</returns>
        public static string GetSessionId(this IHttpRequest httpReq)
        {
            var sessionOptions = GetSessionOptions(httpReq);

            return sessionOptions.Contains(SessionOptions.Permanent)
                ? httpReq.GetItemOrCookie(SessionFeature.PermanentSessionId)
                : httpReq.GetItemOrCookie(SessionFeature.SessionId);
        }

        /// <summary>An IHttpRequest extension method that gets permanent session identifier.</summary>
        ///
        /// <param name="httpReq">The HTTP request.</param>
        ///
        /// <returns>The permanent session identifier.</returns>
        public static string GetPermanentSessionId(this IHttpRequest httpReq)
        {
            return httpReq.GetItemOrCookie(SessionFeature.PermanentSessionId);
        }

        /// <summary>An IHttpRequest extension method that gets temporary session identifier.</summary>
        ///
        /// <param name="httpReq">The HTTP request.</param>
        ///
        /// <returns>The temporary session identifier.</returns>
        public static string GetTemporarySessionId(this IHttpRequest httpReq)
        {
            return httpReq.GetItemOrCookie(SessionFeature.SessionId);
        }

        /// <summary>
        /// Create the active Session or Permanent Session Id cookie.
        /// </summary>
        /// <returns></returns>
        public static string CreateSessionId(this IHttpResponse res, IHttpRequest req)
        {
            var sessionOptions = GetSessionOptions(req);
            return sessionOptions.Contains(SessionOptions.Permanent)
                ? res.CreatePermanentSessionId(req)
                : res.CreateTemporarySessionId(req);
        }

        /// <summary>
        /// Create both Permanent and Session Id cookies and return the active sessionId
        /// </summary>
        /// <returns></returns>
        public static string CreateSessionIds(this IHttpResponse res, IHttpRequest req)
        {
            var sessionOptions = GetSessionOptions(req);
            var permId = res.CreatePermanentSessionId(req);
            var tempId = res.CreateTemporarySessionId(req);
            return sessionOptions.Contains(SessionOptions.Permanent)
                ? permId
                : tempId;
        }

        static readonly RandomNumberGenerator randgen = new RNGCryptoServiceProvider();
        internal static string CreateRandomSessionId()
        {
            var data = new byte[15];
            randgen.GetBytes(data);
            return Convert.ToBase64String(data);
        }

        /// <summary>An IHttpResponse extension method that creates permanent session identifier.</summary>
        ///
        /// <param name="res">The res to act on.</param>
        /// <param name="req">The request.</param>
        ///
        /// <returns>The new permanent session identifier.</returns>
        public static string CreatePermanentSessionId(this IHttpResponse res, IHttpRequest req)
        {
            var sessionId = CreateRandomSessionId();
            res.Cookies.AddPermanentCookie(SessionFeature.PermanentSessionId, sessionId);
            req.Items[SessionFeature.PermanentSessionId] = sessionId;
            return sessionId;
        }

        /// <summary>An IHttpResponse extension method that creates temporary session identifier.</summary>
        ///
        /// <param name="res">The res to act on.</param>
        /// <param name="req">The request.</param>
        ///
        /// <returns>The new temporary session identifier.</returns>
        public static string CreateTemporarySessionId(this IHttpResponse res, IHttpRequest req)
        {
            var sessionId = CreateRandomSessionId();
            res.Cookies.AddSessionCookie(SessionFeature.SessionId, sessionId,
                (EndpointHost.Config != null && EndpointHost.Config.OnlySendSessionCookiesSecurely && req.IsSecureConnection));
            req.Items[SessionFeature.SessionId] = sessionId;
            return sessionId;
        }

        /// <summary>An IHttpRequest extension method that gets session options.</summary>
        ///
        /// <param name="httpReq">The HTTP request.</param>
        ///
        /// <returns>The session options.</returns>
        public static HashSet<string> GetSessionOptions(this IHttpRequest httpReq)
        {
            var sessionOptions = httpReq.GetItemOrCookie(SessionFeature.SessionOptionsKey);
            return sessionOptions.IsNullOrEmpty()
                ? new HashSet<string>()
                : sessionOptions.Split(',').ToHashSet();
        }

        /// <summary>An IAuthSession extension method that updates the session.</summary>
        ///
        /// <param name="session"> The session to act on.</param>
        /// <param name="userAuth">The user authentication.</param>
        public static void UpdateSession(this IAuthSession session, UserAuth userAuth)
        {
            if (userAuth == null || session == null) return;
            session.Roles = userAuth.Roles;
            session.Permissions = userAuth.Permissions;
        }

        /// <summary>An IAuthSession extension method that updates from user authentication repo.</summary>
        ///
        /// <param name="session">     The session to act on.</param>
        /// <param name="req">         The request.</param>
        /// <param name="userAuthRepo">The user authentication repo.</param>
        public static void UpdateFromUserAuthRepo(this IAuthSession session, IHttpRequest req, IUserAuthRepository userAuthRepo = null)
        {
            if (userAuthRepo == null)
                userAuthRepo = req.TryResolve<IUserAuthRepository>();

            if (userAuthRepo == null) return;

            var userAuth = userAuthRepo.GetUserAuth(session, null);
            session.UpdateSession(userAuth);
        }

        /// <summary>An IHttpResponse extension method that adds a session options.</summary>
        ///
        /// <param name="res">    The res to act on.</param>
        /// <param name="req">    The request.</param>
        /// <param name="options">Options for controlling the operation.</param>
        ///
        /// <returns>A HashSet&lt;string&gt;</returns>
        public static HashSet<string> AddSessionOptions(this IHttpResponse res, IHttpRequest req, params string[] options)
        {
            if (res == null || req == null || options.Length == 0) return new HashSet<string>();

            var existingOptions = req.GetSessionOptions();
            foreach (var option in options)
            {
                if (option.IsNullOrEmpty()) continue;

                if (option == SessionOptions.Permanent)
                    existingOptions.Remove(SessionOptions.Temporary);
                else if (option == SessionOptions.Temporary)
                    existingOptions.Remove(SessionOptions.Permanent);

                existingOptions.Add(option);
            }

            var strOptions = String.Join(",", existingOptions.ToArray());
            res.Cookies.AddPermanentCookie(SessionFeature.SessionOptionsKey, strOptions);
            req.Items[SessionFeature.SessionOptionsKey] = strOptions;
            
            return existingOptions;
        }

        /// <summary>Gets session key.</summary>
        ///
        /// <param name="httpReq">The HTTP request.</param>
        ///
        /// <returns>The session key.</returns>
        public static string GetSessionKey(IHttpRequest httpReq = null)
        {
            var sessionId = SessionFeature.GetSessionId(httpReq);
            return sessionId == null ? null : SessionFeature.GetSessionKey(sessionId);
        }

        /// <summary>An ICacheClient extension method that session as.</summary>
        ///
        /// <typeparam name="TUserSession">Type of the user session.</typeparam>
        /// <param name="cache">  The cache to act on.</param>
        /// <param name="httpReq">The HTTP request.</param>
        /// <param name="httpRes">The HTTP resource.</param>
        ///
        /// <returns>A TUserSession.</returns>
        public static TUserSession SessionAs<TUserSession>(this ICacheClient cache,
            IHttpRequest httpReq = null, IHttpResponse httpRes = null)
        {
            var sessionKey = GetSessionKey(httpReq);

            if (sessionKey != null)
            {
                var userSession = cache.Get<TUserSession>(sessionKey);
                if (!Equals(userSession, default(TUserSession)))
                    return userSession;
            }

            if (sessionKey == null)
                SessionFeature.CreateSessionIds(httpReq, httpRes);

            var unAuthorizedSession = (TUserSession)typeof(TUserSession).CreateInstance();
            return unAuthorizedSession;
        }

        /// <summary>An ICacheClient extension method that clears the session.</summary>
        ///
        /// <param name="cache">  The cache to act on.</param>
        /// <param name="httpReq">The HTTP request.</param>
        public static void ClearSession(this ICacheClient cache, IHttpRequest httpReq = null)
        {
            cache.Remove(GetSessionKey(httpReq));
        }
    }
}