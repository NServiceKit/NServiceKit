using System;
using System.Diagnostics;
using NServiceKit.Common;
using NServiceKit.Common.Web;
using NServiceKit.Logging;
using NServiceKit.Messaging;
using NServiceKit.ServiceClient.Web;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints;

namespace NServiceKit.ServiceHost
{
    /// <summary>A service runner.</summary>
    /// <typeparam name="TRequest">Type of the request.</typeparam>
    public class ServiceRunner<TRequest> : IServiceRunner<TRequest>
    {
        /// <summary>The log.</summary>
        protected static readonly ILog Log = LogManager.GetLogger(typeof(ServiceRunner<>));

        /// <summary>The application host.</summary>
        protected readonly IAppHost AppHost;
        /// <summary>The service action.</summary>
        protected readonly ActionInvokerFn ServiceAction;
        /// <summary>The request filters.</summary>
        protected readonly IHasRequestFilter[] RequestFilters;
        /// <summary>The response filters.</summary>
        protected readonly IHasResponseFilter[] ResponseFilters;

        /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.ServiceRunner&lt;TRequest&gt; class.</summary>
        public ServiceRunner() { }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.ServiceRunner&lt;TRequest&gt; class.</summary>
        ///
        /// <param name="appHost">      The application host.</param>
        /// <param name="actionContext">Context for the action.</param>
        public ServiceRunner(IAppHost appHost, ActionContext actionContext)
        {
            this.AppHost = appHost;
            this.ServiceAction = actionContext.ServiceAction;
            this.RequestFilters = actionContext.RequestFilters;
            this.ResponseFilters = actionContext.ResponseFilters;
        }

        /// <summary>Gets application host.</summary>
        ///
        /// <returns>The application host.</returns>
        public IAppHost GetAppHost()
        {
            return AppHost ?? EndpointHost.AppHost;
        }

        /// <summary>Try resolve.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        ///
        /// <returns>A T.</returns>
        public T TryResolve<T>()
        {
            return this.GetAppHost() == null
                ? default(T)
                : this.GetAppHost().TryResolve<T>();
        }

        /// <summary>Resolve service.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="requestContext">Context for the request.</param>
        ///
        /// <returns>A T.</returns>
        public T ResolveService<T>(IRequestContext requestContext)
        {
            var service = this.GetAppHost().TryResolve<T>();
            var requiresContext = service as IRequiresRequestContext;
            if (requiresContext != null)
            {
                requiresContext.RequestContext = requestContext;
            }
            return service;
        }

        /// <summary>Before each request.</summary>
        ///
        /// <param name="requestContext">Context for the request.</param>
        /// <param name="request">       The request.</param>
        public virtual void BeforeEachRequest(IRequestContext requestContext, TRequest request)
        {
            OnBeforeExecute(requestContext, request);

            var requestLogger = TryResolve<IRequestLogger>();
            if (requestLogger != null)
            {
                requestContext.SetItem("_requestDurationStopwatch", Stopwatch.StartNew());
            }
        }

        /// <summary>After each request.</summary>
        ///
        /// <param name="requestContext">Context for the request.</param>
        /// <param name="request">       The request.</param>
        /// <param name="response">      The response.</param>
        ///
        /// <returns>An object.</returns>
        public virtual object AfterEachRequest(IRequestContext requestContext, TRequest request, object response)
        {
            var requestLogger = TryResolve<IRequestLogger>();
            if (requestLogger != null)
            {
                try
                {
                    var stopWatch = (Stopwatch)requestContext.GetItem("_requestDurationStopwatch");
                    requestLogger.Log(requestContext, request, response, stopWatch.Elapsed);
                }
                catch (Exception ex)
                {
                    Log.Error("Error while logging request: " + request.Dump(), ex);
                }
            }

            //only call OnAfterExecute if no exception occured
            return response.IsErrorResponse() ? response : OnAfterExecute(requestContext, response);
        }

        /// <summary>Executes the before execute action.</summary>
        ///
        /// <param name="requestContext">Context for the request.</param>
        /// <param name="request">       The request.</param>
        public virtual void OnBeforeExecute(IRequestContext requestContext, TRequest request) { }

        /// <summary>Executes the after execute action.</summary>
        ///
        /// <param name="requestContext">Context for the request.</param>
        /// <param name="response">      The response.</param>
        ///
        /// <returns>An object.</returns>
        public virtual object OnAfterExecute(IRequestContext requestContext, object response)
        {
            return response;
        }

