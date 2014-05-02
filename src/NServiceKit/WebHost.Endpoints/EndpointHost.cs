using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Funq;
using NServiceKit.CacheAccess;
using NServiceKit.CacheAccess.Providers;
using NServiceKit.Common;
using NServiceKit.Common.Web;
using NServiceKit.Html;
using NServiceKit.IO;
using NServiceKit.Messaging;
using NServiceKit.MiniProfiler;
using NServiceKit.ServiceHost;
using NServiceKit.VirtualPath;
using NServiceKit.ServiceModel.Serialization;
using NServiceKit.WebHost.Endpoints.Extensions;
using NServiceKit.WebHost.Endpoints.Formats;
using NServiceKit.WebHost.Endpoints.Support;
using NServiceKit.WebHost.Endpoints.Utils;

namespace NServiceKit.WebHost.Endpoints
{
    /// <summary>An endpoint host.</summary>
    public class EndpointHost
    {
        /// <summary>Gets the application host.</summary>
        ///
        /// <value>The application host.</value>
        public static IAppHost AppHost { get; internal set; }

        /// <summary>Gets or sets the content type filter.</summary>
        ///
        /// <value>The content type filter.</value>
        public static IContentTypeFilter ContentTypeFilter { get; set; }

        /// <summary>Gets the raw request filters.</summary>
        ///
        /// <value>The raw request filters.</value>
        public static List<Action<IHttpRequest, IHttpResponse>> RawRequestFilters { get; private set; }

        /// <summary>Gets the request filters.</summary>
        ///
        /// <value>The request filters.</value>
        public static List<Action<IHttpRequest, IHttpResponse, object>> RequestFilters { get; private set; }

        /// <summary>Gets the response filters.</summary>
        ///
        /// <value>The response filters.</value>
        public static List<Action<IHttpRequest, IHttpResponse, object>> ResponseFilters { get; private set; }

        /// <summary>Gets or sets the view engines.</summary>
        ///
        /// <value>The view engines.</value>
        public static List<IViewEngine> ViewEngines { get; set; }

        /// <summary>TODO: rename to UncaughtExceptionsHandler.</summary>
        ///
        /// <value>The exception handler.</value>
        public static HandleUncaughtExceptionDelegate ExceptionHandler { get; set; }

        /// <summary>TODO: rename to ServiceExceptionsHandler.</summary>
        ///
        /// <value>The service exception handler.</value>
        public static HandleServiceExceptionDelegate ServiceExceptionHandler { get; set; }

        /// <summary>Gets or sets the catch all handlers.</summary>
        ///
        /// <value>The catch all handlers.</value>
        public static List<HttpHandlerResolverDelegate> CatchAllHandlers { get; set; }

        private static bool pluginsLoaded = false;

        /// <summary>Gets or sets the plugins.</summary>
        ///
        /// <value>The plugins.</value>
        public static List<IPlugin> Plugins { get; set; }

        /// <summary>Gets or sets the virtual path provider.</summary>
        ///
        /// <value>The virtual path provider.</value>
        public static IVirtualPathProvider VirtualPathProvider { get; set; }

        /// <summary>Gets or sets the Date/Time of the started at.</summary>
        ///
        /// <value>The started at.</value>
        public static DateTime StartedAt { get; set; }

        /// <summary>Gets or sets the Date/Time of the ready at.</summary>
        ///
        /// <value>The ready at.</value>
        public static DateTime ReadyAt { get; set; }

        private static void Reset()
        {
            ContentTypeFilter = HttpResponseFilter.Instance;
            RawRequestFilters = new List<Action<IHttpRequest, IHttpResponse>>();
            RequestFilters = new List<Action<IHttpRequest, IHttpResponse, object>>();
            ResponseFilters = new List<Action<IHttpRequest, IHttpResponse, object>>();
            ViewEngines = new List<IViewEngine>();
            CatchAllHandlers = new List<HttpHandlerResolverDelegate>();
            Plugins = new List<IPlugin> {
                new HtmlFormat(),
                new CsvFormat(),
                new MarkdownFormat(),
                new PredefinedRoutesFeature(),
                new MetadataFeature(),
            };

            //Default Config for projects that want to use components but not WebFramework (e.g. MVC)
            Config = new EndpointHostConfig(
                "Empty Config",
                new ServiceManager(new Container(), new ServiceController(null)));
        }

