using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Configuration;
using System.Xml.Linq;
using MarkdownSharp;
using NServiceKit.Common.ServiceModel;
using NServiceKit.Common.Utils;
using NServiceKit.Common.Web;
using NServiceKit.Configuration;
using NServiceKit.Logging;
using NServiceKit.Logging.Support.Logging;
using NServiceKit.Markdown;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceModel;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints.Extensions;
using NServiceKit.WebHost.Endpoints.Support;

namespace NServiceKit.WebHost.Endpoints
{
    /// <summary>An endpoint host configuration.</summary>
    public class EndpointHostConfig
    {
        private static ILog log = LogManager.GetLogger(typeof(EndpointHostConfig));

        /// <summary>The public key.</summary>
        public static readonly string PublicKey = "<RSAKeyValue><Modulus>xRzMrP3m+3kvT6239OP1YuWIfc/S7qF5NJiPe2/kXnetXiuYtSL4bQRIX1qYh4Cz+dXqZE/sNGJJ4jl2iJQa1tjp+rK28EG6gcuTDHJdvOBBF+aSwJy1MSiT8D0KtP6pe2uvjl9m3jZP/8uRePZTSkt/GjlPOk85JXzOsyzemlaLFiJoGImGvp8dw8vQ7jzA3Ynmywpt5OQxklJfrfALHJ93ny1M5lN5Q+bGPEHLXNCXfF05EA0l9mZpa4ouicYvlbY/OAwefFXIwPQN9ER6Pu7Eq9XWLvnh1YUH8HDckuKK+ESWbAuOgnVbUDEF1BreoWutJ//a/oLDR87Q36cmwQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        /// <summary>.</summary>
        public static readonly string LicensePublicKey = "<RSAKeyValue><Modulus>19kx2dJoOIrMYypMTf8ssiCALJ7RS/Iz2QG0rJtYJ2X0+GI+NrgOCapkh/9aDVBieobdClnuBgW08C5QkfBdLRqsptiSu50YIqzVaNBMwZPT0e7Ke02L/fV/M/fVPsolHwzMstKhdWGdK8eNLF4SsLEcvnb79cx3/GnZbXku/ro5eOrTseKL3s4nM4SdMRNn7rEAU0o0Ijb3/RQbhab8IIRB4pHwk1mB+j/mcAQAtMerwpHfwpEBLWlQyVpu0kyKJCEkQjbaPzvfglDRpyBOT5GMUnrcTT/sBr5kSJYpYrgHnA5n4xJnvrnyFqdzXwgGFlikRTbc60pk1cQEWcHgYw==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        /// <summary>The skip path validation.</summary>
        public static bool SkipPathValidation = false;
        /// <summary>
        /// Use: \[Route\("[^\/]  regular expression to find violating routes in your sln
        /// </summary>
        public static bool SkipRouteValidation = false;

        /// <summary>Full pathname of the service kit file.</summary>
        public static string NServiceKitPath = null;

        private static EndpointHostConfig instance;

