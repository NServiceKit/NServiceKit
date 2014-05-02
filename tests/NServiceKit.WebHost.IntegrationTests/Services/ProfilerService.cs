using System.Collections.Generic;
using NServiceKit.MiniProfiler;
using NServiceKit.OrmLite;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.IntegrationTests.Services
{
    /// <summary>A mini profiler.</summary>
	[Route("/profiler", "GET")]
	[Route("/profiler/{Type}", "GET")]
	public class MiniProfiler
	{
        /// <summary>Gets or sets the type.</summary>
        ///
        /// <value>The type.</value>
		public string Type { get; set; }
	}

    /// <summary>A mini profiler service.</summary>
	public class MiniProfilerService : ServiceInterface.Service
	{
        /// <summary>Gets or sets the database factory.</summary>
        ///
        /// <value>The database factory.</value>
		public IDbConnectionFactory DbFactory { get; set; }

        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Any(MiniProfiler request)
		{
			var profiler = Profiler.Current;

			using (var db = DbFactory.OpenDbConnection())
			using (profiler.Step("MiniProfiler Service"))
			{
				if (request.Type == "n1")
				{
					using (profiler.Step("N + 1 query"))
					{
						var results = new List<Movie>();
						foreach (var movie in db.Select<Movie>())
						{
							results.Add(db.QueryById<Movie>(movie.Id));
						}
						return results;
					}
				}

				using (profiler.Step("Simple Select all"))
				{
					return db.Select<Movie>();
				}
			}
		}
	}
}