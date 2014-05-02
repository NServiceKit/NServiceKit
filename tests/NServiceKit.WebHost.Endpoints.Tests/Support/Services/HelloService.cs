using System.Runtime.Serialization;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.Endpoints.Tests.Support.Services
{
	/// Create the name of your Web Service (i.e. the Request DTO)
	[DataContract]
	[Route("/hello")] //Optional: Define an alternate REST-ful url for this service
	[Route("/hello/{Name}")]
	public class Hello
	{
        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
		[DataMember]
		public string Name { get; set; }
	}

	/// Define your Web Service response (i.e. Response DTO)
	[DataContract]
	public class HelloResponse
	{
        /// <summary>Gets or sets the result.</summary>
        ///
        /// <value>The result.</value>
		[DataMember]
		public string Result { get; set; }
	}

	/// Create your Web Service implementation 
	public class HelloService : ServiceInterface.Service
	{
        /// <summary>Executes the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>A HelloResponse.</returns>
        public HelloResponse Execute(Hello request)
		{
			return new HelloResponse { Result = "Hello, " + request.Name };
		}
	}

}