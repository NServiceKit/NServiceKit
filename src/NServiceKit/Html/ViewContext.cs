using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Web;

namespace NServiceKit.Html
{
    using IView = ITemplatePage;

    /// <summary>A view context.</summary>
    public class ViewContext
    {
        private const string ClientValidationScript = @"<script type=""text/javascript"">
//<![CDATA[
if (!window.mvcClientValidationMetadata) {{ window.mvcClientValidationMetadata = []; }}
window.mvcClientValidationMetadata.push({0});
//]]>
</script>";

        internal static readonly string ClientValidationKeyName = "ClientValidationEnabled";
        internal static readonly string UnobtrusiveJavaScriptKeyName = "UnobtrusiveJavaScriptEnabled";

        // Some values have to be stored in HttpContext.Items in order to be propagated between calls
        // to RenderPartial(), RenderAction(), etc.
        private static readonly object _formContextKey = new object();
        private static readonly object _lastFormNumKey = new object();

        private Func<IDictionary<object, object>> _scopeThunk;
        private IDictionary<object, object> _transientScope;

        private HttpContextBase _httpContext;

#if NET_4_0
        private DynamicViewDataDictionary _dynamicViewDataDictionary;
#endif
        private Func<string> _formIdGenerator;

        // We need a default FormContext if the user uses html <form> instead of an MvcForm
        private FormContext _defaultFormContext = new FormContext();

        /// <summary>parameterless constructor used for mocking.</summary>
        public ViewContext()
        {
        }

        /// <summary>Initializes a new instance of the NServiceKit.Html.ViewContext class.</summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or illegal values.</exception>
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="httpContext">The HTTP context.</param>
        /// <param name="view">       The view.</param>
        /// <param name="viewData">   Information describing the view.</param>
        /// <param name="writer">     The writer.</param>
        public ViewContext(HttpContextBase httpContext, IView view, ViewDataDictionary viewData, TextWriter writer)
        {
            if (httpContext == null) {
                throw new ArgumentException("httpContext");
            }
            if (view == null) {
                throw new ArgumentNullException("view");
            }
            if (viewData == null) {
                throw new ArgumentNullException("viewData");
            }
            if (writer == null) {
                throw new ArgumentNullException("writer");
            }

            View = view;
            ViewData = viewData;
            Writer = writer;
            HttpContext = httpContext;
        }

        /// <summary>Gets or sets a value indicating whether the client validation is enabled.</summary>
        ///
        /// <value>true if client validation enabled, false if not.</value>
        public virtual bool ClientValidationEnabled
        {
            get { return GetClientValidationEnabled(Scope, HttpContext); }
            set { SetClientValidationEnabled(value, Scope, HttpContext); }
        }

        /// <summary>Gets or sets a context for the form.</summary>
        ///
        /// <value>The form context.</value>
        public virtual FormContext FormContext
        {
            get
            {
                // Never return a null form context, this is important for validation purposes
                return HttpContext.Items[_formContextKey] as FormContext ?? _defaultFormContext;
            }
            set { HttpContext.Items[_formContextKey] = value; }
        }

        internal Func<string> FormIdGenerator
        {
            get
            {
                if (_formIdGenerator == null) {
                    _formIdGenerator = DefaultFormIdGenerator;
                }
                return _formIdGenerator;
            }
            set { _formIdGenerator = value; }
        }

        internal static Func<IDictionary<object, object>> GlobalScopeThunk { get; set; }

        private IDictionary<object, object> Scope
        {
            get
            {
                if (ScopeThunk != null) {
                    return ScopeThunk();
                }
                if (_transientScope == null) {
                    _transientScope = new Dictionary<object, object>();
                }
                return _transientScope;
            }
        }

        internal Func<IDictionary<object, object>> ScopeThunk
        {
            get { return _scopeThunk ?? GlobalScopeThunk; }
            set { _scopeThunk = value; }
        }

        /// <summary>Gets or sets a value indicating whether the unobtrusive java script is enabled.</summary>
        ///
        /// <value>true if unobtrusive java script enabled, false if not.</value>
        public virtual bool UnobtrusiveJavaScriptEnabled
        {
            get { return GetUnobtrusiveJavaScriptEnabled(Scope, HttpContext); }
            set { SetUnobtrusiveJavaScriptEnabled(value, Scope, HttpContext); }
        }

        /// <summary>Gets or sets the view.</summary>
        ///
        /// <value>The view.</value>
        public virtual IView View { get; set; }

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

        /// <summary>Gets or sets information describing the view.</summary>
        ///
        /// <value>Information describing the view.</value>
        public virtual ViewDataDictionary ViewData { get; set; }

        /// <summary>Gets or sets the writer.</summary>
        ///
        /// <value>The writer.</value>
        public virtual TextWriter Writer { get; set; }

        private string DefaultFormIdGenerator()
        {
            int formNum = IncrementFormCount(HttpContext.Items);
            return String.Format(CultureInfo.InvariantCulture, "form{0}", formNum);
        }

