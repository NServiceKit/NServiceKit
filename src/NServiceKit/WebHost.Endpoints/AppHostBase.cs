using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using Funq;
using NServiceKit.Common;
using NServiceKit.Configuration;
using NServiceKit.Html;
using NServiceKit.IO;
using NServiceKit.Logging;
using NServiceKit.ServiceHost;
using NServiceKit.WebHost.Endpoints.Extensions;

namespace NServiceKit.WebHost.Endpoints
{
	/// <summary>
	/// Inherit from this class if you want to host your web services inside an
	/// ASP.NET application.
	/// </summary>
	public abstract class AppHostBase
        : IFunqlet, IDisposable, IAppHost, IHasContainer
	{
		private readonly ILog log = LogManager.GetLogger(typeof(AppHostBase));

        /// <summary>Gets the instance.</summary>
        ///
        /// <value>The instance.</value>
		public static AppHostBase Instance { get; protected set; }

        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.AppHostBase class.</summary>
        ///
        /// <param name="serviceName">           Name of the service.</param>
        /// <param name="assembliesWithServices">A variable-length parameters list containing assemblies with services.</param>
		protected AppHostBase(string serviceName, params Assembly[] assembliesWithServices)
		{
			EndpointHost.ConfigureHost(this, serviceName, CreateServiceManager(assembliesWithServices));
		}

        /// <summary>Creates service manager.</summary>
        ///
        /// <param name="assembliesWithServices">A variable-length parameters list containing assemblies with services.</param>
        ///
        /// <returns>The new service manager.</returns>
		protected virtual ServiceManager CreateServiceManager(params Assembly[] assembliesWithServices)
		{
			return new ServiceManager(assembliesWithServices);
			//Alternative way to inject Container + Service Resolver strategy
			//return new ServiceManager(new Container(),
			//    new ServiceController(() => assembliesWithServices.ToList().SelectMany(x => x.GetTypes())));
		}

        /// <summary>Gets the service controller.</summary>
        ///
        /// <value>The service controller.</value>
		protected IServiceController ServiceController
		{
			get
			{
				return EndpointHost.Config.ServiceController;
			}
		}

        /// <summary>Register user-defined custom routes.</summary>
        ///
        /// <value>The routes.</value>
		public IServiceRoutes Routes
		{
			get { return EndpointHost.Config.ServiceController.Routes; }
		}

        /// <summary>Gets the container.</summary>
        ///
        /// <value>The container.</value>
		public Container Container
		{
			get
			{
				return EndpointHost.Config.ServiceManager != null
					? EndpointHost.Config.ServiceManager.Container : null;
			}
		}

        /// <summary>Initialises this object.</summary>
        ///
        /// <exception cref="InvalidDataException">Thrown when an Invalid Data error condition occurs.</exception>
		public void Init()
		{
			if (Instance != null)
			{
				throw new InvalidDataException("AppHostBase.Instance has already been set");
			}

			Instance = this;

			var serviceManager = EndpointHost.Config.ServiceManager;
			if (serviceManager != null)
			{
				serviceManager.Init();

				Configure(EndpointHost.Config.ServiceManager.Container);
			}
			else
			{
				Configure(null);
			}

			EndpointHost.AfterInit();
		}

        /// <summary>Configure the given container with the registrations provided by the funqlet.</summary>
        ///
        /// <param name="container">Container to register.</param>
		public abstract void Configure(Container container);

        /// <summary>Sets a configuration.</summary>
        ///
        /// <param name="config">The configuration.</param>
		public void SetConfig(EndpointHostConfig config)
		{
			if (config.ServiceName == null)
				config.ServiceName = EndpointHostConfig.Instance.ServiceName;

			if (config.ServiceManager == null)
				config.ServiceManager = EndpointHostConfig.Instance.ServiceManager;

			config.ServiceManager.ServiceController.EnableAccessRestrictions = config.EnableAccessRestrictions;

			EndpointHost.Config = config;
		}

        /// <summary>AutoWired Registration of an interface with a concrete type in AppHost IOC on Startup.</summary>
        ///
        /// <typeparam name="T">  .</typeparam>
        /// <typeparam name="TAs">.</typeparam>
		public void RegisterAs<T, TAs>() where T : TAs
		{
			this.Container.RegisterAutoWiredAs<T, TAs>();
		}

        /// <summary>Allows the clean up for executed autowired services and filters. Calls directly after services and filters are executed.</summary>
        ///
        /// <param name="instance">.</param>
        public virtual void Release(object instance)
        {
            try
            {
                var iocAdapterReleases = Container.Adapter as IRelease;
                if (iocAdapterReleases != null)
                {
                    iocAdapterReleases.Release(instance);
                }
                else
                {
                    var disposable = instance as IDisposable;
                    if (disposable != null)
                        disposable.Dispose();
                }
            }
            catch {/*ignore*/}
        }

        /// <summary>Called at the end of each request. Enables Request Scope.</summary>
        public virtual void OnEndRequest()
        {
            foreach (var item in HostContext.Instance.Items.Values)
            {
                Release(item);
            }

            HostContext.Instance.EndRequest();
        }

        /// <summary>Register dependency in AppHost IOC on Startup.</summary>
        ///
        /// <typeparam name="T">.</typeparam>
        /// <param name="instance">.</param>
	    public void Register<T>(T instance)
		{
			this.Container.Register(instance);
		}

        /// <summary>Try resolve.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        ///
        /// <returns>A T.</returns>
		public T TryResolve<T>()
		{
			return this.Container.TryResolve<T>();
		}

        /// <summary>
        /// Resolves from IoC container a specified type instance.
        /// </summary>
        /// <typeparam name="T">Type to be resolved.</typeparam>
        /// <returns>Instance of <typeparamref name="T"/>.</returns>
        public static T Resolve<T>()
        {
            if (Instance == null) throw new InvalidOperationException("AppHostBase is not initialized.");
            return Instance.Container.Resolve<T>();
        }

        /// <summary>
        /// Resolves and auto-wires a NServiceKit Service
        /// </summary>
        /// <typeparam name="T">Type to be resolved.</typeparam>
        /// <returns>Instance of <typeparamref name="T"/>.</returns>
        public static T ResolveService<T>(HttpContext httpCtx) where T : class, IRequiresRequestContext
        {
            if (Instance == null) throw new InvalidOperationException("AppHostBase is not initialized.");
            var service = Instance.Container.Resolve<T>();
            if (service == null) return null;
            service.RequestContext = httpCtx.ToRequestContext();
            return service;
        }

        /// <summary>Provide a custom model minder for a specific Request DTO.</summary>
        ///
        /// <value>The request binders.</value>
        public Dictionary<Type, Func<IHttpRequest, object>> RequestBinders
		{
			get { return EndpointHost.ServiceManager.ServiceController.RequestTypeFactoryMap; }
		}

        /// <summary>Register custom ContentType serializers.</summary>
        ///
        /// <value>The content type filters.</value>
		public IContentTypeFilter ContentTypeFilters
		{
			get
			{
				return EndpointHost.ContentTypeFilter;
			}
		}

        /// <summary>Add Request Filters, to be applied before the dto is deserialized.</summary>
        ///
        /// <value>The pre request filters.</value>
        public List<Action<IHttpRequest, IHttpResponse>> PreRequestFilters
		{
			get
			{
				return EndpointHost.RawRequestFilters;
			}
		}

        /// <summary>Add Request Filters.</summary>
        ///
        /// <value>The request filters.</value>
		public List<Action<IHttpRequest, IHttpResponse, object>> RequestFilters
		{
			get
			{
				return EndpointHost.RequestFilters;
			}
		}

        /// <summary>Add Response Filters.</summary>
        ///
        /// <value>The response filters.</value>
		public List<Action<IHttpRequest, IHttpResponse, object>> ResponseFilters
		{
			get
			{
				return EndpointHost.ResponseFilters;
			}
		}

        /// <summary>Add alternative HTML View Engines.</summary>
        ///
        /// <value>The view engines.</value>
        public List<IViewEngine> ViewEngines
		{
			get
			{
				return EndpointHost.ViewEngines;
			}
		}

        /// <summary>Provide an exception handler for un-caught exceptions.</summary>
        ///
        /// <value>The exception handler.</value>
        public HandleUncaughtExceptionDelegate ExceptionHandler
        {
            get { return EndpointHost.ExceptionHandler; }
            set { EndpointHost.ExceptionHandler = value; }
        }

        /// <summary>Provide an exception handler for unhandled exceptions.</summary>
        ///
        /// <value>The service exception handler.</value>
        public HandleServiceExceptionDelegate ServiceExceptionHandler
        {
            get { return EndpointHost.ServiceExceptionHandler; }
            set { EndpointHost.ServiceExceptionHandler = value; }
        }

        /// <summary>Provide a catch-all handler that doesn't match any routes.</summary>
        ///
        /// <value>The catch all handlers.</value>
		public List<HttpHandlerResolverDelegate> CatchAllHandlers
		{
			get { return EndpointHost.CatchAllHandlers; }
		}

        /// <summary>The AppHost config.</summary>
        ///
        /// <value>The configuration.</value>
		public EndpointHostConfig Config
		{
			get { return EndpointHost.Config; }
		}

        /// <summary>List of pre-registered and user-defined plugins to be enabled in this AppHost.</summary>
        ///
        /// <value>The plugins.</value>
		public List<IPlugin> Plugins
		{
			get { return EndpointHost.Plugins; }
		}

        /// <summary>Virtual access to file resources.</summary>
        ///
        /// <value>The virtual path provider.</value>
		public IVirtualPathProvider VirtualPathProvider
		{
			get { return EndpointHost.VirtualPathProvider; }
			set { EndpointHost.VirtualPathProvider = value; }
		}

        /// <summary>Create a service runner for IService actions.</summary>
        ///
        /// <typeparam name="TRequest">Type of the request.</typeparam>
        /// <param name="actionContext">Context for the action.</param>
        ///
        /// <returns>The new service runner.</returns>
        public virtual IServiceRunner<TRequest> CreateServiceRunner<TRequest>(ActionContext actionContext)
        {
            //cached per service action
            return new ServiceRunner<TRequest>(this, actionContext);
        }

        /// <summary>Resolve the absolute url for this request.</summary>
        ///
        /// <param name="virtualPath">Full pathname of the virtual file.</param>
        /// <param name="httpReq">    The HTTP request.</param>
        ///
        /// <returns>A string.</returns>
        public virtual string ResolveAbsoluteUrl(string virtualPath, IHttpRequest httpReq)
        {
            return Config.WebHostUrl == null 
                ? VirtualPathUtility.ToAbsolute(virtualPath) 
                : httpReq.GetAbsoluteUrl(virtualPath);
        }

        /// <summary>Apply plugins to this AppHost.</summary>
        ///
        /// <param name="plugins">.</param>
	    public virtual void LoadPlugin(params IPlugin[] plugins)
		{
			foreach (var plugin in plugins)
			{
				try
				{
					plugin.Register(this);
				}
				catch (Exception ex)
				{
					log.Warn("Error loading plugin " + plugin.GetType().Name, ex);
				}
			}
		}

        /// <summary>Executes the service operation.</summary>
        ///
        /// <param name="requestDto">The request dto.</param>
        ///
        /// <returns>An object.</returns>
		public virtual object ExecuteService(object requestDto)
		{
			return ExecuteService(requestDto, EndpointAttributes.None);
		}

        /// <summary>Executes the service operation.</summary>
        ///
        /// <param name="requestDto">        The request dto.</param>
        /// <param name="endpointAttributes">The endpoint attributes.</param>
        ///
        /// <returns>An object.</returns>
		public object ExecuteService(object requestDto, EndpointAttributes endpointAttributes)
		{
			return EndpointHost.Config.ServiceController.Execute(requestDto,
				new HttpRequestContext(requestDto, endpointAttributes));
		}

        /// <summary>Register an Adhoc web service on Startup.</summary>
        ///
        /// <param name="serviceType">.</param>
        /// <param name="atRestPaths">.</param>
		public void RegisterService(Type serviceType, params string[] atRestPaths)
		{
			var genericService = EndpointHost.Config.ServiceManager.RegisterService(serviceType);
            if (genericService != null)
            {
                var requestType = genericService.GetGenericArguments()[0];
                foreach (var atRestPath in atRestPaths)
                {
                    this.Routes.Add(requestType, atRestPath, null);
                }
            }
            else
            {
                var reqAttr = serviceType.GetCustomAttributes(true).OfType<DefaultRequestAttribute>().FirstOrDefault();
                if (reqAttr != null)
                {
                    foreach (var atRestPath in atRestPaths)
                    {
                        this.Routes.Add(reqAttr.RequestType, atRestPath, null);
                    }
                }
            }
		}
		
        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
		public virtual void Dispose()
		{
			if (EndpointHost.Config.ServiceManager != null)
			{
				EndpointHost.Config.ServiceManager.Dispose();
			}
		}
	}
}
