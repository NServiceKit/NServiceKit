using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NServiceKit.Html;
using NServiceKit.IO;
using NServiceKit.Logging;
using NServiceKit.Razor.Managers;
using NServiceKit.ServiceHost;
using NServiceKit.Text;
using NServiceKit.VirtualPath;
using NServiceKit.WebHost.Endpoints;

namespace NServiceKit.Razor
{
    /// <summary>A razor format.</summary>
    public class RazorFormat : IPlugin, IRazorPlugin, IRazorConfig
    {
        /// <summary>The template place holder.</summary>
        public const string TemplatePlaceHolder = "@RenderBody()";

        private static readonly ILog Log = LogManager.GetLogger(typeof(RazorFormat));
        /// <summary>The instance.</summary>
        public static RazorFormat Instance;

        /// <summary>Initializes a new instance of the NServiceKit.Razor.RazorFormat class.</summary>
        public RazorFormat()
        {
            this.RazorFileExtension = ".cshtml";
            this.DefaultPageName = "default.cshtml";
            this.PageBaseType = typeof(ViewPage);
            this.LiveReloadFactory = CreateLiveReload;

            Deny = new List<Predicate<string>> {
                DenyPathsWithLeading_,
            };
        }

        //configs
        public string RazorFileExtension { get; set; }

        /// <summary>Gets or sets the type of the page base.</summary>
        ///
        /// <value>The type of the page base.</value>
        public Type PageBaseType { get; set; }

        /// <summary>Gets or sets the default page name.</summary>
        ///
        /// <value>The default page name.</value>
        public string DefaultPageName { get; set; }

        /// <summary>Gets or sets URL of the web host.</summary>
        ///
        /// <value>The web host URL.</value>
        public string WebHostUrl { get; set; }

        /// <summary>Gets or sets the full pathname of the scan root file.</summary>
        ///
        /// <value>The full pathname of the scan root file.</value>
        public string ScanRootPath { get; set; }

        /// <summary>Gets or sets the enable live reload.</summary>
        ///
        /// <value>The enable live reload.</value>
        public bool? EnableLiveReload { get; set; }

        /// <summary>Gets or sets the deny.</summary>
        ///
        /// <value>The deny.</value>
        public List<Predicate<string>> Deny { get; set; }

        /// <summary>Gets or sets a value indicating whether the precompile pages.</summary>
        ///
        /// <value>true if precompile pages, false if not.</value>
        public bool PrecompilePages { get; set; }

        /// <summary>Gets or sets a value indicating whether the wait for precompilation startup.</summary>
        ///
        /// <value>true if wait for precompilation startup, false if not.</value>
        public bool WaitForPrecompilationOnStartup { get; set; }

        /// <summary>Gets or sets the virtual path provider.</summary>
        ///
        /// <value>The virtual path provider.</value>
        public IVirtualPathProvider VirtualPathProvider { get; set; }

        /// <summary>Gets or sets the live reload.</summary>
        ///
        /// <value>The live reload.</value>
        public ILiveReload LiveReload { get; set; }

        /// <summary>Gets or sets the live reload factory.</summary>
        ///
        /// <value>The live reload factory.</value>
        public Func<RazorViewManager, ILiveReload> LiveReloadFactory { get; set; }

        /// <summary>Gets or sets the render partial function.</summary>
        ///
        /// <value>The render partial function.</value>
        public RenderPartialDelegate RenderPartialFn { get; set; }

        static bool DenyPathsWithLeading_(string path)
        {
            return Path.GetFileName(path).StartsWith("_");
        }

        /// <summary>Gets or sets a value indicating whether the watch for modified pages.</summary>
        ///
        /// <value>true if watch for modified pages, false if not.</value>
        public bool WatchForModifiedPages
        {
            get { return EnableLiveReload.GetValueOrDefault(); }
            set { EnableLiveReload = value; }
        }

        //managers
        protected RazorViewManager ViewManager;
        /// <summary>The page resolver.</summary>
        protected RazorPageResolver PageResolver;

        /// <summary>Registers this object.</summary>
        ///
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        ///
        /// <param name="appHost">The application host.</param>
        public void Register(IAppHost appHost)
        {
            this.ScanRootPath = this.ScanRootPath ?? appHost.Config.WebHostPhysicalPath;
            this.VirtualPathProvider = VirtualPathProvider ?? appHost.VirtualPathProvider;
            this.WebHostUrl = WebHostUrl ?? appHost.Config.WebHostUrl;
            this.EnableLiveReload = this.EnableLiveReload ?? appHost.Config.DebugMode;

            try
            {
                Init();

                BindToAppHost(appHost);
            }
            catch (Exception ex)
            {
                ex.StackTrace.PrintDump();
                throw;
            }
        }

