using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using System.Web;
using NServiceKit.Common;
using NServiceKit.Common.Web;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints;
using HttpRequestWrapper = NServiceKit.WebHost.Endpoints.Extensions.HttpRequestWrapper;

namespace NServiceKit.ServiceHost
{
    /// <summary>A HTTP request extensions.</summary>
	public static class HttpRequestExtensions
	{
	    /// <summary>
		/// Gets string value from Items[name] then Cookies[name] if exists.
		/// Useful when *first* setting the users response cookie in the request filter.
		/// To access the value for this initial request you need to set it in Items[].
		/// </summary>
		/// <returns>string value or null if it doesn't exist</returns>
		public static string GetItemOrCookie(this IHttpRequest httpReq, string name)
		{
			object value;
			if (httpReq.Items.TryGetValue(name, out value)) return value.ToString();

			Cookie cookie;
			if (httpReq.Cookies.TryGetValue(name, out cookie)) return cookie.Value;

			return null;
		}

		/// <summary>
		/// Gets request paramater string value by looking in the following order:
		/// - QueryString[name]
		/// - FormData[name]
		/// - Cookies[name]
		/// - Items[name]
		/// </summary>
		/// <returns>string value or null if it doesn't exist</returns>
		public static string GetParam(this IHttpRequest httpReq, string name)
		{
			string value;
			if ((value = httpReq.Headers[HttpHeaders.XParamOverridePrefix + name]) != null) return value;
			if ((value = httpReq.QueryString[name]) != null) return value;
			if ((value = httpReq.FormData[name]) != null) return value;

            //IIS will assign null to params without a name: .../?some_value can be retrieved as req.Params[null]
            //TryGetValue is not happy with null dictionary keys, so we should bail out here
            if (string.IsNullOrEmpty(name)) return null;

			Cookie cookie;
			if (httpReq.Cookies.TryGetValue(name, out cookie)) return cookie.Value;

			object oValue;
			if (httpReq.Items.TryGetValue(name, out oValue)) return oValue.ToString();

			return null;
		}

        /// <summary>An IHttpRequest extension method that gets the parent absolute path.</summary>
        ///
        /// <param name="httpReq">The httpReq to act on.</param>
        ///
        /// <returns>The parent absolute path.</returns>
		public static string GetParentAbsolutePath(this IHttpRequest httpReq)
		{
			return httpReq.GetAbsolutePath().ToParentPath();
		}

        /// <summary>An IHttpRequest extension method that gets absolute path.</summary>
        ///
        /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or illegal values.</exception>
        ///
        /// <param name="httpReq">The httpReq to act on.</param>
        ///
        /// <returns>The absolute path.</returns>
		public static string GetAbsolutePath(this IHttpRequest httpReq)
		{
			var resolvedPathInfo = httpReq.PathInfo;

			var pos = httpReq.RawUrl.IndexOf(resolvedPathInfo, StringComparison.InvariantCultureIgnoreCase);
			if (pos == -1)
				throw new ArgumentException(
					String.Format("PathInfo '{0}' is not in Url '{1}'", resolvedPathInfo, httpReq.RawUrl));

			return httpReq.RawUrl.Substring(0, pos + resolvedPathInfo.Length);
		}

        /// <summary>An IHttpRequest extension method that gets the parent path URL.</summary>
        ///
        /// <param name="httpReq">The httpReq to act on.</param>
        ///
        /// <returns>The parent path URL.</returns>
		public static string GetParentPathUrl(this IHttpRequest httpReq)
		{
			return httpReq.GetPathUrl().ToParentPath();
		}

        /// <summary>An IHttpRequest extension method that gets path URL.</summary>
        ///
        /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or illegal values.</exception>
        ///
        /// <param name="httpReq">The httpReq to act on.</param>
        ///
        /// <returns>The path URL.</returns>
		public static string GetPathUrl(this IHttpRequest httpReq)
		{
			var resolvedPathInfo = httpReq.PathInfo;

			var pos = resolvedPathInfo == String.Empty
				? httpReq.AbsoluteUri.Length
				: httpReq.AbsoluteUri.IndexOf(resolvedPathInfo, StringComparison.InvariantCultureIgnoreCase);

			if (pos == -1)
				throw new ArgumentException(
					String.Format("PathInfo '{0}' is not in Url '{1}'", resolvedPathInfo, httpReq.RawUrl));

			return httpReq.AbsoluteUri.Substring(0, pos + resolvedPathInfo.Length);
		}

