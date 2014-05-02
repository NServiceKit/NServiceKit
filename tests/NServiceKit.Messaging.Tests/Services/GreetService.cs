using System.Runtime.Serialization;
using NServiceKit.ServiceInterface;

namespace NServiceKit.Messaging.Tests.Services
{
    /// <summary>A greet.</summary>
	[DataContract]
	public class Greet
	{
        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
		[DataMember]
		public string Name { get; set; }
	}

    /// <summary>A greet response.</summary>
	[DataContract]
	public class GreetResponse
	{
        /// <summary>Gets or sets the result.</summary>
        ///
        /// <value>The result.</value>
		[DataMember]
		public string Result { get; set; }
	}

    /// <summary>A greet service.</summary>
	public class GreetService : ServiceInterface.Service
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
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public object Any(Greet request)
		{
			this.TimesCalled++;

			Result = "Hello, " + request.Name;
			return new GreetResponse { Result = Result };
		}

        /// <summary>Executes the asynchronous operation.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
	    public object ExecuteAsync(IMessage<Greet> request)
	    {
	        return Any(request.GetBody());
	    }
	}

}