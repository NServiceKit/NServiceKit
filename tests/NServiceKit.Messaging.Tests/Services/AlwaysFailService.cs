using System;
using System.Runtime.Serialization;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface;

namespace NServiceKit.Messaging.Tests.Services
{
    /// <summary>The always fail.</summary>
	[DataContract]
	public class AlwaysFail
	{
        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
		[DataMember]
		public string Name { get; set; }
	}

    /// <summary>The always fail response.</summary>
	[DataContract]
	public class AlwaysFailResponse
	{
        /// <summary>Gets or sets the result.</summary>
        ///
        /// <value>The result.</value>
		[DataMember]
		public string Result { get; set; }
	}

    /// <summary>The always fail service.</summary>
	public class AlwaysFailService : ServiceInterface.Service
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
        /// <exception cref="NotSupportedException">Thrown when the requested operation is not supported.</exception>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public object Any(AlwaysFail request)
		{
			this.TimesCalled++;
			throw new NotSupportedException("This service always fails");
		}

        /// <summary>Executes the asynchronous operation.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
	    public object ExecuteAsync(IMessage<AlwaysFail> request)
	    {
	        return Any(request.GetBody());
	    }
	}

}