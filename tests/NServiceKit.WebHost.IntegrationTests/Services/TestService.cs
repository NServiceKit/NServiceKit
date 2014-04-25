using System.ComponentModel;
using System.Runtime.Serialization;
using NServiceKit.ServiceClient.Web;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.IntegrationTests.Services
{
	[DataContract]
	[Description("NServiceKit's Test World web service.")]
	[Route("/test")]
	[Route("/test/{Name}")]
	public class Test
	{
		[DataMember]
		public string Name { get; set; }
	}

	[DataContract]
	public class TestResponse
	{
		[DataMember]
		public string Result { get; set; }
	}

	public class TestService : ServiceInterface.Service
	{
        public TestResponse Any(Test request)
		{
			var client = new Soap12ServiceClient("http://localhost/NServiceKit.WebHost.IntegrationTests/api/");
			var response = client.Send<HelloResponse>(new Hello { Name = request.Name });
			return new TestResponse { Result = response.Result };
		}
	}

}