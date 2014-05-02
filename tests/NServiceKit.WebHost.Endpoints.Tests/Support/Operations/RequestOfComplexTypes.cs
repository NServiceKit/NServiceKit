using System.Collections.Generic;
using System.Runtime.Serialization;
using NServiceKit.Common.Extensions;

namespace NServiceKit.WebHost.Endpoints.Tests.Support.Operations
{
    /// <summary>A request of complex types.</summary>
	[DataContract]
	public class RequestOfComplexTypes
	{
        /// <summary>Gets or sets a list of ints.</summary>
        ///
        /// <value>A List of ints.</value>
		[DataMember]
		public List<int> IntList { get; set; }

        /// <summary>Gets or sets a list of strings.</summary>
        ///
        /// <value>A List of strings.</value>
		[DataMember]
		public List<string> StringList { get; set; }

        /// <summary>Gets or sets an array of ints.</summary>
        ///
        /// <value>An Array of ints.</value>
		[DataMember]
		public int[] IntArray { get; set; }

        /// <summary>Gets or sets an array of strings.</summary>
        ///
        /// <value>An Array of strings.</value>
		[DataMember]
		public string[] StringArray { get; set; }

        /// <summary>Gets or sets the int map.</summary>
        ///
        /// <value>The int map.</value>
		[DataMember]
		public Dictionary<int, int> IntMap { get; set; }

        /// <summary>Gets or sets the string map.</summary>
        ///
        /// <value>The string map.</value>
		[DataMember]
		public Dictionary<string, string> StringMap { get; set; }

        /// <summary>Gets or sets the string int map.</summary>
        ///
        /// <value>The string int map.</value>
		[DataMember]
		public Dictionary<string, int> StringIntMap { get; set; }

        /// <summary>Gets or sets a list of types of the request of alls.</summary>
        ///
        /// <value>A list of types of the request of alls.</value>
		[DataMember]
		public RequestOfAllTypes RequestOfAllTypes { get; set; }

        /// <summary>Creates a new RequestOfComplexTypes.</summary>
        ///
        /// <param name="i">Zero-based index of the.</param>
        ///
        /// <returns>The RequestOfComplexTypes.</returns>
		public static RequestOfComplexTypes Create(int i)
		{
			return new RequestOfComplexTypes {
				IntArray = new[] { i, i + 1 },
				IntList = new List<int> { i, i + 1 },
				IntMap = new Dictionary<int, int> { { i, i + 1 }, { i + 2, i + 3 } },
				StringArray = new[] { "String" + i, "String" + i + 1 },
				StringList = new List<string> { "String" + i, "String" + (i + 1) },
				StringMap = new Dictionary<string, string> { { "String" + i, "String" + "String" + i + 1 }, { "String" + i + 2, "String" + i + 3 } },
				StringIntMap = new Dictionary<string, int> { { "String" + i, i }, { "String" + i + 1, i + 1 } },
				RequestOfAllTypes = RequestOfAllTypes.Create(i),
			};
		}

        /// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />.</summary>
        ///
        /// <param name="obj">The object to compare with the current object.</param>
        ///
        /// <returns>true if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />; otherwise, false.</returns>
		public override bool Equals(object obj)
		{
			var other = obj as RequestOfComplexTypes;
			if (other == null) return false;

			return this.IntArray.EquivalentTo(other.IntArray)
			       && this.IntList.EquivalentTo(other.IntList)
			       && this.IntMap.EquivalentTo(other.IntMap)
			       && this.StringArray.EquivalentTo(other.StringArray)
			       && this.StringList.EquivalentTo(other.StringList)
			       && this.StringIntMap.EquivalentTo(other.StringIntMap)
			       && this.RequestOfAllTypes.Equals(other.RequestOfAllTypes);
		}

        /// <summary>Serves as a hash function for a particular type.</summary>
        ///
        /// <returns>A hash code for the current <see cref="T:System.Object" />.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
	}
}