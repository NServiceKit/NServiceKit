using System.Web;
using System.Xml;
using NServiceKit.Common.Web;
using NServiceKit.ServiceHost;
using NServiceKit.WebHost.Endpoints.Metadata;
using NServiceKit.WebHost.Endpoints.Support;

namespace NServiceKit.WebHost.Endpoints
{
    /// <summary>A SOAP 12 handler.</summary>
    public class Soap12Handler : SoapHandler
    {
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Soap12Handler class.</summary>
        ///
        /// <param name="soapType">Type of the SOAP.</param>
        public Soap12Handler(EndpointAttributes soapType) : base(soapType) { }

        /// <summary>Gets request message from stream.</summary>
        ///
        /// <param name="requestStream">The request stream.</param>
        ///
        /// <returns>The request message from stream.</returns>
        protected override System.ServiceModel.Channels.Message GetRequestMessageFromStream(System.IO.Stream requestStream)
        {
            return GetSoap12RequestMessage(requestStream);
        }
    }

    /// <summary>A SOAP 12 handlers.</summary>
    public class Soap12Handlers : Soap12Handler
    {
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Soap12Handlers class.</summary>
        public Soap12Handlers() : base(EndpointAttributes.Soap12) { }
    }

    /// <summary>A SOAP 12 asynchronous one way handler.</summary>
    public class Soap12AsyncOneWayHandler : Soap12Handler
    {
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Soap12AsyncOneWayHandler class.</summary>
        public Soap12AsyncOneWayHandler() : base(EndpointAttributes.Soap12) { }
    }

    /// <summary>A SOAP 12 message asynchronous one way HTTP handler.</summary>
    public class Soap12MessageAsyncOneWayHttpHandler
        : Soap12Handler, IHttpHandler
    {
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Soap12MessageAsyncOneWayHttpHandler class.</summary>
        public Soap12MessageAsyncOneWayHttpHandler() : base(EndpointAttributes.Soap12) { }

        /// <summary>Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler" /> interface.</summary>
        ///
        /// <param name="context">An <see cref="T:System.Web.HttpContext" /> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to
        /// service HTTP requests.
        /// </param>
        public new void ProcessRequest(HttpContext context)
        {
            if (context.Request.HttpMethod == HttpMethods.Get)
            {
                var wsdl = new Soap12WsdlMetadataHandler();
                wsdl.Execute(context);
                return;
            }

            SendOneWay(null);
        }
    }

    /// <summary>A SOAP 12 message synchronise reply HTTP handler.</summary>
    public class Soap12MessageSyncReplyHttpHandler : Soap12Handler, IHttpHandler
    {
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Soap12MessageSyncReplyHttpHandler class.</summary>
        public Soap12MessageSyncReplyHttpHandler() : base(EndpointAttributes.Soap12) { }

        /// <summary>Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler" /> interface.</summary>
        ///
        /// <param name="context">An <see cref="T:System.Web.HttpContext" /> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to
        /// service HTTP requests.
        /// </param>
        public new void ProcessRequest(HttpContext context)
        {
            if (context.Request.HttpMethod == HttpMethods.Get)
            {
                var wsdl = new Soap12WsdlMetadataHandler();
                wsdl.Execute(context);
                return;
            }

            var responseMessage = Send(null);

            context.Response.ContentType = GetSoapContentType(context.Request.ContentType);
            using (var writer = XmlWriter.Create(context.Response.OutputStream))
            {
                responseMessage.WriteMessage(writer);
            }
        }

        /// <summary>Process the request.</summary>
        ///
        /// <param name="httpReq">      The HTTP request.</param>
        /// <param name="httpRes">      The HTTP resource.</param>
        /// <param name="operationName">Name of the operation.</param>
        public override void ProcessRequest(IHttpRequest httpReq, IHttpResponse httpRes, string operationName)
        {
            if (httpReq.HttpMethod == HttpMethods.Get)
            {
                var wsdl = new Soap12WsdlMetadataHandler();
                wsdl.Execute(httpReq, httpRes);
                return;
            }

            var responseMessage = Send(null, httpReq, httpRes);

            httpRes.ContentType = GetSoapContentType(httpReq.ContentType);
            using (var writer = XmlWriter.Create(httpRes.OutputStream))
            {
                responseMessage.WriteMessage(writer);
            }
        }
    }

}