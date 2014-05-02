using System;
using System.Runtime.Serialization;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface;

namespace NServiceKit.Messaging.Tests.Services
{
    /// <summary>An un retryable fail.</summary>
	[DataContract]
	public class UnRetryableFail
	{
        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
		[DataMember]
		public string Name { get; set; }
	}

    /// <summary>An un retryable fail response.</summary>
	[DataContract]
	public class UnRetryableFailResponse
	{
        /// <summary>Gets or sets the result.</summary>
        ///
        /// <value>The result.</value>
		[DataMember]
		public string Result { get; set; }
	}

    /// <summary>An un retryable fail service.</summary>
    public class UnRetryableFailService : ServiceInterface.Service
	{
        /// <summary>Gets or sets the times called.</summary>
        ///
        /// <value>The times called.</value>
		public int TimesCalled { get; set; }

        /// <summary>Gets or sets the result.</summary>
        ///
        /// <value>The result.</value>
		public string Result { get; set; }

        /// <summary>Anies the given request.</summary>
        ///
        /// <exception cref="UnRetryableMessagingException">Thrown when an Un Retryable Messaging error condition occurs.</exception>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public object Any(UnRetryableFail request)
		{
			this.TimesCalled++;

			throw new UnRetryableMessagingException(
				"This request should not get retried",
				new NotSupportedException("This service always fails"));
		}

        /// <summary>Executes the asynchronous operation.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object ExecuteAsync(IMessage<UnRetryableFail> request)
        {
            return Any(request.GetBody());
        }
	}

}