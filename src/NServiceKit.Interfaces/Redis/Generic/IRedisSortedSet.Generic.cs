//
// https://github.com/mythz/NServiceKit.Redis
// NServiceKit.Redis: ECMA CLI Binding to the Redis key-value storage system
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2013 ServiceStack.
//
// Licensed under the same terms of Redis and ServiceStack: new BSD license.
//

using System.Collections.Generic;
using NServiceKit.DesignPatterns.Model;

namespace NServiceKit.Redis.Generic
{
    /// <summary>Interface for redis sorted set.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
	public interface IRedisSortedSet<T> : ICollection<T>, IHasStringId
	{
        /// <summary>Pops the item with highest score.</summary>
        ///
        /// <returns>A T.</returns>
		T PopItemWithHighestScore();

        /// <summary>Pops the item with lowest score.</summary>
        ///
        /// <returns>A T.</returns>
		T PopItemWithLowestScore();

        /// <summary>Increment item.</summary>
        ///
        /// <param name="item">       The item.</param>
        /// <param name="incrementBy">Amount to increment by.</param>
        ///
        /// <returns>A double.</returns>
		double IncrementItem(T item, double incrementBy);

        /// <summary>Index of the given item.</summary>
        ///
        /// <param name="item">The item.</param>
        ///
        /// <returns>An int.</returns>
		int IndexOf(T item);

        /// <summary>Searches for the first descending.</summary>
        ///
        /// <param name="item">The item.</param>
        ///
        /// <returns>The zero-based index of the found descending, or -1 if no match was found.</returns>
		long IndexOfDescending(T item);

        /// <summary>Gets all.</summary>
        ///
        /// <returns>all.</returns>
		List<T> GetAll();

        /// <summary>Gets all descending.</summary>
        ///
        /// <returns>all descending.</returns>
		List<T> GetAllDescending();

        /// <summary>Finds the range of the given arguments.</summary>
        ///
        /// <param name="fromRank">from rank.</param>
        /// <param name="toRank">  to rank.</param>
        ///
        /// <returns>The calculated range.</returns>
		List<T> GetRange(int fromRank, int toRank);

        /// <summary>Gets range by lowest score.</summary>
        ///
        /// <param name="fromScore">from score.</param>
        /// <param name="toScore">  to score.</param>
        ///
        /// <returns>The range by lowest score.</returns>
		List<T> GetRangeByLowestScore(double fromScore, double toScore);

        /// <summary>Gets range by lowest score.</summary>
        ///
        /// <param name="fromScore">from score.</param>
        /// <param name="toScore">  to score.</param>
        /// <param name="skip">     The skip.</param>
        /// <param name="take">     The take.</param>
        ///
        /// <returns>The range by lowest score.</returns>
		List<T> GetRangeByLowestScore(double fromScore, double toScore, int? skip, int? take);

        /// <summary>Gets range by highest score.</summary>
        ///
        /// <param name="fromScore">from score.</param>
        /// <param name="toScore">  to score.</param>
        ///
        /// <returns>The range by highest score.</returns>
		List<T> GetRangeByHighestScore(double fromScore, double toScore);

        /// <summary>Gets range by highest score.</summary>
        ///
        /// <param name="fromScore">from score.</param>
        /// <param name="toScore">  to score.</param>
        /// <param name="skip">     The skip.</param>
        /// <param name="take">     The take.</param>
        ///
        /// <returns>The range by highest score.</returns>
		List<T> GetRangeByHighestScore(double fromScore, double toScore, int? skip, int? take);

        /// <summary>Removes the range.</summary>
        ///
        /// <param name="minRank">The minimum rank.</param>
        /// <param name="maxRank">The maximum rank.</param>
        ///
        /// <returns>A long.</returns>
		long RemoveRange(int minRank, int maxRank);

        /// <summary>Removes the range by score.</summary>
        ///
        /// <param name="fromScore">from score.</param>
        /// <param name="toScore">  to score.</param>
        ///
        /// <returns>A long.</returns>
		long RemoveRangeByScore(double fromScore, double toScore);

        /// <summary>Gets item score.</summary>
        ///
        /// <param name="item">The item.</param>
        ///
        /// <returns>The item score.</returns>
		double GetItemScore(T item);

        /// <summary>Populate with intersect of the given set identifiers.</summary>
        ///
        /// <param name="setIds">List of identifiers for the sets.</param>
        ///
        /// <returns>A long.</returns>
		long PopulateWithIntersectOf(params IRedisSortedSet<T>[] setIds);

        /// <summary>Populate with union of the given set identifiers.</summary>
        ///
        /// <param name="setIds">List of identifiers for the sets.</param>
        ///
        /// <returns>A long.</returns>
		long PopulateWithUnionOf(params IRedisSortedSet<T>[] setIds);
	}
}