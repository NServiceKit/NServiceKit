using System.Web;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.Endpoints.Support
{
    /// <summary>Interface for in service kit HTTP handler.</summary>
	public interface INServiceKitHttpHandler
	{
        /// <summary>Process the request.</summary>
        ///
        /// <param name="httpReq">      The HTTP request.</param>
        /// <param name="httpRes">      The HTTP resource.</param>
        /// <param name="operationName">Name of the operation.</param>
		void ProcessRequest(IHttpRequest httpReq, IHttpResponse httpRes, string operationName);
	}
}