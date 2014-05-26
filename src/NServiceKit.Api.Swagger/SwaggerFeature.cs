using System.Linq;
using System.Text.RegularExpressions;
using NServiceKit.WebHost.Endpoints;

namespace NServiceKit.Api.Swagger
{
    /// <summary>A swagger feature.</summary>
    public class SwaggerFeature : IPlugin
    {
        public const string SwaggerVersion = "1.1";

        /// <summary>
        /// Gets or sets <see cref="Regex"/> pattern to filter available resources. 
        /// </summary>
        public string ResourceFilterPattern { get; set; }

        /// <summary>Gets or sets a value indicating whether this object use camel case model property names.</summary>
        ///
        /// <value>true if use camel case model property names, false if not.</value>
        public bool UseCamelCaseModelPropertyNames { get; set; }

        /// <summary>Gets or sets a value indicating whether this object use lowercase underscore model property names.</summary>
        ///
        /// <value>true if use lowercase underscore model property names, false if not.</value>
        public bool UseLowercaseUnderscoreModelPropertyNames { get; set; }

        /// <summary>Gets or sets a value indicating whether the automatic dto in body parameter is disabled.</summary>
        ///
        /// <value>true if disable automatic dto in body parameter, false if not.</value>
        public bool DisableAutoDtoInBodyParam { get; set; }

        /// <summary>Registers this object.</summary>
        ///
        /// <param name="appHost">The application host.</param>
        public void Register(IAppHost appHost)
        {
            if (ResourceFilterPattern != null)
                SwaggerResourcesService.ResourceFilterRegex = new Regex(ResourceFilterPattern, RegexOptions.Compiled);

            SwaggerApiService.UseCamelCaseModelPropertyNames = UseCamelCaseModelPropertyNames;
            SwaggerApiService.UseLowercaseUnderscoreModelPropertyNames = UseLowercaseUnderscoreModelPropertyNames;
            SwaggerApiService.DisableAutoDtoInBodyParam = DisableAutoDtoInBodyParam;

            appHost.RegisterService(typeof(SwaggerResourcesService), new[] { "/resources" });
            appHost.RegisterService(typeof(SwaggerApiService), new[] { SwaggerResourcesService.ResourcePath + "/{Name*}" });
        }

        /// <summary>Gets a value indicating whether this object is enabled.</summary>
        ///
        /// <value>true if this object is enabled, false if not.</value>
        public static bool IsEnabled
        {
            get { return EndpointHost.Plugins.Any(x => x is SwaggerFeature); }
        }
    }
}