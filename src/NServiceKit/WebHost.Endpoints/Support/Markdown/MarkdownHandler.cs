using System.Net;
using NServiceKit.ServiceHost;
using NServiceKit.WebHost.Endpoints.Extensions;
using NServiceKit.WebHost.Endpoints.Formats;

namespace NServiceKit.WebHost.Endpoints.Support.Markdown
{
    /// <summary>
    /// 
    /// </summary>
    public class MarkdownHandler : EndpointHandlerBase
    {
        /// <summary>
        /// Gets or sets the markdown format.
        /// </summary>
        /// <value>
        /// The markdown format.
        /// </value>
        public MarkdownFormat MarkdownFormat { get; set; }
        /// <summary>
        /// Gets or sets the markdown page.
        /// </summary>
        /// <value>
        /// The markdown page.
        /// </value>
        public MarkdownPage MarkdownPage { get; set; }
        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        public object Model { get; set; }

        /// <summary>
        /// Gets or sets the path information.
        /// </summary>
        /// <value>
        /// The path information.
        /// </value>
        public string PathInfo { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkdownHandler"/> class.
        /// </summary>
        /// <param name="pathInfo">The path information.</param>
        public MarkdownHandler(string pathInfo)
        {
            PathInfo = pathInfo;
        }

        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="httpReq">The HTTP req.</param>
        /// <param name="httpRes">The HTTP resource.</param>
        /// <param name="operationName">Name of the operation.</param>
        public override void ProcessRequest(IHttpRequest httpReq, IHttpResponse httpRes, string operationName)
        {
            if (MarkdownFormat == null)
                MarkdownFormat = MarkdownFormat.Instance;

            var contentPage = MarkdownPage ?? MarkdownFormat.FindByPathInfo(PathInfo);
            if (contentPage == null)
            {
                httpRes.StatusCode = (int)HttpStatusCode.NotFound;
                httpRes.EndHttpHandlerRequest();
                return;
            }

            MarkdownFormat.ReloadModifiedPageAndTemplates(contentPage);

            if (httpReq.DidReturn304NotModified(contentPage.GetLastModified(), httpRes))
                return;

            var model = Model;
            if (model == null)
                httpReq.Items.TryGetValue("Model", out model);

            MarkdownFormat.ProcessMarkdownPage(httpReq, contentPage, model, httpRes);
        }

        /// <summary>
        /// Creates the request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="operationName">Name of the operation.</param>
        /// <returns></returns>
        public override object CreateRequest(IHttpRequest request, string operationName)
        {
            return null;
        }

        /// <summary>
        /// Gets the response.
        /// </summary>
        /// <param name="httpReq">The HTTP req.</param>
        /// <param name="httpRes">The HTTP resource.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public override object GetResponse(IHttpRequest httpReq, IHttpResponse httpRes, object request)
        {
            return null;
        }
    }
}