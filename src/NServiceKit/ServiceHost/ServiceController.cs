using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using NServiceKit.Configuration;
using NServiceKit.Logging;
using NServiceKit.Messaging;
using NServiceKit.ServiceModel.Serialization;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints;

namespace NServiceKit.ServiceHost
{
    /// <summary>Service execute function.</summary>
    ///
    /// <param name="requestContext">Context for the request.</param>
    /// <param name="request">       The request.</param>
    ///
    /// <returns>An object.</returns>
    public delegate object ServiceExecFn(IRequestContext requestContext, object request);

    /// <summary>Instance execute function.</summary>
    ///
    /// <param name="requestContext">Context for the request.</param>
    /// <param name="intance">       The intance.</param>
    /// <param name="request">       The request.</param>
    ///
    /// <returns>An object.</returns>
    public delegate object InstanceExecFn(IRequestContext requestContext, object intance, object request);

    /// <summary>Action invoker function.</summary>
    ///
    /// <param name="intance">The intance.</param>
    /// <param name="request">The request.</param>
    ///
    /// <returns>An object.</returns>
    public delegate object ActionInvokerFn(object intance, object request);

    /// <summary>Void action invoker function.</summary>
    ///
    /// <param name="intance">The intance.</param>
    /// <param name="request">The request.</param>
    public delegate void VoidActionInvokerFn(object intance, object request);

