using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using NServiceKit.Common.Web;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints;

namespace NServiceKit.ServiceHost
{
    /// <summary>Interface for in service execute.</summary>
    public interface INServiceExec
    {
        /// <summary>Executes.</summary>
        ///
        /// <param name="requestContext">Context for the request.</param>
        /// <param name="instance">      The instance.</param>
        /// <param name="request">       The request.</param>
        ///
        /// <returns>An object.</returns>
        object Execute(IRequestContext requestContext, object instance, object request);
    }

    /// <summary>A service request execute.</summary>
    /// <typeparam name="TService">Type of the service.</typeparam>
    /// <typeparam name="TRequest">Type of the request.</typeparam>
    public class NServiceRequestExec<TService, TRequest> : INServiceExec
    {
        static NServiceRequestExec()
        {
            try
            {
                NServiceExec<TService>.CreateServiceRunnersFor<TRequest>();
            }
            catch (Exception ex)
            {
                ex.Message.Print();
                throw;
            }
        }

        /// <summary>Executes.</summary>
        ///
        /// <param name="requestContext">Context for the request.</param>
        /// <param name="instance">      The instance.</param>
        /// <param name="request">       The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Execute(IRequestContext requestContext, object instance, object request)
        {
            return NServiceExec<TService>.Execute(requestContext, instance, request,
                typeof(TRequest).Name);
        }
    }

    /// <summary>A service execute extensions.</summary>
    public static class NServiceExecExtensions
    {
        /// <summary>Gets the actions in this collection.</summary>
        ///
        /// <param name="serviceType">The serviceType to act on.</param>
        ///
        /// <returns>An enumerator that allows foreach to be used to process the actions in this collection.</returns>
        public static IEnumerable<MethodInfo> GetActions(this Type serviceType)
        {
            foreach (var mi in serviceType.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                if (mi.GetParameters().Length != 1)
                    continue;

                var actionName = mi.Name.ToUpper();
                if (!HttpMethods.AllVerbs.Contains(actionName) && actionName != ActionContext.AnyAction)
                    continue;

                yield return mi;
            }
        }
    }

    /// <summary>A service execute.</summary>
    /// <typeparam name="TService">Type of the service.</typeparam>
    public class NServiceExec<TService>
    {
        private static Dictionary<Type, List<ActionContext>> actionMap
            = new Dictionary<Type, List<ActionContext>>();

        private static Dictionary<string, InstanceExecFn> execMap 
            = new Dictionary<string, InstanceExecFn>();

        static NServiceExec()
        {
            foreach (var mi in typeof(TService).GetActions())
            {
                var actionName = mi.Name.ToUpper();
                var args = mi.GetParameters();

                var requestType = args[0].ParameterType;
                var actionCtx = new ActionContext {
                    Id = ActionContext.Key(actionName, requestType.Name),
                    ServiceType = typeof(TService),
                    RequestType = requestType,
                };

                try
                {
                    actionCtx.ServiceAction = CreateExecFn(requestType, mi);
                }
                catch
                {
                    //Potential problems with MONO, using reflection for fallback
                    actionCtx.ServiceAction = (service, request) =>
                        mi.Invoke(service, new[] { request });
                }

                var reqFilters = new List<IHasRequestFilter>();
                var resFilters = new List<IHasResponseFilter>();

                foreach (var attr in mi.GetCustomAttributes(false))
                {
                    var hasReqFilter = attr as IHasRequestFilter;
                    var hasResFilter = attr as IHasResponseFilter;

                    if (hasReqFilter != null)
                        reqFilters.Add(hasReqFilter);

                    if (hasResFilter != null)
                        resFilters.Add(hasResFilter);
                }

                if (reqFilters.Count > 0)
                    actionCtx.RequestFilters = reqFilters.ToArray();

                if (resFilters.Count > 0)
                    actionCtx.ResponseFilters = resFilters.ToArray();

                if (!actionMap.ContainsKey(requestType))
                    actionMap[requestType] = new List<ActionContext>();

                actionMap[requestType].Add(actionCtx);
            }
        }

        /// <summary>Creates execute function.</summary>
        ///
        /// <param name="requestType">Type of the request.</param>
        /// <param name="mi">         The mi.</param>
        ///
        /// <returns>The new execute function.</returns>
        public static ActionInvokerFn CreateExecFn(Type requestType, MethodInfo mi)
        {
            var serviceType = typeof(TService);

            var serviceParam = Expression.Parameter(typeof(object), "serviceObj");
            var serviceStrong = Expression.Convert(serviceParam, serviceType);

            var requestDtoParam = Expression.Parameter(typeof(object), "requestDto");
            var requestDtoStrong = Expression.Convert(requestDtoParam, requestType);

            Expression callExecute = Expression.Call(
            serviceStrong, mi, requestDtoStrong);

            if (mi.ReturnType != typeof(void))
            {
                var executeFunc = Expression.Lambda<ActionInvokerFn>
                (callExecute, serviceParam, requestDtoParam).Compile();

                return executeFunc;
            }
            else
            {
                var executeFunc = Expression.Lambda<VoidActionInvokerFn>
                (callExecute, serviceParam, requestDtoParam).Compile();

                return (service, request) => {
                  
                    executeFunc(service, request);
                    return null;
                };
            }
        }

        /// <summary>Gets actions for.</summary>
        ///
        /// <typeparam name="TRequest">Type of the request.</typeparam>
        ///
        /// <returns>The actions for.</returns>
        public static List<ActionContext> GetActionsFor<TRequest>()
        {
            List<ActionContext> requestActions;
            return actionMap.TryGetValue(typeof(TRequest), out requestActions)
                   ? requestActions
                   : new List<ActionContext>();
        }

        /// <summary>Creates service runners for.</summary>
        ///
        /// <typeparam name="TRequest">Type of the request.</typeparam>
        public static void CreateServiceRunnersFor<TRequest>()
        {
            foreach (var actionCtx in GetActionsFor<TRequest>())
            {
                if (execMap.ContainsKey(actionCtx.Id)) continue;

                var serviceRunner = EndpointHost.CreateServiceRunner<TRequest>(actionCtx);
                execMap[actionCtx.Id] = serviceRunner.Process;
            }
        }

        /// <summary>Executes.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <param name="requestContext">Context for the request.</param>
        /// <param name="instance">      The instance.</param>
        /// <param name="request">       The request.</param>
        /// <param name="requestName">   Name of the request.</param>
        ///
        /// <returns>An object.</returns>
        public static object Execute(IRequestContext requestContext,
                                     object instance, object request, string requestName)
        {
            var actionName = requestContext != null
                ? requestContext.Get<IHttpRequest>().HttpMethod
                : HttpMethods.Post; //MQ Services

            InstanceExecFn action;
            if (execMap.TryGetValue(ActionContext.Key(actionName, requestName), out action)
                || execMap.TryGetValue(ActionContext.AnyKey(requestName), out action))
            {
                return action(requestContext, instance, request);
            }

            var expectedMethodName = actionName.Substring(0, 1) + actionName.Substring(1).ToLower();
            throw new NotImplementedException(
                "Could not find method named {1}({0}) or Any({0}) on Service {2}"
                .Fmt(request.GetType().Name, expectedMethodName, typeof(TService).Name));
        }
    }
}