using System.IO;
using NServiceKit.ServiceHost;

namespace NServiceKit.Service
{
    /// <summary>Interface for partial writer.</summary>
	public interface IPartialWriter
	{
        /// <summary>
        /// Whether this HttpResult allows Partial Response
        /// </summary>
        bool IsPartialRequest { get; }

        /// <summary>
        /// Write a partial content result
        /// </summary>
        void WritePartialTo(IHttpResponse response);
	}
}