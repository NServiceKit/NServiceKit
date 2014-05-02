// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using NServiceKit.Common.Web;
using NServiceKit.Html.AntiXsrf;
using NServiceKit.ServiceHost;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints.Support.Markdown;

namespace NServiceKit.Html
{
    /// <summary>A HTML helper.</summary>
	public class HtmlHelper
    {
        /// <summary>Name of the validation input CSS class.</summary>
        public static readonly string ValidationInputCssClassName = "input-validation-error";
        /// <summary>Name of the validation input valid CSS class.</summary>
        public static readonly string ValidationInputValidCssClassName = "input-validation-valid";
        /// <summary>Name of the validation message CSS class.</summary>
        public static readonly string ValidationMessageCssClassName = "field-validation-error";
        /// <summary>Name of the validation message valid CSS class.</summary>
        public static readonly string ValidationMessageValidCssClassName = "field-validation-valid";
        /// <summary>Name of the validation summary CSS class.</summary>
        public static readonly string ValidationSummaryCssClassName = "validation-summary-errors";
        /// <summary>Name of the validation summary valid CSS class.</summary>
        public static readonly string ValidationSummaryValidCssClassName = "validation-summary-valid";
#if NET_4_0
        private DynamicViewDataDictionary _dynamicViewDataDictionary;
#endif
        /// <summary>The HTML extensions.</summary>
		public static List<Type> HtmlExtensions = new List<Type> 
		{
			typeof(DisplayTextExtensions),
			typeof(InputExtensions),
			typeof(LabelExtensions),
			typeof(TextAreaExtensions),
            typeof(SelectExtensions)
		};

        /// <summary>Gets a method.</summary>
        ///
        /// <param name="methodName">Name of the method.</param>
        ///
        /// <returns>The method.</returns>
		public static MethodInfo GetMethod(string methodName)
		{
			foreach (var htmlExtension in HtmlExtensions)
			{
				var mi = htmlExtension.GetMethods().ToList()
					.FirstOrDefault(x => x.Name == methodName);

				if (mi != null) return mi;
			}
			return null;
		}

		private delegate string HtmlEncoder(object value);
		private static readonly HtmlEncoder htmlEncoder = GetHtmlEncoder();

        /// <summary>Gets a value indicating whether the HTML should be rendered.</summary>
        ///
        /// <value>true if render html, false if not.</value>
		public bool RenderHtml { get; protected set; }

        /// <summary>Gets or sets the HTTP request.</summary>
        ///
        /// <value>The HTTP request.</value>
        public IHttpRequest HttpRequest { get; set; }

        /// <summary>Gets or sets the HTTP response.</summary>
        ///
        /// <value>The HTTP response.</value>
        public IHttpResponse HttpResponse { get; set; }

        /// <summary>Gets or sets the writer.</summary>
        ///
        /// <value>The writer.</value>
        public StreamWriter Writer { get; set; }

        /// <summary>Gets or sets the view engine.</summary>
        ///
        /// <value>The view engine.</value>
        public IViewEngine ViewEngine { get; set; }

        /// <summary>Gets the razor page.</summary>
        ///
        /// <value>The razor page.</value>
        public IRazorView RazorPage { get; protected set; }

        /// <summary>Gets the markdown page.</summary>
        ///
        /// <value>The markdown page.</value>
        public MarkdownPage MarkdownPage { get; protected set; }

        /// <summary>Gets the scope arguments.</summary>
        ///
        /// <value>The scope arguments.</value>
		public Dictionary<string, object> ScopeArgs { get; protected set; }
	    private ViewDataDictionary viewData;

        /// <summary>Initialises this object.</summary>
        ///
        /// <param name="viewEngine">The view engine.</param>
        /// <param name="httpReq">   The HTTP request.</param>
        /// <param name="httpRes">   The HTTP resource.</param>
        /// <param name="razorPage"> The razor page.</param>
        /// <param name="scopeArgs"> The scope arguments.</param>
        /// <param name="viewData">  Information describing the view.</param>
        public void Init(IViewEngine viewEngine, IHttpRequest httpReq, IHttpResponse httpRes, IRazorView razorPage, 
            Dictionary<string, object> scopeArgs = null, ViewDataDictionary viewData = null)
        {
            ViewEngine = viewEngine;
            HttpRequest = httpReq;
            HttpResponse = httpRes;
            RazorPage = razorPage;
            //ScopeArgs = scopeArgs;
            this.viewData = viewData;
        }

	    private static int counter = 0;
        private int id = 0;

