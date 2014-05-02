using System.Net;
using NServiceKit.Common.Web;
using NServiceKit.Razor.Managers;
using NServiceKit.ServiceHost;
using NServiceKit.WebHost.Endpoints.Extensions;
using NServiceKit.WebHost.Endpoints.Support;

namespace NServiceKit.Razor
{
    /// <summary>A razor handler.</summary>
    public class RazorHandler : EndpointHandlerBase
    {
        /// <summary>Gets or sets the razor format.</summary>
        ///
        /// <value>The razor format.</value>
        public RazorFormat RazorFormat { get; set; }

        /// <summary>Gets or sets the razor page.</summary>
        ///
        /// <value>The razor page.</value>
        public RazorPage RazorPage { get; set; }

        /// <summary>Gets or sets the model.</summary>
        ///
        /// <value>The model.</value>
        public object Model { get; set; }

        /// <summary>Gets or sets information describing the path.</summary>
        ///
        /// <value>Information describing the path.</value>
        public string PathInfo { get; set; }

        /// <summary>Initializes a new instance of the NServiceKit.Razor.RazorHandler class.</summary>
        ///
        /// <param name="pathInfo">Information describing the path.</param>
        public RazorHandler(string pathInfo)
        {
            PathInfo = pathInfo;
        }

        /// <summary>Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler" /> interface.</summary>
        ///
        /// <param name="httpReq">      The HTTP request.</param>
        /// <param name="httpRes">      The HTTP resource.</param>
        /// <param name="operationName">Name of the operation.</param>
        public override void ProcessRequest(IHttpRequest httpReq, IHttpResponse httpRes, string operationName)
        {
            httpRes.ContentType = ContentType.Html;
            if (RazorFormat == null)
                RazorFormat = RazorFormat.Instance;

            var contentPage = RazorPage ?? RazorFormat.FindByPathInfo(PathInfo);
            if (contentPage == null)
            {
                httpRes.StatusCode = (int)HttpStatusCode.NotFound;
                httpRes.EndHttpHandlerRequest();
                return;
            }

            var model = Model;
            if (model == null)
                httpReq.Items.TryGetValue("Model", out model);
            if (model == null)
            {
                var modelType = RazorPage != null ? RazorPage.ModelType : null;
                model = modelType == null || modelType == typeof(DynamicRequestObject)
                    ? null
                    : DeserializeHttpRequest(modelType, httpReq, httpReq.ContentType);
            }

            RazorFormat.ProcessRazorPage(httpReq, contentPage, model, httpRes);
        }

        /// <summary>Creates a request.</summary>
        ///
        /// <param name="request">      The request.</param>
        /// <param name="operationName">Name of the operation.</param>
        ///
        /// <returns>The new request.</returns>
        public override object CreateRequest(IHttpRequest request, string operationName)
        {
            return null;
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
            return null;
        }
    }
}