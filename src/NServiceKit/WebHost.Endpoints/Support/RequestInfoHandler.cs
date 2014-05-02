using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Web;
using NServiceKit.Common.Web;
using NServiceKit.ServiceHost;
using NServiceKit.Text;
using HttpRequestWrapper = NServiceKit.WebHost.Endpoints.Extensions.HttpRequestWrapper;
using HttpResponseWrapper = NServiceKit.WebHost.Endpoints.Extensions.HttpResponseWrapper;

namespace NServiceKit.WebHost.Endpoints.Support
{
    /// <summary>Information about the request.</summary>
	[DataContract]
	public class RequestInfo { }

    /// <summary>A request information response.</summary>
	[DataContract]
	public class RequestInfoResponse
	{
        /// <summary>Gets or sets the host.</summary>
        ///
        /// <value>The host.</value>
		[DataMember]
		public string Host { get; set; }

        /// <summary>Gets or sets the Date/Time of the date.</summary>
        ///
        /// <value>The date.</value>
		[DataMember]
		public DateTime Date { get; set; }

        /// <summary>Gets or sets the name of the service.</summary>
        ///
        /// <value>The name of the service.</value>
		[DataMember]
		public string ServiceName { get; set; }

        /// <summary>Gets or sets the full pathname of the handler file.</summary>
        ///
        /// <value>The full pathname of the handler file.</value>
		[DataMember]
		public string HandlerPath { get; set; }

        /// <summary>Gets or sets the user host address.</summary>
        ///
        /// <value>The user host address.</value>
		[DataMember]
		public string UserHostAddress { get; set; }

        /// <summary>Gets or sets the HTTP method.</summary>
        ///
        /// <value>The HTTP method.</value>
		[DataMember]
		public string HttpMethod { get; set; }

        /// <summary>Gets or sets information describing the path.</summary>
        ///
        /// <value>Information describing the path.</value>
		[DataMember]
		public string PathInfo { get; set; }

        /// <summary>Gets or sets information describing the resolved path.</summary>
        ///
        /// <value>Information describing the resolved path.</value>
		[DataMember]
		public string ResolvedPathInfo { get; set; }

        /// <summary>Gets or sets the full pathname of the file.</summary>
        ///
        /// <value>The full pathname of the file.</value>
		[DataMember]
		public string Path { get; set; }

        /// <summary>Gets or sets URI of the absolute.</summary>
        ///
        /// <value>The absolute URI.</value>
		[DataMember]
		public string AbsoluteUri { get; set; }

        /// <summary>Gets or sets the full pathname of the application file.</summary>
        ///
        /// <value>The full pathname of the application file.</value>
		[DataMember]
		public string ApplicationPath { get; set; }

        /// <summary>Gets or sets the handler factory arguments.</summary>
        ///
        /// <value>The handler factory arguments.</value>
		[DataMember]
		public string HandlerFactoryArgs { get; set; }

        /// <summary>Gets or sets URL of the raw.</summary>
        ///
        /// <value>The raw URL.</value>
		[DataMember]
		public string RawUrl { get; set; }

        /// <summary>Gets or sets URL of the document.</summary>
        ///
        /// <value>The URL.</value>
		[DataMember]
		public string Url { get; set; }

        /// <summary>Gets or sets the type of the content.</summary>
        ///
        /// <value>The type of the content.</value>
		[DataMember]
		public string ContentType { get; set; }

        /// <summary>Gets or sets the status.</summary>
        ///
        /// <value>The status.</value>
		[DataMember]
		public int Status { get; set; }

        /// <summary>Gets or sets the length of the content.</summary>
        ///
        /// <value>The length of the content.</value>
		[DataMember]
		public long ContentLength { get; set; }

        /// <summary>Gets or sets the headers.</summary>
        ///
        /// <value>The headers.</value>
		[DataMember]
		public Dictionary<string, string> Headers { get; set; }

        /// <summary>Gets or sets the query string.</summary>
        ///
        /// <value>The query string.</value>
		[DataMember]
		public Dictionary<string, string> QueryString { get; set; }

        /// <summary>Gets or sets information describing the form.</summary>
        ///
        /// <value>Information describing the form.</value>
		[DataMember]
		public Dictionary<string, string> FormData { get; set; }

        /// <summary>Gets or sets a list of types of the accepts.</summary>
        ///
        /// <value>A list of types of the accepts.</value>
		[DataMember]
		public List<string> AcceptTypes { get; set; }

        /// <summary>Gets or sets the name of the operation.</summary>
        ///
        /// <value>The name of the operation.</value>
		[DataMember]
		public string OperationName { get; set; }

        /// <summary>Gets or sets the type of the response content.</summary>
        ///
        /// <value>The type of the response content.</value>
		[DataMember]
		public string ResponseContentType { get; set; }

        /// <summary>Gets or sets the error code.</summary>
        ///
        /// <value>The error code.</value>
		[DataMember]
		public string ErrorCode { get; set; }

        /// <summary>Gets or sets a message describing the error.</summary>
        ///
        /// <value>A message describing the error.</value>
		[DataMember]
		public string ErrorMessage { get; set; }