        /// <summary>Pre user config.</summary>
        ///
        /// <param name="appHost">       The application host.</param>
        /// <param name="serviceName">   Name of the service.</param>
        /// <param name="serviceManager">The service manager.</param>
        public static void ConfigureHost(IAppHost appHost, string serviceName, ServiceManager serviceManager)
        {
            Reset();
            AppHost = appHost;

            EndpointHostConfig.Instance.ServiceName = serviceName;
            EndpointHostConfig.Instance.ServiceManager = serviceManager;

            var config = EndpointHostConfig.Instance;
            Config = config; // avoid cross-dependency on Config setter
            VirtualPathProvider = new FileSystemVirtualPathProvider(AppHost, Config.WebHostPhysicalPath);

            Config.DebugMode = appHost.GetType().Assembly.IsDebugBuild();
            if (Config.DebugMode)
            {
                Plugins.Add(new RequestInfoFeature());
            }
        }

        // Config has changed
        private static void ApplyConfigChanges()
        {
            config.ServiceEndpointsMetadataConfig = ServiceEndpointsMetadataConfig.Create(config.NServiceKitHandlerFactoryPath);

            JsonDataContractSerializer.Instance.UseBcl = config.UseBclJsonSerializers;
            JsonDataContractDeserializer.Instance.UseBcl = config.UseBclJsonSerializers;
        }

        /// <summary>After configure called.</summary>
        public static void AfterInit()
        {
            StartedAt = DateTime.UtcNow;

            if (config.EnableFeatures != Feature.All)
            {
                if ((Feature.Xml & config.EnableFeatures) != Feature.Xml)
                    config.IgnoreFormatsInMetadata.Add("xml");
                if ((Feature.Json & config.EnableFeatures) != Feature.Json)
                    config.IgnoreFormatsInMetadata.Add("json");
                if ((Feature.Jsv & config.EnableFeatures) != Feature.Jsv)
                    config.IgnoreFormatsInMetadata.Add("jsv");
                if ((Feature.Csv & config.EnableFeatures) != Feature.Csv)
                    config.IgnoreFormatsInMetadata.Add("csv");
                if ((Feature.Html & config.EnableFeatures) != Feature.Html)
                    config.IgnoreFormatsInMetadata.Add("html");
                if ((Feature.Soap11 & config.EnableFeatures) != Feature.Soap11)
                    config.IgnoreFormatsInMetadata.Add("soap11");
                if ((Feature.Soap12 & config.EnableFeatures) != Feature.Soap12)
                    config.IgnoreFormatsInMetadata.Add("soap12");
            }

            if ((Feature.Html & config.EnableFeatures) != Feature.Html)
                Plugins.RemoveAll(x => x is HtmlFormat);

            if ((Feature.Csv & config.EnableFeatures) != Feature.Csv)
                Plugins.RemoveAll(x => x is CsvFormat);

            if ((Feature.Markdown & config.EnableFeatures) != Feature.Markdown)
                Plugins.RemoveAll(x => x is MarkdownFormat);

            if ((Feature.PredefinedRoutes & config.EnableFeatures) != Feature.PredefinedRoutes)
                Plugins.RemoveAll(x => x is PredefinedRoutesFeature);

            if ((Feature.Metadata & config.EnableFeatures) != Feature.Metadata)
                Plugins.RemoveAll(x => x is MetadataFeature);

            if ((Feature.RequestInfo & config.EnableFeatures) != Feature.RequestInfo)
                Plugins.RemoveAll(x => x is RequestInfoFeature);

            if ((Feature.Razor & config.EnableFeatures) != Feature.Razor)
                Plugins.RemoveAll(x => x is IRazorPlugin);    //external

            if ((Feature.ProtoBuf & config.EnableFeatures) != Feature.ProtoBuf)
                Plugins.RemoveAll(x => x is IProtoBufPlugin); //external

            if ((Feature.MsgPack & config.EnableFeatures) != Feature.MsgPack)
                Plugins.RemoveAll(x => x is IMsgPackPlugin);  //external

            if (ExceptionHandler == null)
            {
                ExceptionHandler = (httpReq, httpRes, operationName, ex) =>
                {
                    var errorMessage = String.Format("Error occured while Processing Request: {0}", ex.Message);
                    var statusCode = ex.ToStatusCode();
                    //httpRes.WriteToResponse always calls .Close in it's finally statement so 
                    //if there is a problem writing to response, by now it will be closed
                    if (!httpRes.IsClosed)
                    {
                        httpRes.WriteErrorToResponse(httpReq, httpReq.ResponseContentType, operationName, errorMessage, ex, statusCode);
                    }
                };
            }

            if (config.NServiceKitHandlerFactoryPath != null)
                config.NServiceKitHandlerFactoryPath = config.NServiceKitHandlerFactoryPath.TrimStart('/');

            var specifiedContentType = config.DefaultContentType; //Before plugins loaded

            ConfigurePlugins();

            AppHost.LoadPlugin(Plugins.ToArray());
            pluginsLoaded = true;

            AfterPluginsLoaded(specifiedContentType);

            var registeredCacheClient = AppHost.TryResolve<ICacheClient>();
            using (registeredCacheClient)
            {
                if (registeredCacheClient == null)
                {
                    Container.Register<ICacheClient>(new MemoryCacheClient());
                }
            }

            var registeredMqService = AppHost.TryResolve<IMessageService>();
            var registeredMqFactory = AppHost.TryResolve<IMessageFactory>();
            if (registeredMqService != null && registeredMqFactory == null)
            {
                Container.Register(c => registeredMqService.MessageFactory);
            }

            ReadyAt = DateTime.UtcNow;
        }

