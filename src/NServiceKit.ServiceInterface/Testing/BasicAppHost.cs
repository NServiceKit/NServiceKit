using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Funq;
using NServiceKit.Html;
using NServiceKit.IO;
using NServiceKit.VirtualPath;
using NServiceKit.ServiceHost;
using NServiceKit.WebHost.Endpoints;
using NServiceKit.WebHost.Endpoints.Extensions;

namespace NServiceKit.ServiceInterface.Testing
{
    /// <summary>A basic application host.</summary>
    public class BasicAppHost : IAppHost, IHasContainer, IDisposable
    {
        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.Testing.BasicAppHost class.</summary>
        public BasicAppHost()
        {
            this.Container = new Container();
            this.PreRequestFilters = new List<Action<IHttpRequest, IHttpResponse>>();
            this.RequestFilters = new List<Action<IHttpRequest, IHttpResponse, object>>();
            this.ResponseFilters = new List<Action<IHttpRequest, IHttpResponse, object>>();
            this.ViewEngines = new List<IViewEngine>();
            this.CatchAllHandlers = new List<HttpHandlerResolverDelegate>();
            VirtualPathProvider = new FileSystemVirtualPathProvider(this, "~".MapServerPath());
        }

        /// <summary>Registers as.</summary>
        ///
        /// <typeparam name="T">  Generic type parameter.</typeparam>
        /// <typeparam name="TAs">Type of as.</typeparam>
        public void RegisterAs<T, TAs>() where T : TAs
        {
            this.Container.RegisterAs<T, TAs>();
        }

        /// <summary>Allows the clean up for executed autowired services and filters. Calls directly after services and filters are executed.</summary>
        ///
        /// <param name="instance">.</param>
        public virtual void Release(object instance) { }
        
        /// <summary>Called at the end of each request. Enables Request Scope.</summary>
        public void OnEndRequest() {}

        /// <summary>Register user-defined custom routes.</summary>
        ///
        /// <value>The routes.</value>
        public IServiceRoutes Routes { get; private set; }

        /// <summary>Registers this object.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="instance">The instance.</param>
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

        /// <summary>Gets or sets the container.</summary>
        ///
        /// <value>The container.</value>
        public Container Container { get; set; }

        /// <summary>Register custom ContentType serializers.</summary>
        ///
        /// <value>The content type filters.</value>
        public IContentTypeFilter ContentTypeFilters { get; set; }

        /// <summary>Add Request Filters, to be applied before the dto is deserialized.</summary>
        ///
        /// <value>The pre request filters.</value>
        public List<Action<IHttpRequest, IHttpResponse>> PreRequestFilters { get; set; }

        /// <summary>Add Request Filters.</summary>
        ///
        /// <value>The request filters.</value>
        public List<Action<IHttpRequest, IHttpResponse, object>> RequestFilters { get; set; }

        /// <summary>Add Response Filters.</summary>
        ///
        /// <value>The response filters.</value>
        public List<Action<IHttpRequest, IHttpResponse, object>> ResponseFilters { get; set; }

        /// <summary>Add alternative HTML View Engines.</summary>
        ///
        /// <value>The view engines.</value>
        public List<IViewEngine> ViewEngines { get; set; }

        /// <summary>Provide an exception handler for un-caught exceptions.</summary>
        ///
        /// <value>The exception handler.</value>
        public HandleUncaughtExceptionDelegate ExceptionHandler { get; set; }

        /// <summary>Provide an exception handler for unhandled exceptions.</summary>
        ///
        /// <value>The service exception handler.</value>
        public HandleServiceExceptionDelegate ServiceExceptionHandler { get; set; }

        /// <summary>Provide a catch-all handler that doesn't match any routes.</summary>
        ///
        /// <value>The catch all handlers.</value>
        public List<HttpHandlerResolverDelegate> CatchAllHandlers { get; set; }

        /// <summary>Provide a custom model minder for a specific Request DTO.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <value>The request binders.</value>
        public Dictionary<Type, Func<IHttpRequest, object>> RequestBinders
        {
            get { throw new NotImplementedException(); }
        }

        private EndpointHostConfig config;

        /// <summary>The AppHost config.</summary>
        ///
        /// <value>The configuration.</value>
        public EndpointHostConfig Config
        {
            get
            {
                return config ?? (new EndpointHostConfig("BasicAppHost", new ServiceManager(Container, Assembly.GetExecutingAssembly())));
            }
            set { config = value; }
        }

        /// <summary>Register an Adhoc web service on Startup.</summary>
        ///
        /// <param name="serviceType">.</param>
        /// <param name="atRestPaths">.</param>
        public void RegisterService(Type serviceType, params string[] atRestPaths)
        {
            Config.ServiceManager.RegisterService(serviceType);
        }

        /// <summary>List of pre-registered and user-defined plugins to be enabled in this AppHost.</summary>
        ///
        /// <value>The plugins.</value>
        public List<IPlugin> Plugins { get; private set; }

        /// <summary>Apply plugins to this AppHost.</summary>
        ///
        /// <param name="plugins">.</param>
        public void LoadPlugin(params IPlugin[] plugins)
        {
            plugins.ToList().ForEach(x => x.Register(this));
        }

        /// <summary>Virtual access to file resources.</summary>
        ///
        /// <value>The virtual path provider.</value>
		public IVirtualPathProvider VirtualPathProvider { get; set; }

        /// <summary>Creates service runner.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <typeparam name="TRequest">Type of the request.</typeparam>
        /// <param name="actionContext">Context for the action.</param>
        ///
        /// <returns>The new service runner.</returns>
        public IServiceRunner<TRequest> CreateServiceRunner<TRequest>(ActionContext actionContext)
        {
            throw new NotImplementedException();
        }

        /// <summary>Resolve the absolute url for this request.</summary>
        ///
        /// <param name="virtualPath">Full pathname of the virtual file.</param>
        /// <param name="httpReq">    The HTTP request.</param>
        ///
        /// <returns>A string.</returns>
        public virtual string ResolveAbsoluteUrl(string virtualPath, IHttpRequest httpReq)
        {
            return httpReq.GetAbsoluteUrl(virtualPath);
        }

        /// <summary>Initialises this object.</summary>
        ///
        /// <returns>A BasicAppHost.</returns>
        public BasicAppHost Init()
        {
            EndpointHost.ConfigureHost(this, GetType().Name, Config.ServiceManager);
            return this;
        }


        private bool disposed;

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        ///
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            lock (this)
            {
                if (disposed) return;

                if (disposing)
                {
                    if (EndpointHost.Config != null && EndpointHost.Config.ServiceManager != null)
                    {
                        EndpointHost.Config.ServiceManager.Dispose();
                    }

                    EndpointHost.Dispose();
                }

                //release unmanaged resources here...
                disposed = true;
            }
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}