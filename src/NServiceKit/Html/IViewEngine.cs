using System.IO;
using NServiceKit.ServiceHost;

namespace NServiceKit.Html
{
    /// <summary>Interface for view engine.</summary>
    public interface IViewEngine
    {
        /// <summary>Query if 'viewName' has view.</summary>
        ///
        /// <param name="viewName">Name of the view.</param>
        /// <param name="httpReq"> The HTTP request.</param>
        ///
        /// <returns>true if view, false if not.</returns>
        bool HasView(string viewName, IHttpRequest httpReq = null);

        /// <summary>Renders the partial.</summary>
        ///
        /// <param name="pageName">  Name of the page.</param>
        /// <param name="model">     The model.</param>
        /// <param name="renderHtml">true to render HTML.</param>
        /// <param name="writer">    The writer.</param>
        /// <param name="htmlHelper">The HTML helper.</param>
        ///
        /// <returns>A string.</returns>
        string RenderPartial(string pageName, object model, bool renderHtml, StreamWriter writer = null, HtmlHelper htmlHelper = null);

        /// <summary>Process the request.</summary>
        ///
        /// <param name="httpReq">The HTTP request.</param>
        /// <param name="httpRes">The HTTP resource.</param>
        /// <param name="dto">    The dto.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        bool ProcessRequest(IHttpRequest httpReq, IHttpResponse httpRes, object dto);
    }
}