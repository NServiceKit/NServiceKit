using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Funq;
using NServiceKit.Common.Utils;
using NServiceKit.Logging;
using NServiceKit.ServiceHost;
using NServiceKit.Text;

namespace NServiceKit.WebHost.Endpoints
{
    /// <summary>An application host extensions.</summary>
	public static class AppHostExtensions
	{
		private static ILog log = LogManager.GetLogger(typeof(AppHostExtensions));

        /// <summary>An IAppHost extension method that registers the service.</summary>
        ///
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <param name="appHost">    .</param>
        /// <param name="atRestPaths">A variable-length parameters list containing at rest paths.</param>
		public static void RegisterService<TService>(this IAppHost appHost, params string[] atRestPaths)
		{
			appHost.RegisterService(typeof(TService), atRestPaths);
		}

        /// <summary>An IAppHost extension method that registers the request binder.</summary>
        ///
        /// <typeparam name="TRequest">Type of the request.</typeparam>
        /// <param name="appHost">.</param>
        /// <param name="binder"> The binder.</param>
		public static void RegisterRequestBinder<TRequest>(this IAppHost appHost, Func<IHttpRequest, object> binder)
		{
			appHost.RequestBinders[typeof(TRequest)] = binder;
		}

        /// <summary>An IAppHost extension method that adds the plugins from assembly to 'assembliesWithPlugins'.</summary>
        ///
        /// <param name="appHost">              .</param>
        /// <param name="assembliesWithPlugins">A variable-length parameters list containing assemblies with plugins.</param>
		public static void AddPluginsFromAssembly(this IAppHost appHost, params Assembly[] assembliesWithPlugins)
		{
			foreach (Assembly assembly in assembliesWithPlugins)
			{
				var pluginTypes =
					from t in assembly.GetExportedTypes()
					where t.GetInterfaces().Any(x => x == typeof(IPlugin))
					select t;

				foreach (var pluginType in pluginTypes)
				{
					try
					{
                        var plugin = pluginType.CreateInstance() as IPlugin;
						if (plugin != null)
						{
							EndpointHost.AddPlugin(plugin);
						}
					}
					catch (Exception ex)
					{
						log.Error("Error adding new Plugin " + pluginType.Name, ex);
					}
				}
			}
		}

        /// <summary>
        /// Get an IAppHost container. 
        /// Note: Registering dependencies should only be done during setup/configuration 
        /// stage and remain immutable there after for thread-safety.
        /// </summary>
        /// <param name="appHost"></param>
        /// <returns></returns>
        public static Container GetContainer(this IAppHost appHost)
        {
            var hasContainer = appHost as IHasContainer;
            return hasContainer != null ? hasContainer.Container : null;
        }
	}

}