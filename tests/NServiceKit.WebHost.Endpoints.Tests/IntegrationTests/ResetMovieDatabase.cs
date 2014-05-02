using System.Runtime.Serialization;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface.ServiceModel;

namespace NServiceKit.WebHost.Endpoints.Tests.IntegrationTests
{
    /// <summary>A reset movie database.</summary>
	[DataContract(Namespace = ExampleConfig.DefaultNamespace)]
	public class ResetMovieDatabase
	{
	}

    /// <summary>A reset movie database response.</summary>
	[DataContract(Namespace = ExampleConfig.DefaultNamespace)]
	public class ResetMovieDatabaseResponse
	{
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.IntegrationTests.ResetMovieDatabaseResponse class.</summary>
		public ResetMovieDatabaseResponse()
		{
			this.ResponseStatus = new ResponseStatus();
		}

        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
		[DataMember]
		public ResponseStatus ResponseStatus { get; set; }
	}
}