        /// <summary>Initializes a new instance of the NServiceKit.Html.HtmlHelper class.</summary>
	    public HtmlHelper()
	    {
            this.RenderHtml = true;
            id = counter++;
	    }

        /// <summary>Initialises this object.</summary>
        ///
        /// <param name="markdownPage">The markdown page.</param>
        /// <param name="scopeArgs">   The scope arguments.</param>
        /// <param name="renderHtml">  true to render HTML.</param>
        /// <param name="viewData">    Information describing the view.</param>
        /// <param name="htmlHelper">  The HTML helper.</param>
        public void Init(MarkdownPage markdownPage, Dictionary<string, object> scopeArgs,
            bool renderHtml, ViewDataDictionary viewData, HtmlHelper htmlHelper)
		{
            Init(null, null, markdownPage.Markdown, viewData, htmlHelper);

            this.RenderHtml = renderHtml;
			this.MarkdownPage = markdownPage;
			this.ScopeArgs = scopeArgs;
		}

        /// <summary>Initialises this object.</summary>
        ///
        /// <param name="httpReq">   The HTTP request.</param>
        /// <param name="httpRes">   The HTTP resource.</param>
        /// <param name="viewEngine">The view engine.</param>
        /// <param name="viewData">  Information describing the view.</param>
        /// <param name="htmlHelper">The HTML helper.</param>
        public void Init(IHttpRequest httpReq, IHttpResponse httpRes, IViewEngine viewEngine, ViewDataDictionary viewData, HtmlHelper htmlHelper)
		{
            this.RenderHtml = true;
            this.HttpRequest = httpReq ?? (htmlHelper != null ? htmlHelper.HttpRequest : null);
            this.HttpResponse = httpRes ?? (htmlHelper != null ? htmlHelper.HttpResponse : null);
            this.ViewEngine = viewEngine;
			this.ViewData = viewData;
			this.ViewData.PopulateModelState();
		}

        /// <summary>Partials.</summary>
        ///
        /// <param name="viewName">Name of the view.</param>
        ///
        /// <returns>A MvcHtmlString.</returns>
		public MvcHtmlString Partial(string viewName)
		{
		    return Partial(viewName, null);
		}

        /// <summary>Partials.</summary>
        ///
        /// <param name="viewName">Name of the view.</param>
        /// <param name="model">   The model.</param>
        ///
        /// <returns>A MvcHtmlString.</returns>
		public MvcHtmlString Partial(string viewName, object model)
		{
		    var masterModel = this.viewData;
            try
            {
                this.viewData = new ViewDataDictionary(model);
                var result = ViewEngine.RenderPartial(viewName, model, this.RenderHtml, Writer, this);
                return MvcHtmlString.Create(result);
            }
            finally
            {
                this.viewData = masterModel;
            }
        }

        /// <summary>Debugs the given model.</summary>
        ///
        /// <param name="model">The model.</param>
        ///
        /// <returns>A string.</returns>
        public string Debug(object model)
        {
            if (model != null)
            {
                model.PrintDump();
            }

            return null;
        }

        /// <summary>Gets or sets a value indicating whether the client validation is enabled.</summary>
        ///
        /// <value>true if client validation enabled, false if not.</value>
        public static bool ClientValidationEnabled
        {
            get { return ViewContext.GetClientValidationEnabled(); }
            set { ViewContext.SetClientValidationEnabled(value); }
        }

        internal Func<string, ModelMetadata, IEnumerable<ModelClientValidationRule>> ClientValidationRuleFactory { get; set; }

        /// <summary>Gets or sets a value indicating whether the unobtrusive java script is enabled.</summary>
        ///
        /// <value>true if unobtrusive java script enabled, false if not.</value>
        public static bool UnobtrusiveJavaScriptEnabled
        {
            get { return ViewContext.GetUnobtrusiveJavaScriptEnabled(); }
            set { ViewContext.SetUnobtrusiveJavaScriptEnabled(value); }
        }

#if NET_4_0
        public dynamic ViewBag
        {
            get
            {
                if (_dynamicViewDataDictionary == null) {
                    _dynamicViewDataDictionary = new DynamicViewDataDictionary(() => ViewData);
                }
                return _dynamicViewDataDictionary;
            }
        }
#endif

        /// <summary>Gets a context for the view.</summary>
        ///
        /// <value>The view context.</value>
        public ViewContext ViewContext { get; private set; }

