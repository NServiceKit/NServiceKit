using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using NServiceKit.Common.Web;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints.Extensions;

namespace NServiceKit.ServiceHost
{
    /// <summary>A HTTP response extensions.</summary>
	public static class HttpResponseExtensions
	{
        /// <summary>An IHttpResponse extension method that redirect to URL.</summary>
        ///
        /// <param name="httpRes">           The httpRes to act on.</param>
        /// <param name="url">               URL of the document.</param>
        /// <param name="redirectStatusCode">The redirect status code.</param>
		public static void RedirectToUrl(this IHttpResponse httpRes, string url, HttpStatusCode redirectStatusCode=HttpStatusCode.Redirect)
		{
		    httpRes.StatusCode = (int) redirectStatusCode;
			httpRes.AddHeader(HttpHeaders.Location, url);
            httpRes.EndRequest();
        }

        /// <summary>An IHttpResponse extension method that transmit file.</summary>
        ///
        /// <param name="httpRes"> The httpRes to act on.</param>
        /// <param name="filePath">Full pathname of the file.</param>
		public static void TransmitFile(this IHttpResponse httpRes, string filePath)
		{
			var aspNetRes = httpRes as HttpResponseWrapper;
			if (aspNetRes != null)
			{
				aspNetRes.Response.TransmitFile(filePath);
				return;
			}

			using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
			{
				fs.WriteTo(httpRes.OutputStream);
			}

            httpRes.EndRequest();
        }

        /// <summary>An IHttpResponse extension method that writes a file.</summary>
        ///
        /// <param name="httpRes"> The httpRes to act on.</param>
        /// <param name="filePath">Full pathname of the file.</param>
		public static void WriteFile(this IHttpResponse httpRes, string filePath)
		{
			var aspNetRes = httpRes as HttpResponseWrapper;
			if (aspNetRes != null)
			{
				aspNetRes.Response.WriteFile(filePath);
				return;
			}

			using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
			{
				fs.WriteTo(httpRes.OutputStream);
			}

            httpRes.EndRequest();
        }

        /// <summary>An IHttpResponse extension method that redirects.</summary>
        ///
        /// <param name="httpRes">The httpRes to act on.</param>
        /// <param name="url">    URL of the document.</param>
		public static void Redirect(this IHttpResponse httpRes, string url)
		{
			httpRes.AddHeader(HttpHeaders.Location, url);
            httpRes.EndRequest();
        }

        /// <summary>An IHttpResponse extension method that returns authentication required.</summary>
        ///
        /// <param name="httpRes">The httpRes to act on.</param>
		public static void ReturnAuthRequired(this IHttpResponse httpRes)
		{
			httpRes.ReturnAuthRequired("Auth Required");
		}

        /// <summary>An IHttpResponse extension method that returns authentication required.</summary>
        ///
        /// <param name="httpRes">  The httpRes to act on.</param>
        /// <param name="authRealm">The authentication realm.</param>
		public static void ReturnAuthRequired(this IHttpResponse httpRes, string authRealm)
		{
            httpRes.ReturnAuthRequired(AuthenticationHeaderType.Basic, authRealm);
		}

        /// <summary>An IHttpResponse extension method that returns authentication required.</summary>
        ///
        /// <param name="httpRes">  The httpRes to act on.</param>
        /// <param name="authType"> Type of the authentication.</param>
        /// <param name="authRealm">The authentication realm.</param>
        public static void ReturnAuthRequired(this IHttpResponse httpRes, AuthenticationHeaderType authType, string authRealm)
        {
            httpRes.StatusCode = (int)HttpStatusCode.Unauthorized;
            httpRes.AddHeader(HttpHeaders.WwwAuthenticate, string.Format("{0} realm=\"{1}\"",authType,authRealm));
            httpRes.EndRequest();
        }

		/// <summary>
		/// Sets a persistent cookie which never expires
		/// </summary>
		public static void SetPermanentCookie(this IHttpResponse httpRes, string cookieName, string cookieValue)
		{
			httpRes.Cookies.AddPermanentCookie(cookieName, cookieValue);
		}

		/// <summary>
		/// Sets a session cookie which expires after the browser session closes
		/// </summary>
		public static void SetSessionCookie(this IHttpResponse httpRes, string cookieName, string cookieValue)
		{
			httpRes.Cookies.AddSessionCookie(cookieName, cookieValue);
		}

		/// <summary>
		/// Sets a persistent cookie which expires after the given time
		/// </summary>
		public static void SetCookie(this IHttpResponse httpRes, string cookieName, string cookieValue, TimeSpan expiresIn)
		{
			httpRes.Cookies.AddCookie(new Cookie(cookieName, cookieValue) {
				Expires = DateTime.UtcNow + expiresIn
			});
		}

		/// <summary>
		/// Sets a persistent cookie with an expiresAt date
		/// </summary>
		public static void SetCookie(this IHttpResponse httpRes, string cookieName,
			string cookieValue, DateTime expiresAt, string path = "/")
		{
			httpRes.Cookies.AddCookie(new Cookie(cookieName, cookieValue, path) {
				Expires = expiresAt,
			});
		}

		/// <summary>
		/// Deletes a specified cookie by setting its value to empty and expiration to -1 days
		/// </summary>
		public static void DeleteCookie(this IHttpResponse httpRes, string cookieName)
		{
			httpRes.Cookies.DeleteCookie(cookieName);
		}

        /// <summary>An IHttpResponse extension method that cookies as dictionary.</summary>
        ///
        /// <param name="httpRes">The httpRes to act on.</param>
        ///
        /// <returns>A Dictionary&lt;string,string&gt;</returns>
		public static Dictionary<string, string> CookiesAsDictionary(this IHttpResponse httpRes)
		{
			var map = new Dictionary<string, string>();
			var aspNet = httpRes.OriginalResponse as System.Web.HttpResponse;
			if (aspNet != null)
			{
				foreach (var name in aspNet.Cookies.AllKeys)
				{
					var cookie = aspNet.Cookies[name];
					if (cookie == null) continue;
					map[name] = cookie.Value;
				}
			}
			else
			{
				var httpListener = httpRes.OriginalResponse as HttpListenerResponse;
				if (httpListener != null)
				{
					for (var i = 0; i < httpListener.Cookies.Count; i++)
					{
						var cookie = httpListener.Cookies[i];
						if (cookie == null || cookie.Name == null) continue;
						map[cookie.Name] = cookie.Value;
					}
				}
			}
			return map;
		}

        /// <summary>An IHttpResponse extension method that adds a header last modified to 'lastModified'.</summary>
        ///
        /// <param name="httpRes">     The httpRes to act on.</param>
        /// <param name="lastModified">The last modified.</param>
		public static void AddHeaderLastModified(this IHttpResponse httpRes, DateTime? lastModified)
		{
			if (!lastModified.HasValue) return;
			var lastWt = lastModified.Value.ToUniversalTime();
			httpRes.AddHeader(HttpHeaders.LastModified, lastWt.ToString("r"));
		}
	}
    /// <summary>Values that represent AuthenticationHeaderType.</summary>
    public enum AuthenticationHeaderType
    {
        /// <summary>An enum constant representing the basic option.</summary>
        Basic,

        /// <summary>An enum constant representing the digest option.</summary>
        Digest
    }
}