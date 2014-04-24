using System.Runtime.Serialization;

namespace NServiceKit.ServiceHost.Tests.Support
{
	[DataContract]
	public class BasicRequest { }

	[DataContract]
	public class BasicRequestResponse { }

	public class BasicService : ServiceInterface.Service
	{
        public BasicRequestResponse Any(BasicRequest request)
		{
			return new BasicRequestResponse();
		}
	}
}