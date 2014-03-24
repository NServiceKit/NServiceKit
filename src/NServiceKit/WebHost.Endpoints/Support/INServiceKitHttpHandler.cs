using System.Web;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.Endpoints.Support
{
	public interface INServiceKitHttpHandler
	{
		void ProcessRequest(IHttpRequest httpReq, IHttpResponse httpRes, string operationName);
	}
}