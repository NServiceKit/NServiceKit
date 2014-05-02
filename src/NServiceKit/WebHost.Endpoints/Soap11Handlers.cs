using System.Web;
using System.Xml;
using NServiceKit.Common.Web;
using NServiceKit.ServiceHost;
using NServiceKit.WebHost.Endpoints.Metadata;
using NServiceKit.WebHost.Endpoints.Support;

namespace NServiceKit.WebHost.Endpoints
{
    /// <summary>A SOAP 11 handler.</summary>
    public class Soap11Handler : SoapHandler
    {
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Soap11Handler class.</summary>
        ///
        /// <param name="soapType">Type of the SOAP.</param>
        public Soap11Handler(EndpointAttributes soapType) : base(soapType) { }

        /// <summary>Gets request message from stream.</summary>
        ///
        /// <param name="requestStream">The request stream.</param>
        ///
        /// <returns>The request message from stream.</returns>
        protected override System.ServiceModel.Channels.Message GetRequestMessageFromStream(System.IO.Stream requestStream)
        {
            return GetSoap11RequestMessage(requestStream);
        }
    }

    /// <summary>A SOAP 11 synchronise reply handler.</summary>
    public class Soap11SyncReplyHandler : Soap11Handler
    {
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Soap11SyncReplyHandler class.</summary>
        public Soap11SyncReplyHandler() : base(EndpointAttributes.Soap11) { }
    }

    /// <summary>A SOAP 11 asynchronous one way handler.</summary>
    public class Soap11AsyncOneWayHandler : Soap11Handler
    {
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Soap11AsyncOneWayHandler class.</summary>
        public Soap11AsyncOneWayHandler() : base(EndpointAttributes.Soap11) { }

        /// <summary>Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler" /> interface.</summary>
        ///
        /// <param name="context">An <see cref="T:System.Web.HttpContext" /> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to
        /// service HTTP requests.
        /// </param>
        public override void ProcessRequest(HttpContext context)
        {
            if (context.Request.HttpMethod == HttpMethods.Get)
            {
                var wsdl = new Soap11WsdlMetadataHandler();
                wsdl.Execute(context);
                return;
            }

            SendOneWay(null);
        }
    }

    /// <summary>A SOAP 11 message synchronise reply HTTP handler.</summary>
    public class Soap11MessageSyncReplyHttpHandler : Soap11Handler, IHttpHandler
    {
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Soap11MessageSyncReplyHttpHandler class.</summary>
        public Soap11MessageSyncReplyHttpHandler() : base(EndpointAttributes.Soap11) { }

        /// <summary>Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler" /> interface.</summary>
        ///
        /// <param name="context">An <see cref="T:System.Web.HttpContext" /> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to
        /// service HTTP requests.
        /// </param>
        public new void ProcessRequest(HttpContext context)
        {
            if (context.Request.HttpMethod == HttpMethods.Get)
            {
                var wsdl = new Soap11WsdlMetadataHandler();
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
                var wsdl = new Soap11WsdlMetadataHandler();
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