        /// <summary>Gets the instance.</summary>
        ///
        /// <value>The instance.</value>
        public static EndpointHostConfig Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EndpointHostConfig
                    {
                        MetadataTypesConfig = new MetadataTypesConfig(addDefaultXmlNamespace: "http://schemas.NServiceKit.net/types"),
                        WsdlServiceNamespace = "http://schemas.NServiceKit.net/types",
                        WsdlSoapActionNamespace = "http://schemas.NServiceKit.net/types",
                        MetadataPageBodyHtml = @"<br />
                            <h3><a href=""https://github.com/NServiceKit/NServiceKit/wiki/Clients-overview"">Clients Overview</a></h3>",
                        MetadataOperationPageBodyHtml = @"<br />
                            <h3><a href=""https://github.com/NServiceKit/NServiceKit/wiki/Clients-overview"">Clients Overview</a></h3>",
                        MetadataCustomPath = "Views/Templates/Metadata/",
                        UseCustomMetadataTemplates = false,

                        LogFactory = new NullLogFactory(),
                        EnableAccessRestrictions = true,
                        WebHostPhysicalPath = "~".MapServerPath(),
                        NServiceKitHandlerFactoryPath = NServiceKitPath,
                        MetadataRedirectPath = null,
                        DefaultContentType = null,
                        AllowJsonpRequests = true,
                        AllowRouteContentTypeExtensions = true,
                        AllowNonHttpOnlyCookies = false,
                        UseHttpsLinks = false,
                        DebugMode = false,
                        DefaultDocuments = new List<string> {
							"default.htm",
							"default.html",
							"default.cshtml",
							"default.md",
							"index.htm",
							"index.html",
							"default.aspx",
							"default.ashx",
						},
                        GlobalResponseHeaders = new Dictionary<string, string> { { "X-Powered-By", Env.ServerUserAgent } },
                        IgnoreFormatsInMetadata = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase),
                        AllowFileExtensions = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)
						{
							"js", "css", "htm", "html", "shtm", "txt", "xml", "rss", "csv", "pdf",  
							"jpg", "jpeg", "gif", "png", "bmp", "ico", "tif", "tiff", "svg",
							"avi", "divx", "m3u", "mov", "mp3", "mpeg", "mpg", "qt", "vob", "wav", "wma", "wmv", 
							"flv", "xap", "xaml", "ogg", "mp4", "webm", "eot", "ttf", "woff"
						},
                        DebugAspNetHostEnvironment = Env.IsMono ? "FastCGI" : "IIS7",
                        DebugHttpListenerHostEnvironment = Env.IsMono ? "XSP" : "WebServer20",
                        EnableFeatures = Feature.All,
                        WriteErrorsToResponse = true,
                        ReturnsInnerException = true,
                        MarkdownOptions = new MarkdownOptions(),
                        MarkdownBaseType = typeof(MarkdownViewBase),
                        MarkdownGlobalHelpers = new Dictionary<string, Type>(),
                        HtmlReplaceTokens = new Dictionary<string, string>(),
                        AddMaxAgeForStaticMimeTypes = new Dictionary<string, TimeSpan> {
							{ "image/gif", TimeSpan.FromHours(1) },
							{ "image/png", TimeSpan.FromHours(1) },
							{ "image/jpeg", TimeSpan.FromHours(1) },
						},
                        AppendUtf8CharsetOnContentTypes = new HashSet<string> { ContentType.Json, },
                        RawHttpHandlers = new List<Func<IHttpRequest, IHttpHandler>>(),
                        RouteNamingConventions = new List<RouteNamingConventionDelegate> {
					        RouteNamingConvention.WithRequestDtoName,
					        RouteNamingConvention.WithMatchingAttributes,
					        RouteNamingConvention.WithMatchingPropertyNames
                        },
                        CustomHttpHandlers = new Dictionary<HttpStatusCode, INServiceKitHttpHandler>(),
                        GlobalHtmlErrorHttpHandler = null,
                        MapExceptionToStatusCode = new Dictionary<Type, int>(),
                        OnlySendSessionCookiesSecurely = false,
                        RestrictAllCookiesToDomain = null,
                        DefaultJsonpCacheExpiration = new TimeSpan(0, 20, 0),
                        MetadataVisibility = EndpointAttributes.Any,
                        Return204NoContentForEmptyResponse = true,
                        AllowPartialResponses = true,
                        AllowAclUrlReservation = true,
                        IgnoreWarningsOnPropertyNames = new List<string> {
                            "format", "callback", "debug", "_", "authsecret"
                        }
                    };

                    if (instance.NServiceKitHandlerFactoryPath == null)
                    {
                        InferHttpHandlerPath();
                    }
                }
                return instance;
            }
        }

        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.EndpointHostConfig class.</summary>
        ///
        /// <param name="serviceName">   The name of the service.</param>
        /// <param name="serviceManager">The service manager.</param>
        public EndpointHostConfig(string serviceName, ServiceManager serviceManager)
            : this()
        {
            this.ServiceName = serviceName;
            this.ServiceManager = serviceManager;

        }

        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.EndpointHostConfig class.</summary>
        public EndpointHostConfig()
        {
            if (instance == null) return;

            //Get a copy of the singleton already partially configured
            this.MetadataTypesConfig = instance.MetadataTypesConfig;
            this.WsdlServiceNamespace = instance.WsdlServiceNamespace;
            this.WsdlSoapActionNamespace = instance.WsdlSoapActionNamespace;
            this.MetadataPageBodyHtml = instance.MetadataPageBodyHtml;
            this.MetadataOperationPageBodyHtml = instance.MetadataOperationPageBodyHtml;
            this.MetadataCustomPath = instance.MetadataCustomPath;
            this.UseCustomMetadataTemplates = instance.UseCustomMetadataTemplates;
            this.EnableAccessRestrictions = instance.EnableAccessRestrictions;
            this.ServiceEndpointsMetadataConfig = instance.ServiceEndpointsMetadataConfig;
            this.LogFactory = instance.LogFactory;
            this.EnableAccessRestrictions = instance.EnableAccessRestrictions;
            this.WebHostUrl = instance.WebHostUrl;
            this.WebHostPhysicalPath = instance.WebHostPhysicalPath;
            this.DefaultRedirectPath = instance.DefaultRedirectPath;
            this.MetadataRedirectPath = instance.MetadataRedirectPath;
            this.NServiceKitHandlerFactoryPath = instance.NServiceKitHandlerFactoryPath;
            this.DefaultContentType = instance.DefaultContentType;
            this.AllowJsonpRequests = instance.AllowJsonpRequests;
            this.AllowRouteContentTypeExtensions = instance.AllowRouteContentTypeExtensions;
            this.DebugMode = instance.DebugMode;
            this.DefaultDocuments = instance.DefaultDocuments;
            this.GlobalResponseHeaders = instance.GlobalResponseHeaders;
            this.IgnoreFormatsInMetadata = instance.IgnoreFormatsInMetadata;
            this.AllowFileExtensions = instance.AllowFileExtensions;
            this.EnableFeatures = instance.EnableFeatures;
            this.WriteErrorsToResponse = instance.WriteErrorsToResponse;
            this.ReturnsInnerException = instance.ReturnsInnerException;
            this.MarkdownOptions = instance.MarkdownOptions;
            this.MarkdownBaseType = instance.MarkdownBaseType;
            this.MarkdownGlobalHelpers = instance.MarkdownGlobalHelpers;
            this.HtmlReplaceTokens = instance.HtmlReplaceTokens;
            this.AddMaxAgeForStaticMimeTypes = instance.AddMaxAgeForStaticMimeTypes;
            this.AppendUtf8CharsetOnContentTypes = instance.AppendUtf8CharsetOnContentTypes;
            this.RawHttpHandlers = instance.RawHttpHandlers;
            this.RouteNamingConventions = instance.RouteNamingConventions;
            this.CustomHttpHandlers = instance.CustomHttpHandlers;
            this.GlobalHtmlErrorHttpHandler = instance.GlobalHtmlErrorHttpHandler;
            this.MapExceptionToStatusCode = instance.MapExceptionToStatusCode;
            this.OnlySendSessionCookiesSecurely = instance.OnlySendSessionCookiesSecurely;
            this.RestrictAllCookiesToDomain = instance.RestrictAllCookiesToDomain;
            this.DefaultJsonpCacheExpiration = instance.DefaultJsonpCacheExpiration;
            this.MetadataVisibility = instance.MetadataVisibility;
            this.Return204NoContentForEmptyResponse = Return204NoContentForEmptyResponse;
            this.AllowNonHttpOnlyCookies = instance.AllowNonHttpOnlyCookies;
            this.AllowPartialResponses = instance.AllowPartialResponses;
            this.IgnoreWarningsOnPropertyNames = instance.IgnoreWarningsOnPropertyNames;
            this.PreExecuteServiceFilter = instance.PreExecuteServiceFilter;
            this.PostExecuteServiceFilter = instance.PostExecuteServiceFilter;
            this.FallbackRestPath = instance.FallbackRestPath;
            this.AllowAclUrlReservation = instance.AllowAclUrlReservation;
            this.AdminAuthSecret = instance.AdminAuthSecret;
        }

        /// <summary>Gets application configuration path.</summary>
        ///
        /// <returns>The application configuration path.</returns>
        public static string GetAppConfigPath()
        {
            if (EndpointHost.AppHost == null) return null;

            var configPath = "~/web.config".MapHostAbsolutePath();
            if (File.Exists(configPath))
                return configPath;

            configPath = "~/Web.config".MapHostAbsolutePath(); //*nix FS FTW!
            if (File.Exists(configPath))
                return configPath;

            var appHostDll = new FileInfo(EndpointHost.AppHost.GetType().Assembly.Location).Name;
            configPath = "~/{0}.config".Fmt(appHostDll).MapAbsolutePath();
            return File.Exists(configPath) ? configPath : null;
        }

        const string NamespacesAppSettingsKey = "NServiceKit.razor.namespaces";
        private static HashSet<string> razorNamespaces;

        /// <summary>Gets the razor namespaces.</summary>
        ///
        /// <value>The razor namespaces.</value>
        public static HashSet<string> RazorNamespaces
        {
            get
            {
                if (razorNamespaces != null)
                    return razorNamespaces;

                razorNamespaces = new HashSet<string>();
                //Infer from <system.web.webPages.razor> - what VS.NET's intell-sense uses
                var configPath = GetAppConfigPath();
                if (configPath != null)
                {
                    var xml = configPath.ReadAllText();
                    var doc = XElement.Parse(xml);
                    doc.AnyElement("system.web.webPages.razor")
                        .AnyElement("pages")
                            .AnyElement("namespaces")
                                .AllElements("add").ToList()
                                    .ForEach(x => razorNamespaces.Add(x.AnyAttribute("namespace").Value));
                }

                //E.g. <add key="NServiceKit.razor.namespaces" value="System,NServiceKit.Text" />
                if (ConfigUtils.GetNullableAppSetting(NamespacesAppSettingsKey) != null)
                {
                    ConfigUtils.GetListFromAppSetting(NamespacesAppSettingsKey)
                        .ForEach(x => razorNamespaces.Add(x));
                }

                //log.Debug("Loaded Razor Namespaces: in {0}: {1}: {2}"
                //    .Fmt(configPath, "~/Web.config".MapHostAbsolutePath(), razorNamespaces.Dump()));

                return razorNamespaces;
            }
        }

        private static System.Configuration.Configuration GetAppConfig()
        {
            Assembly entryAssembly;

            //Read the user-defined path in the Web.Config
            if (EndpointHost.AppHost is AppHostBase)
                return WebConfigurationManager.OpenWebConfiguration("~/");

            if ((entryAssembly = Assembly.GetEntryAssembly()) != null)
                return ConfigurationManager.OpenExeConfiguration(entryAssembly.Location);

            return null;
        }

        private static void InferHttpHandlerPath()
        {
            try
            {
                var config = GetAppConfig();
                if (config == null) return;

                SetPathsFromConfiguration(config, null);

                if (instance.MetadataRedirectPath == null)
                {
                    foreach (ConfigurationLocation location in config.Locations)
                    {
                        SetPathsFromConfiguration(location.OpenConfiguration(), (location.Path ?? "").ToLower());

                        if (instance.MetadataRedirectPath != null) { break; }
                    }
                }

                if (!SkipPathValidation && instance.MetadataRedirectPath == null)
                {
                    throw new ConfigurationErrorsException(
                        "Unable to infer NServiceKit's <httpHandler.Path/> from the Web.Config\n"
                        + "Check with http://www.NServiceKit.net/NServiceKit.Hello/ to ensure you have configured NServiceKit properly.\n"
                        + "Otherwise you can explicitly set your httpHandler.Path by setting: EndpointHostConfig.NServiceKitPath");
                }
            }
            catch (Exception) { }
        }

        private static void SetPathsFromConfiguration(System.Configuration.Configuration config, string locationPath)
        {
            if (config == null)
                return;

            //standard config
            var handlersSection = config.GetSection("system.web/httpHandlers") as HttpHandlersSection;
            if (handlersSection != null)
            {
                for (var i = 0; i < handlersSection.Handlers.Count; i++)
                {
                    var httpHandler = handlersSection.Handlers[i];
                    if (!httpHandler.Type.StartsWith("NServiceKit"))
                        continue;

                    SetPaths(httpHandler.Path, locationPath);
                    break;
                }
            }

            //IIS7+ integrated mode system.webServer/handlers
            var pathsNotSet = instance.MetadataRedirectPath == null;
            if (pathsNotSet)
            {
                var webServerSection = config.GetSection("system.webServer");
                if (webServerSection != null)
                {
                    var rawXml = webServerSection.SectionInformation.GetRawXml();
                    if (!String.IsNullOrEmpty(rawXml))
                    {
                        SetPaths(ExtractHandlerPathFromWebServerConfigurationXml(rawXml), locationPath);
                    }
                }

                //In some MVC Hosts auto-inferencing doesn't work, in these cases assume the most likely default of "/api" path
                pathsNotSet = instance.MetadataRedirectPath == null;
                if (pathsNotSet)
                {
                    var isMvcHost = Type.GetType("System.Web.Mvc.Controller") != null;
                    if (isMvcHost)
                    {
                        SetPaths("api", null);
                    }
                }
            }
        }

        private static void SetPaths(string handlerPath, string locationPath)
        {
            if (handlerPath == null) return;

            if (locationPath == null)
            {
                handlerPath = handlerPath.Replace("*", String.Empty);
            }

            instance.NServiceKitHandlerFactoryPath = locationPath ??
                (String.IsNullOrEmpty(handlerPath) ? null : handlerPath);

            instance.MetadataRedirectPath = PathUtils.CombinePaths(
                null != locationPath ? instance.NServiceKitHandlerFactoryPath : handlerPath
                , "metadata");
        }

        private static string ExtractHandlerPathFromWebServerConfigurationXml(string rawXml)
        {
            return XDocument.Parse(rawXml).Root.Element("handlers")
                .Descendants("add")
                .Where(handler => EnsureHandlerTypeAttribute(handler).StartsWith("NServiceKit"))
                .Select(handler => handler.Attribute("path").Value)
                .FirstOrDefault();
        }

        private static string EnsureHandlerTypeAttribute(XElement handler)
        {
            if (handler.Attribute("type") != null && !String.IsNullOrEmpty(handler.Attribute("type").Value))
            {
                return handler.Attribute("type").Value;
            }
            return String.Empty;
        }

        /// <summary>Gets the manager for service.</summary>
        ///
        /// <value>The service manager.</value>
        public ServiceManager ServiceManager { get; internal set; }

        /// <summary>Gets the metadata.</summary>
        ///
        /// <value>The metadata.</value>
        public ServiceMetadata Metadata { get { return ServiceManager.Metadata; } }

        /// <summary>Gets the service controller.</summary>
        ///
        /// <value>The service controller.</value>
        public IServiceController ServiceController { get { return ServiceManager.ServiceController; } }

        /// <summary>Gets or sets the metadata types configuration.</summary>
        ///
        /// <value>The metadata types configuration.</value>
        public MetadataTypesConfig MetadataTypesConfig { get; set; }

        /// <summary>Gets or sets the wsdl service namespace.</summary>
        ///
        /// <value>The wsdl service namespace.</value>
        public string WsdlServiceNamespace { get; set; }

        /// <summary>Gets or sets the wsdl SOAP action namespace.</summary>
        ///
        /// <value>The wsdl SOAP action namespace.</value>
        public string WsdlSoapActionNamespace { get; set; }

        private EndpointAttributes metadataVisibility;

        /// <summary>Gets or sets the metadata visibility.</summary>
        ///
        /// <value>The metadata visibility.</value>
        public EndpointAttributes MetadataVisibility
        {
            get { return metadataVisibility; }
            set { metadataVisibility = value.ToAllowedFlagsSet(); }
        }

        /// <summary>Gets or sets the metadata page body HTML.</summary>
        ///
        /// <value>The metadata page body HTML.</value>
        public string MetadataPageBodyHtml { get; set; }

        /// <summary>Gets or sets the metadata operation page body HTML.</summary>
        ///
        /// <value>The metadata operation page body HTML.</value>
        public string MetadataOperationPageBodyHtml { get; set; }

        /// <summary>Gets or sets the full pathname of the metadata custom file.</summary>
        ///
        /// <value>The full pathname of the metadata custom file.</value>
        public string MetadataCustomPath { get; set; }

        /// <summary>Gets or sets a value indicating whether this object use custom metadata templates.</summary>
        ///
        /// <value>true if use custom metadata templates, false if not.</value>
        public bool UseCustomMetadataTemplates { get; set; }

        /// <summary>Gets or sets the name of the service.</summary>
        ///
        /// <value>The name of the service.</value>
        public string ServiceName { get; set; }

        /// <summary>Gets or sets the name of the SOAP service.</summary>
        ///
        /// <value>The name of the SOAP service.</value>
        public string SoapServiceName { get; set; }

        /// <summary>Gets or sets the default content type.</summary>
        ///
        /// <value>The default content type.</value>
        public string DefaultContentType { get; set; }

        /// <summary>Gets or sets a value indicating whether we allow jsonp requests.</summary>
        ///
        /// <value>true if allow jsonp requests, false if not.</value>
        public bool AllowJsonpRequests { get; set; }

        /// <summary>Gets or sets a value indicating whether we allow route content type extensions.</summary>
        ///
        /// <value>true if allow route content type extensions, false if not.</value>
        public bool AllowRouteContentTypeExtensions { get; set; }

        /// <summary>Gets or sets a value indicating whether the debug mode.</summary>
        ///
        /// <value>true if debug mode, false if not.</value>
        public bool DebugMode { get; set; }

        /// <summary>Gets or sets a value indicating whether the debug only return request information.</summary>
        ///
        /// <value>true if debug only return request information, false if not.</value>
        public bool DebugOnlyReturnRequestInfo { get; set; }

        /// <summary>Gets or sets the debug ASP net host environment.</summary>
        ///
        /// <value>The debug ASP net host environment.</value>
        public string DebugAspNetHostEnvironment { get; set; }

        /// <summary>Gets or sets the debug HTTP listener host environment.</summary>
        ///
        /// <value>The debug HTTP listener host environment.</value>
        public string DebugHttpListenerHostEnvironment { get; set; }

        /// <summary>Gets the default documents.</summary>
        ///
        /// <value>The default documents.</value>
        public List<string> DefaultDocuments { get; private set; }

        /// <summary>Gets a list of names of the ignore warnings on properties.</summary>
        ///
        /// <value>A list of names of the ignore warnings on properties.</value>
        public List<string> IgnoreWarningsOnPropertyNames { get; private set; }

        /// <summary>Gets or sets the ignore formats in metadata.</summary>
        ///
        /// <value>The ignore formats in metadata.</value>
        public HashSet<string> IgnoreFormatsInMetadata { get; set; }

        /// <summary>Gets or sets the allow file extensions.</summary>
        ///
        /// <value>The allow file extensions.</value>
        public HashSet<string> AllowFileExtensions { get; set; }

        /// <summary>Gets or sets URL of the web host.</summary>
        ///
        /// <value>The web host URL.</value>
        public string WebHostUrl { get; set; }

        /// <summary>Gets or sets the full pathname of the web host physical file.</summary>
        ///
        /// <value>The full pathname of the web host physical file.</value>
        public string WebHostPhysicalPath { get; set; }

        /// <summary>Gets or sets the full pathname of the service kit handler factory file.</summary>
        ///
        /// <value>The full pathname of the service kit handler factory file.</value>
        public string NServiceKitHandlerFactoryPath { get; set; }

        /// <summary>Gets or sets the default redirect path.</summary>
        ///
        /// <value>The default redirect path.</value>
        public string DefaultRedirectPath { get; set; }

        /// <summary>Gets or sets the full pathname of the metadata redirect file.</summary>
        ///
        /// <value>The full pathname of the metadata redirect file.</value>
        public string MetadataRedirectPath { get; set; }

        /// <summary>Gets or sets the service endpoints metadata configuration.</summary>
        ///
        /// <value>The service endpoints metadata configuration.</value>
        public ServiceEndpointsMetadataConfig ServiceEndpointsMetadataConfig { get; set; }

        /// <summary>Gets or sets the log factory.</summary>
        ///
        /// <value>The log factory.</value>
        public ILogFactory LogFactory { get; set; }

        /// <summary>Gets or sets a value indicating whether the access restrictions is enabled.</summary>
        ///
        /// <value>true if enable access restrictions, false if not.</value>
        public bool EnableAccessRestrictions { get; set; }

        /// <summary>Gets or sets a value indicating whether this object use bcl JSON serializers.</summary>
        ///
        /// <value>true if use bcl JSON serializers, false if not.</value>
        public bool UseBclJsonSerializers { get; set; }

        /// <summary>Gets or sets the global response headers.</summary>
        ///
        /// <value>The global response headers.</value>
        public Dictionary<string, string> GlobalResponseHeaders { get; set; }

        /// <summary>Gets or sets the enable features.</summary>
        ///
        /// <value>The enable features.</value>
        public Feature EnableFeatures { get; set; }

        /// <summary>Gets or sets a value indicating whether the returns inner exception.</summary>
        ///
        /// <value>true if returns inner exception, false if not.</value>
        public bool ReturnsInnerException { get; set; }

        /// <summary>Gets or sets a value indicating whether the errors to response should be written.</summary>
        ///
        /// <value>true if write errors to response, false if not.</value>
        public bool WriteErrorsToResponse { get; set; }

        /// <summary>Gets or sets options for controlling the markdown.</summary>
        ///
        /// <value>Options that control the markdown.</value>
        public MarkdownOptions MarkdownOptions { get; set; }

        /// <summary>Gets or sets the type of the markdown base.</summary>
        ///
        /// <value>The type of the markdown base.</value>
        public Type MarkdownBaseType { get; set; }

        /// <summary>Gets or sets the markdown global helpers.</summary>
        ///
        /// <value>The markdown global helpers.</value>
        public Dictionary<string, Type> MarkdownGlobalHelpers { get; set; }

        /// <summary>Gets or sets the HTML replace tokens.</summary>
        ///
        /// <value>The HTML replace tokens.</value>
        public Dictionary<string, string> HtmlReplaceTokens { get; set; }

        /// <summary>Gets or sets a list of types of the append UTF 8 charset on contents.</summary>
        ///
        /// <value>A list of types of the append UTF 8 charset on contents.</value>
        public HashSet<string> AppendUtf8CharsetOnContentTypes { get; set; }

        /// <summary>Gets or sets a list of types of the add maximum age for static mimes.</summary>
        ///
        /// <value>A list of types of the add maximum age for static mimes.</value>
        public Dictionary<string, TimeSpan> AddMaxAgeForStaticMimeTypes { get; set; }

        /// <summary>Gets or sets the raw HTTP handlers.</summary>
        ///
        /// <value>The raw HTTP handlers.</value>
        public List<Func<IHttpRequest, IHttpHandler>> RawHttpHandlers { get; set; }

        /// <summary>Gets or sets the route naming conventions.</summary>
        ///
        /// <value>The route naming conventions.</value>
        public List<RouteNamingConventionDelegate> RouteNamingConventions { get; set; }

        /// <summary>Gets or sets the custom HTTP handlers.</summary>
        ///
        /// <value>The custom HTTP handlers.</value>
        public Dictionary<HttpStatusCode, INServiceKitHttpHandler> CustomHttpHandlers { get; set; }

        /// <summary>Gets or sets the global HTML error HTTP handler.</summary>
        ///
        /// <value>The global HTML error HTTP handler.</value>
        public INServiceKitHttpHandler GlobalHtmlErrorHttpHandler { get; set; }

        /// <summary>Gets or sets the map exception to status code.</summary>
        ///
        /// <value>The map exception to status code.</value>
        public Dictionary<Type, int> MapExceptionToStatusCode { get; set; }

        /// <summary>Gets or sets a value indicating whether the only send session cookies securely.</summary>
        ///
        /// <value>true if only send session cookies securely, false if not.</value>
        public bool OnlySendSessionCookiesSecurely { get; set; }

        /// <summary>Gets or sets the restrict all cookies to domain.</summary>
        ///
        /// <value>The restrict all cookies to domain.</value>
        public string RestrictAllCookiesToDomain { get; set; }

        /// <summary>Gets or sets the default jsonp cache expiration.</summary>
        ///
        /// <value>The default jsonp cache expiration.</value>
        public TimeSpan DefaultJsonpCacheExpiration { get; set; }

        /// <summary>Gets or sets a value indicating whether the return 204 no content for empty response.</summary>
        ///
        /// <value>true if return 204 no content for empty response, false if not.</value>
        public bool Return204NoContentForEmptyResponse { get; set; }

        /// <summary>Gets or sets a value indicating whether we allow partial responses.</summary>
        ///
        /// <value>true if allow partial responses, false if not.</value>
        public bool AllowPartialResponses { get; set; }

        /// <summary>Gets or sets a value indicating whether we allow non HTTP only cookies.</summary>
        ///
        /// <value>true if allow non HTTP only cookies, false if not.</value>
        public bool AllowNonHttpOnlyCookies { get; set; }

        /// <summary>Gets or sets a value indicating whether we allow ACL URL reservation.</summary>
        ///
        /// <value>true if allow ACL URL reservation, false if not.</value>
        public bool AllowAclUrlReservation { get; set; }

        /// <summary>Gets or sets a value indicating whether this object use HTTPS links.</summary>
        ///
        /// <value>true if use HTTPS links, false if not.</value>
        public bool UseHttpsLinks { get; set; }

        /// <summary>Gets or sets the admin authentication secret.</summary>
        ///
        /// <value>The admin authentication secret.</value>
        public string AdminAuthSecret { get; set; }

        private string defaultOperationNamespace;

        /// <summary>Gets or sets the default operation namespace.</summary>
        ///
        /// <value>The default operation namespace.</value>
        public string DefaultOperationNamespace
        {
            get
            {
                if (this.defaultOperationNamespace == null)
                {
                    this.defaultOperationNamespace = GetDefaultNamespace();
                }
                return this.defaultOperationNamespace;
            }
            set
            {
                this.defaultOperationNamespace = value;
            }
        }

        private string GetDefaultNamespace()
        {
            if (!String.IsNullOrEmpty(this.defaultOperationNamespace)
                || this.ServiceController == null) return null;

            foreach (var operationType in this.Metadata.RequestTypes)
            {
                var attrs = operationType.GetCustomAttributes(
                    typeof(DataContractAttribute), false);

                if (attrs.Length <= 0) continue;

                var attr = (DataContractAttribute)attrs[0];

                if (String.IsNullOrEmpty(attr.Namespace)) continue;

                return attr.Namespace;
            }

            return null;
        }

        /// <summary>Query if 'feature' has feature.</summary>
        ///
        /// <param name="feature">The feature.</param>
        ///
        /// <returns>true if feature, false if not.</returns>
        public bool HasFeature(Feature feature)
        {
            return (feature & EndpointHost.Config.EnableFeatures) == feature;
        }

        /// <summary>Assert features.</summary>
        ///
        /// <exception cref="UnauthorizedAccessException">Thrown when an Unauthorized Access error condition occurs.</exception>
        ///
        /// <param name="usesFeatures">The uses features.</param>
        public void AssertFeatures(Feature usesFeatures)
        {
            if (EndpointHost.Config.EnableFeatures == Feature.All) return;

            if (!HasFeature(usesFeatures))
            {
                throw new UnauthorizedAccessException(
                    String.Format("'{0}' Features have been disabled by your administrator", usesFeatures));
            }
        }

        /// <summary>Unauthorized access.</summary>
        ///
        /// <param name="requestAttrs">The request attributes.</param>
        ///
        /// <returns>An UnauthorizedAccessException.</returns>
        public UnauthorizedAccessException UnauthorizedAccess(EndpointAttributes requestAttrs)
        {
            return new UnauthorizedAccessException(
                String.Format("Request with '{0}' is not allowed", requestAttrs));
        }

        /// <summary>Assert content type.</summary>
        ///
        /// <param name="contentType">Type of the content.</param>
        public void AssertContentType(string contentType)
        {
            if (EndpointHost.Config.EnableFeatures == Feature.All) return;

            AssertFeatures(contentType.ToFeature());
        }

        /// <summary>Gets the metadata pages configuration.</summary>
        ///
        /// <value>The metadata pages configuration.</value>
        public MetadataPagesConfig MetadataPagesConfig
        {
            get
            {
                return new MetadataPagesConfig(
                    Metadata,
                    ServiceEndpointsMetadataConfig,
                    IgnoreFormatsInMetadata,
                    EndpointHost.ContentTypeFilter.ContentTypeFormats.Keys.ToList());
            }
        }

        /// <summary>Query if 'httpReq' has access to metadata.</summary>
        ///
        /// <param name="httpReq">The HTTP request.</param>
        /// <param name="httpRes">The HTTP resource.</param>
        ///
        /// <returns>true if access to metadata, false if not.</returns>
        public bool HasAccessToMetadata(IHttpRequest httpReq, IHttpResponse httpRes)
        {
            if (!HasFeature(Feature.Metadata))
            {
                EndpointHost.Config.HandleErrorResponse(httpReq, httpRes, HttpStatusCode.Forbidden, "Metadata Not Available");
                return false;
            }

            if (MetadataVisibility != EndpointAttributes.Any)
            {
                var actualAttributes = httpReq.GetAttributes();
                if ((actualAttributes & MetadataVisibility) != MetadataVisibility)
                {
                    HandleErrorResponse(httpReq, httpRes, HttpStatusCode.Forbidden, "Metadata Not Visible");
                    return false;
                }
            }
            return true;
        }

        /// <summary>Handles the error response.</summary>
        ///
        /// <param name="httpReq">               The HTTP request.</param>
        /// <param name="httpRes">               The HTTP resource.</param>
        /// <param name="errorStatus">           The error status.</param>
        /// <param name="errorStatusDescription">Information describing the error status.</param>
        public void HandleErrorResponse(IHttpRequest httpReq, IHttpResponse httpRes, HttpStatusCode errorStatus, string errorStatusDescription = null)
        {
            if (httpRes.IsClosed) return;

            httpRes.StatusDescription = errorStatusDescription;

            var handler = GetHandlerForErrorStatus(errorStatus);

            handler.ProcessRequest(httpReq, httpRes, httpReq.OperationName);
        }

        /// <summary>Gets handler for error status.</summary>
        ///
        /// <param name="errorStatus">The error status.</param>
        ///
        /// <returns>The handler for error status.</returns>
        public INServiceKitHttpHandler GetHandlerForErrorStatus(HttpStatusCode errorStatus)
        {
            var httpHandler = GetCustomErrorHandler(errorStatus);

            switch (errorStatus)
            {
                case HttpStatusCode.Forbidden:
                    return httpHandler ?? new ForbiddenHttpHandler();
                case HttpStatusCode.NotFound:
                    return httpHandler ?? new NotFoundHttpHandler();
            }

            if (CustomHttpHandlers != null)
            {
                CustomHttpHandlers.TryGetValue(HttpStatusCode.NotFound, out httpHandler);
            }

            return httpHandler ?? new NotFoundHttpHandler();
        }

        /// <summary>Handler, called when the get custom error.</summary>
        ///
        /// <param name="errorStatusCode">The error status code.</param>
        ///
        /// <returns>The custom error handler.</returns>
        public INServiceKitHttpHandler GetCustomErrorHandler(int errorStatusCode)
        {
            try
            {
                return GetCustomErrorHandler((HttpStatusCode)errorStatusCode);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>Handler, called when the get custom error.</summary>
        ///
        /// <param name="errorStatus">The error status.</param>
        ///
        /// <returns>The custom error handler.</returns>
        public INServiceKitHttpHandler GetCustomErrorHandler(HttpStatusCode errorStatus)
        {
            INServiceKitHttpHandler httpHandler = null;
            if (CustomHttpHandlers != null)
            {
                CustomHttpHandlers.TryGetValue(errorStatus, out httpHandler);
            }
            return httpHandler ?? GlobalHtmlErrorHttpHandler;
        }

        /// <summary>Handler, called when the get custom error HTTP.</summary>
        ///
        /// <param name="errorStatus">The error status.</param>
        ///
        /// <returns>The custom error HTTP handler.</returns>
        public IHttpHandler GetCustomErrorHttpHandler(HttpStatusCode errorStatus)
        {
            var ssHandler = GetCustomErrorHandler(errorStatus);
            if (ssHandler == null) return null;
            var httpHandler = ssHandler as IHttpHandler;
            return httpHandler ?? new NServiceKitHttpHandler(ssHandler);
        }

        /// <summary>Gets or sets the pre execute service filter.</summary>
        ///
        /// <value>The pre execute service filter.</value>
        public Action<object, IHttpRequest, IHttpResponse> PreExecuteServiceFilter { get; set; }

        /// <summary>Gets or sets the post execute service filter.</summary>
        ///
        /// <value>The post execute service filter.</value>
        public Action<object, IHttpRequest, IHttpResponse> PostExecuteServiceFilter { get; set; }

        /// <summary>Gets or sets the full pathname of the fallback rest file.</summary>
        ///
        /// <value>The full pathname of the fallback rest file.</value>
        public FallbackRestPathDelegate FallbackRestPath { get; set; }

        /// <summary>Query if 'req' has valid authentication secret.</summary>
        ///
        /// <param name="req">The request.</param>
        ///
        /// <returns>true if valid authentication secret, false if not.</returns>
        public bool HasValidAuthSecret(IHttpRequest req)
        {
            if (AdminAuthSecret != null)
            {
                var authSecret = req.GetParam("authsecret");
                return authSecret == EndpointHost.Config.AdminAuthSecret;
            }

            return false;
        }
    }

}
