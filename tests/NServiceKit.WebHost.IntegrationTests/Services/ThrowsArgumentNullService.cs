using System;
using System.Runtime.Serialization;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface;
using NServiceKit.ServiceInterface.ServiceModel;

namespace NServiceKit.WebHost.IntegrationTests.Services
{
	[Route("/throwsargumentnull")]
	[DataContract]
	public class ThrowsArgumentNull
	{
		[DataMember]
		public string Value { get; set; }
	}

	[DataContract]
	public class ThrowsArgumentNullResponse
		: IHasResponseStatus
	{
		public ThrowsArgumentNullResponse()
		{
			this.ResponseStatus = new ResponseStatus();
		}

		[DataMember]
		public string Result { get; set; }

		[DataMember]
		public ResponseStatus ResponseStatus { get; set; }
	}

	public class ThrowsArgumentNullService : ServiceInterface.Service
	{
        public object Any(ThrowsArgumentNull request)
		{
			throw new ArgumentNullException("Name");
		}
	}
}