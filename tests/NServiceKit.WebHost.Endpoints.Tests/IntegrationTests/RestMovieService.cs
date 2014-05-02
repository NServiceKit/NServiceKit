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
        /// <summary>Gets or sets the database factory.</summary>
        ///
        /// <value>The database factory.</value>
		public IDbConnectionFactory DbFactory { get; set; }

        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>A RestMoviesResponse.</returns>
        public RestMoviesResponse Any(RestMovies request)
		{
			return Get(request);
		}

        /// <summary>Gets the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>A RestMoviesResponse.</returns>
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

        /// <summary>Puts the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>A RestMoviesResponse.</returns>
        public RestMoviesResponse Put(RestMovies request)
		{
			DbFactory.Run(db => db.Save(request.Movie));
			return new RestMoviesResponse();
		}

        /// <summary>Deletes the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>A RestMoviesResponse.</returns>
        public RestMoviesResponse Delete(RestMovies request)
		{
            DbFactory.Run(db => db.DeleteById<RestMovie>(request.Id));
			return new RestMoviesResponse();
		}

        /// <summary>Post this message.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>A RestMoviesResponse.</returns>
        public RestMoviesResponse Post(RestMovies request)
		{
            DbFactory.Run(db => db.Update(request.Movie));
			return new RestMoviesResponse();
		}
	}
}