using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web;
using NServiceKit.Common;
using NServiceKit.Common.Extensions;
using NServiceKit.Common.Utils;
using NServiceKit.Common.Web;
using NServiceKit.Logging;
using NServiceKit.ServiceHost;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints.Support;

namespace NServiceKit.WebHost.Endpoints.Extensions
{
	/**
	 * 
		 Input: http://localhost:96/Cambia3/Temp/Test.aspx/path/info?q=item#fragment

		Some HttpRequest path and URL properties:
		Request.ApplicationPath:	/Cambia3
		Request.CurrentExecutionFilePath:	/Cambia3/Temp/Test.aspx
		Request.FilePath:			/Cambia3/Temp/Test.aspx
		Request.Path:				/Cambia3/Temp/Test.aspx/path/info
		Request.PathInfo:			/path/info
		Request.PhysicalApplicationPath:	D:\Inetpub\wwwroot\CambiaWeb\Cambia3\
		Request.QueryString:		/Cambia3/Temp/Test.aspx/path/info?query=arg
		Request.Url.AbsolutePath:	/Cambia3/Temp/Test.aspx/path/info
		Request.Url.AbsoluteUri:	http://localhost:96/Cambia3/Temp/Test.aspx/path/info?query=arg
		Request.Url.Fragment:	
		Request.Url.Host:			localhost
		Request.Url.LocalPath:		/Cambia3/Temp/Test.aspx/path/info
		Request.Url.PathAndQuery:	/Cambia3/Temp/Test.aspx/path/info?query=arg
		Request.Url.Port:			96
		Request.Url.Query:			?query=arg
		Request.Url.Scheme:			http
		Request.Url.Segments:		/
									Cambia3/
									Temp/
									Test.aspx/
									path/
									info
	 * */
    /// <summary>A HTTP request extensions.</summary>
	public static class HttpRequestExtensions
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(HttpRequestExtensions));

		private static string WebHostDirectoryName = "";

		static HttpRequestExtensions()
		{
			WebHostDirectoryName = Path.GetFileName("~".MapHostAbsolutePath());
		}

        /// <summary>A HttpListenerRequest extension method that gets operation name.</summary>
        ///
        /// <param name="request">The request to act on.</param>
        ///
        /// <returns>The operation name.</returns>
		public static string GetOperationName(this HttpRequest request)
		{
			var pathInfo = request.GetLastPathInfo();
			return GetOperationNameFromLastPathInfo(pathInfo);
		}

        /// <summary>Gets operation name from last path information.</summary>
        ///
        /// <param name="lastPathInfo">Information describing the last path.</param>
        ///
        /// <returns>The operation name from last path information.</returns>
		public static string GetOperationNameFromLastPathInfo(string lastPathInfo)
		{
			if (String.IsNullOrEmpty(lastPathInfo)) return null;

			var operationName = lastPathInfo.Substring("/".Length);

			return operationName;
		}

		private static string GetLastPathInfoFromRawUrl(string rawUrl)
		{
			var pathInfo = rawUrl.IndexOf("?") != -1
				? rawUrl.Substring(0, rawUrl.IndexOf("?"))
				: rawUrl;

			pathInfo = pathInfo.Substring(pathInfo.LastIndexOf("/"));

			return pathInfo;
		}

        /// <summary>A HttpListenerRequest extension method that gets the last path information.</summary>
        ///
        /// <param name="request">The request to act on.</param>
        ///
        /// <returns>The last path information.</returns>
		public static string GetLastPathInfo(this HttpRequest request)
		{
			var pathInfo = request.PathInfo;
			if (String.IsNullOrEmpty(pathInfo))
			{
				pathInfo = GetLastPathInfoFromRawUrl(request.RawUrl);
			}

			//Log.DebugFormat("Request.PathInfo: {0}, Request.RawUrl: {1}, pathInfo:{2}",
			//    request.PathInfo, request.RawUrl, pathInfo);

			return pathInfo;
		}

        /// <summary>A HttpRequest extension method that gets URL host name.</summary>
        ///
        /// <param name="request">The request to act on.</param>
        ///
        /// <returns>The URL host name.</returns>
		public static string GetUrlHostName(this HttpRequest request)
		{
			//TODO: Fix bug in mono fastcgi, when trying to get 'Request.Url.Host'
			try
			{
				return request.Url.Host;
			}
			catch (Exception ex)
			{
				Log.ErrorFormat("Error trying to get 'Request.Url.Host'", ex);

				return request.UserHostName;
			}
		}

        /// <summary>
        /// http://localhost/NServiceKit.Examples.Host.Web/Public/Public/Soap12/Wsdl =>
        /// http://localhost/NServiceKit.Examples.Host.Web/Public/Soap12/.
        /// </summary>
        ///
        /// <param name="request">The request to act on.</param>
        ///
        /// <returns>The parent base URL.</returns>
		public static string GetParentBaseUrl(this HttpRequest request)
		{
			var rawUrl = request.RawUrl; // /Cambia3/Temp/Test.aspx/path/info
			var endpointsPath = rawUrl.Substring(0, rawUrl.LastIndexOf('/') + 1);  // /Cambia3/Temp/Test.aspx/path
			return GetAuthority(request) + endpointsPath;
		}

        /// <summary>An IHttpRequest extension method that gets the parent base URL.</summary>
        ///
        /// <param name="request">The request to act on.</param>
        ///
        /// <returns>The parent base URL.</returns>
        public static string GetParentBaseUrl(this IHttpRequest request){
            var rawUrl = request.RawUrl;
            var endpointsPath = rawUrl.Substring(0, rawUrl.LastIndexOf('/') + 1);
            return new Uri(request.AbsoluteUri).GetLeftPart(UriPartial.Authority) + endpointsPath;
        }

        /// <summary>An IHttpRequest extension method that gets base URL.</summary>
        ///
        /// <param name="request">The request to act on.</param>
        ///
        /// <returns>The base URL.</returns>
		public static string GetBaseUrl(this HttpRequest request)
		{
			return GetAuthority(request) + request.RawUrl;
		}

		//=> http://localhost:96 ?? ex=> http://localhost
		private static string GetAuthority(HttpRequest request)
		{
			try
			{
				return request.Url.GetLeftPart(UriPartial.Authority);
			}
			catch (Exception ex)
			{
				Log.Error("Error trying to get: request.Url.GetLeftPart(UriPartial.Authority): " + ex.Message, ex);
				return "http://" + request.UserHostName;
			}
		}

        /// <summary>A HttpListenerRequest extension method that gets operation name.</summary>
        ///
        /// <param name="request">The request to act on.</param>
        ///
        /// <returns>The operation name.</returns>
		public static string GetOperationName(this HttpListenerRequest request)
		{
			return request.Url.Segments[request.Url.Segments.Length - 1];
		}

        /// <summary>A HttpListenerRequest extension method that gets the last path information.</summary>
        ///
        /// <param name="request">The request to act on.</param>
        ///
        /// <returns>The last path information.</returns>
		public static string GetLastPathInfo(this HttpListenerRequest request)
		{
			return GetLastPathInfoFromRawUrl(request.RawUrl);
		}

        /// <summary>Gets path information.</summary>
        ///
        /// <param name="request">The request to act on.</param>
        ///
        /// <returns>The path information.</returns>
		public static string GetPathInfo(this HttpRequest request)
		{
			if (!String.IsNullOrEmpty(request.PathInfo)) return request.PathInfo.TrimEnd('/');

			var mode = EndpointHost.Config.NServiceKitHandlerFactoryPath;
			var appPath = String.IsNullOrEmpty(request.ApplicationPath)
			              ? WebHostDirectoryName
			              : request.ApplicationPath.TrimStart('/');

			//mod_mono: /CustomPath35/api//default.htm
			var path = Env.IsMono ? request.Path.Replace("//", "/") : request.Path;
			return GetPathInfo(path, mode, appPath);
		}

        /// <summary>Gets path information.</summary>
        ///
        /// <param name="fullPath">Full pathname of the full file.</param>
        /// <param name="mode">    The mode.</param>
        /// <param name="appPath"> Full pathname of the application file.</param>
        ///
        /// <returns>The path information.</returns>
		public static string GetPathInfo(string fullPath, string mode, string appPath)
		{
			var pathInfo = ResolvePathInfoFromMappedPath(fullPath, mode);
			if (!String.IsNullOrEmpty(pathInfo)) return pathInfo;
			
			//Wildcard mode relies on this to work out the handlerPath
			pathInfo = ResolvePathInfoFromMappedPath(fullPath, appPath);
			if (!String.IsNullOrEmpty(pathInfo)) return pathInfo;
			
			return fullPath;
		}

        /// <summary>Resolve path information from mapped path.</summary>
        ///
        /// <param name="fullPath">      Full pathname of the full file.</param>
        /// <param name="mappedPathRoot">The mapped path root.</param>
        ///
        /// <returns>A string.</returns>
		public static string ResolvePathInfoFromMappedPath(string fullPath, string mappedPathRoot)
		{
            if (mappedPathRoot == null) return null;

            var sbPathInfo = new StringBuilder();
            var fullPathParts = fullPath.Split('/');
		    var mappedPathRootParts = mappedPathRoot.Split('/');
		    var fullPathIndexOffset = mappedPathRootParts.Length - 1;
            var pathRootFound = false;

            for (var fullPathIndex = 0; fullPathIndex < fullPathParts.Length; fullPathIndex++) {
                if (pathRootFound) {
                    sbPathInfo.Append("/" + fullPathParts[fullPathIndex]);
                } else if (fullPathIndex - fullPathIndexOffset >= 0) {
                    pathRootFound = true;
                    for (var mappedPathRootIndex = 0; mappedPathRootIndex < mappedPathRootParts.Length; mappedPathRootIndex++) {
                        if (!String.Equals(fullPathParts[fullPathIndex - fullPathIndexOffset + mappedPathRootIndex], mappedPathRootParts[mappedPathRootIndex], StringComparison.InvariantCultureIgnoreCase)) {
                            pathRootFound = false;
                            break;
                        }
                    }
                }
            }
            if (!pathRootFound) return null;

            var path = sbPathInfo.ToString();
            return path.Length > 1 ? path.TrimEnd('/') : "/";
		}

        /// <summary>An IHttpRequest extension method that query if 'request' is content type.</summary>
        ///
        /// <param name="request">    The request to act on.</param>
        /// <param name="contentType">Type of the content.</param>
        ///
        /// <returns>true if content type, false if not.</returns>
		public static bool IsContentType(this IHttpRequest request, string contentType)
		{
			return request.ContentType.StartsWith(contentType, StringComparison.InvariantCultureIgnoreCase);
		}

        /// <summary>An IHttpRequest extension method that query if 'request' has any of content types.</summary>
        ///
        /// <param name="request">     The request to act on.</param>
        /// <param name="contentTypes">List of types of the contents.</param>
        ///
        /// <returns>true if any of content types, false if not.</returns>
		public static bool HasAnyOfContentTypes(this IHttpRequest request, params string[] contentTypes)
		{
			if (contentTypes == null || request.ContentType == null) return false;
			foreach (var contentType in contentTypes)
			{
				if (IsContentType(request, contentType)) return true;
			}
			return false;
		}

        /// <summary>A HttpListenerRequest extension method that gets HTTP request.</summary>
        ///
        /// <param name="request">The request to act on.</param>
        ///
        /// <returns>The HTTP request.</returns>
		public static IHttpRequest GetHttpRequest(this HttpRequest request)
		{
			return new HttpRequestWrapper(null, request);
		}

        /// <summary>A HttpListenerRequest extension method that gets HTTP request.</summary>
        ///
        /// <param name="request">The request to act on.</param>
        ///
        /// <returns>The HTTP request.</returns>
		public static IHttpRequest GetHttpRequest(this HttpListenerRequest request)
		{
			return new HttpListenerRequestWrapper(null, request);
		}

        /// <summary>An IHttpRequest extension method that gets request parameters.</summary>
        ///
        /// <param name="request">The request to act on.</param>
        ///
        /// <returns>The request parameters.</returns>
		public static Dictionary<string, string> GetRequestParams(this IHttpRequest request)
		{
			var map = new Dictionary<string, string>();

			foreach (var name in request.QueryString.AllKeys)
			{
				if (name == null) continue; //thank you ASP.NET
				map[name] = request.QueryString[name];
			}

			if ((request.HttpMethod == HttpMethods.Post || request.HttpMethod == HttpMethods.Put)
				&& request.FormData != null)
			{
				foreach (var name in request.FormData.AllKeys)
				{
					if (name == null) continue; //thank you ASP.NET
					map[name] = request.FormData[name];
				}
			}

			return map;
		}

        /// <summary>An IHttpRequest extension method that gets query string content type.</summary>
        ///
        /// <param name="httpReq">The httpReq to act on.</param>
        ///
        /// <returns>The query string content type.</returns>
		public static string GetQueryStringContentType(this IHttpRequest httpReq)
		{
			var callback = httpReq.QueryString["callback"];
			if (!String.IsNullOrEmpty(callback)) return ContentType.Json;

			var format = httpReq.QueryString["format"];
			if (format == null)
			{
				const int formatMaxLength = 4;
				var pi = httpReq.PathInfo;
				if (pi == null || pi.Length <= formatMaxLength) return null;
				if (pi[0] == '/') pi = pi.Substring(1);
				format = pi.SplitOnFirst('/')[0];
				if (format.Length > formatMaxLength) return null;
			}

			format = format.SplitOnFirst('.')[0].ToLower();
			if (format.Contains("json")) return ContentType.Json;
			if (format.Contains("xml")) return ContentType.Xml;
			if (format.Contains("jsv")) return ContentType.Jsv;

			string contentType;
			EndpointHost.ContentTypeFilter.ContentTypeFormats.TryGetValue(format, out contentType);

			return contentType;
		}

        /// <summary>List of types of the preferred contents.</summary>
		public static string[] PreferredContentTypes = new[] {
			ContentType.Html, ContentType.Json, ContentType.Xml, ContentType.Jsv
		};

		/// <summary>
		/// Use this to treat Request.Items[] as a cache by returning pre-computed items to save 
		/// calculating them multiple times.
		/// </summary>
		public static object ResolveItem(this IHttpRequest httpReq,
			string itemKey, Func<IHttpRequest, object> resolveFn)
		{
			object cachedItem;
			if (httpReq.Items.TryGetValue(itemKey, out cachedItem))
				return cachedItem;

			var item = resolveFn(httpReq);
			httpReq.Items[itemKey] = item;

			return item;
		}

        /// <summary>An IHttpRequest extension method that gets response content type.</summary>
        ///
        /// <param name="httpReq">The httpReq to act on.</param>
        ///
        /// <returns>The response content type.</returns>
		public static string GetResponseContentType(this IHttpRequest httpReq)
		{
			var specifiedContentType = GetQueryStringContentType(httpReq);
			if (!String.IsNullOrEmpty(specifiedContentType)) return specifiedContentType;

			var acceptContentTypes = httpReq.AcceptTypes;
			var defaultContentType = httpReq.ContentType;
			if (httpReq.HasAnyOfContentTypes(ContentType.FormUrlEncoded, ContentType.MultiPartFormData))
			{
				defaultContentType = EndpointHost.Config.DefaultContentType;
			}

			var customContentTypes = EndpointHost.ContentTypeFilter.ContentTypeFormats.Values;

			var acceptsAnything = false;
			var hasDefaultContentType = !String.IsNullOrEmpty(defaultContentType);
			if (acceptContentTypes != null)
			{
				var hasPreferredContentTypes = new bool[PreferredContentTypes.Length];
				foreach (var contentType in acceptContentTypes)
				{
					acceptsAnything = acceptsAnything || contentType == "*/*";

					for (var i = 0; i < PreferredContentTypes.Length; i++)
					{
						if (hasPreferredContentTypes[i]) continue;
						var preferredContentType = PreferredContentTypes[i];
						hasPreferredContentTypes[i] = contentType.StartsWith(preferredContentType);

						//Prefer Request.ContentType if it is also a preferredContentType
						if (hasPreferredContentTypes[i] && preferredContentType == defaultContentType)
							return preferredContentType;
					}
				}
				for (var i = 0; i < PreferredContentTypes.Length; i++)
				{
					if (hasPreferredContentTypes[i]) return PreferredContentTypes[i];
				}
				if (acceptsAnything && hasDefaultContentType) return defaultContentType;

				foreach (var contentType in acceptContentTypes)
				{
					foreach (var customContentType in customContentTypes)
					{
						if (contentType.StartsWith(customContentType)) return customContentType;
					}
				}
			}

			//We could also send a '406 Not Acceptable', but this is allowed also
			return EndpointHost.Config.DefaultContentType;
		}

        /// <summary>An IHttpRequest extension method that sets a view.</summary>
        ///
        /// <param name="httpReq"> The httpReq to act on.</param>
        /// <param name="viewName">Name of the view.</param>
        public static void SetView(this IHttpRequest httpReq, string viewName)
        {
            httpReq.SetItem("View", viewName);
        }

        /// <summary>An IHttpRequest extension method that gets a view.</summary>
        ///
        /// <param name="httpReq">The httpReq to act on.</param>
        ///
        /// <returns>The view.</returns>
        public static string GetView(this IHttpRequest httpReq)
        {
            return httpReq.GetItem("View") as string;
        }

        /// <summary>An IHttpRequest extension method that sets a template.</summary>
        ///
        /// <param name="httpReq">     The httpReq to act on.</param>
        /// <param name="templateName">Name of the template.</param>
        public static void SetTemplate(this IHttpRequest httpReq, string templateName)
        {
            httpReq.SetItem("Template", templateName);
        }

        /// <summary>An IHttpRequest extension method that gets a template.</summary>
        ///
        /// <param name="httpReq">The httpReq to act on.</param>
        ///
        /// <returns>The template.</returns>
        public static string GetTemplate(this IHttpRequest httpReq)
        {
            return httpReq.GetItem("Template") as string;
        }

        /// <summary>An IHttpRequest extension method that resolve absolute URL.</summary>
        ///
        /// <param name="httpReq">The httpReq to act on.</param>
        /// <param name="url">    URL of the document.</param>
        ///
        /// <returns>A string.</returns>
        public static string ResolveAbsoluteUrl(this IHttpRequest httpReq, string url)
        {
            return EndpointHost.AppHost.ResolveAbsoluteUrl(url, httpReq);
        }

        /// <summary>An IHttpRequest extension method that resolve base URL.</summary>
        ///
        /// <param name="httpReq">The httpReq to act on.</param>
        ///
        /// <returns>A string.</returns>
        public static string ResolveBaseUrl(this IHttpRequest httpReq)
        {
            return EndpointHost.AppHost.ResolveAbsoluteUrl("~/", httpReq);
        }

        /// <summary>An IHttpRequest extension method that gets absolute URL.</summary>
        ///
        /// <param name="httpReq">The httpReq to act on.</param>
        /// <param name="url">    URL of the document.</param>
        ///
        /// <returns>The absolute URL.</returns>
        public static string GetAbsoluteUrl(this IHttpRequest httpReq, string url)
        {
            if (url.SafeSubstring(0, 2) == "~/")
            {
                url = httpReq.GetBaseUrl().CombineWith(url.Substring(2));
            }
            return url;
        }

        /// <summary>An IHttpRequest extension method that gets base URL.</summary>
        ///
        /// <param name="httpReq">The httpReq to act on.</param>
        ///
        /// <returns>The base URL.</returns>
        public static string GetBaseUrl(this IHttpRequest httpReq)
        {
            var baseUrl = NServiceKitHttpHandlerFactory.GetBaseUrl();
            if (baseUrl != null) return baseUrl;

            var handlerPath = EndpointHost.Config.NServiceKitHandlerFactoryPath;
            if (handlerPath != null)
            {
                var pos = httpReq.AbsoluteUri.IndexOf(handlerPath, StringComparison.InvariantCultureIgnoreCase);
                if (pos >= 0)
                {
                    baseUrl = httpReq.AbsoluteUri.Substring(0, pos + handlerPath.Length);
                    return baseUrl;
                }
                return "/" + handlerPath;
            }

            return "/"; //Can't infer Absolute Uri, fallback to root relative path
        }

        /// <summary>Converts the attrNames to an endpoint attributes.</summary>
        ///
        /// <param name="attrNames">List of names of the attributes.</param>
        ///
        /// <returns>attrNames as the EndpointAttributes.</returns>
        public static EndpointAttributes ToEndpointAttributes(string[] attrNames)
        {
            var attrs = EndpointAttributes.None;
            foreach (var simulatedAttr in attrNames)
            {
                var attr = (EndpointAttributes)Enum.Parse(typeof(EndpointAttributes), simulatedAttr, true);
                attrs |= attr;
            }
            return attrs;
        }

        /// <summary>Gets the attributes.</summary>
        ///
        /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or illegal values.</exception>
        ///
        /// <param name="request">The request to act on.</param>
        ///
        /// <returns>The attributes.</returns>
	    public static EndpointAttributes GetAttributes(this IHttpRequest request)
	    {
            if (EndpointHost.DebugMode 
                && request.QueryString != null) //Mock<IHttpRequest>
	        {
                var simulate = request.QueryString["simulate"];
                if (simulate != null)
                {
                    return ToEndpointAttributes(simulate.Split(','));
                }
	        }

	        var portRestrictions = EndpointAttributes.None;

	        portRestrictions |= HttpMethods.GetEndpointAttribute(request.HttpMethod);
	        portRestrictions |= request.IsSecureConnection ? EndpointAttributes.Secure : EndpointAttributes.InSecure;

	        if (request.UserHostAddress != null)
	        {
	            var isIpv4Address = request.UserHostAddress.IndexOf('.') != -1 
                    && request.UserHostAddress.IndexOf("::", StringComparison.InvariantCulture) == -1;

                string ipAddressNumber = null;
                if (isIpv4Address)
                {
                    ipAddressNumber = request.UserHostAddress.SplitOnFirst(":")[0];
                }
                else
                {
                    if (request.UserHostAddress.Contains("]:"))
                    {
                        ipAddressNumber = request.UserHostAddress.SplitOnLast(":")[0];
                    }
                    else
                    {
                        ipAddressNumber = request.UserHostAddress.LastIndexOf("%", StringComparison.InvariantCulture) > 0 ?
                            request.UserHostAddress.SplitOnLast(":")[0] :
                            request.UserHostAddress;
                    }
                }

	            try
	            {
	                ipAddressNumber = ipAddressNumber.SplitOnFirst(',')[0];
	                var ipAddress = ipAddressNumber.StartsWith("::1")
                        ? IPAddress.IPv6Loopback
                        : IPAddress.Parse(ipAddressNumber);
	                portRestrictions |= GetAttributes(ipAddress);
	            }
	            catch (Exception ex)
	            {
	                throw new ArgumentException("Could not parse Ipv{0} Address: {1} / {2}"
                        .Fmt((isIpv4Address ? 4 : 6), request.UserHostAddress, ipAddressNumber), ex);
	            }
	        }

	        return portRestrictions;
	    }

        /// <summary>Gets the attributes.</summary>
        ///
        /// <param name="ipAddress">The IP address.</param>
        ///
        /// <returns>The attributes.</returns>
	    public static EndpointAttributes GetAttributes(IPAddress ipAddress)
	    {
	        if (IPAddress.IsLoopback(ipAddress))
	            return EndpointAttributes.Localhost;

	        return IsInLocalSubnet(ipAddress)
	               ? EndpointAttributes.LocalSubnet
	               : EndpointAttributes.External;
	    }

        /// <summary>Query if 'ipAddress' is in local subnet.</summary>
        ///
        /// <param name="ipAddress">The IP address.</param>
        ///
        /// <returns>true if in local subnet, false if not.</returns>
	    public static bool IsInLocalSubnet(IPAddress ipAddress)
	    {
	        var ipAddressBytes = ipAddress.GetAddressBytes();
	        switch (ipAddress.AddressFamily)
	        {
	            case AddressFamily.InterNetwork:
	                foreach (var localIpv4AddressAndMask in EndpointHandlerBase.NetworkInterfaceIpv4Addresses)
	                {
	                    if (ipAddressBytes.IsInSameIpv4Subnet(localIpv4AddressAndMask.Key, localIpv4AddressAndMask.Value))
	                    {
	                        return true;
	                    }
	                }
	                break;

	            case AddressFamily.InterNetworkV6:
	                foreach (var localIpv6Address in EndpointHandlerBase.NetworkInterfaceIpv6Addresses)
	                {
	                    if (ipAddressBytes.IsInSameIpv6Subnet(localIpv6Address))
	                    {
	                        return true;
	                    }
	                }
	                break;
	        }

	        return false;
	    }

        /// <summary>A HttpListenerRequest extension method that converts this object to a request.</summary>
        ///
        /// <param name="aspnetHttpReq">The aspnetHttpReq to act on.</param>
        /// <param name="operationName">Name of the operation.</param>
        ///
        /// <returns>The given data converted to an IHttpRequest.</returns>
	    public static IHttpRequest ToRequest(this HttpRequest aspnetHttpReq, string operationName=null)
	    {
	        return new HttpRequestWrapper(aspnetHttpReq) {
                OperationName = operationName,
                Container = AppHostBase.Instance != null ? AppHostBase.Instance.Container : null
            };
	    }

        /// <summary>A HttpListenerRequest extension method that converts this object to a request.</summary>
        ///
        /// <param name="listenerHttpReq">The listenerHttpReq to act on.</param>
        /// <param name="operationName">  Name of the operation.</param>
        ///
        /// <returns>The given data converted to an IHttpRequest.</returns>
        public static IHttpRequest ToRequest(this HttpListenerRequest listenerHttpReq, string operationName = null)
	    {
	        return new HttpListenerRequestWrapper(listenerHttpReq) {
                OperationName = operationName,
                Container = AppHostBase.Instance != null ? AppHostBase.Instance.Container : null
            };
	    }

        /// <summary>A HttpListenerResponse extension method that converts the listenerHttpRes to a response.</summary>
        ///
        /// <param name="aspnetHttpRes">The aspnetHttpRes to act on.</param>
        ///
        /// <returns>listenerHttpRes as an IHttpResponse.</returns>
	    public static IHttpResponse ToResponse(this HttpResponse aspnetHttpRes)
	    {
	        return new HttpResponseWrapper(aspnetHttpRes);
	    }

        /// <summary>A HttpListenerResponse extension method that converts the listenerHttpRes to a response.</summary>
        ///
        /// <param name="listenerHttpRes">The listenerHttpRes to act on.</param>
        ///
        /// <returns>listenerHttpRes as an IHttpResponse.</returns>
	    public static IHttpResponse ToResponse(this HttpListenerResponse listenerHttpRes)
	    {
	        return new HttpListenerResponseWrapper(listenerHttpRes);
	    }

        /// <summary>An IHttpRequest extension method that sets operation name.</summary>
        ///
        /// <param name="httpReq">      The httpReq to act on.</param>
        /// <param name="operationName">Name of the operation.</param>
        public static void SetOperationName(this IHttpRequest httpReq, string operationName)
        {
            if (httpReq.OperationName == null)
            {
                var aspReq = httpReq as HttpRequestWrapper;
                if (aspReq != null)
                {
                    aspReq.OperationName = operationName;
                    return;
                }

                var listenerReq = httpReq as HttpListenerRequestWrapper;
                if (listenerReq != null)
                {
                    listenerReq.OperationName = operationName;
                }
            }
        }

        /// <summary>An IHttpRequest extension method that gets SOAP message.</summary>
        ///
        /// <param name="httpReq">The httpReq to act on.</param>
        ///
        /// <returns>The SOAP message.</returns>
        public static System.ServiceModel.Channels.Message GetSoapMessage(this IHttpRequest httpReq)
        {
            return httpReq.Items["SoapMessage"] as System.ServiceModel.Channels.Message;
        }
    }
    
}