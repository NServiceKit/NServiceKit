using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using NServiceKit.Common;
using NServiceKit.Common.Web;
using NServiceKit.Html;
using NServiceKit.ServiceHost;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints.Extensions;
using NServiceKit.WebHost.Endpoints.Support;
using System.Web.UI.WebControls;
using NServiceKit.IO;
using NServiceKit.Text.Controller;

namespace NServiceKit.Razor.Managers
{
    /// <summary>Renders the partial delegate.</summary>
    ///
    /// <param name="pageName">  Name of the page.</param>
    /// <param name="model">     The model.</param>
    /// <param name="renderHtml">true to render HTML.</param>
    /// <param name="writer">    The writer.</param>
    /// <param name="htmlHelper">The HTML helper.</param>
    /// <param name="httpReq">   The HTTP request.</param>
    ///
    /// <returns>A string.</returns>
    public delegate string RenderPartialDelegate(string pageName, object model, bool renderHtml, StreamWriter writer = null, HtmlHelper htmlHelper = null, IHttpRequest httpReq = null);

    /// <summary>
    /// A common hook into NServiceKit and the hosting infrastructure used to resolve requests.
    /// </summary>
    public class RazorPageResolver : EndpointHandlerBase, IViewEngine
    {
        /// <summary>The view key.</summary>
        public const string ViewKey = "View";
        /// <summary>The layout key.</summary>
        public const string LayoutKey = "Template";
        /// <summary>The query string format key.</summary>
        public const string QueryStringFormatKey = "format";
        /// <summary>The no template format value.</summary>
        public const string NoTemplateFormatValue = "bare";
        /// <summary>The default layout name.</summary>
        public const string DefaultLayoutName = "_Layout";

        private static readonly UTF8Encoding UTF8EncodingWithoutBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier:false);

        private readonly IRazorConfig config;
        private readonly RazorViewManager viewManager;

        /// <summary>Gets or sets the render partial function.</summary>
        ///
        /// <value>The render partial function.</value>
        public RenderPartialDelegate RenderPartialFn { get; set; }

        /// <summary>Initializes a new instance of the NServiceKit.Razor.Managers.RazorPageResolver class.</summary>
        ///
        /// <param name="config">     The configuration.</param>
        /// <param name="viewManager">Manager for view.</param>
        public RazorPageResolver(IRazorConfig config, RazorViewManager viewManager)
        {
            this.RequestName = "Razor_PageResolver";

            this.config = config;
            this.viewManager = viewManager;
        }

        /// <summary>Handler, called when the catch all.</summary>
        ///
        /// <param name="httpmethod">The httpmethod.</param>
        /// <param name="pathInfo">  Information describing the path.</param>
        /// <param name="filepath">  The filepath.</param>
        ///
        /// <returns>An IHttpHandler.</returns>
        public IHttpHandler CatchAllHandler(string httpmethod, string pathInfo, string filepath)
        {
            //does not have a .cshtml extension
            var ext = Path.GetExtension(pathInfo);
            if (ext == config.RazorFileExtension)
            {
                //pathInfo on dir doesn't match existing razor page
                if (string.IsNullOrEmpty(ext) && viewManager.GetPageByPathInfo(pathInfo) == null) return null;

                //if there is any denied predicates for the path, return nothing
                if (this.config.Deny.Any(denined => denined(pathInfo))) return null;

                //Redirect for /default.cshtml => / or /page.cshtml => /page
                if (pathInfo.EndsWith(config.RazorFileExtension))
                {
                    pathInfo = pathInfo.EndsWithIgnoreCase(config.DefaultPageName)
                                   ? pathInfo.Substring(0, pathInfo.Length - config.DefaultPageName.Length)
                                   : pathInfo.WithoutExtension();

                    var webHostUrl = config.WebHostUrl;
                    return new RedirectHttpHandler
                               {
                                   AbsoluteUrl =
                                       webHostUrl.IsNullOrEmpty()
                                           ? null
                                           : webHostUrl.CombineWith(pathInfo),
                                   RelativeUrl = webHostUrl.IsNullOrEmpty() ? pathInfo : null
                               };
                }
            }
            else
            {
                if (this.viewManager.GetVirutalFile(pathInfo) == null)
                {
                    return null;
                }
            }
            
            //only return "this" when we can, indeed, handle the httpReq.
            return this;
        }

