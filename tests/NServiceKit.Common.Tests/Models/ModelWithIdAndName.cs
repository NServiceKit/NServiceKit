using System;
using NUnit.Framework;
using NServiceKit.DataAnnotations;

namespace NServiceKit.Common.Tests.Models
{
    /// <summary>A model with identifier and name.</summary>
	public class ModelWithIdAndName
	{
        /// <summary>Initializes a new instance of the NServiceKit.Common.Tests.Models.ModelWithIdAndName class.</summary>
		public ModelWithIdAndName()
		{
		}

        /// <summary>Initializes a new instance of the NServiceKit.Common.Tests.Models.ModelWithIdAndName class.</summary>
        ///
        /// <param name="id">The identifier.</param>
		public ModelWithIdAndName(int id)
		{
			Id = id;
			Name = "Name" + id;
		}

        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
        [AutoIncrement]
		public int Id { get; set; }

        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
		public string Name { get; set; }

        /// <summary>Creates a new ModelWithIdAndName.</summary>
        ///
        /// <param name="id">The identifier.</param>
        ///
        /// <returns>A ModelWithIdAndName.</returns>
		public static ModelWithIdAndName Create(int id)
		{
			return new ModelWithIdAndName(id);
		}

        /// <summary>Assert is equal.</summary>
        ///
        /// <param name="actual">  The actual.</param>
        /// <param name="expected">The expected.</param>
		public static void AssertIsEqual(ModelWithIdAndName actual, ModelWithIdAndName expected)
		{
			if (actual == null || expected == null)
			{
				Assert.That(actual == expected, Is.True);
				return;
			}

			Assert.That(actual.Id, Is.EqualTo(expected.Id));
			Assert.That(actual.Name, Is.EqualTo(expected.Name));
		}

        /// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />.</summary>
        ///
        /// <param name="other">The model with identifier and name to compare to this object.</param>
        ///
        /// <returns>true if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />; otherwise, false.</returns>
		public bool Equals(ModelWithIdAndName other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return other.Id == Id && Equals(other.Name, Name);
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
			if (obj.GetType() != typeof (ModelWithIdAndName)) return false;
			return Equals((ModelWithIdAndName) obj);
		}

        /// <summary>Serves as a hash function for a particular type.</summary>
        ///
        /// <returns>A hash code for the current <see cref="T:System.Object" />.</returns>
		public override int GetHashCode()
		{
			unchecked
			{
				return (Id*397) ^ (Name != null ? Name.GetHashCode() : 0);
			}
		}
	}
}