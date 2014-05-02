using System;
using System.Runtime.Serialization;

namespace NServiceKit.WebHost.Endpoints.Tests.Support.Operations
{
    /// <summary>A request of all types.</summary>
	[DataContract]
	public class RequestOfAllTypes
	{
        /// <summary>Gets or sets the character.</summary>
        ///
        /// <value>The character.</value>
		[DataMember]
		public char Char { get; set; }

        /// <summary>Gets or sets the string.</summary>
        ///
        /// <value>The string.</value>
		[DataMember]
		public string String { get; set; }

        /// <summary>Gets or sets the byte.</summary>
        ///
        /// <value>The byte.</value>
		[DataMember]
		public byte Byte { get; set; }

        /// <summary>Gets or sets the short.</summary>
        ///
        /// <value>The short.</value>
		[DataMember]
		public short Short { get; set; }

        /// <summary>Gets or sets the int.</summary>
        ///
        /// <value>The int.</value>
		[DataMember]
		public int Int { get; set; }

        /// <summary>Gets or sets the int.</summary>
        ///
        /// <value>The u int.</value>
		[DataMember]
		public uint UInt { get; set; }

        /// <summary>Gets or sets the long.</summary>
        ///
        /// <value>The long.</value>
		[DataMember]
		public long Long { get; set; }

        /// <summary>Gets or sets the long.</summary>
        ///
        /// <value>The u long.</value>
		[DataMember]
		public ulong ULong { get; set; }

        /// <summary>Gets or sets the float.</summary>
        ///
        /// <value>The float.</value>
		[DataMember]
		public float Float { get; set; }

        /// <summary>Gets or sets the double.</summary>
        ///
        /// <value>The double.</value>
		[DataMember]
		public double Double { get; set; }

        /// <summary>Gets or sets the decimal.</summary>
        ///
        /// <value>The decimal.</value>
		[DataMember]
		public decimal Decimal { get; set; }

        /// <summary>Gets or sets a unique identifier.</summary>
        ///
        /// <value>The identifier of the unique.</value>
		[DataMember]
		public Guid Guid { get; set; }

        /// <summary>Gets or sets the date time.</summary>
        ///
        /// <value>The date time.</value>
		[DataMember]
		public DateTime DateTime { get; set; }

        /// <summary>Gets or sets the time span.</summary>
        ///
        /// <value>The time span.</value>
		[DataMember]
		public TimeSpan TimeSpan { get; set; }

        /// <summary>Creates a new RequestOfAllTypes.</summary>
        ///
        /// <param name="i">Zero-based index of the.</param>
        ///
        /// <returns>The RequestOfAllTypes.</returns>
		public static RequestOfAllTypes Create(int i)
		{
			return new RequestOfAllTypes {
				Byte = (byte)i,
				Char = (char)i,
				DateTime = new DateTime(i % 2000, 1, 1),
				Decimal = i,
				Double = i,
				Float = i,
				Guid = System.Guid.NewGuid(),
				Int = i,
				Long = i,
				Short = (short)i,
				String = i.ToString(),
				TimeSpan = new TimeSpan(i, 1, 1, 1),
				UInt = (uint)i,
				ULong = (ulong)i,
			};
		}

        /// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />.</summary>
        ///
        /// <param name="obj">The object to compare with the current object.</param>
        ///
        /// <returns>true if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />; otherwise, false.</returns>
		public override bool Equals(object obj)
		{
			var other = obj as RequestOfAllTypes;
			if (other == null) return false;

			return this.Byte == other.Byte
				   && this.Char == other.Char
				   && this.DateTime == other.DateTime
				   && this.Decimal == other.Decimal
				   && this.Double == other.Double
				   && this.Float == other.Float
				   && this.Guid == other.Guid
				   && this.Int == other.Int
				   && this.Long == other.Long
				   && this.Short == other.Short
				   && this.String == other.String
				   && this.TimeSpan == other.TimeSpan
				   && this.UInt == other.UInt
				   && this.ULong == other.ULong;
		}

        /// <summary>Serves as a hash function for a particular type.</summary>
        ///
        /// <returns>A hash code for the current <see cref="T:System.Object" />.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
	}

    /// <summary>A request of all types response.</summary>
	[DataContract]
	public class RequestOfAllTypesResponse
	{
        /// <summary>Gets or sets the character.</summary>
        ///
        /// <value>The character.</value>
		[DataMember]
		public char Char { get; set; }

        /// <summary>Gets or sets the string.</summary>
        ///
        /// <value>The string.</value>
		[DataMember]
		public string String { get; set; }

        /// <summary>Gets or sets the byte.</summary>
        ///
        /// <value>The byte.</value>
		[DataMember]
		public byte Byte { get; set; }

        /// <summary>Gets or sets the short.</summary>
        ///
        /// <value>The short.</value>
		[DataMember]
		public short Short { get; set; }

        /// <summary>Gets or sets the int.</summary>
        ///
        /// <value>The int.</value>
		[DataMember]
		public int Int { get; set; }

        /// <summary>Gets or sets the int.</summary>
        ///
        /// <value>The u int.</value>
		[DataMember]
		public uint UInt { get; set; }

        /// <summary>Gets or sets the long.</summary>
        ///
        /// <value>The long.</value>
		[DataMember]
		public long Long { get; set; }

        /// <summary>Gets or sets the long.</summary>
        ///
        /// <value>The u long.</value>
		[DataMember]
		public ulong ULong { get; set; }

        /// <summary>Gets or sets the float.</summary>
        ///
        /// <value>The float.</value>
		[DataMember]
		public float Float { get; set; }

        /// <summary>Gets or sets the double.</summary>
        ///
        /// <value>The double.</value>
		[DataMember]
		public double Double { get; set; }

        /// <summary>Gets or sets the decimal.</summary>
        ///
        /// <value>The decimal.</value>
		[DataMember]
		public decimal Decimal { get; set; }

        /// <summary>Gets or sets a unique identifier.</summary>
        ///
        /// <value>The identifier of the unique.</value>
		[DataMember]
		public Guid Guid { get; set; }

        /// <summary>Gets or sets the date time.</summary>
        ///
        /// <value>The date time.</value>
		[DataMember]
		public DateTime DateTime { get; set; }

        /// <summary>Gets or sets the time span.</summary>
        ///
        /// <value>The time span.</value>
		[DataMember]
		public TimeSpan TimeSpan { get; set; }
	}
}