    /// <summary>A controller for handling services.</summary>
    public class ServiceController
        : IServiceController
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ServiceController));
        private const string ResponseDtoSuffix = "Response";

        /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.ServiceController class.</summary>
        ///
        /// <param name="resolveServicesFn">The resolve services function.</param>
        /// <param name="metadata">         The metadata.</param>
        public ServiceController(Func<IEnumerable<Type>> resolveServicesFn, ServiceMetadata metadata = null)
        {
            this.Metadata = metadata ?? new ServiceMetadata();

            this.RequestTypeFactoryMap = new Dictionary<Type, Func<IHttpRequest, object>>();
            this.EnableAccessRestrictions = true;
            this.ResolveServicesFn = resolveServicesFn;
        }

        readonly Dictionary<Type, ServiceExecFn> requestExecMap
			= new Dictionary<Type, ServiceExecFn>();

        readonly Dictionary<Type, RestrictAttribute> requestServiceAttrs
			= new Dictionary<Type, RestrictAttribute>();

        /// <summary>Gets or sets a value indicating whether the access restrictions is enabled.</summary>
        ///
        /// <value>true if enable access restrictions, false if not.</value>
        public bool EnableAccessRestrictions { get; set; }

        /// <summary>Gets the metadata.</summary>
        ///
        /// <value>The metadata.</value>
        public ServiceMetadata Metadata { get; internal set; }

        /// <summary>Gets or sets the request type factory map.</summary>
        ///
        /// <value>The request type factory map.</value>
        public Dictionary<Type, Func<IHttpRequest, object>> RequestTypeFactoryMap { get; set; }

        /// <summary>Gets or sets the default operations namespace.</summary>
        ///
        /// <value>The default operations namespace.</value>
        public string DefaultOperationsNamespace { get; set; }

        /// <summary>Allow the registration of custom routes.</summary>
        ///
        /// <value>The routes.</value>
        public IServiceRoutes Routes { get { return Metadata.Routes; } }

        private IResolver resolver;

        /// <summary>Gets or sets the resolver.</summary>
        ///
        /// <value>The resolver.</value>
        public IResolver Resolver
        {
            get { return resolver ?? EndpointHost.AppHost; }
            set { resolver = value; }
        }

        /// <summary>Gets or sets the resolve services function.</summary>
        ///
        /// <value>The resolve services function.</value>
        public Func<IEnumerable<Type>> ResolveServicesFn { get; set; }


        /// <summary>.</summary>
        [Obsolete("Use the New API (NServiceKit.ServiceInterface.Service) for future services. See: https://github.com/NServiceKit/NServiceKit/wiki/New-Api")]
        public void Register<TReq>(Func<IService<TReq>> invoker)
        {
            var requestType = typeof(TReq);
            ServiceExecFn handlerFn = (requestContext, dto) => {
                var service = invoker();

                InjectRequestContext(service, requestContext);

                return ServiceExec<TReq>.Execute(
                    service, (TReq)dto,
                    requestContext != null ? requestContext.EndpointAttributes : EndpointAttributes.None);
            };

            requestExecMap.Add(requestType, handlerFn);
        }

        /// <summary>Registers this object.</summary>
        ///
        /// <param name="serviceFactoryFn">The service factory function.</param>
        public void Register(ITypeFactory serviceFactoryFn)
        {
            foreach (var serviceType in ResolveServicesFn())
            {
                RegisterGService(serviceFactoryFn, serviceType);
                RegisterNService(serviceFactoryFn, serviceType);
            }
        }

        /// <summary>Registers the g service.</summary>
        ///
        /// <param name="serviceFactoryFn">The service factory function.</param>
        /// <param name="serviceType">     Type of the service.</param>
        public void RegisterGService(ITypeFactory serviceFactoryFn, Type serviceType)
        {
            if (serviceType.IsAbstract || serviceType.ContainsGenericParameters) return;

            //IService<T>
#pragma warning disable 618
            foreach (var service in serviceType.GetInterfaces())
            {
                if (!service.IsGenericType|| service.GetGenericTypeDefinition() != typeof(IService<>)) continue;

                var requestType = service.GetGenericArguments()[0];

                RegisterGServiceExecutor(requestType, serviceType, serviceFactoryFn);

                var responseTypeName = requestType.FullName + ResponseDtoSuffix;
                var responseType = AssemblyUtils.FindType(responseTypeName);

                RegisterCommon(serviceType, requestType, responseType);
            }
#pragma warning restore 618
        }

        /// <summary>Registers the n service.</summary>
        ///
        /// <param name="serviceFactoryFn">The service factory function.</param>
        /// <param name="serviceType">     Type of the service.</param>
        public void RegisterNService(ITypeFactory serviceFactoryFn, Type serviceType)
        {
            var processedReqs = new HashSet<Type>();

            if (typeof(IService).IsAssignableFrom(serviceType)
                && !serviceType.IsAbstract && !serviceType.IsGenericTypeDefinition)
            {
                foreach (var mi in serviceType.GetActions())
                {
                    var requestType = mi.GetParameters()[0].ParameterType;
                    if (processedReqs.Contains(requestType)) continue;
                    processedReqs.Add(requestType);

                    RegisterNServiceExecutor(requestType, serviceType, serviceFactoryFn);

                    var returnMarker = requestType.GetTypeWithGenericTypeDefinitionOf(typeof(IReturn<>));
                    var responseType = returnMarker != null ?
                          returnMarker.GetGenericArguments()[0]
                        : mi.ReturnType != typeof(object) && mi.ReturnType != typeof(void) ?
                          mi.ReturnType
                        : AssemblyUtils.FindType(requestType.FullName + ResponseDtoSuffix);

                    RegisterCommon(serviceType, requestType, responseType);
                }
            }
        }

        /// <summary>Registers the common.</summary>
        ///
        /// <param name="serviceType"> Type of the service.</param>
        /// <param name="requestType"> Type of the request.</param>
        /// <param name="responseType">Type of the response.</param>
        public void RegisterCommon(Type serviceType, Type requestType, Type responseType)
        {
            RegisterRestPaths(requestType);

            Metadata.Add(serviceType, requestType, responseType);

            if (typeof(IRequiresRequestStream).IsAssignableFrom(requestType))
            {
                this.RequestTypeFactoryMap[requestType] = httpReq => {
                    var rawReq = (IRequiresRequestStream)requestType.CreateInstance();
                    rawReq.RequestStream = httpReq.InputStream;
                    return rawReq;
                };
            }

            Log.DebugFormat("Registering {0} service '{1}' with request '{2}'",
                (responseType != null ? "Reply" : "OneWay"), serviceType.Name, requestType.Name);
        }

        /// <summary>The rest path map.</summary>
        public readonly Dictionary<string, List<RestPath>> RestPathMap = new Dictionary<string, List<RestPath>>();

        /// <summary>Registers the rest paths described by requestType.</summary>
        ///
        /// <exception cref="NotSupportedException">Thrown when the requested operation is not supported.</exception>
        ///
        /// <param name="requestType">Type of the request.</param>
        public void RegisterRestPaths(Type requestType)
        {
            var attrs = TypeDescriptor.GetAttributes(requestType).OfType<RouteAttribute>();
            foreach (RouteAttribute attr in attrs)
            {
                var restPath = new RestPath(requestType, attr.Path, attr.Verbs, attr.Summary, attr.Notes);

                var defaultAttr = attr as FallbackRouteAttribute;
                if (defaultAttr != null)
                {
                    if (EndpointHost.Config != null)
                    {
                        if (EndpointHost.Config.FallbackRestPath != null)
                            throw new NotSupportedException(string.Format(
                                "Config.FallbackRestPath is already defined. Only 1 [FallbackRoute] is allowed."));

                        EndpointHost.Config.FallbackRestPath = (httpMethod, pathInfo, filePath) =>
                        {
                            var pathInfoParts = RestPath.GetPathPartsForMatching(pathInfo);
                            return restPath.IsMatch(httpMethod, pathInfoParts) ? restPath : null;
                        };
                    }
                    
                    continue;
                }

                if (!restPath.IsValid)
                    throw new NotSupportedException(string.Format(
                        "RestPath '{0}' on Type '{1}' is not Valid", attr.Path, requestType.Name));

                RegisterRestPath(restPath);
            }
        }

        private static readonly char[] InvalidRouteChars = new[] {'?', '&'};

        /// <summary>Registers the rest path described by restPath.</summary>
        ///
        /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or illegal values.</exception>
        ///
        /// <param name="restPath">Full pathname of the rest file.</param>
        public void RegisterRestPath(RestPath restPath)
        {
            if (!EndpointHostConfig.SkipRouteValidation)
            {
                if (!restPath.Path.StartsWith("/"))
                    throw new ArgumentException("Route '{0}' on '{1}' must start with a '/'".Fmt(restPath.Path, restPath.RequestType.Name));
                if (restPath.Path.IndexOfAny(InvalidRouteChars) != -1)
                    throw new ArgumentException(("Route '{0}' on '{1}' contains invalid chars. " +
                                                "See https://github.com/NServiceKit/NServiceKit/wiki/Routing for info on valid routes.").Fmt(restPath.Path, restPath.RequestType.Name));
            }

            List<RestPath> pathsAtFirstMatch;
            if (!RestPathMap.TryGetValue(restPath.FirstMatchHashKey, out pathsAtFirstMatch))
            {
                pathsAtFirstMatch = new List<RestPath>();
                RestPathMap[restPath.FirstMatchHashKey] = pathsAtFirstMatch;
            }
            pathsAtFirstMatch.Add(restPath);
        }

        /// <summary>After initialise.</summary>
        public void AfterInit()
        {
            //Register any routes configured on Metadata.Routes
            foreach (var restPath in this.Metadata.Routes.RestPaths)
            {
                RegisterRestPath(restPath);
            }

            //Sync the RestPaths collections
            Metadata.Routes.RestPaths.Clear();
            Metadata.Routes.RestPaths.AddRange(RestPathMap.Values.SelectMany(x => x));

            Metadata.AfterInit();
        }

        /// <summary>Returns the first matching RestPath.</summary>
        ///
        /// <param name="httpMethod">.</param>
        /// <param name="pathInfo">  .</param>
        ///
        /// <returns>The rest path for request.</returns>
        public IRestPath GetRestPathForRequest(string httpMethod, string pathInfo)
        {
            var matchUsingPathParts = RestPath.GetPathPartsForMatching(pathInfo);

            List<RestPath> firstMatches;

            var yieldedHashMatches = RestPath.GetFirstMatchHashKeys(matchUsingPathParts);
            foreach (var potentialHashMatch in yieldedHashMatches)
            {
                if (!this.RestPathMap.TryGetValue(potentialHashMatch, out firstMatches)) continue;

                var bestScore = -1;
                foreach (var restPath in firstMatches)
                {
                    var score = restPath.MatchScore(httpMethod, matchUsingPathParts);
                    if (score > bestScore) bestScore = score;
                }
                if (bestScore > 0)
                {
                    foreach (var restPath in firstMatches)
                    {
                        if (bestScore == restPath.MatchScore(httpMethod, matchUsingPathParts))
                            return restPath;
                    }
                }
            }

            var yieldedWildcardMatches = RestPath.GetFirstMatchWildCardHashKeys(matchUsingPathParts);
            foreach (var potentialHashMatch in yieldedWildcardMatches)
            {
                if (!this.RestPathMap.TryGetValue(potentialHashMatch, out firstMatches)) continue;

                var bestScore = -1;
                foreach (var restPath in firstMatches)
                {
                    var score = restPath.MatchScore(httpMethod, matchUsingPathParts);
                    if (score > bestScore) bestScore = score;
                }
                if (bestScore > 0)
                {
                    foreach (var restPath in firstMatches)
                    {
                        if (bestScore == restPath.MatchScore(httpMethod, matchUsingPathParts))
                            return restPath;
                    }
                }
            }

            return null;
        }

        internal class TypeFactoryWrapper : ITypeFactory
        {
            private readonly Func<Type, object> typeCreator;

            /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.ServiceController.TypeFactoryWrapper class.</summary>
            ///
            /// <param name="typeCreator">The type creator.</param>
            public TypeFactoryWrapper(Func<Type, object> typeCreator)
            {
                this.typeCreator = typeCreator;
            }

            /// <summary>Creates an instance.</summary>
            ///
            /// <param name="type">The type.</param>
            ///
            /// <returns>The new instance.</returns>
            public object CreateInstance(Type type)
            {
                return typeCreator(type);
            }
        }

        /// <summary>Registers this object.</summary>
        ///
        /// <param name="requestType">Type of the request.</param>
        /// <param name="serviceType">Type of the service.</param>
        public void Register(Type requestType, Type serviceType)
        {
            var handlerFactoryFn = Expression.Lambda<Func<Type, object>>
                (
                    Expression.New(serviceType),
                    Expression.Parameter(typeof(Type), "serviceType")
                ).Compile();

            RegisterGServiceExecutor(requestType, serviceType, new TypeFactoryWrapper(handlerFactoryFn));
        }

        /// <summary>Registers this object.</summary>
        ///
        /// <param name="requestType">     Type of the request.</param>
        /// <param name="serviceType">     Type of the service.</param>
        /// <param name="handlerFactoryFn">The handler factory function.</param>
        public void Register(Type requestType, Type serviceType, Func<Type, object> handlerFactoryFn)
        {
            RegisterGServiceExecutor(requestType, serviceType, new TypeFactoryWrapper(handlerFactoryFn));
        }

        /// <summary>Registers the g service executor.</summary>
        ///
        /// <param name="requestType">     Type of the request.</param>
        /// <param name="serviceType">     Type of the service.</param>
        /// <param name="serviceFactoryFn">The service factory function.</param>
        public void RegisterGServiceExecutor(Type requestType, Type serviceType, ITypeFactory serviceFactoryFn)
        {
            var typeFactoryFn = CallServiceExecuteGeneric(requestType, serviceType);

            ServiceExecFn handlerFn = (requestContext, dto) => {
                var service = serviceFactoryFn.CreateInstance(serviceType);

                var endpointAttrs = requestContext != null
                    ? requestContext.EndpointAttributes
                    : EndpointAttributes.None;

                ServiceExecFn serviceExec = (reqCtx, req) =>
                    typeFactoryFn(req, service, endpointAttrs);

                return ManagedServiceExec(serviceExec, service, requestContext, dto);
            };

            AddToRequestExecMap(requestType, serviceType, handlerFn);
        }

        /// <summary>Registers the n service executor.</summary>
        ///
        /// <param name="requestType">     Type of the request.</param>
        /// <param name="serviceType">     Type of the service.</param>
        /// <param name="serviceFactoryFn">The service factory function.</param>
        public void RegisterNServiceExecutor(Type requestType, Type serviceType, ITypeFactory serviceFactoryFn)
        {
            var serviceExecDef = typeof(NServiceRequestExec<,>).MakeGenericType(serviceType, requestType);
            var iserviceExec = (INServiceExec)serviceExecDef.CreateInstance();

            ServiceExecFn handlerFn = (requestContext, dto) => {
                var service = serviceFactoryFn.CreateInstance(serviceType);

                ServiceExecFn serviceExec = (reqCtx, req) =>
                    iserviceExec.Execute(reqCtx, service, req);

                return ManagedServiceExec(serviceExec, service, requestContext, dto);
            };

            AddToRequestExecMap(requestType, serviceType, handlerFn);
        }

        private void AddToRequestExecMap(Type requestType, Type serviceType, ServiceExecFn handlerFn)
        {
            if (requestExecMap.ContainsKey(requestType))
            {
                throw new AmbiguousMatchException(
                    string.Format(
                    "Could not register Request '{0}' with service '{1}' as it has already been assigned to another service.\n"
                    + "Each Request DTO can only be handled by 1 service.",
                    requestType.FullName, serviceType.FullName));
            }

            requestExecMap.Add(requestType, handlerFn);

            var serviceAttrs = requestType.GetCustomAttributes(typeof(RestrictAttribute), false);
            if (serviceAttrs.Length > 0)
            {
                requestServiceAttrs.Add(requestType, (RestrictAttribute)serviceAttrs[0]);
            }
        }

        private static object ManagedServiceExec(
            ServiceExecFn serviceExec,
            object service, IRequestContext requestContext, object dto)
        {
            try
            {
                InjectRequestContext(service, requestContext);

                try
                {
                    if (EndpointHost.Config != null && EndpointHost.Config.PreExecuteServiceFilter != null)
                    {
                        EndpointHost.Config.PreExecuteServiceFilter(service, 
                            requestContext.Get<IHttpRequest>(),
                            requestContext.Get<IHttpResponse>());
                    }

                    //Executes the service and returns the result
                    var response = serviceExec(requestContext, dto);

                    if (EndpointHost.Config != null && EndpointHost.Config.PostExecuteServiceFilter != null)
                    {
                        EndpointHost.Config.PostExecuteServiceFilter(service, 
                            requestContext.Get<IHttpRequest>(),
                            requestContext.Get<IHttpResponse>());

                    }

                    return response;
                }
                finally
                {
                    if (EndpointHost.AppHost != null)
                    {
                        //Gets disposed by AppHost or ContainerAdapter if set
                        EndpointHost.AppHost.Release(service);
                    }
                    else
                    {
                        using (service as IDisposable) { }
                    }
                }
            }
            catch (TargetInvocationException tex)
            {
                //Mono invokes using reflection
                throw tex.InnerException ?? tex;
            }
        }

        private static void InjectRequestContext(object service, IRequestContext requestContext)
        {
            if (requestContext == null) return;

            var serviceRequiresContext = service as IRequiresRequestContext;
            if (serviceRequiresContext != null)
            {
                serviceRequiresContext.RequestContext = requestContext;
            }

            var servicesRequiresHttpRequest = service as IRequiresHttpRequest;
            if (servicesRequiresHttpRequest != null)
                servicesRequiresHttpRequest.HttpRequest = requestContext.Get<IHttpRequest>();
        }

        private static Func<object, object, EndpointAttributes, object> CallServiceExecuteGeneric(
            Type requestType, Type serviceType)
        {
            var mi = GServiceExec.GetExecMethodInfo(serviceType, requestType);

            try
            {
                var requestDtoParam = Expression.Parameter(typeof(object), "requestDto");
                var requestDtoStrong = Expression.Convert(requestDtoParam, requestType);

                var serviceParam = Expression.Parameter(typeof(object), "serviceObj");
                var serviceStrong = Expression.Convert(serviceParam, serviceType);

                var attrsParam = Expression.Parameter(typeof(EndpointAttributes), "attrs");

                Expression callExecute = Expression.Call(
                    mi, new Expression[] { serviceStrong, requestDtoStrong, attrsParam });

                var executeFunc = Expression.Lambda<Func<object, object, EndpointAttributes, object>>
                    (callExecute, requestDtoParam, serviceParam, attrsParam).Compile();

                return executeFunc;

            }
            catch (Exception)
            {
                //problems with MONO, using reflection for fallback
                return (request, service, attrs) => mi.Invoke(null, new[] { service, request, attrs });
            }
        }

        /// <summary>Execute MQ.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="mqMessage">Message describing the mq.</param>
        ///
        /// <returns>An object.</returns>
        public object ExecuteMessage<T>(IMessage<T> mqMessage)
        {
            return Execute(mqMessage.Body, new MqRequestContext(this.Resolver, mqMessage));
        }

        /// <summary>Execute MQ with requestContext.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dto">           The dto.</param>
        /// <param name="requestContext">Context for the request.</param>
        ///
        /// <returns>An object.</returns>
        public object ExecuteMessage<T>(IMessage<T> dto, IRequestContext requestContext)
        {
            return Execute(dto.Body, requestContext);
        }

        /// <summary>Executes the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Execute(object request)
        {
            return Execute(request, null);
        }

        /// <summary>Execute HTTP.</summary>
        ///
        /// <param name="request">       .</param>
        /// <param name="requestContext">.</param>
        ///
        /// <returns>An object.</returns>
        public object Execute(object request, IRequestContext requestContext)
        {
            var requestType = request.GetType();

            if (EnableAccessRestrictions)
            {
                AssertServiceRestrictions(requestType,
                    requestContext != null ? requestContext.EndpointAttributes : EndpointAttributes.None);
            }

            var handlerFn = GetService(requestType);
            return handlerFn(requestContext, request);
        }

        /// <summary>Gets a service.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <param name="requestType">Type of the request.</param>
        ///
        /// <returns>The service.</returns>
        public ServiceExecFn GetService(Type requestType)
        {
            ServiceExecFn handlerFn;
            if (!requestExecMap.TryGetValue(requestType, out handlerFn))
            {
                throw new NotImplementedException(string.Format("Unable to resolve service '{0}'", requestType.Name));
            }

            return handlerFn;
        }

        /// <summary>Executes the text operation.</summary>
        ///
        /// <param name="requestXml">    The request XML.</param>
        /// <param name="requestType">   Type of the request.</param>
        /// <param name="requestContext">Context for the request.</param>
        ///
        /// <returns>An object.</returns>
        public object ExecuteText(string requestXml, Type requestType, IRequestContext requestContext)
        {
            var request = DataContractDeserializer.Instance.Parse(requestXml, requestType);
            var response = Execute(request, requestContext);
            var responseXml = DataContractSerializer.Instance.Parse(response);
            return responseXml;
        }

        /// <summary>Assert service restrictions.</summary>
        ///
        /// <exception cref="UnauthorizedAccessException">Thrown when an Unauthorized Access error condition occurs.</exception>
        ///
        /// <param name="requestType">     Type of the request.</param>
        /// <param name="actualAttributes">The actual attributes.</param>
        public void AssertServiceRestrictions(Type requestType, EndpointAttributes actualAttributes)
        {
            if (EndpointHost.Config != null && !EndpointHost.Config.EnableAccessRestrictions) return;

            RestrictAttribute restrictAttr;
            var hasNoAccessRestrictions = !requestServiceAttrs.TryGetValue(requestType, out restrictAttr)
                || restrictAttr.HasNoAccessRestrictions;

            if (hasNoAccessRestrictions)
            {
                return;
            }

            var failedScenarios = new StringBuilder();
            foreach (var requiredScenario in restrictAttr.AccessibleToAny)
            {
                var allServiceRestrictionsMet = (requiredScenario & actualAttributes) == actualAttributes;
                if (allServiceRestrictionsMet)
                {
                    return;
                }

                var passed = requiredScenario & actualAttributes;
                var failed = requiredScenario & ~(passed);

                failedScenarios.AppendFormat("\n -[{0}]", failed);
            }

            var internalDebugMsg = (EndpointAttributes.InternalNetworkAccess & actualAttributes) != 0
                ? "\n Unauthorized call was made from: " + actualAttributes
                : "";

            throw new UnauthorizedAccessException(
                string.Format("Could not execute service '{0}', The following restrictions were not met: '{1}'" + internalDebugMsg,
                    requestType.Name, failedScenarios));
        }
    }

}