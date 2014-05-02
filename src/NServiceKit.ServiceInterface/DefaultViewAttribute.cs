using System;
using NServiceKit.ServiceHost;

namespace NServiceKit.ServiceInterface
{
    /// <summary>Attribute for default view.</summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class DefaultViewAttribute : RequestFilterAttribute
    {
        /// <summary>Gets or sets the view.</summary>
        ///
        /// <value>The view.</value>
        public string View { get; set; }

        /// <summary>Gets or sets the template.</summary>
        ///
        /// <value>The template.</value>
        public string Template { get; set; }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.DefaultViewAttribute class.</summary>
        public DefaultViewAttribute() { }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.DefaultViewAttribute class.</summary>
        ///
        /// <param name="view">The view.</param>
        public DefaultViewAttribute(string view) : this(view, null) { }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.DefaultViewAttribute class.</summary>
        ///
        /// <param name="view">    The view.</param>
        /// <param name="template">The template.</param>
        public DefaultViewAttribute(string view, string template)
        {
            View = view;
            Template = template;
        }

        /// <summary>This method is only executed if the HTTP method matches the <see cref="ApplyTo"/> property.</summary>
        ///
        /// <param name="req">       The http request wrapper.</param>
        /// <param name="res">       The http response wrapper.</param>
        /// <param name="requestDto">The request DTO.</param>
        public override void Execute(IHttpRequest req, IHttpResponse res, object requestDto)
        {
            if (!string.IsNullOrEmpty(View))
            {
                object currentView;
                if (!req.Items.TryGetValue("View", out currentView) || string.IsNullOrEmpty(currentView as string))
                    req.Items["View"] = View;
            }
            if (!string.IsNullOrEmpty(Template))
            {
                object currentTemplate;
                if (!req.Items.TryGetValue("Template", out currentTemplate) || string.IsNullOrEmpty(currentTemplate as string))
                    req.Items["Template"] = Template;
            }
        }
    }
}