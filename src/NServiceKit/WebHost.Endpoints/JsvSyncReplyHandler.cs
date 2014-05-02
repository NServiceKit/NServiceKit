using System;
using System.Text;
using NServiceKit.Common.Web;
using NServiceKit.ServiceHost;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints.Extensions;

namespace NServiceKit.WebHost.Endpoints
{
    /// <summary>A jsv asynchronous one way handler.</summary>
    public class JsvAsyncOneWayHandler : GenericHandler
    {
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.JsvAsyncOneWayHandler class.</summary>
        public JsvAsyncOneWayHandler()
            : base(ContentType.Jsv, EndpointAttributes.OneWay | EndpointAttributes.Jsv, Feature.Jsv) { }
    }

    /// <summary>A jsv synchronise reply handler.</summary>
    public class JsvSyncReplyHandler : GenericHandler
    {
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.JsvSyncReplyHandler class.</summary>
        public JsvSyncReplyHandler()
            : base(ContentType.JsvText, EndpointAttributes.Reply | EndpointAttributes.Jsv, Feature.Jsv) { }

        private static void WriteDebugRequest(IRequestContext requestContext, object dto, IHttpResponse httpRes)
        {
            var bytes = Encoding.UTF8.GetBytes(dto.SerializeAndFormat());
            httpRes.OutputStream.Write(bytes, 0, bytes.Length);
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
            var isDebugRequest = httpReq.RawUrl.ToLower().Contains("debug");
            if (!isDebugRequest)
            {
                base.ProcessRequest(httpReq, httpRes, operationName);
                return;
            }

            try
            {
                var request = CreateRequest(httpReq, operationName);

                var response = ExecuteService(request,
                    HandlerAttributes | httpReq.GetAttributes(), httpReq, httpRes);

                WriteDebugResponse(httpRes, response);
            }
            catch (Exception ex)
            {
                if (!EndpointHost.Config.WriteErrorsToResponse) throw;
                HandleException(httpReq, httpRes, operationName, ex);
            }
        }

        /// <summary>Writes a debug response.</summary>
        ///
        /// <param name="httpRes"> The HTTP resource.</param>
        /// <param name="response">The response.</param>
        public static void WriteDebugResponse(IHttpResponse httpRes, object response)
        {
            httpRes.WriteToResponse(response, WriteDebugRequest,
                new SerializationContext(ContentType.PlainText));

            httpRes.EndRequest();
        }
    }
}