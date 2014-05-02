using System.Web;

namespace NServiceKit.WebHost.Endpoints.Support.Metadata.Controls
{
    internal class Soap11OperationControl : OperationControl
    {
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Support.Metadata.Controls.Soap11OperationControl class.</summary>
        public Soap11OperationControl()
        {
            Format = ServiceHost.Format.Soap11;
        }

        /// <summary>Gets URI of the request.</summary>
        ///
        /// <value>The request URI.</value>
        public override string RequestUri
        {
            get
            {
                var endpointConfig = MetadataConfig.Soap11;
                var endpontPath = ResponseMessage != null ? endpointConfig.SyncReplyUri : endpointConfig.AsyncOneWayUri;
                return string.Format("{0}", endpontPath);
            }
        }

        /// <summary>Gets the HTTP request template.</summary>
        ///
        /// <value>The HTTP request template.</value>
        public override string HttpRequestTemplate
        {
            get
            {
                return string.Format(
@"POST {0} HTTP/1.1 
Host: {1} 
Content-Type: text/xml; charset=utf-8
Content-Length: <span class=""value"">length</span>
SOAPAction: {2}

{3}", RequestUri, HostName, base.OperationName, HttpUtility.HtmlEncode(RequestMessage));
            }
        }

    }
}