        /// <summary>
        /// This is called by the hosting environment via CatchAll usually for content pages.
        /// </summary>
        public override void ProcessRequest(IHttpRequest httpReq, IHttpResponse httpRes, string operationName)
        {
            ////ToDo: Figure out binary assets like jpgs and pngs. 

            httpRes.ContentType = ContentType.PlainText;

            if (httpReq.PathInfo.EndsWith("js"))
            {
                httpRes.ContentType = ContentType.JavaScript;
            }
            else if (httpReq.PathInfo.EndsWith("css"))
            {
                httpRes.ContentType = ContentType.css;
            }

            this.ResolveRazorPageAsset(httpReq, httpRes, null);
            httpRes.EndRequest(skipHeaders: true);
        }

        /// <summary>
        /// Called by the HtmlFormat:IPlugin who checks to see if any registered view engines can handle the response DTO.
        /// If this view engine can handle the response DTO, then process it, otherwise, returning false will
        /// allow another view engine to attempt to process it. If no view engines can process the DTO,
        /// HtmlFormat will simply handle it itself.
        /// </summary>
        public virtual bool ProcessRequest(IHttpRequest httpReq, IHttpResponse httpRes, object dto)
        {
            //for compatibility
            var httpResult = dto as IHttpResult;
            if (httpResult != null)
                dto = httpResult.Response;

            var existingRazorPage = FindRazorPage(httpReq, dto);
            if (existingRazorPage == null)
            {
                return false;
            }

            ResolveAndExecuteRazorPage(httpReq, httpRes, dto, existingRazorPage);

            httpRes.EndRequest();
            return true;
        }

        private RazorPage FindRazorPage(IHttpRequest httpReq, object model)
        {
            var viewName = httpReq.GetItem(ViewKey) as string;
            if (viewName != null)
            {
                return this.viewManager.GetPageByName(viewName, httpReq, model);
            }
            var razorPage = this.viewManager.GetPageByName(httpReq.OperationName) //Request DTO
                         ?? this.viewManager.GetPage(httpReq, model); // Response DTO
            return razorPage;
        }


        private IVirtualFile FindRazorPageAsset(IHttpRequest httpReq, object model)
        {
            var viewName = httpReq.PathInfo; 

            return this.viewManager.GetVirutalFile(viewName);  ////, httpReq, model);
        }

        /// <summary>Resolve and execute razor page.</summary>
        ///
        /// <param name="httpReq">  The HTTP request.</param>
        /// <param name="httpRes">  The HTTP resource.</param>
        /// <param name="model">    The model.</param>
        /// <param name="razorPage">The razor page.</param>
        ///
        /// <returns>An IRazorView.</returns>
        public IRazorView ResolveAndExecuteRazorPage(IHttpRequest httpReq, IHttpResponse httpRes, object model, RazorPage razorPage=null)
        {
            razorPage = razorPage ?? FindRazorPage(httpReq, model);

            if (razorPage == null)
            {
                httpRes.StatusCode = (int)HttpStatusCode.NotFound;
                return null;
            }

            var page = CreateRazorPageInstance(httpReq, httpRes, model, razorPage);

            var includeLayout = !(httpReq.GetParam(QueryStringFormatKey) ?? "").Contains(NoTemplateFormatValue);
            if (includeLayout)
            {
                var result = ExecuteRazorPageWithLayout(httpReq, httpRes, model, page, () => 
                    httpReq.GetItem(LayoutKey) as string
                    ?? page.Layout
                    ?? DefaultLayoutName);

                using (var writer = new StreamWriter(httpRes.OutputStream, UTF8EncodingWithoutBom))
                {
                    writer.Write(result.Item2);
                }
                return result.Item1;
            }

            using (var writer = new StreamWriter(httpRes.OutputStream, UTF8EncodingWithoutBom))
            {
                page.WriteTo(writer);
            }
            return page;
        }

