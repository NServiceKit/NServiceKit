using System.Runtime.Serialization;
using NServiceKit.Common;
using NServiceKit.OrmLite;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface;
using NServiceKit.WebHost.Endpoints.Tests.Support.Host;

namespace NServiceKit.WebHost.Endpoints.Tests.IntegrationTests
{
	[DataContract]
	[Route("/cached/movies", "GET")]
    [Route("/cached/movies/genres/{Genre}")]
	public class CachedMovies
	{
		[DataMember]
		public string Genre { get; set; }
	}

	public class CachedMoviesService : ServiceInterface.Service
	{
		public IDbConnectionFactory DbFactory { get; set; }

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