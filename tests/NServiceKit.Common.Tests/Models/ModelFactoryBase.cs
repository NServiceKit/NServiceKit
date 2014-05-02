using System.Collections.Generic;
using NUnit.Framework;

namespace NServiceKit.Common.Tests.Models
{
    /// <summary>A model factory base.</summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
	public abstract class ModelFactoryBase<T>
		: IModelFactory<T>
	{
		#region Implementation of IModelFactory<T>

        /// <summary>Assert lists are equal.</summary>
        ///
        /// <param name="actualList">  List of actuals.</param>
        /// <param name="expectedList">List of expected.</param>
		public void AssertListsAreEqual(List<T> actualList, IList<T> expectedList)
		{
			Assert.That(actualList, Has.Count.EqualTo(expectedList.Count));
			var i = 0;

			actualList.ForEach(x =>
				AssertIsEqual(x, expectedList[i++]));
		}

        /// <summary>Creates an instance.</summary>
        ///
        /// <param name="i">Zero-based index of the.</param>
        ///
        /// <returns>The new instance.</returns>
		public abstract T CreateInstance(int i);

        /// <summary>Assert is equal.</summary>
        ///
        /// <param name="actual">  The actual.</param>
        /// <param name="expected">The expected.</param>
		public abstract void AssertIsEqual(T actual, T expected);

        /// <summary>Gets the existing value.</summary>
        ///
        /// <value>The existing value.</value>
		public T ExistingValue
		{
			get
			{
				return CreateInstance(4);
			}
		}

        /// <summary>Gets the non existing value.</summary>
        ///
        /// <value>The non existing value.</value>
		public T NonExistingValue
		{
			get
			{
				return CreateInstance(5);
			}
		}

        /// <summary>Creates the list.</summary>
        ///
        /// <returns>The new list.</returns>
		public List<T> CreateList()
		{
			return new List<T> 
			{
				CreateInstance(1),
				CreateInstance(2),
				CreateInstance(3),
				CreateInstance(4),
			};
		}

        /// <summary>Creates list 2.</summary>
        ///
        /// <returns>The new list 2.</returns>
		public List<T> CreateList2()
		{
			return new List<T> 
			{
				CreateInstance(5),
			    CreateInstance(6),
			    CreateInstance(7),
			};
		}

		#endregion
	}
}