        private IVirtualFile ResolveRazorPageAsset(IHttpRequest httpReq, IHttpResponse httpRes, object model)
        {
            IVirtualFile razerPageAsset = this.FindRazorPageAsset(httpReq, model);

            if (razerPageAsset == null)
            {
                httpRes.StatusCode = (int)HttpStatusCode.NotFound;
                return null;
            }

            using (var writer = new StreamWriter(httpRes.OutputStream, UTF8EncodingWithoutBom))
            {
                writer.Write(razerPageAsset.ReadAllText());
            }


            return razerPageAsset;
        }

        private Tuple<IRazorView, string> ExecuteRazorPageWithLayout(IHttpRequest httpReq, IHttpResponse httpRes, object model, IRazorView page, Func<string> layout)
        {
            using (var ms = new MemoryStream())
            {
                using (var childWriter = new StreamWriter(ms, UTF8EncodingWithoutBom))
                {
                    //child page needs to execute before master template to populate ViewBags, sections, etc
                    page.WriteTo(childWriter);
                    var childBody = ms.ToArray().FromUtf8Bytes();

                    var layoutName = layout();
                    if (!String.IsNullOrEmpty(layoutName))
                    {
                        var layoutPage = this.viewManager.GetPageByName(layoutName, httpReq, model);
                        if (layoutPage != null)
                        {
                            var layoutView = CreateRazorPageInstance(httpReq, httpRes, model, layoutPage);
                            layoutView.SetChildPage(page, childBody);
                            return ExecuteRazorPageWithLayout(httpReq, httpRes, model, layoutView, () => layoutView.Layout);
                        }
                    }

                    return Tuple.Create(page, childBody);
                }
            }
        }

        private IRazorView CreateRazorPageInstance(IHttpRequest httpReq, IHttpResponse httpRes, object dto, RazorPage razorPage)
        {
            viewManager.EnsureCompiled(razorPage);

            //don't proceed any further, the background compiler found there was a problem compiling the page, so throw instead
            if (razorPage.CompileException != null)
            {
                throw razorPage.CompileException;
            }

            //else, EnsureCompiled() ensures we have a page type to work with so, create an instance of the page
            var page = (IRazorView) razorPage.ActivateInstance();

            page.Init(viewEngine: this, httpReq: httpReq, httpRes: httpRes);

            //deserialize the model.
            PrepareAndSetModel(page, httpReq, dto);
        
            return page;
        }

        private void PrepareAndSetModel(IRazorView page, IHttpRequest httpReq, object dto)
        {
            var hasModel = page as IHasModel;
            if (hasModel == null) return;

            if (hasModel.ModelType == typeof(DynamicRequestObject))
                dto = new DynamicRequestObject(httpReq, dto);

            var model = dto ?? DeserializeHttpRequest(hasModel.ModelType, httpReq, httpReq.ContentType);

            if (model.GetType().IsAnonymousType())
            {
                model = new DynamicRequestObject(httpReq, model);
            }

            hasModel.SetModel(model);
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

        /// <summary>Query if 'viewName' has view.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <param name="viewName">Name of the view.</param>
        /// <param name="httpReq"> The HTTP request.</param>
        ///
        /// <returns>true if view, false if not.</returns>
        public bool HasView(string viewName, IHttpRequest httpReq = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>Renders the partial.</summary>
        ///
        /// <param name="pageName">  Name of the page.</param>
        /// <param name="model">     The model.</param>
        /// <param name="renderHtml">true to render HTML.</param>
        /// <param name="writer">    The writer.</param>
        /// <param name="htmlHelper">The HTML helper.</param>
        ///
        /// <returns>A string.</returns>
        public virtual string RenderPartial(string pageName, object model, bool renderHtml, StreamWriter writer, HtmlHelper htmlHelper)
        {
            var httpReq = htmlHelper.HttpRequest;
            var razorPage = this.viewManager.GetPageByName(pageName, httpReq, model);
            if (razorPage != null)
            {
                var page = CreateRazorPageInstance(httpReq, htmlHelper.HttpResponse, model, razorPage);
                page.ParentPage = htmlHelper.RazorPage;
                page.WriteTo(writer);
            }
            else
            {
                if (RenderPartialFn != null)
                {
                    RenderPartialFn(pageName, model, renderHtml, writer, htmlHelper, httpReq);
                }
                else
                {
                    writer.Write("<!--No RenderPartialFn, skipping {0}-->".Fmt(pageName));                    
                }
            }
            return null;
        }
    }

}