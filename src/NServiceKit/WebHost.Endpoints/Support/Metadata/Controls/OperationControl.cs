using System.Web;
using System.Web.UI;
using NServiceKit.Common;
using NServiceKit.ServiceHost;
using NServiceKit.WebHost.Endpoints.Support.Templates;

namespace NServiceKit.WebHost.Endpoints.Support.Metadata.Controls
{
    /// <summary>An operation control.</summary>
    public class OperationControl
    {
        /// <summary>Gets or sets the metadata configuration.</summary>
        ///
        /// <value>The metadata configuration.</value>
        public ServiceEndpointsMetadataConfig MetadataConfig { get; set; }

        /// <summary>Sets the format to use.</summary>
        ///
        /// <value>The format.</value>
        public Format Format
        {
            set
            {
                this.ContentType = Common.Web.ContentType.ToContentType(value);
                this.ContentFormat = Common.Web.ContentType.GetContentFormat(value);
            }
        }

        /// <summary>Gets or sets the HTTP request.</summary>
        ///
        /// <value>The HTTP request.</value>
        public IHttpRequest HttpRequest { get; set; }

        /// <summary>Gets or sets the type of the content.</summary>
        ///
        /// <value>The type of the content.</value>
        public string ContentType { get; set; }

        /// <summary>Gets or sets the content format.</summary>
        ///
        /// <value>The content format.</value>
        public string ContentFormat { get; set; }

        /// <summary>Gets or sets the title.</summary>
        ///
        /// <value>The title.</value>
        public string Title { get; set; }

        /// <summary>Gets or sets the name of the operation.</summary>
        ///
        /// <value>The name of the operation.</value>
        public string OperationName { get; set; }

        /// <summary>Gets or sets the name of the host.</summary>
        ///
        /// <value>The name of the host.</value>
        public string HostName { get; set; }

        /// <summary>Gets or sets a message describing the request.</summary>
        ///
        /// <value>A message describing the request.</value>
        public string RequestMessage { get; set; }

        /// <summary>Gets or sets a message describing the response.</summary>
        ///
        /// <value>A message describing the response.</value>
        public string ResponseMessage { get; set; }

        /// <summary>Gets or sets the metadata HTML.</summary>
        ///
        /// <value>The metadata HTML.</value>
        public string MetadataHtml { get; set; }

        /// <summary>Gets URI of the request.</summary>
        ///
        /// <value>The request URI.</value>
        public virtual string RequestUri
        {
            get
            {
                var endpointConfig = MetadataConfig.GetEndpointConfig(ContentType);
                var endpontPath = ResponseMessage != null
                    ? endpointConfig.SyncReplyUri : endpointConfig.AsyncOneWayUri;
                return string.Format("{0}/{1}", endpontPath, OperationName);
            }
        }

        /// <summary>Renders the given output.</summary>
        ///
        /// <param name="output">The output.</param>
        public void Render(HtmlTextWriter output)
        {
            var baseUrl = HttpRequest.GetParentAbsolutePath().ToParentPath();
            if (string.IsNullOrEmpty(baseUrl))
                baseUrl = "/";
            // use a fully qualified path if WebHostUrl is set
            if (EndpointHost.Config.WebHostUrl != null)
            {
                baseUrl = EndpointHost.Config.WebHostUrl.CombineWith(baseUrl);
            }

            var renderedTemplate = HtmlTemplates.Format(HtmlTemplates.OperationControlTemplate,
                Title,
                baseUrl.CombineWith(MetadataConfig.DefaultMetadataUri),
                ContentFormat.ToUpper(),
                OperationName,
                HttpRequestTemplate,
                ResponseTemplate,
                MetadataHtml);

            output.Write(renderedTemplate);
        }

        /// <summary>Gets the HTTP request template.</summary>
        ///
        /// <value>The HTTP request template.</value>
        public virtual string HttpRequestTemplate
        {
            get
            {
                return string.Format(
@"POST {0} HTTP/1.1 
Host: {1} 
Content-Type: {2}
Content-Length: <span class=""value"">length</span>

{3}", RequestUri, HostName, ContentType, HttpUtility.HtmlEncode(RequestMessage));
            }
        }

        /// <summary>Gets the response template.</summary>
        ///
        /// <value>The response template.</value>
        public virtual string ResponseTemplate
        {
            get
            {
                var httpResponse = this.HttpResponseTemplate;
                return string.IsNullOrEmpty(httpResponse) ? null : string.Format(@"
<div class=""response"">
<pre>
{0}
</pre>
</div>
", httpResponse);
            }
        }

        /// <summary>Gets the HTTP response template.</summary>
        ///
        /// <value>The HTTP response template.</value>
        public virtual string HttpResponseTemplate
        {
            get
            {
                if (string.IsNullOrEmpty(ResponseMessage)) return null;
                return string.Format(
@"HTTP/1.1 200 OK
Content-Type: {0}
Content-Length: length

{1}", ContentType, HttpUtility.HtmlEncode(ResponseMessage));
            }
        }
    }
}