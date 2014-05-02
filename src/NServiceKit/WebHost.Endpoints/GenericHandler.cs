using System;
using NServiceKit.Common.Web;
using NServiceKit.MiniProfiler;
using NServiceKit.ServiceHost;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints.Extensions;
using NServiceKit.WebHost.Endpoints.Support;

namespace NServiceKit.WebHost.Endpoints
{
    /// <summary>A generic handler.</summary>
	public class GenericHandler : EndpointHandlerBase
	{
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.GenericHandler class.</summary>
        ///
        /// <param name="contentType">      Type of the content.</param>
        /// <param name="handlerAttributes">The handler attributes.</param>
        /// <param name="format">           Describes the format to use.</param>
		public GenericHandler(string contentType, EndpointAttributes handlerAttributes, Feature format)
		{
			this.HandlerContentType = contentType;
			this.ContentTypeAttribute = ContentType.GetEndpointAttributes(contentType);
			this.HandlerAttributes = handlerAttributes;
			this.format = format;
		}

        private Feature format;

        /// <summary>Gets or sets the type of the handler content.</summary>
        ///
        /// <value>The type of the handler content.</value>
		public string HandlerContentType { get; set; }

        /// <summary>Gets or sets the content type attribute.</summary>
        ///
        /// <value>The content type attribute.</value>
		public EndpointAttributes ContentTypeAttribute { get; set; }

        /// <summary>Creates a request.</summary>
        ///
        /// <param name="request">      The request.</param>
        /// <param name="operationName">Name of the operation.</param>
        ///
        /// <returns>The new request.</returns>
		public override object CreateRequest(IHttpRequest request, string operationName)
		{
			return GetRequest(request, operationName);
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
			var response = ExecuteService(request,
                HandlerAttributes | httpReq.GetAttributes(), httpReq, httpRes);
			
			return response;
		}

        /// <summary>Gets a request.</summary>
        ///
        /// <param name="httpReq">      The HTTP request.</param>
        /// <param name="operationName">Name of the operation.</param>
        ///
        /// <returns>The request.</returns>
		public object GetRequest(IHttpRequest httpReq, string operationName)
		{
			var requestType = GetOperationType(operationName);
			AssertOperationExists(operationName, requestType);

			using (Profiler.Current.Step("Deserialize Request"))
			{
				var requestDto = GetCustomRequestFromBinder(httpReq, requestType);
				return requestDto ?? DeserializeHttpRequest(requestType, httpReq, HandlerContentType)
                    ?? requestType.CreateInstance();
			}
		}

        /// <summary>Process the request.</summary>
        ///
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        ///
        /// <param name="httpReq">      The HTTP request.</param>
        /// <param name="httpRes">      The HTTP resource.</param>
        /// <param name="operationName">Name of the operation.</param>
		public override void ProcessRequest(IHttpRequest httpReq, IHttpResponse httpRes, string operationName)
		{
			try
			{
                EndpointHost.Config.AssertFeatures(format);

                if (EndpointHost.ApplyPreRequestFilters(httpReq, httpRes)) return;

				httpReq.ResponseContentType = httpReq.GetQueryStringContentType() ?? this.HandlerContentType;
				var callback = httpReq.QueryString["callback"];
				var doJsonp = EndpointHost.Config.AllowJsonpRequests
							  && !string.IsNullOrEmpty(callback);

				var request = CreateRequest(httpReq, operationName);
				if (EndpointHost.ApplyRequestFilters(httpReq, httpRes, request)) return;

				var response = GetResponse(httpReq, httpRes, request);
				if (EndpointHost.ApplyResponseFilters(httpReq, httpRes, response)) return;

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

	}
}
