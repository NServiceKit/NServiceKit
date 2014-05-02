using System.Runtime.Serialization;
using NServiceKit.Common;
using NServiceKit.OrmLite;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface;
using NServiceKit.WebHost.Endpoints.Tests.Support.Host;

namespace NServiceKit.WebHost.Endpoints.Tests.IntegrationTests
{
    /// <summary>A cached movies.</summary>
	[DataContract]
	[Route("/cached/movies", "GET")]
    [Route("/cached/movies/genres/{Genre}")]
	public class CachedMovies
	{
        /// <summary>Gets or sets the genre.</summary>
        ///
        /// <value>The genre.</value>
		[DataMember]
		public string Genre { get; set; }
	}

    /// <summary>A cached movies service.</summary>
	public class CachedMoviesService : ServiceInterface.Service
	{
        /// <summary>Gets or sets the database factory.</summary>
        ///
        /// <value>The database factory.</value>
		public IDbConnectionFactory DbFactory { get; set; }

        /// <summary>Gets the given request.</summary>
        ///
        /// <param name="request">The request to get.</param>
        ///
        /// <returns>An object.</returns>
		public object Get(CachedMovies request)
		{
			var service = base.ResolveService<MoviesService>();

			return base.RequestContext.ToOptimizedResultUsingCache(
				this.GetCacheClient(), UrnId.Create<Movies>(request.Genre ?? "all"), () =>
				{
					return (MoviesResponse)service.Get(new Movies { Genre = request.Genre });
				});
		}
	}
}