        /// <summary>An IHttpRequest extension method that gets URL host name.</summary>
        ///
        /// <param name="httpReq">The httpReq to act on.</param>
        ///
        /// <returns>The URL host name.</returns>
		public static string GetUrlHostName(this IHttpRequest httpReq)
		{
			var aspNetReq = httpReq as HttpRequestWrapper;
			if (aspNetReq != null)
			{
				return aspNetReq.UrlHostName;
			}
			var uri = httpReq.AbsoluteUri;

			var pos = uri.IndexOf("://") + "://".Length;
			var partialUrl = uri.Substring(pos);
			var endPos = partialUrl.IndexOf('/');
			if (endPos == -1) endPos = partialUrl.Length;
			var hostName = partialUrl.Substring(0, endPos).Split(':')[0];
			return hostName;
		}

        /// <summary>An IHttpRequest extension method that gets physical path.</summary>
        ///
        /// <param name="httpReq">The httpReq to act on.</param>
        ///
        /// <returns>The physical path.</returns>
		public static string GetPhysicalPath(this IHttpRequest httpReq)
		{
		    var aspNetReq = httpReq as HttpRequestWrapper;
			var res = aspNetReq != null 
                ? aspNetReq.Request.PhysicalPath 
                : EndpointHostConfig.Instance.WebHostPhysicalPath.CombineWith(httpReq.PathInfo);

			return res;
		}

        /// <summary>An IHttpRequest extension method that gets application URL.</summary>
        ///
        /// <param name="httpReq">The httpReq to act on.</param>
        ///
        /// <returns>The application URL.</returns>
        public static string GetApplicationUrl(this HttpRequest httpReq)
        {
            var appPath = httpReq.ApplicationPath;
            var baseUrl = httpReq.Url.Scheme + "://" + httpReq.Url.Host;
            if (httpReq.Url.Port != 80) baseUrl += ":" + httpReq.Url.Port;
            var appUrl = baseUrl.CombineWith(appPath);
            return appUrl;
        }

        /// <summary>An IHttpRequest extension method that gets application URL.</summary>
        ///
        /// <param name="httpReq">The httpReq to act on.</param>
        ///
        /// <returns>The application URL.</returns>
        public static string GetApplicationUrl(this IHttpRequest httpReq)
        {
            var url = new Uri(httpReq.AbsoluteUri);
            var baseUrl = url.Scheme + "://" + url.Host;
            if (url.Port != 80) baseUrl += ":" + url.Port;
            var appUrl = baseUrl.CombineWith(EndpointHost.Config.NServiceKitHandlerFactoryPath);
            return appUrl;
        }

        /// <summary>An IHttpRequest extension method that gets HTTP method override.</summary>
        ///
        /// <param name="httpReq">The httpReq to act on.</param>
        ///
        /// <returns>The HTTP method override.</returns>
		public static string GetHttpMethodOverride(this IHttpRequest httpReq)
		{
			var httpMethod = httpReq.HttpMethod;

			if (httpMethod != HttpMethods.Post)
				return httpMethod;			

			var overrideHttpMethod = 
				httpReq.Headers[HttpHeaders.XHttpMethodOverride].ToNullIfEmpty()
				?? httpReq.FormData[HttpHeaders.XHttpMethodOverride].ToNullIfEmpty()
				?? httpReq.QueryString[HttpHeaders.XHttpMethodOverride].ToNullIfEmpty();

			if (overrideHttpMethod != null)
			{
				if (overrideHttpMethod != HttpMethods.Get && overrideHttpMethod != HttpMethods.Post)
					httpMethod = overrideHttpMethod;
			}

			return httpMethod;
		}

