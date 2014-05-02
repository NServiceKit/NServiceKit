using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace NServiceKit.Common.Tests.Models
{
    /// <summary>A model with complex types.</summary>
	public class ModelWithComplexTypes
	{
        /// <summary>Initializes a new instance of the NServiceKit.Common.Tests.Models.ModelWithComplexTypes class.</summary>
		public ModelWithComplexTypes()
		{
			this.StringList = new List<string>();
			this.IntList = new List<int>();
			this.StringMap = new Dictionary<string, string>();
			this.IntMap = new Dictionary<int, int>();
		}

        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
		public long Id { get; set; }

        /// <summary>Gets or sets a list of strings.</summary>
        ///
        /// <value>A List of strings.</value>
		public List<string> StringList { get; set; }

        /// <summary>Gets or sets a list of ints.</summary>
        ///
        /// <value>A List of ints.</value>
		public List<int> IntList { get; set; }

        /// <summary>Gets or sets the string map.</summary>
        ///
        /// <value>The string map.</value>
		public Dictionary<string, string> StringMap { get; set; }

        /// <summary>Gets or sets the int map.</summary>
        ///
        /// <value>The int map.</value>
		public Dictionary<int, int> IntMap { get; set; }

        /// <summary>Gets or sets the child.</summary>
        ///
        /// <value>The child.</value>
		public ModelWithComplexTypes Child { get; set; }

        /// <summary>Creates a new ModelWithComplexTypes.</summary>
        ///
        /// <param name="id">The identifier.</param>
        ///
        /// <returns>The ModelWithComplexTypes.</returns>
		public static ModelWithComplexTypes Create(int id)
		{
			var row = new ModelWithComplexTypes {
				Id = id,
				StringList = { "val" + id + 1, "val" + id + 2, "val" + id + 3 },
				IntList = { id + 1, id + 2, id + 3 },
				StringMap =
            		{
            			{"key" + id + 1, "val" + id + 1},
            			{"key" + id + 2, "val" + id + 2},
            			{"key" + id + 3, "val" + id + 3},
            		},
				IntMap =
            		{
            			{id + 1, id + 2},
            			{id + 3, id + 4},
            			{id + 5, id + 6},
            		},
				Child = new ModelWithComplexTypes { Id = id * 2 },
			};

			return row;
		}

        /// <summary>Creates a constant.</summary>
        ///
        /// <param name="i">Zero-based index of the.</param>
        ///
        /// <returns>The new constant.</returns>
		public static ModelWithComplexTypes CreateConstant(int i)
		{
			return Create(i);
		}

        /// <summary>Assert is equal.</summary>
        ///
        /// <param name="actual">  The actual.</param>
        /// <param name="expected">The expected.</param>
		public static void AssertIsEqual(ModelWithComplexTypes actual, ModelWithComplexTypes expected)
		{
			Assert.That(actual.Id, Is.EqualTo(expected.Id));
			Assert.That(actual.StringList, Is.EquivalentTo(expected.StringList));
			Assert.That(actual.IntList, Is.EquivalentTo(expected.IntList));
			Assert.That(actual.StringMap, Is.EquivalentTo(expected.StringMap));
			Assert.That(actual.IntMap, Is.EquivalentTo(expected.IntMap));

			if (expected.Child == null)
			{
				Assert.That(actual.Child, Is.Null);
			}
			else
			{
				Assert.That(actual.Child, Is.Not.Null);
				AssertIsEqual(actual.Child, expected.Child);
			}
		}
	}
}