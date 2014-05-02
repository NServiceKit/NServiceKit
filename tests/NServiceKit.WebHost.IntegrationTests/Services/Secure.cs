using System;
using System.Runtime.Serialization;
using NServiceKit.ServiceInterface;

namespace NServiceKit.WebHost.IntegrationTests.Services
{
    /// <summary>Interface for requires session.</summary>
	public interface IRequiresSession
	{
        /// <summary>Gets the identifier of the session.</summary>
        ///
        /// <value>The identifier of the session.</value>
		Guid SessionId { get; }
	}

    /// <summary>A secure.</summary>
	[DataContract]
	public class Secure : IRequiresSession
	{
        /// <summary>Gets or sets the identifier of the session.</summary>
        ///
        /// <value>The identifier of the session.</value>
		[DataMember]
		public Guid SessionId { get; set;}

        /// <summary>Gets or sets the status code.</summary>
        ///
        /// <value>The status code.</value>
		[DataMember]
		public int StatusCode { get; set; }
	}

    /// <summary>A secure response.</summary>
	[DataContract]
	public class SecureResponse
	{
        /// <summary>Gets or sets the value.</summary>
        ///
        /// <value>The value.</value>
		[DataMember]
		public string Value { get; set; }
	}

    /// <summary>A secure service.</summary>
	public class SecureService : ServiceInterface.Service
	{
        /// <summary>Anies the given request.</summary>
        ///
        /// <exception cref="UnauthorizedAccessException">Thrown when an Unauthorized Access error condition occurs.</exception>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Any(Secure request)
		{
			throw new UnauthorizedAccessException("You shouldn't be able to see this");
		}
	}
}