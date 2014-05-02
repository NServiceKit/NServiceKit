using System;
using System.Collections.Generic;
using System.Linq;
using NServiceKit.Logging;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints;

namespace NServiceKit.ServiceHost
{
    /// <summary>A service routes.</summary>
    public class ServiceRoutes : IServiceRoutes
    {
        private static ILog log = LogManager.GetLogger(typeof(ServiceRoutes));

        /// <summary>The rest paths.</summary>
        public readonly List<RestPath> RestPaths = new List<RestPath>();

        /// <summary>Adds restPath.</summary>
        ///
        /// <typeparam name="TRequest">Type of the request.</typeparam>
        /// <param name="restPath">The path to map the request DTO to. See <see cref="RouteAttribute.Path">RestServiceAttribute.Path</see>   for details on the correct format.</param>
        ///
        /// <returns>The IServiceRoutes.</returns>
        public IServiceRoutes Add<TRequest>(string restPath)
        {
            if (HasExistingRoute(typeof(TRequest), restPath)) return this;

            RestPaths.Add(new RestPath(typeof(TRequest), restPath));
            return this;
        }

        /// <summary>Adds restPath.</summary>
        ///
        /// <typeparam name="TRequest">Type of the request.</typeparam>
        /// <param name="restPath">The path to map the request DTO to. See <see cref="RouteAttribute.Path">RestServiceAttribute.Path</see>   for details on the correct format.</param>
        /// <param name="verbs">   The comma-delimited list of HTTP verbs supported by the path, such as "GET,PUT,DELETE".</param>
        ///
        /// <returns>The IServiceRoutes.</returns>
        public IServiceRoutes Add<TRequest>(string restPath, string verbs)
        {
            if (HasExistingRoute(typeof(TRequest), restPath)) return this;

            RestPaths.Add(new RestPath(typeof(TRequest), restPath, verbs));
            return this;
        }

        /// <summary>Maps the specified REST path to the specified request DTO, specifies the HTTP verbs supported by the path, and indicates the default MIME type of the returned response.</summary>
        ///
        /// <param name="requestType">The type of request DTO to map the path to.</param>
        /// <param name="restPath">   The path to map the request DTO to. See <see cref="RouteAttribute.Path">RestServiceAttribute.Path</see>   for details on the correct format.</param>
        /// <param name="verbs">      The comma-delimited list of HTTP verbs supported by the path, such as "GET,PUT,DELETE".</param>
        ///
        /// <returns>
        /// The same <see cref="IServiceRoutes"/> instance;
        /// never <see langword="null"/>.
        /// </returns>
        public IServiceRoutes Add(Type requestType, string restPath, string verbs)
        {
            if (HasExistingRoute(requestType, restPath)) return this;

            RestPaths.Add(new RestPath(requestType, restPath, verbs));
            return this;
        }

        /// <summary>Maps the specified REST path to the specified request DTO, specifies the HTTP verbs supported by the path, and indicates the default MIME type of the returned response.</summary>
        ///
        /// <param name="requestType">The type of request DTO to map the path to.</param>
        /// <param name="restPath">   The path to map the request DTO to. See <see cref="RouteAttribute.Path">RestServiceAttribute.Path</see>   for details on the correct format.</param>
        /// <param name="verbs">      The comma-delimited list of HTTP verbs supported by the path, such as "GET,PUT,DELETE".</param>
        /// <param name="summary">    The short summary of what the REST does.</param>
        /// <param name="notes">      The longer text to explain the behaviour of the REST.</param>
        ///
        /// <returns>
        /// The same <see cref="IServiceRoutes"/> instance;
        /// never <see langword="null"/>.
        /// </returns>
        public IServiceRoutes Add(Type requestType, string restPath, string verbs, string summary, string notes)
        {
            if (HasExistingRoute(requestType, restPath)) return this;

            RestPaths.Add(new RestPath(requestType, restPath, verbs, summary, notes));
            return this;
        }

        private bool HasExistingRoute(Type requestType, string restPath)
	    {
	        var existingRoute = RestPaths.FirstOrDefault(
	            x => x.RequestType == requestType && x.Path == restPath);
	        
            if (existingRoute != null)
	        {
                var existingRouteMsg = "Existing Route for '{0}' at '{1}' already exists".Fmt(requestType.Name, restPath);
                
                //if (!EndpointHostConfig.SkipRouteValidation) //wait till next deployment
                //    throw new Exception(existingRouteMsg);
	
                log.Warn(existingRouteMsg);
	            return true;
	        }

            return false;
	    }
    }
}