        /// <summary>An IHttpRequest extension method that gets format modifier.</summary>
        ///
        /// <param name="httpReq">The httpReq to act on.</param>
        ///
        /// <returns>The format modifier.</returns>
		public static string GetFormatModifier(this IHttpRequest httpReq)
		{
			var format = httpReq.QueryString["format"];
			if (format == null) return null;
			var parts = format.SplitOnFirst('.');
			return parts.Length > 1 ? parts[1] : null;
		}

        /// <summary>An IHttpRequest extension method that query if 'httpReq' has not modified since.</summary>
        ///
        /// <param name="httpReq"> The httpReq to act on.</param>
        /// <param name="dateTime">The date time.</param>
        ///
        /// <returns>true if not modified since, false if not.</returns>
		public static bool HasNotModifiedSince(this IHttpRequest httpReq, DateTime? dateTime)
		{
			if (!dateTime.HasValue) return false;
			var strHeader = httpReq.Headers[HttpHeaders.IfModifiedSince];
			try
			{
				if (strHeader != null)
				{
					var dateIfModifiedSince = DateTime.ParseExact(strHeader, "r", null);
					var utcFromDate = dateTime.Value.ToUniversalTime();
					//strip ms
					utcFromDate = new DateTime(
						utcFromDate.Ticks - (utcFromDate.Ticks % TimeSpan.TicksPerSecond),
						utcFromDate.Kind
					);

					return utcFromDate <= dateIfModifiedSince;
				}
				return false;
			}
			catch
			{
				return false;
			}
		}

        /// <summary>An IHttpRequest extension method that did return 304 not modified.</summary>
        ///
        /// <param name="httpReq"> The httpReq to act on.</param>
        /// <param name="dateTime">The date time.</param>
        /// <param name="httpRes"> The HTTP resource.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		public static bool DidReturn304NotModified(this IHttpRequest httpReq, DateTime? dateTime, IHttpResponse httpRes)
		{
			if (httpReq.HasNotModifiedSince(dateTime))
			{
				httpRes.StatusCode = (int) HttpStatusCode.NotModified;
				return true;
			}
			return false;
		}

        /// <summary>An IHttpRequest extension method that callback, called when the get jsonp.</summary>
        ///
        /// <param name="httpReq">The httpReq to act on.</param>
        ///
        /// <returns>The jsonp callback.</returns>
		public static string GetJsonpCallback(this IHttpRequest httpReq)
		{
			return httpReq == null ? null : httpReq.QueryString["callback"];
		}

        /// <summary>An IHttpRequest extension method that cookies as dictionary.</summary>
        ///
        /// <param name="httpReq">The httpReq to act on.</param>
        ///
        /// <returns>A Dictionary&lt;string,string&gt;</returns>
		public static Dictionary<string, string> CookiesAsDictionary(this IHttpRequest httpReq)
		{
			var map = new Dictionary<string, string>();
			var aspNet = httpReq.OriginalRequest as HttpRequest;
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
				var httpListener = httpReq.OriginalRequest as HttpListenerRequest;
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

        /// <summary>An Exception extension method that converts an ex to the status code.</summary>
        ///
        /// <param name="ex">The ex to act on.</param>
        ///
        /// <returns>ex as an int.</returns>
        public static int ToStatusCode(this Exception ex)
        {
            int errorStatus;
            if (EndpointHost.Config != null && EndpointHost.Config.MapExceptionToStatusCode.TryGetValue(ex.GetType(), out errorStatus))
            {
                return errorStatus;
            }

            if (ex is HttpError) return ((HttpError)ex).Status;
            if (ex is NotImplementedException || ex is NotSupportedException) return (int)HttpStatusCode.MethodNotAllowed;
            if (ex is ArgumentException || ex is SerializationException) return (int)HttpStatusCode.BadRequest;
            if (ex is UnauthorizedAccessException) return (int) HttpStatusCode.Forbidden;
            return (int)HttpStatusCode.InternalServerError;
	    }

        /// <summary>An Exception extension method that converts an ex to an error code.</summary>
        ///
        /// <param name="ex">The ex to act on.</param>
        ///
        /// <returns>ex as a string.</returns>
        public static string ToErrorCode(this Exception ex)
        {
            if (ex is HttpError) return ((HttpError)ex).ErrorCode;
            return ex.GetType().Name;
        }
	}
}