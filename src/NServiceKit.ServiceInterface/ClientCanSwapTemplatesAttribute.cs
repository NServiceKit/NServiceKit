using System;
using NServiceKit.ServiceHost;

namespace NServiceKit.ServiceInterface
{
    /// <summary>Attribute for client can swap templates.</summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class ClientCanSwapTemplatesAttribute : RequestFilterAttribute
    {
        /// <summary>This method is only executed if the HTTP method matches the <see cref="ApplyTo"/> property.</summary>
        ///
        /// <param name="req">       The http request wrapper.</param>
        /// <param name="res">       The http response wrapper.</param>
        /// <param name="requestDto">The request DTO.</param>
        public override void Execute(IHttpRequest req, IHttpResponse res, object requestDto)
        {
            req.Items["View"] = req.GetParam("View");
            req.Items["Template"] = req.GetParam("Template");
        }
    }
}