using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using NServiceKit.Common.Extensions;
using NServiceKit.Common.Web;
using NServiceKit.DataAnnotations;
using NServiceKit.OrmLite;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface;
using NServiceKit.Text;

namespace NServiceKit.WebHost.IntegrationTests.Services
{

    /// <summary>A movie.</summary>
	[Route("/movies", "POST,PUT,PATCH")]
	[Route("/movies/{Id}")]
	[DataContract]
	public class Movie
	{
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.IntegrationTests.Services.Movie class.</summary>
		public Movie()
		{
			this.Genres = new List<string>();
		}

        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
        [DataMember(Order = 1)]
		[AutoIncrement]
		public int Id { get; set; }

        /// <summary>Gets or sets the identifier of the imdb.</summary>
        ///
        /// <value>The identifier of the imdb.</value>
        [DataMember(Order = 2)]
		public string ImdbId { get; set; }

        /// <summary>Gets or sets the title.</summary>
        ///
        /// <value>The title.</value>
        [DataMember(Order = 3)]
		public string Title { get; set; }

        /// <summary>Gets or sets the rating.</summary>
        ///
        /// <value>The rating.</value>
        [DataMember(Order = 4)]
		public decimal Rating { get; set; }

        /// <summary>Gets or sets the director.</summary>
        ///
        /// <value>The director.</value>
        [DataMember(Order = 5)]
		public string Director { get; set; }

        /// <summary>Gets or sets the release date.</summary>
        ///
        /// <value>The release date.</value>
        [DataMember(Order = 6)]
		public DateTime ReleaseDate { get; set; }

        /// <summary>Gets or sets the tag line.</summary>
        ///
        /// <value>The tag line.</value>
        [DataMember(Order = 7)]
		public string TagLine { get; set; }

        /// <summary>Gets or sets the genres.</summary>
        ///
        /// <value>The genres.</value>
        [DataMember(Order = 8)]
		public List<string> Genres { get; set; }

		#region AutoGen ReSharper code, only required by tests

        /// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />.</summary>
        ///
        /// <param name="other">The movie to compare to this object.</param>
        ///
        /// <returns>true if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />; otherwise, false.</returns>
		public bool Equals(Movie other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.ImdbId, ImdbId)
				&& Equals(other.Title, Title)
				&& other.Rating == Rating
				&& Equals(other.Director, Director)
				&& other.ReleaseDate.Equals(ReleaseDate)
				&& Equals(other.TagLine, TagLine)
				&& Genres.EquivalentTo(other.Genres);
		}

        /// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />.</summary>
        ///
        /// <param name="obj">The object to compare with the current object.</param>
        ///
        /// <returns>true if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />; otherwise, false.</returns>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(Movie)) return false;
			return Equals((Movie)obj);
		}

        /// <summary>Serves as a hash function for a particular type.</summary>
        ///
        /// <returns>A hash code for the current <see cref="T:System.Object" />.</returns>
		public override int GetHashCode()
		{
			return ImdbId != null ? ImdbId.GetHashCode() : 0;
		}
		#endregion
	}

    /// <summary>A movie response.</summary>
	[DataContract]
	public class MovieResponse
	{
        /// <summary>Gets or sets the movie.</summary>
        ///
        /// <value>The movie.</value>
		[DataMember]
		public Movie Movie { get; set; }
	}

    /// <summary>A movie service.</summary>
	public class MovieService : ServiceInterface.Service
	{
		/// <summary>
		/// GET /movies/{Id} 
		/// </summary>
		public object Get(Movie movie)
		{
			return new MovieResponse {
				Movie = Db.GetById<Movie>(movie.Id)
			};
		}

		/// <summary>
		/// POST /movies
		/// </summary>
		public object Post(Movie movie)
		{
            Db.Insert(movie);
			var newMovieId = Db.GetLastInsertId();

			var newMovie = new MovieResponse {
				Movie = Db.GetById<Movie>(newMovieId)
			};
			return new HttpResult(newMovie) {
				StatusCode = HttpStatusCode.Created,
				Headers = {
					{ HttpHeaders.Location, this.RequestContext.AbsoluteUri.WithTrailingSlash() + newMovieId }
				}
			};
		}

		/// <summary>
		/// PUT /movies
		/// </summary>
		public object Put(Movie movie)
		{
		    Db.Update(movie);
			return new MovieResponse();
		}

		/// <summary>
		/// DELETE /movies/{Id}
		/// </summary>
		public object Delete(Movie request)
		{
			Db.DeleteById<Movie>(request.Id);
			return new MovieResponse();
		}

		/// <summary>
		/// PATCH /movies
		/// </summary>
		public object Patch(Movie movie)
		{
            var existingMovie = Db.GetById<Movie>(movie.Id);
            if (movie.Title != null)
                existingMovie.Title = movie.Title;
            Db.Save(existingMovie);
            
            return new MovieResponse();
		}
	}
}