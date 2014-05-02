using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using NServiceKit.OrmLite;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface.ServiceModel;

namespace NServiceKit.WebHost.IntegrationTests.Services
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
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.IntegrationTests.Services.ResetMoviesResponse class.</summary>
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
        /// <summary>The top 5 movies.</summary>
		public static List<Movie> Top5Movies = new List<Movie>
		{
			new Movie { ImdbId = "tt0111161", Title = "The Shawshank Redemption", Rating = 9.2m, Director = "Frank Darabont", ReleaseDate = new DateTime(1995,2,17), TagLine = "Fear can hold you prisoner. Hope can set you free.", Genres = new List<string>{"Crime","Drama"}, },
			new Movie { ImdbId = "tt0068646", Title = "The Godfather", Rating = 9.2m, Director = "Francis Ford Coppola", ReleaseDate = new DateTime(1972,3,24), TagLine = "An offer you can't refuse.", Genres = new List<string> {"Crime","Drama", "Thriller"}, },
			new Movie { ImdbId = "tt1375666", Title = "Inception", Rating = 9.2m, Director = "Christopher Nolan", ReleaseDate = new DateTime(2010,7,16), TagLine = "Your mind is the scene of the crime", Genres = new List<string>{"Action", "Mystery", "Sci-Fi", "Thriller"}, },
			new Movie { ImdbId = "tt0071562", Title = "The Godfather: Part II", Rating = 9.0m, Director = "Francis Ford Coppola", ReleaseDate = new DateTime(1974,12,20), Genres = new List<string> {"Crime","Drama", "Thriller"}, },
			new Movie { ImdbId = "tt0060196", Title = "The Good, the Bad and the Ugly", Rating = 9.0m, Director = "Sergio Leone", ReleaseDate = new DateTime(1967,12,29), TagLine = "They formed an alliance of hate to steal a fortune in dead man's gold", Genres = new List<string>{"Adventure","Western"}, },
		};

        /// <summary>Gets or sets the database factory.</summary>
        ///
        /// <value>The database factory.</value>
		public IDbConnectionFactory DbFactory { get; set; }

        /// <summary>Post this message.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public object Post(ResetMovies request)
		{
            const bool overwriteTable = true;
            Db.CreateTable<Movie>(overwriteTable);
            Db.SaveAll(Top5Movies);

			return new ResetMoviesResponse();
		}
	}
}