using System;
using System.Collections.Generic;
using NServiceKit.OrmLite;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.Endpoints.Tests.IntegrationTests
{
	/// <summary>
	/// An example of a very basic web service
	/// </summary>
	public class RestMovieService : ServiceInterface.Service
	{
		public IDbConnectionFactory DbFactory { get; set; }

        public RestMoviesResponse Any(RestMovies request)
		{
			return Get(request);
		}

        public RestMoviesResponse Get(RestMovies request)
		{
			var response = new RestMoviesResponse();

			DbFactory.Run(db =>
			{
				if (request.Id != null)
				{
					var movie = db.GetByIdOrDefault<RestMovie>(request.Id);
					if (movie != null)
					{
						response.Movies.Add(movie);
					}
				}
				else
				{
					response.Movies = db.Select<RestMovie>();
				}
			});

			return response;
		}

        public RestMoviesResponse Put(RestMovies request)
		{
			DbFactory.Run(db => db.Save(request.Movie));
			return new RestMoviesResponse();
		}

        public RestMoviesResponse Delete(RestMovies request)
		{
            DbFactory.Run(db => db.DeleteById<RestMovie>(request.Id));
			return new RestMoviesResponse();
		}

        public RestMoviesResponse Post(RestMovies request)
		{
            DbFactory.Run(db => db.Update(request.Movie));
			return new RestMoviesResponse();
		}
	}
}