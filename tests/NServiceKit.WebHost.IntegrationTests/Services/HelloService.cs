using System.ComponentModel;
using System.Runtime.Serialization;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface;

namespace NServiceKit.WebHost.IntegrationTests.Services
{
    /// <summary>A hello.</summary>
	[DataContract]
	[Description("NServiceKit's Hello World web service.")]
    [Route("/hello", Summary = @"Default hello service.", Notes = "Longer description for hello service.")]
    [Route("/hello/{Name}", "GET", Summary = @"Says ""Hello"" to provided Name with GET.", 
        Notes = "Longer description of the GET method which says \"Hello\"")]
    [Route("/hello/{Name}", "POST", Summary = @"Says ""Hello"" to provided Name with POST.", 
        Notes = "Longer description of the POST method which says \"Hello\"")]
    [Route("/hello/{Name}", "PUT", Summary = @"Says ""Hello"" to provided Name with PUT.", 
        Notes = "Longer description of the PUT method which says \"Hello\"")]
    [Route("/hello/{Name}", "DELETE", Summary = @"Says ""Hello"" to provided Name with DELETE.", 
        Notes = "Longer description of the DELETE method which says \"Hello\"")]
    public class Hello
	{
        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
        [DataMember]
        [ApiMember(Name = "Name", Description = "Name Description", ParameterType = "path", 
            DataType = "string", IsRequired = true)]
		public string Name { get; set; }
	}

    /// <summary>A hello response.</summary>
	[DataContract]
	public class HelloResponse
	{
        /// <summary>Gets or sets the result.</summary>
        ///
        /// <value>The result.</value>
		[DataMember]
		public string Result { get; set; }
	}

    /// <summary>A hello service.</summary>
	public class HelloService : ServiceInterface.Service
	{
        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>A HelloResponse.</returns>
        public HelloResponse Any(Hello request)
		{
			return new HelloResponse { Result = "Hello, " + request.Name };
		}
	}

    /// <summary>Attribute for test filter.</summary>
	public class TestFilterAttribute : ResponseFilterAttribute
	{
        /// <summary>This method is only executed if the HTTP method matches the <see cref="ApplyTo"/> property.</summary>
        ///
        /// <param name="req">       The http request wrapper.</param>
        /// <param name="res">       The http response wrapper.</param>
        /// <param name="requestDto">The response DTO.</param>
		public override void Execute(IHttpRequest req, IHttpResponse res, object requestDto)
		{
		}
	}

    /// <summary>A hello 2.</summary>
	[Route("/hello2")]
	[Route("/hello2/{Name}")]
	public class Hello2
	{
        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
		[DataMember]
		public string Name { get; set; }
	}

    /// <summary>A hello 2 response.</summary>
	[DataContract]
	public class Hello2Response
	{
        /// <summary>Gets or sets the result.</summary>
        ///
        /// <value>The result.</value>
		[DataMember]
		public string Result { get; set; }
	}

    /// <summary>A hello 2 service.</summary>
	[TestFilter]
	public class Hello2Service : ServiceInterface.Service
	{
        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public object Any(Hello2 request)
		{
			return request.Name;
		}
	}
}