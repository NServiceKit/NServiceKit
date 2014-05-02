using System.Collections.Generic;
using NUnit.Framework;

namespace NServiceKit.Common.Tests.Models
{
    /// <summary>List of model with map ands.</summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
	public class ModelWithMapAndList<T>
	{
        /// <summary>Initializes a new instance of the NServiceKit.Common.Tests.Models.ModelWithMapAndList&lt;T&gt; class.</summary>
		public ModelWithMapAndList()
		{
			this.Map = new Dictionary<T, T>();
			this.List = new List<T>();
		}

        /// <summary>Initializes a new instance of the NServiceKit.Common.Tests.Models.ModelWithMapAndList&lt;T&gt; class.</summary>
        ///
        /// <param name="id">The identifier.</param>
		public ModelWithMapAndList(int id)
			: this()
		{
			Id = id;
			Name = "Name" + id;
		}

        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
		public int Id { get; set; }

        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
		public string Name { get; set; }

        /// <summary>Gets or sets the map.</summary>
        ///
        /// <value>The map.</value>
		public Dictionary<T, T> Map { get; set; }

        /// <summary>Gets or sets the list.</summary>
        ///
        /// <value>The list.</value>
		public List<T> List { get; set; }

        /// <summary>Creates a new ModelWithMapAndList&lt;T&gt;</summary>
        ///
        /// <typeparam name="U">Generic type parameter.</typeparam>
        /// <param name="id">The identifier.</param>
        ///
        /// <returns>A list of.</returns>
		public static ModelWithMapAndList<T> Create<U>(int id)
		{
			return new ModelWithMapAndList<T>(id);
		}

        /// <summary>Assert is equal.</summary>
        ///
        /// <param name="actual">  The actual.</param>
        /// <param name="expected">The expected.</param>
		public static void AssertIsEqual(ModelWithMapAndList<T> actual, ModelWithMapAndList<T> expected)
		{
			Assert.That(actual.Id, Is.EqualTo(expected.Id));
			Assert.That(actual.Name, Is.EqualTo(expected.Name));
			Assert.That(actual.Map, Is.EquivalentTo(expected.Map));
			Assert.That(actual.List, Is.EquivalentTo(expected.List));
		}
	}
}