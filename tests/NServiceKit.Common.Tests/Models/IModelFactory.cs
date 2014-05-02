using System.Collections.Generic;

namespace NServiceKit.Common.Tests.Models
{
    /// <summary>Interface for model factory.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
	public interface IModelFactory<T>
	{
        /// <summary>Assert lists are equal.</summary>
        ///
        /// <param name="actualList">  List of actuals.</param>
        /// <param name="expectedList">List of expected.</param>
		void AssertListsAreEqual(List<T> actualList, IList<T> expectedList);

        /// <summary>Assert is equal.</summary>
        ///
        /// <param name="actual">  The actual.</param>
        /// <param name="expected">The expected.</param>
		void AssertIsEqual(T actual, T expected);

        /// <summary>Gets the existing value.</summary>
        ///
        /// <value>The existing value.</value>
		T ExistingValue { get; }

        /// <summary>Gets the non existing value.</summary>
        ///
        /// <value>The non existing value.</value>
		T NonExistingValue { get; }

        /// <summary>Creates the list.</summary>
        ///
        /// <returns>The new list.</returns>
		List<T> CreateList();

        /// <summary>Creates list 2.</summary>
        ///
        /// <returns>The new list 2.</returns>
		List<T> CreateList2();

        /// <summary>Creates an instance.</summary>
        ///
        /// <param name="i">Zero-based index of the.</param>
        ///
        /// <returns>The new instance.</returns>
		T CreateInstance(int i);
	}
}