        /// <summary>Gets information describing the view.</summary>
        ///
        /// <value>Information describing the view.</value>
	    public ViewDataDictionary ViewData
	    {
	        get { return viewData ?? (viewData = new ViewDataDictionary()); }
	        protected set { viewData = value; }
	    }

        /// <summary>Sets a model.</summary>
        ///
        /// <param name="model">The model.</param>
        public void SetModel(object model)
        {
			ViewData.Model = model;
        }

        /// <summary>Gets the view data container.</summary>
        ///
        /// <value>The view data container.</value>
        public IViewDataContainer ViewDataContainer { get; internal set; }

        /// <summary>Anonymous object to HTML attributes.</summary>
        ///
        /// <param name="htmlAttributes">The HTML attributes.</param>
        ///
        /// <returns>A RouteValueDictionary.</returns>
		public static RouteValueDictionary AnonymousObjectToHtmlAttributes(object htmlAttributes)
		{
			var result = new RouteValueDictionary();

			if (htmlAttributes != null)
			{
				foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(htmlAttributes))
				{
					result.Add(property.Name.Replace('_', '-'), property.GetValue(htmlAttributes));
				}
			}

			return result;
		}

        /// <summary>Anti forgery token.</summary>
        ///
        /// <returns>A MvcHtmlString.</returns>
        public MvcHtmlString AntiForgeryToken()
        {
            return MvcHtmlString.Create(AntiForgery.GetHtml().ToString());
        }

        /// <summary>Anti forgery token.</summary>
        ///
        /// <exception cref="NotSupportedException">Thrown when the requested operation is not supported.</exception>
        ///
        /// <param name="salt">The salt.</param>
        ///
        /// <returns>A MvcHtmlString.</returns>
        public MvcHtmlString AntiForgeryToken(string salt)
        {
            if (!String.IsNullOrEmpty(salt)) {
                throw new NotSupportedException("This method is deprecated. Use the AntiForgeryToken() method instead. To specify custom data to be embedded within the token, use the static AntiForgeryConfig.AdditionalDataProvider property.");
            }

            return AntiForgeryToken();
        }

        /// <summary>Anti forgery token.</summary>
        ///
        /// <exception cref="NotSupportedException">Thrown when the requested operation is not supported.</exception>
        ///
        /// <param name="salt">  The salt.</param>
        /// <param name="domain">The domain.</param>
        /// <param name="path">  Full pathname of the file.</param>
        ///
        /// <returns>A MvcHtmlString.</returns>
        public MvcHtmlString AntiForgeryToken(string salt, string domain, string path)
        {
            if (!String.IsNullOrEmpty(salt) || !String.IsNullOrEmpty(domain) || !String.IsNullOrEmpty(path)) {
                throw new NotSupportedException("This method is deprecated. Use the AntiForgeryToken() method instead. To specify a custom domain for the generated cookie, use the <httpCookies> configuration element. To specify custom data to be embedded within the token, use the static AntiForgeryConfig.AdditionalDataProvider property.");
            }

            return AntiForgeryToken();
        }

        /// <summary>Attribute encode.</summary>
        ///
        /// <param name="value">The value.</param>
        ///
        /// <returns>A string.</returns>
		public string AttributeEncode(string value)
		{
			return !string.IsNullOrEmpty(value) ? HttpUtility.HtmlAttributeEncode(value) : String.Empty;
		}

        /// <summary>Attribute encode.</summary>
        ///
        /// <param name="value">The value.</param>
        ///
        /// <returns>A string.</returns>
		public string AttributeEncode(object value)
		{
			return AttributeEncode(Convert.ToString(value, CultureInfo.InvariantCulture));
		}

        /// <summary>Enables the client validation.</summary>
        public void EnableClientValidation()
        {
            EnableClientValidation(enabled: true);
        }

        /// <summary>Enables the client validation.</summary>
        ///
        /// <param name="enabled">true to enable, false to disable.</param>
        public void EnableClientValidation(bool enabled)
        {
            ViewContext.ClientValidationEnabled = enabled;
        }

        /// <summary>Enables the unobtrusive java script.</summary>
        public void EnableUnobtrusiveJavaScript()
        {
            EnableUnobtrusiveJavaScript(enabled: true);
        }

        /// <summary>Enables the unobtrusive java script.</summary>
        ///
        /// <param name="enabled">true to enable, false to disable.</param>
        public void EnableUnobtrusiveJavaScript(bool enabled)
        {
            ViewContext.UnobtrusiveJavaScriptEnabled = enabled;
        }

