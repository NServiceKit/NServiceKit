using System;
using System.Runtime.Serialization;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.Endpoints.Tests.Support.Services
{
    /// <summary>A request filter.</summary>
	[DataContract]
	public class RequestFilter
	{
        /// <summary>Gets or sets the status code.</summary>
        ///
        /// <value>The status code.</value>
		[DataMember]
		public int StatusCode { get; set; }

        /// <summary>Gets or sets the name of the header.</summary>
        ///
        /// <value>The name of the header.</value>
		[DataMember]
		public string HeaderName { get; set; }

        /// <summary>Gets or sets the header value.</summary>
        ///
        /// <value>The header value.</value>
		[DataMember]
		public string HeaderValue { get; set; }
	}

    /// <summary>A request filter response.</summary>
	[DataContract]
	public class RequestFilterResponse
	{
        /// <summary>Gets or sets the value.</summary>
        ///
        /// <value>The value.</value>
		[DataMember]
		public string Value { get; set; }
	}

    /// <summary>The status code service.</summary>
	public class StatusCodeService : TestServiceBase<RequestFilter>
	{
        /// <summary>Runs the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		protected override object Run(RequestFilter request)
		{
			return new RequestFilterResponse();
		}
	}

}