        /// <summary>Gets or sets the debug string.</summary>
        ///
        /// <value>The debug string.</value>
        [DataMember]
        public string DebugString { get; set; }

        /// <summary>Gets or sets a list of names of the operations.</summary>
        ///
        /// <value>A list of names of the operations.</value>
        [DataMember]
        public List<string> OperationNames { get; set; }

        /// <summary>Gets or sets a list of names of all operations.</summary>
        ///
        /// <value>A list of names of all operations.</value>
        [DataMember]
        public List<string> AllOperationNames { get; set; }

        /// <summary>Gets or sets the request response map.</summary>
        ///
        /// <value>The request response map.</value>
        [DataMember]
        public Dictionary<string, string> RequestResponseMap { get; set; }
    }

    /// <summary>A request information handler.</summary>
	public class RequestInfoHandler
		: INServiceKitHttpHandler, IHttpHandler
	{
        /// <summary>Full pathname of the rest file.</summary>
		public const string RestPath = "_requestinfo";

        /// <summary>Gets or sets information describing the request.</summary>
        ///
        /// <value>Information describing the request.</value>
		public RequestInfoResponse RequestInfo { get; set; }

        /// <summary>Process the request.</summary>
        ///
        /// <param name="httpReq">      The HTTP request.</param>
        /// <param name="httpRes">      The HTTP resource.</param>
        /// <param name="operationName">Name of the operation.</param>
		public void ProcessRequest(IHttpRequest httpReq, IHttpResponse httpRes, string operationName)
		{
			var response = this.RequestInfo ?? GetRequestInfo(httpReq);
			response.HandlerFactoryArgs = NServiceKitHttpHandlerFactory.DebugLastHandlerArgs;
			response.DebugString = "";
			if (HttpContext.Current != null)
			{
				response.DebugString += HttpContext.Current.Request.GetType().FullName
					+ "|" + HttpContext.Current.Response.GetType().FullName;
			}

			var json = JsonSerializer.SerializeToString(response);
			httpRes.ContentType = ContentType.Json;
			httpRes.Write(json);
		}

        /// <summary>Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler" /> interface.</summary>
        ///
        /// <param name="context">An <see cref="T:System.Web.HttpContext" /> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to
        /// service HTTP requests.
        /// </param>
		public void ProcessRequest(HttpContext context)
		{
			ProcessRequest(
				new HttpRequestWrapper(typeof(RequestInfo).Name, context.Request),
				new HttpResponseWrapper(context.Response),
				typeof(RequestInfo).Name);
		}

        /// <summary>Converts a nvc to a dictionary.</summary>
        ///
        /// <param name="nvc">The nvc.</param>
        ///
        /// <returns>nvc as a Dictionary&lt;string,string&gt;</returns>
		public static Dictionary<string, string> ToDictionary(NameValueCollection nvc)
		{
			var map = new Dictionary<string, string>();
			for (var i = 0; i < nvc.Count; i++)
			{
				map[nvc.GetKey(i)] = nvc.Get(i);
			}
			return map;
		}

        /// <summary>Convert this object into a string representation.</summary>
        ///
        /// <param name="nvc">The nvc.</param>
        ///
        /// <returns>A string that represents this object.</returns>
		public static string ToString(NameValueCollection nvc)
		{
			var map = ToDictionary(nvc);
			return TypeSerializer.SerializeToString(map);
		}

        /// <summary>Gets request information.</summary>
        ///
        /// <param name="httpReq">The HTTP request.</param>
        ///
        /// <returns>The request information.</returns>
		public static RequestInfoResponse GetRequestInfo(IHttpRequest httpReq)
		{
			var response = new RequestInfoResponse
			{
				Host = EndpointHost.Config.DebugHttpListenerHostEnvironment + "_v" + Env.NServiceKitVersion + "_" + EndpointHost.Config.ServiceName,
				Date = DateTime.UtcNow,
				ServiceName = EndpointHost.Config.ServiceName,
				UserHostAddress = httpReq.UserHostAddress,
				HttpMethod = httpReq.HttpMethod,
				AbsoluteUri = httpReq.AbsoluteUri,
				RawUrl = httpReq.RawUrl,
				ResolvedPathInfo = httpReq.PathInfo,
				ContentType = httpReq.ContentType,
				Headers = ToDictionary(httpReq.Headers),
				QueryString = ToDictionary(httpReq.QueryString),
				FormData = ToDictionary(httpReq.FormData),
				AcceptTypes = new List<string>(httpReq.AcceptTypes ?? new string[0]),
				ContentLength = httpReq.ContentLength,
				OperationName = httpReq.OperationName,
				ResponseContentType = httpReq.ResponseContentType,
			};
			return response;
		}

        /// <summary>Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler" /> instance.</summary>
        ///
        /// <value>true if the <see cref="T:System.Web.IHttpHandler" /> instance is reusable; otherwise, false.</value>
		public bool IsReusable
		{
			get { return false; }
		}
	}
}