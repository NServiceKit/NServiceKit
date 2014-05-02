using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using NServiceKit.Common.Extensions;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface.ServiceModel;

/*
 * Examples of preliminery REST method support in NServiceKit
 */
namespace NServiceKit.WebHost.Endpoints.Tests.IntegrationTests
{
    /// <summary>A rest movies.</summary>
	[DataContract(Namespace = ExampleConfig.DefaultNamespace)]
	[Route("/restmovies/{Id}")]
	public class RestMovies
	{
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
		[DataMember(EmitDefaultValue = false)]
		public string Id { get; set; }

        /// <summary>Gets or sets the movie.</summary>
        ///
        /// <value>The movie.</value>
		[DataMember(EmitDefaultValue = false)]
		public RestMovie Movie { get; set; }
	}

    /// <summary>A rest movies response.</summary>
	[DataContract(Namespace = ExampleConfig.DefaultNamespace)]
	public class RestMoviesResponse
	{
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.IntegrationTests.RestMoviesResponse class.</summary>
		public RestMoviesResponse()
		{
			this.ResponseStatus = new ResponseStatus();
			this.Movies = new List<RestMovie>();
		}

        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
		[DataMember]
		public ResponseStatus ResponseStatus { get; set; }

        /// <summary>Gets or sets the movies.</summary>
        ///
        /// <value>The movies.</value>
		[DataMember(EmitDefaultValue = false)]
		public List<RestMovie> Movies { get; set; }
	}

    /// <summary>A rest movie.</summary>
	[DataContract(Namespace = ExampleConfig.DefaultNamespace)]
	public class RestMovie 
	{
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.IntegrationTests.RestMovie class.</summary>
		public RestMovie()
		{
			this.Genres = new List<string>();
		}

        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
		[DataMember]
		public string Id { get; set; }

        /// <summary>Gets or sets the identifier of the imdb.</summary>
        ///
        /// <value>The identifier of the imdb.</value>
		[DataMember]
		public string ImdbId { get; set; }

        /// <summary>Gets or sets the title.</summary>
        ///
        /// <value>The title.</value>
		[DataMember]
		public string Title { get; set; }

        /// <summary>Gets or sets the rating.</summary>
        ///
        /// <value>The rating.</value>
		[DataMember]
		public decimal Rating { get; set; }

        /// <summary>Gets or sets the director.</summary>
        ///
        /// <value>The director.</value>
		[DataMember]
		public string Director { get; set; }

        /// <summary>Gets or sets the release date.</summary>
        ///
        /// <value>The release date.</value>
		[DataMember]
		public DateTime ReleaseDate { get; set; }

        /// <summary>Gets or sets the tag line.</summary>
        ///
        /// <value>The tag line.</value>
		[DataMember]
		public string TagLine { get; set; }

        /// <summary>Gets or sets the genres.</summary>
        ///
        /// <value>The genres.</value>
		[DataMember]
		public List<string> Genres { get; set; }

        /// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />.</summary>
        ///
        /// <param name="other">The rest movie to compare to this object.</param>
        ///
        /// <returns>true if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />; otherwise, false.</returns>
		public bool Equals(RestMovie other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.Id, Id) && Equals(other.Title, Title) && other.Rating == Rating && Equals(other.Director, Director) && other.ReleaseDate.Equals(ReleaseDate) && Equals(other.TagLine, TagLine) && Genres.EquivalentTo(other.Genres);
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
			if (obj.GetType() != typeof (RestMovie)) return false;
			return Equals((RestMovie) obj);
		}

        /// <summary>Serves as a hash function for a particular type.</summary>
        ///
        /// <returns>A hash code for the current <see cref="T:System.Object" />.</returns>
		public override int GetHashCode()
		{
			unchecked
			{
				int result = (Id != null ? Id.GetHashCode() : 0);
				result = (result*397) ^ (Title != null ? Title.GetHashCode() : 0);
				result = (result*397) ^ Rating.GetHashCode();
				result = (result*397) ^ (Director != null ? Director.GetHashCode() : 0);
				result = (result*397) ^ ReleaseDate.GetHashCode();
				result = (result*397) ^ (TagLine != null ? TagLine.GetHashCode() : 0);
				result = (result*397) ^ (Genres != null ? Genres.GetHashCode() : 0);
				return result;
			}
		}
	}
}