        private void BindToAppHost(IAppHost appHost)
        {
            appHost.CatchAllHandlers.Add(this.PageResolver.CatchAllHandler);
            appHost.ViewEngines.Add(this.PageResolver);

            if (this.RenderPartialFn == null)
            {
                this.RenderPartialFn = (pageName, model, renderHtml, writer, htmlHelper, httpReq) =>
                {
                    foreach (var viewEngine in appHost.ViewEngines)
                    {
                        if (viewEngine == PageResolver || !viewEngine.HasView(pageName, httpReq)) continue;
                        return viewEngine.RenderPartial(pageName, model, renderHtml, writer, htmlHelper);
                    }
                    writer.Write("<!--{0} not found-->".Fmt(pageName));
                    return null;
                };
            }
            this.PageResolver.RenderPartialFn = this.RenderPartialFn;
        }

        /// <summary>Initialises this object.</summary>
        ///
        /// <returns>A RazorFormat.</returns>
        public virtual RazorFormat Init()
        {
            if (Instance != null)
            {
                Log.Warn("RazorFormat plugin should only be initialized once");

                if (ViewManager != null && PageResolver != null)
                    return this;

                Log.Warn("Incomplete initialization, RazorFormat.Instance set but ViewManager/PageResolver is null");
            }

            Instance = this;

            this.ViewManager = CreateViewManager();
            this.PageResolver = CreatePageResolver();

            this.ViewManager.Init();

            if (EnableLiveReload.GetValueOrDefault())
            {
                this.LiveReload = LiveReloadFactory(this.ViewManager);
                this.LiveReload.StartWatching(this.ScanRootPath);
            }
            return this;
        }

        /// <summary>Creates page resolver.</summary>
        ///
        /// <returns>The new page resolver.</returns>
        public virtual RazorPageResolver CreatePageResolver()
        {
            return new RazorPageResolver(this, this.ViewManager);
        }

        /// <summary>Creates view manager.</summary>
        ///
        /// <returns>The new view manager.</returns>
        public virtual RazorViewManager CreateViewManager()
        {
            return new RazorViewManager(this, VirtualPathProvider);
        }

        static ILiveReload CreateLiveReload(RazorViewManager viewManager)
        {
            return new FileSystemWatcherLiveReload(viewManager);
        }

        /// <summary>Searches for the first path information.</summary>
        ///
        /// <param name="pathInfo">Information describing the path.</param>
        ///
        /// <returns>The found path information.</returns>
        public RazorPage FindByPathInfo(string pathInfo)
        {
            return ViewManager.GetPageByPathInfo(pathInfo);
        }

        /// <summary>Process the razor page.</summary>
        ///
        /// <param name="httpReq">    The HTTP request.</param>
        /// <param name="contentPage">The content page.</param>
        /// <param name="model">      The model.</param>
        /// <param name="httpRes">    The HTTP resource.</param>
        public void ProcessRazorPage(IHttpRequest httpReq, RazorPage contentPage, object model, IHttpResponse httpRes)
        {
            PageResolver.ResolveAndExecuteRazorPage(httpReq, httpRes, model, contentPage);
        }

        /// <summary>Process the request.</summary>
        ///
        /// <param name="httpReq">The HTTP request.</param>
        /// <param name="httpRes">The HTTP resource.</param>
        /// <param name="dto">    The dto.</param>
        public void ProcessRequest(IHttpRequest httpReq, IHttpResponse httpRes, object dto)
        {
            PageResolver.ProcessRequest(httpReq, httpRes, dto);
        }

        /// <summary>Adds a page.</summary>
        ///
        /// <param name="filePath">Full pathname of the file.</param>
        ///
        /// <returns>A RazorPage.</returns>
        public RazorPage AddPage(string filePath)
        {
            return ViewManager.AddPage(filePath);
        }

        /// <summary>Gets page by name.</summary>
        ///
        /// <param name="pageName">Name of the page.</param>
        ///
        /// <returns>The page by name.</returns>
        public RazorPage GetPageByName(string pageName)
        {
            return ViewManager.GetPageByName(pageName);
        }

        /// <summary>Gets page by path information.</summary>
        ///
        /// <param name="pathInfo">Information describing the path.</param>
        ///
        /// <returns>The page by path information.</returns>
        public RazorPage GetPageByPathInfo(string pathInfo)
        {
            return ViewManager.GetPageByPathInfo(pathInfo);
        }

