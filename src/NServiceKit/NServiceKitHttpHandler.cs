using System.Web;
using NServiceKit.ServiceHost;
using NServiceKit.WebHost.Endpoints.Extensions;
using NServiceKit.WebHost.Endpoints.Support;

namespace NServiceKit
{
    public class NServiceKitHttpHandler : IHttpHandler, INServiceKitHttpHandler
    {
        INServiceKitHttpHandler NServiceKitHandler;

        public NServiceKitHttpHandler(INServiceKitHttpHandler NServiceKitHandler)
        {
            this.NServiceKitHandler = NServiceKitHandler;
        }

        public virtual void ProcessRequest(HttpContext context)
        {
            ProcessRequest(
                context.Request.ToRequest(), 
                context.Response.ToResponse(),
                null);
        }

        public bool IsReusable
        {
            get { return false; }
        }

        public virtual void ProcessRequest(IHttpRequest httpReq, IHttpResponse httpRes, string operationName)
        {
            NServiceKitHandler.ProcessRequest(httpReq, httpRes, operationName ?? httpReq.OperationName);
        }
    }
}