        /// <summary>Try resolve.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        ///
        /// <returns>A T.</returns>
        public static T TryResolve<T>()
        {
            return AppHost != null ? AppHost.TryResolve<T>() : default(T);
        }

        /// <summary>
        /// The AppHost.Container. Note: it is not thread safe to register dependencies after AppStart.
        /// </summary>
        public static Container Container
        {
            get
            {
                var aspHost = AppHost as AppHostBase;
                if (aspHost != null)
                    return aspHost.Container;
                var listenerHost = AppHost as HttpListenerBase;
                return listenerHost != null ? listenerHost.Container : new Container(); //testing may use alt AppHost
            }
        }

        private static void ConfigurePlugins()
        {
            //Some plugins need to initialize before other plugins are registered.

            foreach (var plugin in Plugins)
            {
                var preInitPlugin = plugin as IPreInitPlugin;
                if (preInitPlugin != null)
                {
                    preInitPlugin.Configure(AppHost);
                }
            }
        }

        private static void AfterPluginsLoaded(string specifiedContentType)
        {
            if (!String.IsNullOrEmpty(specifiedContentType))
                config.DefaultContentType = specifiedContentType;
            else if (String.IsNullOrEmpty(config.DefaultContentType))
                config.DefaultContentType = ContentType.Json;

            config.ServiceManager.AfterInit();
            ServiceManager = config.ServiceManager; //reset operations
        }

        /// <summary>Gets the plugin.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        ///
        /// <returns>The plugin.</returns>
        public static T GetPlugin<T>() where T : class, IPlugin
        {
            return Plugins.FirstOrDefault(x => x is T) as T;
        }

        /// <summary>Adds a plugin.</summary>
        ///
        /// <param name="plugins">A variable-length parameters list containing plugins.</param>
        public static void AddPlugin(params IPlugin[] plugins)
        {
            if (pluginsLoaded)
            {
                AppHost.LoadPlugin(plugins);
            }
            else
            {
                foreach (var plugin in plugins)
                {
                    Plugins.Add(plugin);
                }
            }
        }

        /// <summary>Gets or sets the manager for service.</summary>
        ///
        /// <value>The service manager.</value>
        public static ServiceManager ServiceManager
        {
            get { return config.ServiceManager; }
            set { config.ServiceManager = value; }
        }

        private static EndpointHostConfig config;

        /// <summary>Gets or sets the configuration.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <value>The configuration.</value>
        public static EndpointHostConfig Config
        {
            get
            {
                return config;
            }
            set
            {
                if (value.ServiceName == null)
                    throw new ArgumentNullException("ServiceName");

                if (value.ServiceController == null)
                    throw new ArgumentNullException("ServiceController");

                config = value;
                ApplyConfigChanges();
            }
        }

        /// <summary>Assert test configuration.</summary>
        ///
        /// <param name="assemblies">A variable-length parameters list containing assemblies.</param>
        public static void AssertTestConfig(params Assembly[] assemblies)
        {
            if (Config != null)
                return;

            var config = EndpointHostConfig.Instance;
            config.ServiceName = "Test Services";
            config.ServiceManager = new ServiceManager(assemblies.Length == 0 ? new[] { Assembly.GetCallingAssembly() } : assemblies);
            Config = config;
        }

        /// <summary>Gets a value indicating whether the debug mode.</summary>
        ///
        /// <value>true if debug mode, false if not.</value>
        public static bool DebugMode
        {
            get { return Config != null && Config.DebugMode; }
        }

        /// <summary>Gets the metadata.</summary>
        ///
        /// <value>The metadata.</value>
        public static ServiceMetadata Metadata { get { return Config.Metadata; } }

        /// <summary>
        /// Applies the raw request filters. Returns whether or not the request has been handled 
        /// and no more processing should be done.
        /// </summary>
        /// <returns></returns>
        public static bool ApplyPreRequestFilters(IHttpRequest httpReq, IHttpResponse httpRes)
        {
            foreach (var requestFilter in RawRequestFilters)
            {
                requestFilter(httpReq, httpRes);
                if (httpRes.IsClosed) break;
            }

            return httpRes.IsClosed;
        }