        /// <summary>Creates a page.</summary>
        ///
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        ///
        /// <param name="razorContents">The razor contents.</param>
        ///
        /// <returns>The new page.</returns>
        public RazorPage CreatePage(string razorContents)
        {
            if (this.VirtualPathProvider == null)
                throw new ArgumentNullException("VirtualPathProvider");

            var writableFileProvider = this.VirtualPathProvider as IWriteableVirtualPathProvider;
            if (writableFileProvider == null)
                throw new InvalidOperationException("VirtualPathProvider is not IWriteableVirtualPathProvider");

            var tmpPath = "/__tmp/{0}.cshtml".Fmt(Guid.NewGuid().ToString("N"));
            writableFileProvider.AddFile(tmpPath, razorContents);

            return ViewManager.AddPage(tmpPath);
        }

        /// <summary>Renders to HTML.</summary>
        ///
        /// <exception cref="FileNotFoundException">Thrown when the requested file is not present.</exception>
        ///
        /// <param name="filePath">Full pathname of the file.</param>
        /// <param name="model">   The model.</param>
        /// <param name="layout">  The layout.</param>
        ///
        /// <returns>A string.</returns>
        public string RenderToHtml(string filePath, object model = null, string layout = null)
        {
            var razorView = ViewManager.GetPage(filePath);
            if (razorView == null)
                throw new FileNotFoundException("Razor file not found", filePath);

            return RenderToHtml(razorView, model: model, layout: layout);
        }

        /// <summary>Creates and render to HTML.</summary>
        ///
        /// <param name="razorContents">The razor contents.</param>
        /// <param name="model">        The model.</param>
        /// <param name="layout">       The layout.</param>
        ///
        /// <returns>The new and render to HTML.</returns>
        public string CreateAndRenderToHtml(string razorContents, object model = null, string layout = null)
        {
            var page = CreatePage(razorContents);
            return RenderToHtml(page, model: model, layout: layout);
        }

        /// <summary>Renders to HTML.</summary>
        ///
        /// <param name="razorPage">The razor page.</param>
        /// <param name="model">    The model.</param>
        /// <param name="layout">   The layout.</param>
        ///
        /// <returns>A string.</returns>
        public string RenderToHtml(RazorPage razorPage, object model = null, string layout = null)
        {
            IRazorView razorView;
            return RenderToHtml(razorPage, out razorView, model: model, layout: layout);
        }

        /// <summary>Renders to HTML.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="razorPage">The razor page.</param>
        /// <param name="razorView">The razor view.</param>
        /// <param name="model">    The model.</param>
        /// <param name="layout">   The layout.</param>
        ///
        /// <returns>A string.</returns>
        public string RenderToHtml(RazorPage razorPage, out IRazorView razorView, object model = null, string layout = null)
        {
            if (razorPage == null)
                throw new ArgumentNullException("razorPage");

            var mqContext = new MqRequestContext();

            var httpReq = new MqRequest(mqContext);
            if (layout != null)
            {
                httpReq.Items[RazorPageResolver.LayoutKey] = layout;
            }

            var httpRes = new MqResponse(mqContext);

            razorView = PageResolver.ResolveAndExecuteRazorPage(
                httpReq: httpReq,
                httpRes: httpRes,
                model: model,
                razorPage: razorPage);

            var ms = (MemoryStream)httpRes.OutputStream;
            return ms.ToArray().FromUtf8Bytes();
        }
    }

    /// <summary>Interface for razor configuration.</summary>
    public interface IRazorConfig
    {
        /// <summary>Gets the razor file extension.</summary>
        ///
        /// <value>The razor file extension.</value>
        string RazorFileExtension { get; }

        /// <summary>Gets the type of the page base.</summary>
        ///
        /// <value>The type of the page base.</value>
        Type PageBaseType { get; }

        /// <summary>Gets the default page name.</summary>
        ///
        /// <value>The default page name.</value>
        string DefaultPageName { get; }

        /// <summary>Gets the full pathname of the scan root file.</summary>
        ///
        /// <value>The full pathname of the scan root file.</value>
        string ScanRootPath { get; }

        /// <summary>Gets URL of the web host.</summary>
        ///
        /// <value>The web host URL.</value>
        string WebHostUrl { get; }

        /// <summary>Gets the deny.</summary>
        ///
        /// <value>The deny.</value>
        List<Predicate<string>> Deny { get; }

        /// <summary>Gets or sets a value indicating whether the precompile pages.</summary>
        ///
        /// <value>true if precompile pages, false if not.</value>
        bool PrecompilePages { get; set; }

        /// <summary>Gets or sets a value indicating whether the wait for precompilation startup.</summary>
        ///
        /// <value>true if wait for precompilation startup, false if not.</value>
        bool WaitForPrecompilationOnStartup { get; set; }
    }

}