using System;
using Funq;
using NServiceKit.Configuration;
using NServiceKit.IntegrationTests.ServiceInterface;
using NServiceKit.Logging;
using NServiceKit.Logging.Support.Logging;
using NServiceKit.WebHost.Endpoints;

namespace NServiceKit.IntegrationTests.Host.Web
{
	/// <summary>
	/// An example of a AppHost to have your services running inside a webserver.
	/// </summary>
	public class AppHost
		: AppHostBase
	{
		private static ILog log;

		public AppHost()
			: base("NServiceKit IntegrationTests", typeof(PingService).Assembly)
		{
			LogManager.LogFactory = new DebugLogFactory();
			log = LogManager.GetLogger(typeof(AppHost));
		}

		public override void Configure(Container container)
		{
			container.Register<IResourceManager>(new ConfigurationResourceManager());

			var config = container.Resolve<IResourceManager>();

			log.InfoFormat("AppHost Configured: " + DateTime.Now);
		}
	}

}