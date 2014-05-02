using System.ComponentModel;
using System.Runtime.Serialization;
using NServiceKit.OrmLite;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface.ServiceModel;

namespace NServiceKit.WebHost.Endpoints.Tests.IntegrationTests
{
    /// <summary>A reset movies.</summary>
	[DataContract]
	[Description("Resets the database back to the original Top 5 movies.")]
	[Route("/reset-movies")]
	public class ResetMovies { }

    /// <summary>A reset movies response.</summary>
	[DataContract]
	public class ResetMoviesResponse : IHasResponseStatus
	{
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.IntegrationTests.ResetMoviesResponse class.</summary>
		public ResetMoviesResponse()
		{
			this.ResponseStatus = new ResponseStatus();
		}

        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
		[DataMember]
		public ResponseStatus ResponseStatus { get; set; }
	}

    /// <summary>A reset movies service.</summary>
	public class ResetMoviesService : ServiceInterface.Service
	{
        /// <summary>Executes the post action.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public object OnPost(ResetMovies request)
		{
			ConfigureDatabase.Init(TryResolve<IDbConnectionFactory>());

			return new ResetMoviesResponse();
		}
	}
}