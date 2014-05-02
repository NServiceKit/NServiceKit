using System.ComponentModel;
using System.Runtime.Serialization;
using NServiceKit.ServiceClient.Web;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.IntegrationTests.Services
{
    /// <summary>A test.</summary>
	[DataContract]
	[Description("NServiceKit's Test World web service.")]
	[Route("/test")]
	[Route("/test/{Name}")]
	public class Test
	{
        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
		[DataMember]
		public string Name { get; set; }
	}

    /// <summary>A test response.</summary>
	[DataContract]
	public class TestResponse
	{
        /// <summary>Gets or sets the result.</summary>
        ///
        /// <value>The result.</value>
		[DataMember]
		public string Result { get; set; }
	}

    /// <summary>A test service.</summary>
	public class TestService : ServiceInterface.Service
	{
        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>A TestResponse.</returns>
        public TestResponse Any(Test request)
		{
			var client = new Soap12ServiceClient("http://localhost/NServiceKit.WebHost.IntegrationTests/api/");
			var response = client.Send<HelloResponse>(new Hello { Name = request.Name });
			return new TestResponse { Result = response.Result };
		}
	}

}