using System;
using NServiceKit.VirtualPath;
using NServiceKit.WebHost.Endpoints;
using NServiceKit.Plugins.Embedded.VirtualPath;

namespace NServiceKit.Plugins.Embedded
{
	public class EmbeddedFeature : IPlugin
	{
		public void Register(IAppHost appHost)
		{
			appHost.VirtualPathProvider = new MultiVirtualPathProvider(appHost, 
				new ResourceVirtualPathProvider(appHost), 
				new FileSystemVirtualPathProvider(appHost));
		}
	}
}

