using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using NServiceKit.Common.Extensions;

namespace NServiceKit.Common.Tests.Models
{
    /// <summary>A movie.</summary>
	[DataContract]
	public class Movie
	{
        /// <summary>Initializes a new instance of the NServiceKit.Common.Tests.Models.Movie class.</summary>
		public Movie()
		{
			this.Genres = new List<string>();
		}

        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
		[DataMember]
		public string Id { get; set; }

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
        /// <param name="other">The movie to compare to this object.</param>
        ///
        /// <returns>true if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />; otherwise, false.</returns>
		public bool Equals(Movie other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.Id, Id) && Equals(other.Title, Title) && other.Rating == Rating && Equals(other.Director, Director) && other.ReleaseDate.Equals(ReleaseDate) && Equals(other.TagLine, TagLine) && Genres.EquivalentTo(other.Genres);
		}

        /// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />.</summary>
        ///
        /// <param name="obj">The <see cref="T:System.Object" /> to compare with the current <see cref="T:System.Object" />.</param>
        ///
        /// <returns>true if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />; otherwise, false.</returns>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (Movie)) return false;
			return Equals((Movie) obj);
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