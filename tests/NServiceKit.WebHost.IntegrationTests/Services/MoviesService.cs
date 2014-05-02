using System.Collections.Generic;
using System.Runtime.Serialization;
using NServiceKit.Common.Extensions;
using NServiceKit.OrmLite;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.IntegrationTests.Services
{
    /// <summary>A movies.</summary>
	[DataContract]
	[Route("/movies", "GET, OPTIONS")]
	[Route("/movies/genres/{Genre}")]
	public class Movies
	{
        /// <summary>Gets or sets the genre.</summary>
        ///
        /// <value>The genre.</value>
		[DataMember]
		public string Genre { get; set; }

        /// <summary>Gets or sets the movie.</summary>
        ///
        /// <value>The movie.</value>
		[DataMember]
		public Movie Movie { get; set; }
	}

    /// <summary>The movies response.</summary>
	[DataContract]
	public class MoviesResponse
	{
        /// <summary>Gets or sets the movies.</summary>
        ///
        /// <value>The movies.</value>
		[DataMember(Order = 1)]
		public List<Movie> Movies { get; set; }
	}

    /// <summary>The movies service.</summary>
	public class MoviesService : ServiceInterface.Service
	{
		/// <summary>
		/// GET /movies 
		/// GET /movies/genres/{Genre}
		/// </summary>
		public object Get(Movies request)
		{
			return new MoviesResponse
			{
				Movies = request.Genre.IsNullOrEmpty()
					? Db.Select<Movie>()
					: Db.Select<Movie>("Genres LIKE {0}", "%" + request.Genre + "%")
			};
		}
	}
}