        /// <summary>Encodes the given value.</summary>
        ///
        /// <param name="value">The value.</param>
        ///
        /// <returns>A string.</returns>
        public string Encode(string value)
        {
            return (!String.IsNullOrEmpty(value)) ? HttpUtility.HtmlEncode(value) : String.Empty;
        }

        /// <summary>Encodes the given value.</summary>
        ///
        /// <param name="value">The value.</param>
        ///
        /// <returns>A string.</returns>
		public string Encode(object value)
		{
			return htmlEncoder(value);
		}

		// method used if HttpUtility.HtmlEncode(object) method does not exist
		private static string EncodeLegacy(object value)
		{
			var stringVal = Convert.ToString(value, CultureInfo.CurrentCulture);
			return !string.IsNullOrEmpty(stringVal) ? HttpUtility.HtmlEncode(stringVal) : String.Empty;
		}

		// selects the v3.5 (legacy) or v4 HTML encoder
		private static HtmlEncoder GetHtmlEncoder()
		{
			return TypeHelpers.CreateDelegate<HtmlEncoder>(TypeHelpers.SystemWebAssembly, "System.Web.HttpUtility", "HtmlEncode", null)
				?? EncodeLegacy;
		}

        internal string EvalString(string key)
        {
            return Convert.ToString(ViewData.Eval(key), CultureInfo.CurrentCulture);
        }

        internal string EvalString(string key, string format)
        {
            return Convert.ToString(ViewData.Eval(key, format), CultureInfo.CurrentCulture);
        }

        /// <summary>Format value.</summary>
        ///
        /// <param name="value"> The value.</param>
        /// <param name="format">Describes the format to use.</param>
        ///
        /// <returns>The formatted value.</returns>
        public string FormatValue(object value, string format)
        {
            return ViewDataDictionary.FormatValueInternal(value, format);
        }

        internal bool EvalBoolean(string key)
        {
            return Convert.ToBoolean(ViewData.Eval(key), CultureInfo.InvariantCulture);
        }

        /// <summary>Generates an identifier from name.</summary>
        ///
        /// <param name="name">The name.</param>
        ///
        /// <returns>The identifier from name.</returns>
        public static string GenerateIdFromName(string name)
        {
            return GenerateIdFromName(name, TagBuilder.IdAttributeDotReplacement);
        }

        /// <summary>Generates an identifier from name.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="name">                     The name.</param>
        /// <param name="idAttributeDotReplacement">The identifier attribute dot replacement.</param>
        ///
        /// <returns>The identifier from name.</returns>
        public static string GenerateIdFromName(string name, string idAttributeDotReplacement)
        {
            if (name == null) {
                throw new ArgumentNullException("name");
            }

            if (idAttributeDotReplacement == null) {
                throw new ArgumentNullException("idAttributeDotReplacement");
            }

            // TagBuilder.CreateSanitizedId returns null for empty strings, return String.Empty instead to avoid breaking change
            if (name.Length == 0) {
                return String.Empty;
            }

            return TagBuilder.CreateSanitizedId(name, idAttributeDotReplacement);
        }

        /// <summary>Gets form method string.</summary>
        ///
        /// <param name="method">The method.</param>
        ///
        /// <returns>The form method string.</returns>
        public static string GetFormMethodString(FormMethod method)
        {
            switch (method) {
                case FormMethod.Get:
                    return "get";
                case FormMethod.Post:
                    return "post";
                default:
                    return "post";
            }
        }

        /// <summary>Gets input type string.</summary>
        ///
        /// <param name="inputType">Type of the input.</param>
        ///
        /// <returns>The input type string.</returns>
		public static string GetInputTypeString(InputType inputType)
		{
			switch (inputType)
			{
				case InputType.CheckBox:
					return "checkbox";
				case InputType.Hidden:
					return "hidden";
				case InputType.Password:
					return "password";
				case InputType.Radio:
					return "radio";
				case InputType.Text:
					return "text";
				default:
					return "text";
			}
		}

        internal object GetModelStateValue(string key, Type destinationType)
        {
            ModelState modelState;
            if (ViewData.ModelState.TryGetValue(key, out modelState)) {
                if (modelState.Value != null) {
                    return modelState.Value.ConvertTo(destinationType, null /* culture */);
                }
            }
            return null;
        }

        /// <summary>Gets unobtrusive validation attributes.</summary>
        ///
        /// <param name="name">The name.</param>
        ///
        /// <returns>The unobtrusive validation attributes.</returns>
        public IDictionary<string, object> GetUnobtrusiveValidationAttributes(string name)
        {
            return GetUnobtrusiveValidationAttributes(name, metadata: null);
        }

