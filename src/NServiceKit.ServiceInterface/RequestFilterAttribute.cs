using System;
using NServiceKit.ServiceHost;
using NServiceKit.Common;

namespace NServiceKit.ServiceInterface
{
    /// <summary>Values that represent RequestFilterPriority.</summary>
    public enum RequestFilterPriority : int
    {
        /// <summary>An enum constant representing the authenticate option.</summary>
        Authenticate = -100,

        /// <summary>An enum constant representing the required role option.</summary>
        RequiredRole = -90,

        /// <summary>An enum constant representing the required permission option.</summary>
        RequiredPermission = -80,
    }

    /// <summary>
    /// Base class to create request filter attributes only for specific HTTP methods (GET, POST...)
    /// </summary>
    public abstract class RequestFilterAttribute : Attribute, IHasRequestFilter
    {
        /// <summary>Order in which Request Filters are executed. &lt;0 Executed before global request filters &gt;0 Executed after global request filters.</summary>
        ///
        /// <value>The priority.</value>
        public int Priority { get; set; }

        /// <summary>Gets or sets the apply to.</summary>
        ///
        /// <value>The apply to.</value>
        public ApplyTo ApplyTo { get; set; }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.RequestFilterAttribute class.</summary>
        public RequestFilterAttribute()
        {
            ApplyTo = ApplyTo.All;
        }

        /// <summary>
        /// Creates a new <see cref="RequestFilterAttribute"/>
        /// </summary>
        /// <param name="applyTo">Defines when the filter should be executed</param>
        public RequestFilterAttribute(ApplyTo applyTo)
        {
            ApplyTo = applyTo;
        }

        /// <summary>The request filter is executed before the service.</summary>
        ///
        /// <param name="req">       The http request wrapper.</param>
        /// <param name="res">       The http response wrapper.</param>
        /// <param name="requestDto">The request DTO.</param>
        public void RequestFilter(IHttpRequest req, IHttpResponse res, object requestDto)
        {
            ApplyTo httpMethod = req.HttpMethodAsApplyTo();
            if (ApplyTo.Has(httpMethod))
                this.Execute(req, res, requestDto);
        }

        /// <summary>
        /// This method is only executed if the HTTP method matches the <see cref="ApplyTo"/> property.
        /// </summary>
        /// <param name="req">The http request wrapper</param>
        /// <param name="res">The http response wrapper</param>
        /// <param name="requestDto">The request DTO</param>
        public abstract void Execute(IHttpRequest req, IHttpResponse res, object requestDto);

        /// <summary>
        /// Create a ShallowCopy of this instance.
        /// </summary>
        /// <returns></returns>
        public virtual IHasRequestFilter Copy()
        {
            return (IHasRequestFilter)this.MemberwiseClone();
        }
    }
}
