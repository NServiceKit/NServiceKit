using System;
using NServiceKit.DesignPatterns.Model;

namespace NServiceKit.Common.Tests.Models
{
    /// <summary>A shipper.</summary>
	public class Shipper
		: IHasIntId
	{
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
		public int Id { get; set; }

        /// <summary>Gets or sets the name of the company.</summary>
        ///
        /// <value>The name of the company.</value>
		public string CompanyName { get; set; }

        /// <summary>Gets or sets the type of the shipper.</summary>
        ///
        /// <value>The type of the shipper.</value>
		public ShipperType ShipperType { get; set; }

        /// <summary>Gets or sets the Date/Time of the date created.</summary>
        ///
        /// <value>The date created.</value>
		public DateTime DateCreated { get; set; }

        /// <summary>Gets or sets the unique reference.</summary>
        ///
        /// <value>The unique reference.</value>
		public Guid UniqueRef { get; set; }

        /// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />.</summary>
        ///
        /// <param name="obj">The <see cref="T:System.Object" /> to compare with the current <see cref="T:System.Object" />.</param>
        ///
        /// <returns>true if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />; otherwise, false.</returns>
		public override bool Equals(object obj)
		{
			var other = obj as Shipper;
			if (other == null) return false;
			return this.Id == other.Id && this.UniqueRef == other.UniqueRef;
		}

        /// <summary>Serves as a hash function for a particular type.</summary>
        ///
        /// <returns>A hash code for the current <see cref="T:System.Object" />.</returns>
		public override int GetHashCode()
		{
			return string.Concat(Id, UniqueRef).GetHashCode();
		}
	}

    /// <summary>Values that represent ShipperType.</summary>
	public enum ShipperType
	{
        /// <summary>An enum constant representing all option.</summary>
		All = Planes | Trains | Automobiles,

        /// <summary>An enum constant representing the unknown option.</summary>
		Unknown = 0,

        /// <summary>An enum constant representing the planes option.</summary>
		Planes = 1,

        /// <summary>An enum constant representing the trains option.</summary>
		Trains = 2,

        /// <summary>An enum constant representing the automobiles option.</summary>
		Automobiles = 4
	}
}