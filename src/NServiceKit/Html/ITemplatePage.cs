using System.Collections.Generic;
using NServiceKit.WebHost.Endpoints;

namespace NServiceKit.Html
{
    /// <summary>Interface for template page.</summary>
	public interface ITemplatePage
	{
        /// <summary>Gets or sets the view engine.</summary>
        ///
        /// <value>The view engine.</value>
        IViewEngine ViewEngine { get; set; }

        /// <summary>Gets or sets the application host.</summary>
        ///
        /// <value>The application host.</value>
        IAppHost AppHost { get; set; }

        /// <summary>Gets the get.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        ///
        /// <returns>A T.</returns>
		T Get<T>();

        /// <summary>Gets or sets the scope arguments.</summary>
        ///
        /// <value>The scope arguments.</value>
		Dictionary<string, object> ScopeArgs { get; set; }
	}
}