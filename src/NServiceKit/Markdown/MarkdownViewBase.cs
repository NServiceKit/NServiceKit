using System.Collections.Generic;
using NServiceKit.Html;
using NServiceKit.WebHost.Endpoints.Support.Markdown;
using NServiceKit.WebHost.Endpoints;

namespace NServiceKit.Markdown
{
    /// <summary>A markdown view base.</summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
	public abstract class MarkdownViewBase<T> : MarkdownViewBase
	{
		private HtmlHelper<T> html;

        /// <summary>Gets the HTML.</summary>
        ///
        /// <value>The HTML.</value>
		public new HtmlHelper<T> Html
		{
			get { return html ?? (html = (HtmlHelper<T>)base.Html); }
		}

        /// <summary>Ensure the same instance is used for subclasses.</summary>
        ///
        /// <returns>The HTML helper.</returns>
		protected override HtmlHelper GetHtmlHelper()
		{
			return base.Html ?? new HtmlHelper<T>();
		}

        /// <summary>Initialises this object.</summary>
        ///
        /// <param name="appHost">     The AppHost so you can access configuration and resolve dependencies, etc.</param>
        /// <param name="markdownPage">This precompiled Markdown page with Metadata.</param>
        /// <param name="scopeArgs">   All variables passed to and created by your page. The Response DTO is stored and accessible via the 'Model' variable.
        /// 
        ///  All variables and outputs created are stored in ScopeArgs which is what's available to your website template. The Generated page is stored in the 'Body' variable.
        /// </param>
        /// <param name="model">       .</param>
        /// <param name="renderHtml">  Whether HTML or Markdown output is requested.</param>
		public override void Init(IAppHost appHost, MarkdownPage markdownPage, Dictionary<string, object> scopeArgs, object model, bool renderHtml)
		{
			this.AppHost = appHost;
			this.RenderHtml = renderHtml;
			this.ScopeArgs = scopeArgs;
			this.MarkdownPage = markdownPage;

			var typedModel = (T)model;
			Html.Init(markdownPage, scopeArgs, renderHtml, new ViewDataDictionary<T>(typedModel), null);

			InitHelpers();
		}
	}

    /// <summary>A markdown view base.</summary>
	public abstract class MarkdownViewBase : ITemplatePage
	{
        /// <summary>
        /// Reference to MarkdownViewEngine
        /// </summary>
        public IViewEngine ViewEngine { get; set; }

	    /// <summary>
		/// The AppHost so you can access configuration and resolve dependencies, etc.
		/// </summary>
		public IAppHost AppHost { get; set; }

		/// <summary>
		/// This precompiled Markdown page with Metadata
		/// </summary>
		public MarkdownPage MarkdownPage { get; protected set; }

		/// <summary>
		/// ASP.NET MVC's HtmlHelper
		/// </summary>
		public HtmlHelper Html { get; protected set; }

		/// <summary>
		/// All variables passed to and created by your page. 
		/// The Response DTO is stored and accessible via the 'Model' variable.
		///  
		/// All variables and outputs created are stored in ScopeArgs which is what's available
		/// to your website template. The Generated page is stored in the 'Body' variable.
		/// </summary>
		public Dictionary<string, object> ScopeArgs { get; set; }

		/// <summary>
		/// Whether HTML or Markdown output is requested
		/// </summary>
		public bool RenderHtml { get; protected set; }

		/// <summary>
		/// The Response DTO
		/// </summary>
		public object Model { get; protected set; }

        /// <summary>Initializes a new instance of the NServiceKit.Markdown.MarkdownViewBase class.</summary>
		protected MarkdownViewBase()
		{
			Html = GetHtmlHelper();
		}

		/// <summary>
		/// Ensure the same instance is used for subclasses
		/// </summary>
		protected virtual HtmlHelper GetHtmlHelper()
		{
			return Html ?? new HtmlHelper();
		}

        /// <summary>Initialises this object.</summary>
        ///
        /// <param name="appHost">     The AppHost so you can access configuration and resolve dependencies, etc.</param>
        /// <param name="markdownPage">This precompiled Markdown page with Metadata.</param>
        /// <param name="scopeArgs">   All variables passed to and created by your page. The Response DTO is stored and accessible via the 'Model' variable.
        /// 
        /// All variables and outputs created are stored in ScopeArgs which is what's available to your website template. The Generated page is stored in the 'Body' variable.
        /// </param>
        /// <param name="model">       .</param>
        /// <param name="renderHtml">  Whether HTML or Markdown output is requested.</param>
		public virtual void Init(IAppHost appHost, MarkdownPage markdownPage, Dictionary<string, object> scopeArgs, object model, bool renderHtml)
		{
			this.AppHost = appHost;
			this.RenderHtml = renderHtml;
			this.ScopeArgs = scopeArgs;
			this.MarkdownPage = markdownPage;

			Html.Init(markdownPage, scopeArgs, renderHtml, new ViewDataDictionary(model), null);

			InitHelpers();
		}

		/// <summary>
		/// Called before page is executed
		/// </summary>
		public virtual void InitHelpers() { }

		/// <summary>
		/// Called after page is executed but before it's merged with the 
		/// website template if any.
		/// </summary>
		public virtual void OnLoad() { }

		/// <summary>
		/// Don't HTML encode safe output
		/// </summary>
		/// <param name="content"></param>
		/// <returns></returns>
		public MvcHtmlString Raw(string content)
		{
			return Html.Raw(content);
		}

		/// <summary>
		/// Return the output of a different view with the specified name 
		/// using the supplied model
		/// </summary>
		/// <param name="viewName"></param>
		/// <param name="model"></param>
		/// <returns></returns>
		public MvcHtmlString Partial(string viewName, object model)
		{
			return Html.Partial(viewName, model);
		}

		/// <summary>
		/// Resolve registered Assemblies
		/// </summary>
		/// <returns></returns>
		public T Get<T>()
		{
			return this.AppHost.TryResolve<T>();
		}

        /// <summary>Lowers.</summary>
        ///
        /// <param name="name">The name.</param>
        ///
        /// <returns>A string.</returns>
		public string Lower(string name)
		{
			return name == null ? null : name.ToLower();
		}

        /// <summary>Uppers.</summary>
        ///
        /// <param name="name">The name.</param>
        ///
        /// <returns>A string.</returns>
		public string Upper(string name)
		{
			return name == null ? null : name.ToUpper();
		}

        /// <summary>Combines.</summary>
        ///
        /// <param name="separator">The separator.</param>
        /// <param name="parts">    A variable-length parameters list containing parts.</param>
        ///
        /// <returns>A string.</returns>
		public string Combine(string separator, params string[] parts)
		{
			return string.Join(separator, parts);
		}
	}
}