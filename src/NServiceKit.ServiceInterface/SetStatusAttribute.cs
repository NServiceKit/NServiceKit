using System;
using System.Net;
using NServiceKit.ServiceHost;

namespace NServiceKit.ServiceInterface
{
    /// <summary>Attribute for set status.</summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class SetStatusAttribute : RequestFilterAttribute
    {
        /// <summary>Gets or sets the status.</summary>
        ///
        /// <value>The status.</value>
        public int? Status { get; set; }

        /// <summary>Gets or sets the status code.</summary>
        ///
        /// <value>The status code.</value>
        public HttpStatusCode? StatusCode { get; set; }

        /// <summary>Gets or sets the description.</summary>
        ///
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.SetStatusAttribute class.</summary>
        public SetStatusAttribute() {}

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.SetStatusAttribute class.</summary>
        ///
        /// <param name="status">     The status.</param>
        /// <param name="description">The description.</param>
        public SetStatusAttribute(int status, string description)
        {
            Status = status;
            Description = description;
        }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.SetStatusAttribute class.</summary>
        ///
        /// <param name="statusCode"> The status code.</param>
        /// <param name="description">The description.</param>
        public SetStatusAttribute(HttpStatusCode statusCode, string description)
        : this((int)statusCode, description) {}

        /// <summary>This method is only executed if the HTTP method matches the <see cref="ApplyTo"/> property.</summary>
        ///
        /// <param name="req">       The http request wrapper.</param>
        /// <param name="res">       The http response wrapper.</param>
        /// <param name="requestDto">The request DTO.</param>
        public override void Execute(IHttpRequest req, IHttpResponse res, object requestDto)
        {
            if (Status.HasValue)
                res.StatusCode = Status.Value;

            if (!string.IsNullOrEmpty(Description))
                res.StatusDescription = Description;
        }
    }
}