using System;
using System.Runtime.Serialization;
using NServiceKit.Common.Web;
using NServiceKit.MiniProfiler;
using NServiceKit.ServiceHost;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints.Extensions;
using NServiceKit.WebHost.Endpoints.Support;

namespace NServiceKit.WebHost.Endpoints
{
    /// <summary>A rest handler.</summary>
    public class RestHandler : EndpointHandlerBase
    {
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.RestHandler class.</summary>
        public RestHandler()
        {
            this.HandlerAttributes = EndpointAttributes.Reply;
        }

        /// <summary>Searches for the first matching rest path.</summary>
        ///
        /// <param name="httpMethod"> The HTTP method.</param>
        /// <param name="pathInfo">   Information describing the path.</param>
        /// <param name="contentType">Type of the content.</param>
        ///
        /// <returns>The found matching rest path.</returns>
        public static IRestPath FindMatchingRestPath(string httpMethod, string pathInfo, out string contentType)
        {
            var controller = ServiceManager != null
                ? ServiceManager.ServiceController
                : EndpointHost.Config.ServiceController;

            pathInfo = GetSanitizedPathInfo(pathInfo, out contentType);

            return controller.GetRestPathForRequest(httpMethod, pathInfo);
        }

        private static string GetSanitizedPathInfo(string pathInfo, out string contentType)
        {
            contentType = null;
            if (EndpointHost.Config.AllowRouteContentTypeExtensions)
            {
                var pos = pathInfo.LastIndexOf('.');
                if (pos >= 0)
                {
                    var format = pathInfo.Substring(pos + 1);
                    contentType = EndpointHost.ContentTypeFilter.GetFormatContentType(format);
                    if (contentType != null)
                    {
                        pathInfo = pathInfo.Substring(0, pos);
                    }
                }
            }
            return pathInfo;
        }

        /// <summary>Gets rest path.</summary>
        ///
        /// <param name="httpMethod">The HTTP method.</param>
        /// <param name="pathInfo">  Information describing the path.</param>
        ///
        /// <returns>The rest path.</returns>
        public IRestPath GetRestPath(string httpMethod, string pathInfo)
        {
            if (this.RestPath == null)
            {
                string contentType;
                this.RestPath = FindMatchingRestPath(httpMethod, pathInfo, out contentType);
                
                if (contentType != null)
                    ResponseContentType = contentType;
            }
            return this.RestPath;
        }

        /// <summary>Gets or sets the full pathname of the rest file.</summary>
        ///
        /// <value>The full pathname of the rest file.</value>
        public IRestPath RestPath { get; set; }

        /// <summary>Set from SSHHF.GetHandlerForPathInfo()</summary>
        ///
        /// <value>The type of the response content.</value>
        public string ResponseContentType { get; set; }

        /// <summary>Process the request.</summary>
        ///
        /// <exception cref="NotSupportedException">Thrown when the requested operation is not supported.</exception>
        /// <exception cref="Exception">            Thrown when an exception error condition occurs.</exception>
        ///
        /// <param name="httpReq">      The HTTP request.</param>
        /// <param name="httpRes">      The HTTP resource.</param>
        /// <param name="operationName">Name of the operation.</param>
        public override void ProcessRequest(IHttpRequest httpReq, IHttpResponse httpRes, string operationName)
        {
            try
            {
                if (EndpointHost.ApplyPreRequestFilters(httpReq, httpRes)) return;

                var restPath = GetRestPath(httpReq.HttpMethod, httpReq.PathInfo);
                if (restPath == null)
                    throw new NotSupportedException("No RestPath found for: " + httpReq.HttpMethod + " " + httpReq.PathInfo);

                operationName = restPath.RequestType.Name;

                var callback = httpReq.GetJsonpCallback();
                var doJsonp = EndpointHost.Config.AllowJsonpRequests
                              && !string.IsNullOrEmpty(callback);

                if (ResponseContentType != null)
                    httpReq.ResponseContentType = ResponseContentType;

                var responseContentType = httpReq.ResponseContentType;
                EndpointHost.Config.AssertContentType(responseContentType);

                var request = GetRequest(httpReq, restPath);
                if (EndpointHost.ApplyRequestFilters(httpReq, httpRes, request)) return;

                var response = GetResponse(httpReq, httpRes, request);
                if (EndpointHost.ApplyResponseFilters(httpReq, httpRes, response)) return;

                if (responseContentType.Contains("jsv") && !string.IsNullOrEmpty(httpReq.QueryString["debug"]))
                {
                    JsvSyncReplyHandler.WriteDebugResponse(httpRes, response);
                    return;
                }

                if (doJsonp && !(response is CompressedResult))
                    httpRes.WriteToResponse(httpReq, response, (callback + "(").ToUtf8Bytes(), ")".ToUtf8Bytes());
                else
                    httpRes.WriteToResponse(httpReq, response);
            }
            catch (Exception ex)
            {
                if (!EndpointHost.Config.WriteErrorsToResponse) throw;
                HandleException(httpReq, httpRes, operationName, ex);
            }
        }

        /// <summary>Gets a response.</summary>
        ///
        /// <param name="httpReq">The HTTP request.</param>
        /// <param name="httpRes">The HTTP resource.</param>
        /// <param name="request">The request.</param>
        ///
        /// <returns>The response.</returns>
        public override object GetResponse(IHttpRequest httpReq, IHttpResponse httpRes, object request)
        {
            var requestContentType = ContentType.GetEndpointAttributes(httpReq.ResponseContentType);

            return ExecuteService(request,
                HandlerAttributes | requestContentType | httpReq.GetAttributes(), httpReq, httpRes);
        }

        private static object GetRequest(IHttpRequest httpReq, IRestPath restPath)
        {
            var requestType = restPath.RequestType;
            using (Profiler.Current.Step("Deserialize Request"))
            {
                try
                {
                    var requestDto = GetCustomRequestFromBinder(httpReq, requestType);
                    if (requestDto != null) return requestDto;

                    var requestParams = httpReq.GetRequestParams();
                    requestDto = CreateContentTypeRequest(httpReq, requestType, httpReq.ContentType);

                    string contentType;
                    var pathInfo = !restPath.IsWildCardPath 
                        ? GetSanitizedPathInfo(httpReq.PathInfo, out contentType)
                        : httpReq.PathInfo;

                    return restPath.CreateRequest(pathInfo, requestParams, requestDto);
                }
                catch (SerializationException e)
                {
                    throw new RequestBindingException("Unable to bind request", e);
                }
                catch (ArgumentException e)
                {
                    throw new RequestBindingException("Unable to bind request", e);
                }
            }
        }

        /// <summary>
        /// Used in Unit tests
        /// </summary>
        /// <returns></returns>
        public override object CreateRequest(IHttpRequest httpReq, string operationName)
        {
            if (this.RestPath == null)
                throw new ArgumentNullException("No RestPath found");

            return GetRequest(httpReq, this.RestPath);
        }
    }

}
