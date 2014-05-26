namespace NServiceKit.Api.Swagger
{
    using System.Linq;
    using System.Text.RegularExpressions;

    using NServiceKit.WebHost.Endpoints;
    using NServiceKit.Razor;

    /// <summary>
    /// As servicestack swagger plugin that is compatible with Swagger 1.1.
    /// </summary>
    public class SwaggerEmbeddedFeature : IPlugin
    {
        /// <summary>
        /// The swagger version this plugin is compatible with.
        /// </summary>
        public const string SwaggerVersion = "1.1";

        /// <summary>
        /// The default swagger url to bind to.
        /// </summary>
        public const string DefaultSwaggerRouteUrl = "/api-docs";

        /// <summary>
        /// The default url to run the swagger ui at.
        /// </summary>
        public const string DefaultSwaggerUiRouteUrl = "/swaggerui";

        /// <inheritdoc/>
        public static bool IsEnabled
        {
            get
            {
                return EndpointHost.Plugins.Any(x => x is SwaggerEmbeddedFeature);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether disable auto dto in body param.
        /// </summary>
        public bool DisableAutoDtoInBodyParam { get; set; }

        /// <summary>
        /// Gets or sets <see cref="Regex"/> pattern to filter available resources. 
        /// </summary>
        public string ResourceFilterPattern { get; set; }

        /// <summary>
        /// Gets or sets <see cref="Regex"/> pattern to filter available resources by their path. 
        /// </summary>
        public string ResourcePathFilterPattern { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether use camel case model property names.
        /// </summary>
        public bool UseCamelCaseModelPropertyNames { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether use lowercase underscore model property names.
        /// </summary>
        public bool UseLowercaseUnderscoreModelPropertyNames { get; set; }

        /// <summary>
        /// The route to publish the swagger data on.
        /// Defaults to /api-docs if not provided (the value in DefaultSwaggerRouteUrl).
        /// </summary>
        public string SwaggerRoute { get; set; }

        /// <summary>
        /// What URI to display the swagger UI at, if enabled.
        /// </summary>
        public string SwaggerUiRoute { get; set; }

        /// <summary>
        /// If set to true, the swagger UI will be returned from GET requests made to the SwaggerUiRoute.
        /// </summary>
        public bool SwaggerUiEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the API version to use.
        /// </summary>
        public string ApiVersion { get; set; }

        /// <inheritdoc/>
        public void Register(IAppHost appHost)
        {
            if (string.IsNullOrEmpty(this.SwaggerRoute))
            {
                this.SwaggerRoute = DefaultSwaggerRouteUrl;
            }

            if (string.IsNullOrEmpty(this.SwaggerUiRoute))
            {
                this.SwaggerUiRoute = DefaultSwaggerUiRouteUrl;
            }

            if (!string.IsNullOrEmpty(this.ResourceFilterPattern))
            {
                SwaggerResourcesService.ResourceFilterRegex = new Regex(
                    this.ResourceFilterPattern,
                    RegexOptions.Compiled);
            }

            if (!string.IsNullOrEmpty(this.ResourcePathFilterPattern))
            {
                SwaggerResourcesService.ResourcePathFilterRegex = new Regex(
                    this.ResourcePathFilterPattern,
                    RegexOptions.Compiled);
            }

            SwaggerResourcesService.ApiVersion = this.ApiVersion;
            SwaggerResourcesService.ResourcePath = this.SwaggerRoute;

            SwaggerApiService.UseCamelCaseModelPropertyNames = this.UseCamelCaseModelPropertyNames;
            SwaggerApiService.UseLowercaseUnderscoreModelPropertyNames =
                this.UseLowercaseUnderscoreModelPropertyNames;
            SwaggerApiService.DisableAutoDtoInBodyParam = this.DisableAutoDtoInBodyParam;
            SwaggerApiService.ApiVersion = this.ApiVersion;

            appHost.RegisterService(typeof(SwaggerResourcesService), new[] { this.SwaggerRoute });
            appHost.RegisterService(
                typeof(SwaggerApiService),
                new[] { this.SwaggerRoute + "/{Name*}" });

            if (this.SwaggerUiEnabled)
            {
                appHost.RegisterService(typeof(SwaggerUiService), new[] { this.SwaggerUiRoute });
            }
        }
    }
}