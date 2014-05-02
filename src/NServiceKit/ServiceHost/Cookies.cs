using System;
using System.Net;
using System.Text;
using System.Web;
using NServiceKit.Common.Web;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints;

namespace NServiceKit.ServiceHost
{
    /// <summary>A cookies.</summary>
    public class Cookies : ICookies
    {
        readonly IHttpResponse httpRes;
        private static readonly DateTime Session = DateTime.MinValue;
        private const string RootPath = "/";

        /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.Cookies class.</summary>
        ///
        /// <param name="httpRes">The HTTP resource.</param>
        public Cookies(IHttpResponse httpRes)
        {
            this.httpRes = httpRes;
        }

        /// <summary>
        /// Sets a persistent cookie which never expires
        /// </summary>
        public void AddPermanentCookie(string cookieName, string cookieValue, bool? secureOnly = null)
        {
            var cookie = new Cookie(cookieName, cookieValue, RootPath) {
                Expires = DateTime.UtcNow.AddYears(20)
            };
            if (secureOnly != null)
            {
                cookie.Secure = secureOnly.Value;
            }
            AddCookie(cookie);
        }

        /// <summary>
        /// Sets a session cookie which expires after the browser session closes
        /// </summary>
        public void AddSessionCookie(string cookieName, string cookieValue, bool? secureOnly = null)
        {
            var cookie = new Cookie(cookieName, cookieValue, RootPath);
            if (secureOnly != null)
            {
                cookie.Secure = secureOnly.Value;
            }
            this.AddCookie(cookie);
        }

        /// <summary>
        /// Deletes a specified cookie by setting its value to empty and expiration to -1 days
        /// </summary>
        public void DeleteCookie(string cookieName)
        {
            var cookie = new Cookie(cookieName, string.Empty, "/") {
                Expires = DateTime.UtcNow.AddDays(-1)
            };
            AddCookie(cookie);
        }

        /// <summary>Converts a cookie to a HTTP cookie.</summary>
        ///
        /// <param name="cookie">The cookie.</param>
        ///
        /// <returns>cookie as a HttpCookie.</returns>
        public HttpCookie ToHttpCookie(Cookie cookie)
        {
            var httpCookie = new HttpCookie(cookie.Name, cookie.Value) {
                Path = cookie.Path,
                Expires = cookie.Expires,
                HttpOnly = (EndpointHost.Config == null || !EndpointHost.Config.AllowNonHttpOnlyCookies) 
                    || cookie.HttpOnly,
                Secure = cookie.Secure
            };
            if (!string.IsNullOrEmpty(cookie.Domain))
            {
                httpCookie.Domain = cookie.Domain;
            }
            else if (EndpointHost.Config != null && EndpointHost.Config.RestrictAllCookiesToDomain != null)
            {
                httpCookie.Domain = EndpointHost.Config.RestrictAllCookiesToDomain;
            }
            return httpCookie;
        }

        /// <summary>Gets header value.</summary>
        ///
        /// <param name="cookie">The cookie.</param>
        ///
        /// <returns>The header value.</returns>
        public string GetHeaderValue(Cookie cookie)
        {
            var path = cookie.Expires == Session
                ? "/"
                : cookie.Path ?? "/";

            var sb = new StringBuilder();

            sb.AppendFormat("{0}={1};path={2}", cookie.Name, cookie.Value, path);

            if (cookie.Expires != Session)
            {
                sb.AppendFormat(";expires={0}", cookie.Expires.ToString("R"));
            }

            if (!string.IsNullOrEmpty(cookie.Domain))
            {
                sb.AppendFormat(";domain={0}", cookie.Domain);
            }
            else if (EndpointHost.Config.RestrictAllCookiesToDomain != null)
            {
                sb.AppendFormat(";domain={0}", EndpointHost.Config.RestrictAllCookiesToDomain);
            }

            if (cookie.Secure)
            {
                sb.Append(";Secure");
            }
            if (cookie.HttpOnly)
            {
                sb.Append(";HttpOnly");
            }
            
            return sb.ToString();
        }

        /// <summary>
        /// Sets a persistent cookie which expires after the given time
        /// </summary>
        public void AddCookie(Cookie cookie)
        {
            var aspNet = this.httpRes.OriginalResponse as HttpResponse;
            if (aspNet != null)
            {
                var httpCookie = ToHttpCookie(cookie);
                aspNet.SetCookie(httpCookie);
                return;
            }
            var httpListener = this.httpRes.OriginalResponse as HttpListenerResponse;
            if (httpListener != null)
            {
                var cookieStr = GetHeaderValue(cookie);
                httpListener.Headers.Add(HttpHeaders.SetCookie, cookieStr);
            }
        }
    }
}
