using System;
using System.Runtime.Serialization;

namespace NServiceKit.ServiceHost.Tests.Support
{
	[DataContract]
	public class RequiresContext { }

	[DataContract]
	public class RequiresContextResponse { }

	public class RequiresContextService : ServiceInterface.Service
	{
        public RequiresContextResponse Any(RequiresContext requires)
		{
			if (RequestContext == null)
				throw new ArgumentNullException("RequestContext");
	
			return new RequiresContextResponse();
		}
	}
}