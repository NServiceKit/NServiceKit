using System;

namespace NServiceKit.Service
{
	public interface IServiceClient : IServiceClientAsync, IOneWayClient
#if !(SILVERLIGHT || MONOTOUCH || ANDROIDINDIE)
		, IReplyClient
#endif
	{
	}

}