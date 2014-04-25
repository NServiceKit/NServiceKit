using System;

namespace NServiceKit.Service
{
    /// <summary>
    /// 
    /// </summary>
	public interface IServiceClient : IServiceClientAsync, IOneWayClient
#if !(SILVERLIGHT || MONOTOUCH || ANDROIDINDIE)
		, IReplyClient
#endif
	{
	}

}