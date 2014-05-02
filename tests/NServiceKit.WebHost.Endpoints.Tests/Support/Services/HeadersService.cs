using System;
using System.Runtime.Serialization;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.Endpoints.Tests.Support.Services
{
    /// <summary>A headers.</summary>
	[DataContract]
	public class Headers
	{
        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
		[DataMember]
		public string Name { get; set; }
	}

    /// <summary>The headers response.</summary>
	[DataContract]
	public class HeadersResponse
	{
        /// <summary>Gets or sets the value.</summary>
        ///
        /// <value>The value.</value>
		[DataMember]
		public string Value { get; set; }
	}

    /// <summary>The headers service.</summary>
	public class HeadersService
		: TestServiceBase<Headers>, IRequiresHttpRequest
	{
        /// <summary>Gets or sets the HTTP request.</summary>
        ///
        /// <value>The HTTP request.</value>
		public IHttpRequest HttpRequest { get; set; }

        /// <summary>Runs the given request.</summary>
        ///
        /// <exception cref="NullReferenceException">Thrown when a value was unexpectedly null.</exception>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		protected override object Run(Headers request)
		{
			var header = RequestContext.GetHeader(request.Name);
			if (header != HttpRequest.Headers[request.Name])
				throw new NullReferenceException();

			return new HeadersResponse
			{
				Value = header
			};
		}
	}

}