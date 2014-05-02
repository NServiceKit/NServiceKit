using System;
using System.Collections.Generic;
using System.Reflection;

namespace NServiceKit.ServiceHost
{
    /// <summary>A service execute.</summary>
    public class GServiceExec
    {
        /// <summary>The execute.</summary>
        public const string Execute = "Execute";
        /// <summary>The execute asynchronous.</summary>
        public const string ExecuteAsync = "ExecuteAsync";

        /// <summary>The rest get.</summary>
        public const string RestGet = "Get";
        /// <summary>The rest post.</summary>
        public const string RestPost = "Post";
        /// <summary>The rest put.</summary>
        public const string RestPut = "Put";
        /// <summary>The rest delete.</summary>
        public const string RestDelete = "Delete";
        /// <summary>The rest patch.</summary>
        public const string RestPatch = "Patch";


        private static readonly Dictionary<Type, MethodInfo> ServiceExecCache = new Dictionary<Type, MethodInfo>();

        /// <summary>Gets execute method information.</summary>
        ///
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="requestType">Type of the request.</param>
        ///
        /// <returns>The execute method information.</returns>
        public static MethodInfo GetExecMethodInfo(Type serviceType, Type requestType)
        {
            MethodInfo mi;
            lock (ServiceExecCache)
            {
                if (!ServiceExecCache.TryGetValue(requestType /*serviceType */, out mi))
                {
                    var genericType = typeof(ServiceExec<>).MakeGenericType(requestType);

                    mi = genericType.GetMethod("Execute", BindingFlags.Public | BindingFlags.Static);

                    ServiceExecCache.Add(requestType /* serviceType */, mi);
                }
            }

            return mi;
        }

        /// <summary>Gets run time execute method.</summary>
        ///
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="requestType">Type of the request.</param>
        /// <param name="attrs">      The attributes.</param>
        ///
        /// <returns>The run time execute method.</returns>
        public static MethodInfo GetRunTimeExecMethod(Type serviceType, Type requestType, EndpointAttributes attrs)
        {
            if ((attrs & EndpointAttributes.OneWay) == EndpointAttributes.OneWay)
            {
                var mi = serviceType.GetMethod(ExecuteAsync, new[] { requestType });
                if (mi != null) return mi;
            }
            else if ((attrs & EndpointAttributes.HttpGet) == EndpointAttributes.HttpGet)
            {
                var mi = serviceType.GetMethod(RestGet, new[] { requestType });
                if (mi != null) return mi;
            }
            else if ((attrs & EndpointAttributes.HttpPost) == EndpointAttributes.HttpPost)
            {
                var mi = serviceType.GetMethod(RestPost, new[] { requestType });
                if (mi != null) return mi;
            }
            else if ((attrs & EndpointAttributes.HttpPut) == EndpointAttributes.HttpPut)
            {
                var mi = serviceType.GetMethod(RestPut, new[] { requestType });
                if (mi != null) return mi;
            }
            else if ((attrs & EndpointAttributes.HttpDelete) == EndpointAttributes.HttpDelete)
            {
                var mi = serviceType.GetMethod(RestDelete, new[] { requestType });
                if (mi != null) return mi;
            }
            else if ((attrs & EndpointAttributes.HttpPatch) == EndpointAttributes.HttpPatch)
            {
                var mi = serviceType.GetMethod(RestPatch, new[] { requestType });
                if (mi != null) return mi;
            }
            return serviceType.GetMethod(Execute, new[] { requestType });
        }
    }

    /// <summary>A service execute.</summary>
    /// <typeparam name="TReq">Type of the request.</typeparam>
    public class ServiceExec<TReq>
    {
        /// <summary>Name of the execute method.</summary>
        public const string ExecuteMethodName = "Execute";

        /// <summary>.</summary>
        [Obsolete("Use the New API (NServiceKit.ServiceInterface.Service) for future services. See: https://github.com/NServiceKit/NServiceKit/wiki/New-Api")]
        public static object Execute(IService<TReq> service, TReq request, EndpointAttributes attrs)
        {
            if ((attrs & EndpointAttributes.OneWay) == EndpointAttributes.OneWay)
            {
                var asyncService = service as IAsyncService<TReq>;
                if (asyncService != null) return asyncService.ExecuteAsync(request);
            }
            else if ((attrs & EndpointAttributes.HttpGet) == EndpointAttributes.HttpGet)
            {
                var restService = service as IRestGetService<TReq>;
                if (restService != null) return restService.Get(request);
            }
            else if ((attrs & EndpointAttributes.HttpPost) == EndpointAttributes.HttpPost)
            {
                var restService = service as IRestPostService<TReq>;
                if (restService != null) return restService.Post(request);
            }
            else if ((attrs & EndpointAttributes.HttpPut) == EndpointAttributes.HttpPut)
            {
                var restService = service as IRestPutService<TReq>;
                if (restService != null) return restService.Put(request);
            }
            else if ((attrs & EndpointAttributes.HttpDelete) == EndpointAttributes.HttpDelete)
            {
                var restService = service as IRestDeleteService<TReq>;
                if (restService != null) return restService.Delete(request);
            }
            else if ((attrs & EndpointAttributes.HttpOptions) == EndpointAttributes.HttpOptions)
            {
                var restService = service as IRestOptionsService<TReq>;
                if (restService != null) return restService.Options(request);
            }
            else if ((attrs & EndpointAttributes.HttpPatch) == EndpointAttributes.HttpPatch)
            {
                var restService = service as IRestPatchService<TReq>;
                if (restService != null) return restService.Patch(request);
            }
            return service.Execute(request);
        }
    }
}