        /// <summary>Executes.</summary>
        ///
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        ///
        /// <param name="requestContext">Context for the request.</param>
        /// <param name="instance">      The instance.</param>
        /// <param name="request">       The request.</param>
        ///
        /// <returns>An object.</returns>
        public virtual object Execute(IRequestContext requestContext, object instance, TRequest request)
        {
            try
            {
                BeforeEachRequest(requestContext, request);

                var appHost = GetAppHost();
                var container = appHost != null ? appHost.Config.ServiceManager.Container : null;
                var httpReq = requestContext != null ? requestContext.Get<IHttpRequest>() : null;
                var httpRes = requestContext != null ? requestContext.Get<IHttpResponse>() : null;

                if (RequestFilters != null)
                {
                    foreach (var requestFilter in RequestFilters)
                    {
                        var attrInstance = requestFilter.Copy();
                        if (container != null)
                            container.AutoWire(attrInstance);
                        attrInstance.RequestFilter(httpReq, httpRes, request);
                        if (appHost != null)
                            appHost.Release(attrInstance);
                        if (httpRes != null && httpRes.IsClosed) return null;
                    }
                }

                var response = AfterEachRequest(requestContext, request, ServiceAction(instance, request));

                if (ResponseFilters != null)
                {
                    foreach (var responseFilter in ResponseFilters)
                    {
                        var attrInstance = responseFilter.Copy();
                        if (container != null)
                            container.AutoWire(attrInstance);
                        attrInstance.ResponseFilter(httpReq, httpRes, response);
                        if (appHost != null)
                            appHost.Release(attrInstance);
                        if (httpRes != null && httpRes.IsClosed) return null;
                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                var result = HandleException(requestContext, request, ex);

                if (result == null) throw;

                return result;
            }
        }

        /// <summary>Executes.</summary>
        ///
        /// <param name="requestContext">Context for the request.</param>
        /// <param name="instance">      The instance.</param>
        /// <param name="request">       The request.</param>
        ///
        /// <returns>An object.</returns>
        public virtual object Execute(IRequestContext requestContext, object instance, IMessage<TRequest> request)
        {
            return Execute(requestContext, instance, request.GetBody());
        }

        /// <summary>Handles the exception.</summary>
        ///
        /// <param name="requestContext">Context for the request.</param>
        /// <param name="request">       The request.</param>
        /// <param name="ex">            The ex.</param>
        ///
        /// <returns>An object.</returns>
        public virtual object HandleException(IRequestContext requestContext, TRequest request, Exception ex)
        {
            var useAppHost = GetAppHost();

            object errorResponse = null;

            if (useAppHost != null && useAppHost.ServiceExceptionHandler != null)
                errorResponse = useAppHost.ServiceExceptionHandler(requestContext.Get<IHttpRequest>(), request, ex);

            if (errorResponse == null)
                errorResponse = DtoUtils.HandleException(useAppHost, request, ex);

            AfterEachRequest(requestContext, request, errorResponse ?? ex);
            
            return errorResponse;
        }

        /// <summary>Executes the one way operation.</summary>
        ///
        /// <param name="requestContext">Context for the request.</param>
        /// <param name="instance">      The instance.</param>
        /// <param name="request">       The request.</param>
        ///
        /// <returns>An object.</returns>
        public object ExecuteOneWay(IRequestContext requestContext, object instance, TRequest request)
        {
            var msgFactory = TryResolve<IMessageFactory>();
            if (msgFactory == null)
            {
                return Execute(requestContext, instance, request);
            }

            //Capture and persist this async request on this Services 'In Queue' 
            //for execution after this request has been completed
            using (var producer = msgFactory.CreateMessageProducer())
            {
                producer.Publish(request);
            }

            return WebRequestUtils.GetErrorResponseDtoType(request).CreateInstance();
        }

        /// <summary>signature matches ServiceExecFn.</summary>
        ///
        /// <param name="requestContext">Context for the request.</param>
        /// <param name="instance">      The instance.</param>
        /// <param name="request">       The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Process(IRequestContext requestContext, object instance, object request)
        {
            return requestContext != null && requestContext.EndpointAttributes.Has(EndpointAttributes.OneWay) 
                ? ExecuteOneWay(requestContext, instance, (TRequest)request) 
                : Execute(requestContext, instance, (TRequest)request);
        }
    }

}