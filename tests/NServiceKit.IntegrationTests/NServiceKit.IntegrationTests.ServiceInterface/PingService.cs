using System;
using NServiceKit.IntegrationTests.ServiceModel;
using NServiceKit.ServiceHost;

namespace NServiceKit.IntegrationTests.ServiceInterface
{
	public class PingService
		: IService<Ping>
	{
		public object Execute(Ping request)
		{
			return new PingResponse { Text = "Pong " + request.Text };
		}
	}
}