        internal static bool GetClientValidationEnabled(IDictionary<object, object> scope = null, HttpContextBase httpContext = null)
        {
            return ScopeCache.Get(scope, httpContext).ClientValidationEnabled;
        }

        internal FormContext GetFormContextForClientValidation()
        {
            return (ClientValidationEnabled) ? FormContext : null;
        }

        internal static bool GetUnobtrusiveJavaScriptEnabled(IDictionary<object, object> scope = null, HttpContextBase httpContext = null)
        {
            return ScopeCache.Get(scope, httpContext).UnobtrusiveJavaScriptEnabled;
        }

        private static int IncrementFormCount(IDictionary items)
        {
            object lastFormNum = items[_lastFormNumKey];
            int newFormNum = (lastFormNum != null) ? ((int)lastFormNum) + 1 : 0;
            items[_lastFormNumKey] = newFormNum;
            return newFormNum;
        }

        /// <summary>Output client validation.</summary>
        public void OutputClientValidation()
        {
            FormContext formContext = GetFormContextForClientValidation();
            if (formContext == null || UnobtrusiveJavaScriptEnabled) {
                return; // do nothing
            }

            string scriptWithCorrectNewLines = ClientValidationScript.Replace("\r\n", Environment.NewLine);
            string validationJson = formContext.GetJsonValidationMetadata();
            string formatted = String.Format(CultureInfo.InvariantCulture, scriptWithCorrectNewLines, validationJson);

            Writer.Write(formatted);
        }

        internal static void SetClientValidationEnabled(bool enabled, IDictionary<object, object> scope = null, HttpContextBase httpContext = null)
        {
            ScopeCache.Get(scope, httpContext).ClientValidationEnabled = enabled;
        }

        internal static void SetUnobtrusiveJavaScriptEnabled(bool enabled, IDictionary<object, object> scope = null, HttpContextBase httpContext = null)
        {
            ScopeCache.Get(scope, httpContext).UnobtrusiveJavaScriptEnabled = enabled;
        }

        private static TValue ScopeGet<TValue>(IDictionary<object, object> scope, string name, TValue defaultValue = default(TValue))
        {
            object result;
            if (scope.TryGetValue(name, out result)) {
                return (TValue)Convert.ChangeType(result, typeof(TValue), CultureInfo.InvariantCulture);
            }
            return defaultValue;
        }

        private sealed class ScopeCache
        {
            private static readonly object _cacheKey = new object();
            private bool _clientValidationEnabled;
            private IDictionary<object, object> _scope;
            private bool _unobtrusiveJavaScriptEnabled;

            private ScopeCache(IDictionary<object, object> scope)
            {
                _scope = scope;

                _clientValidationEnabled = ScopeGet(scope, ClientValidationKeyName, false);
                _unobtrusiveJavaScriptEnabled = ScopeGet(scope, UnobtrusiveJavaScriptKeyName, false);
            }

            /// <summary>Gets or sets a value indicating whether the client validation is enabled.</summary>
            ///
            /// <value>true if client validation enabled, false if not.</value>
            public bool ClientValidationEnabled
            {
                get { return _clientValidationEnabled; }
                set
                {
                    _clientValidationEnabled = value;
                    _scope[ClientValidationKeyName] = value;
                }
            }

            /// <summary>Gets or sets a value indicating whether the unobtrusive java script is enabled.</summary>
            ///
            /// <value>true if unobtrusive java script enabled, false if not.</value>
            public bool UnobtrusiveJavaScriptEnabled
            {
                get { return _unobtrusiveJavaScriptEnabled; }
                set
                {
                    _unobtrusiveJavaScriptEnabled = value;
                    _scope[UnobtrusiveJavaScriptKeyName] = value;
                }
            }

            /// <summary>Gets.</summary>
            ///
            /// <param name="scope">      The scope.</param>
            /// <param name="httpContext">Context for the HTTP.</param>
            ///
            /// <returns>A ScopeCache.</returns>
            public static ScopeCache Get(IDictionary<object, object> scope, HttpContextBase httpContext)
            {
                if (httpContext == null && System.Web.HttpContext.Current != null) {
                    httpContext = new HttpContextWrapper(System.Web.HttpContext.Current);
                }

                ScopeCache result = null;
                scope = scope ?? ScopeStorage.CurrentScope;

                if (httpContext != null) {
                    result = httpContext.Items[_cacheKey] as ScopeCache;
                }

                if (result == null || result._scope != scope) {
                    result = new ScopeCache(scope);

                    if (httpContext != null) {
                        httpContext.Items[_cacheKey] = result;
                    }
                }

                return result;
            }
        }

        /// <summary>Gets or sets a context for the HTTP.</summary>
        ///
        /// <value>The HTTP context.</value>
        public virtual HttpContextBase HttpContext
        {
            get
            {
                if (_httpContext == null) {
                    _httpContext = new EmptyHttpContext();
                }
                return _httpContext;
            }
            set { _httpContext = value; }
        }

        private sealed class EmptyHttpContext : HttpContextBase
        {
        }
    }
}
