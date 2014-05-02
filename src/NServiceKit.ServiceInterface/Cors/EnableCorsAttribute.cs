using System;
using NServiceKit.Common.Web;
using NServiceKit.ServiceHost;

namespace NServiceKit.ServiceInterface.Cors
{
    /// <summary>
    /// Attribute marks that specific response class has support for Cross-origin resource sharing (CORS, see http://www.w3.org/TR/access-control/). CORS allows to access resources from different domain which usually forbidden by origin policy. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class EnableCorsAttribute : Attribute, IHasResponseFilter
    {
        /// <summary>Order in which Response Filters are executed. &lt;0 Executed before global response filters &gt;0 Executed after global response filters.</summary>
        ///
        /// <value>The priority.</value>
        public int Priority { get { return 0; } }

        private readonly string allowedOrigins;
        private readonly string allowedMethods;
        private readonly string allowedHeaders;

        private readonly bool allowCredentials;

        /// <summary>
        /// Represents a default constructor with Allow Origin equals to "*", Allowed GET, POST, PUT, DELETE, OPTIONS request and allowed "Content-Type" header.
        /// </summary>
        public EnableCorsAttribute(string allowedOrigins = "*", string allowedMethods = CorsFeature.DefaultMethods, string allowedHeaders = CorsFeature.DefaultHeaders, bool allowCredentials = false)
        {
            this.allowedOrigins = allowedOrigins;
            this.allowedMethods = allowedMethods;
            this.allowedHeaders = allowedHeaders;
            this.allowCredentials = allowCredentials;
        }

        /// <summary>The response filter is executed after the service.</summary>
        ///
        /// <param name="req">     The http request wrapper.</param>
        /// <param name="res">     The http response wrapper.</param>
        /// <param name="response">.</param>
        public void ResponseFilter(IHttpRequest req, IHttpResponse res, object response)
        {
            if (!string.IsNullOrEmpty(allowedOrigins))
                res.AddHeader(HttpHeaders.AllowOrigin, allowedOrigins);
            if (!string.IsNullOrEmpty(allowedMethods))
                res.AddHeader(HttpHeaders.AllowMethods, allowedMethods);
            if (!string.IsNullOrEmpty(allowedHeaders))
                res.AddHeader(HttpHeaders.AllowHeaders, allowedHeaders);
            if (allowCredentials)
                res.AddHeader(HttpHeaders.AllowCredentials, "true");
        }

        /// <summary>A new shallow copy of this filter is used on every request.</summary>
        ///
        /// <returns>An IHasResponseFilter.</returns>
        public IHasResponseFilter Copy()
        {
            return (IHasResponseFilter)MemberwiseClone();
        }
    }
}