        /// <summary>
        /// Only render attributes if unobtrusive client-side validation is enabled, and then only if we've never rendered validation for a field with this name in this form. Also, if there's no form
        /// context, then we can't render the attributes (we'd have no form to attach them to).
        /// </summary>
        ///
        /// <param name="name">    The name.</param>
        /// <param name="metadata">The metadata.</param>
        ///
        /// <returns>The unobtrusive validation attributes.</returns>
        public IDictionary<string, object> GetUnobtrusiveValidationAttributes(string name, ModelMetadata metadata)
        {
            Dictionary<string, object> results = new Dictionary<string, object>();

            //TODO: Awaiting implementation of ViewContext setter
            /*
            // The ordering of these 3 checks (and the early exits) is for performance reasons.
            if (!ViewContext.UnobtrusiveJavaScriptEnabled) {
                return results;
            }

            FormContext formContext = ViewContext.GetFormContextForClientValidation();
            if (formContext == null) {
                return results;
            }

            string fullName = ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            if (formContext.RenderedField(fullName)) {
                return results;
            }

            formContext.RenderedField(fullName, true);

            IEnumerable<ModelClientValidationRule> clientRules = ClientValidationRuleFactory(name, metadata);
            UnobtrusiveValidationAttributesGenerator.GetValidationAttributes(clientRules, results);
            */
            return results;
        }

        /// <summary>HTTP method override.</summary>
        ///
        /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or illegal values.</exception>
        ///
        /// <param name="httpVerb">The HTTP verb.</param>
        ///
        /// <returns>A MvcHtmlString.</returns>
        public MvcHtmlString HttpMethodOverride(HttpVerbs httpVerb)
        {
            string httpMethod;
            switch (httpVerb) {
                case HttpVerbs.Delete:
                    httpMethod = "DELETE";
                    break;
                case HttpVerbs.Head:
                    httpMethod = "HEAD";
                    break;
                case HttpVerbs.Put:
                    httpMethod = "PUT";
                    break;
                case HttpVerbs.Patch:
                    httpMethod = "PATCH";
                    break;
                case HttpVerbs.Options:
                    httpMethod = "OPTIONS";
                    break;
                default:
                    throw new ArgumentException(MvcResources.HtmlHelper_InvalidHttpVerb, "httpVerb");
            }

            return HttpMethodOverride(httpMethod);
        }

        /// <summary>HTTP method override.</summary>
        ///
        /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or illegal values.</exception>
        ///
        /// <param name="httpMethod">The HTTP method.</param>
        ///
        /// <returns>A MvcHtmlString.</returns>
		public MvcHtmlString HttpMethodOverride(string httpMethod)
		{
			if (String.IsNullOrEmpty(httpMethod))
			{
				throw new ArgumentException(MvcResources.Common_NullOrEmpty, "httpMethod");
			}
			if (String.Equals(httpMethod, "GET", StringComparison.OrdinalIgnoreCase) ||
				String.Equals(httpMethod, "POST", StringComparison.OrdinalIgnoreCase))
			{
				throw new ArgumentException(MvcResources.HtmlHelper_InvalidHttpMethod, "httpMethod");
			}

			TagBuilder tagBuilder = new TagBuilder("input");
			tagBuilder.Attributes["type"] = "hidden";
			tagBuilder.Attributes["name"] = HttpHeaders.XHttpMethodOverride;
			tagBuilder.Attributes["value"] = httpMethod;

			return tagBuilder.ToHtmlString(TagRenderMode.SelfClosing);
		}

        /// <summary>Raws the given content.</summary>
        ///
        /// <param name="content">The content.</param>
        ///
        /// <returns>A MvcHtmlString.</returns>
        public MvcHtmlString Raw(object content)
		{
			if (content == null) return null;
			var strContent = content as string;
            return MvcHtmlString.Create(strContent ?? content.ToString()); //MvcHtmlString
		}
    }

    /// <summary>A HTML helper extensions.</summary>
	public static class HtmlHelperExtensions
	{
        /// <summary>A HtmlHelper extension method that gets HTTP request.</summary>
        ///
        /// <param name="html">The HTML to act on.</param>
        ///
        /// <returns>The HTTP request.</returns>
	    public static IHttpRequest GetHttpRequest(this HtmlHelper html)
	    {
	        return html != null ? html.HttpRequest : null;
	    }
	}

}