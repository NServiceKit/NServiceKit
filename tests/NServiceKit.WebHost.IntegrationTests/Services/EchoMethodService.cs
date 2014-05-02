using System.Runtime.Serialization;
using NServiceKit.Common.Web;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.IntegrationTests.Services
{
    /// <summary>An echo method.</summary>
	[Route("/echomethod")]
	public class EchoMethod
	{
	}

    /// <summary>An echo method response.</summary>
	[DataContract]
	public class EchoMethodResponse
	{
        /// <summary>Gets or sets the result.</summary>
        ///
        /// <value>The result.</value>
		[DataMember]
		public string Result { get; set; }
	}

    /// <summary>An echo method service.</summary>
    [DefaultRequest(typeof(EchoMethod))]
	public class EchoMethodService : ServiceInterface.Service
	{
        /// <summary>Gets the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public object Get(EchoMethod request)
		{
			return new EchoMethodResponse { Result = HttpMethods.Get };
		}

        /// <summary>Post this message.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public object Post(EchoMethod request)
		{
			return new EchoMethodResponse { Result = HttpMethods.Post };
		}

        /// <summary>Puts the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public object Put(EchoMethod request)
		{
			return new EchoMethodResponse { Result = HttpMethods.Put };
		}

        /// <summary>Deletes the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public object Delete(EchoMethod request)
		{
			return new EchoMethodResponse { Result = HttpMethods.Delete };
		}

        /// <summary>Patches the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public object Patch(EchoMethod request)
		{
			return new EchoMethodResponse { Result = HttpMethods.Patch };
		}
	}
}