        /// <summary>
        /// Applies the request filters. Returns whether or not the request has been handled 
        /// and no more processing should be done.
        /// </summary>
        /// <returns></returns>
        public static bool ApplyRequestFilters(IHttpRequest httpReq, IHttpResponse httpRes, object requestDto)
        {
            httpReq.ThrowIfNull("httpReq");
            httpRes.ThrowIfNull("httpRes");

            using (Profiler.Current.Step("Executing Request Filters"))
            {
                //Exec all RequestFilter attributes with Priority < 0
                var attributes = FilterAttributeCache.GetRequestFilterAttributes(requestDto.GetType());
                var i = 0;
                for (; i < attributes.Length && attributes[i].Priority < 0; i++)
                {
                    var attribute = attributes[i];
                    ServiceManager.Container.AutoWire(attribute);
                    attribute.RequestFilter(httpReq, httpRes, requestDto);
                    if (AppHost != null) //tests
                        AppHost.Release(attribute);
                    if (httpRes.IsClosed) return httpRes.IsClosed;
                }

                //Exec global filters
                foreach (var requestFilter in RequestFilters)
                {
                    requestFilter(httpReq, httpRes, requestDto);
                    if (httpRes.IsClosed) return httpRes.IsClosed;
                }

                //Exec remaining RequestFilter attributes with Priority >= 0
                for (; i < attributes.Length; i++)
                {
                    var attribute = attributes[i];
                    ServiceManager.Container.AutoWire(attribute);
                    attribute.RequestFilter(httpReq, httpRes, requestDto);
                    if (AppHost != null) //tests
                        AppHost.Release(attribute);
                    if (httpRes.IsClosed) return httpRes.IsClosed;
                }

                return httpRes.IsClosed;
            }
        }

        /// <summary>
        /// Applies the response filters. Returns whether or not the request has been handled 
        /// and no more processing should be done.
        /// </summary>
        /// <returns></returns>
        public static bool ApplyResponseFilters(IHttpRequest httpReq, IHttpResponse httpRes, object response)
        {
            httpReq.ThrowIfNull("httpReq");
            httpRes.ThrowIfNull("httpRes");

            using (Profiler.Current.Step("Executing Response Filters"))
            {
                var responseDto = response.ToResponseDto();
                var attributes = responseDto != null
                    ? FilterAttributeCache.GetResponseFilterAttributes(responseDto.GetType())
                    : null;

                //Exec all ResponseFilter attributes with Priority < 0
                var i = 0;
                if (attributes != null)
                {
                    for (; i < attributes.Length && attributes[i].Priority < 0; i++)
                    {
                        var attribute = attributes[i];
                        ServiceManager.Container.AutoWire(attribute);
                        attribute.ResponseFilter(httpReq, httpRes, response);
                        if (AppHost != null) //tests
                            AppHost.Release(attribute);
                        if (httpRes.IsClosed) return httpRes.IsClosed;
                    }
                }

                //Exec global filters
                foreach (var responseFilter in ResponseFilters)
                {
                    responseFilter(httpReq, httpRes, response);
                    if (httpRes.IsClosed) return httpRes.IsClosed;
                }

                //Exec remaining RequestFilter attributes with Priority >= 0
                if (attributes != null)
                {
                    for (; i < attributes.Length; i++)
                    {
                        var attribute = attributes[i];
                        ServiceManager.Container.AutoWire(attribute);
                        attribute.ResponseFilter(httpReq, httpRes, response);
                        if (AppHost != null) //tests
                            AppHost.Release(attribute);
                        if (httpRes.IsClosed) return httpRes.IsClosed;
                    }
                }

                return httpRes.IsClosed;
            }
        }

        internal static object ExecuteService(object request, EndpointAttributes endpointAttributes, IHttpRequest httpReq, IHttpResponse httpRes)
        {
            using (Profiler.Current.Step("Execute Service"))
            {
                return config.ServiceController.Execute(request,
                    new HttpRequestContext(httpReq, httpRes, request, endpointAttributes));
            }
        }

        /// <summary>Creates service runner.</summary>
        ///
        /// <typeparam name="TRequest">Type of the request.</typeparam>
        /// <param name="actionContext">Context for the action.</param>
        ///
        /// <returns>The new service runner.</returns>
        public static IServiceRunner<TRequest> CreateServiceRunner<TRequest>(ActionContext actionContext)
        {
            return AppHost != null
                ? AppHost.CreateServiceRunner<TRequest>(actionContext)
                : new ServiceRunner<TRequest>(null, actionContext);
        }

        /// <summary>
        /// Call to signal the completion of a NServiceKit-handled Request
        /// </summary>
        internal static void CompleteRequest()
        {
            try
            {
                if (AppHost != null)
                {
                    AppHost.OnEndRequest();
                }
            }
            catch { }
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public static void Dispose()
        {
            AppHost = null;
        }
    }
}