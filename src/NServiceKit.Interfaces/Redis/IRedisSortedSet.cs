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

namespace NServiceKit.Redis
{
    /// <summary>Interface for redis sorted set.</summary>
	public interface IRedisSortedSet
		: ICollection<string>, IHasStringId
	{
        /// <summary>Gets all.</summary>
        ///
        /// <returns>all.</returns>
		List<string> GetAll();

        /// <summary>Finds the range of the given arguments.</summary>
        ///
        /// <param name="startingRank">The starting rank.</param>
        /// <param name="endingRank">  The ending rank.</param>
        ///
        /// <returns>The calculated range.</returns>
		List<string> GetRange(int startingRank, int endingRank);

        /// <summary>Gets range by score.</summary>
        ///
        /// <param name="fromStringScore">from string score.</param>
        /// <param name="toStringScore">  to string score.</param>
        ///
        /// <returns>The range by score.</returns>
		List<string> GetRangeByScore(string fromStringScore, string toStringScore);

        /// <summary>Gets range by score.</summary>
        ///
        /// <param name="fromStringScore">from string score.</param>
        /// <param name="toStringScore">  to string score.</param>
        /// <param name="skip">           The skip.</param>
        /// <param name="take">           The take.</param>
        ///
        /// <returns>The range by score.</returns>
		List<string> GetRangeByScore(string fromStringScore, string toStringScore, int? skip, int? take);

        /// <summary>Gets range by score.</summary>
        ///
        /// <param name="fromScore">from score.</param>
        /// <param name="toScore">  to score.</param>
        ///
        /// <returns>The range by score.</returns>
		List<string> GetRangeByScore(double fromScore, double toScore);

        /// <summary>Gets range by score.</summary>
        ///
        /// <param name="fromScore">from score.</param>
        /// <param name="toScore">  to score.</param>
        /// <param name="skip">     The skip.</param>
        /// <param name="take">     The take.</param>
        ///
        /// <returns>The range by score.</returns>
		List<string> GetRangeByScore(double fromScore, double toScore, int? skip, int? take);

        /// <summary>Removes the range.</summary>
        ///
        /// <param name="fromRank">from rank.</param>
        /// <param name="toRank">  to rank.</param>
		void RemoveRange(int fromRank, int toRank);

        /// <summary>Removes the range by score.</summary>
        ///
        /// <param name="fromScore">from score.</param>
        /// <param name="toScore">  to score.</param>
		void RemoveRangeByScore(double fromScore, double toScore);

        /// <summary>Stores from intersect.</summary>
        ///
        /// <param name="ofSets">Sets the of belongs to.</param>
		void StoreFromIntersect(params IRedisSortedSet[] ofSets);

        /// <summary>Stores from union.</summary>
        ///
        /// <param name="ofSets">Sets the of belongs to.</param>
		void StoreFromUnion(params IRedisSortedSet[] ofSets);

        /// <summary>Gets item index.</summary>
        ///
        /// <param name="value">The value.</param>
        ///
        /// <returns>The item index.</returns>
		long GetItemIndex(string value);

        /// <summary>Gets item score.</summary>
        ///
        /// <param name="value">The value.</param>
        ///
        /// <returns>The item score.</returns>
		double GetItemScore(string value);

        /// <summary>Increment item score.</summary>
        ///
        /// <param name="value">           The value.</param>
        /// <param name="incrementByScore">The increment by score.</param>
		void IncrementItemScore(string value, double incrementByScore);

        /// <summary>Pops the item with highest score.</summary>
        ///
        /// <returns>A string.</returns>
		string PopItemWithHighestScore();

        /// <summary>Pops the item with lowest score.</summary>
        ///
        /// <returns>A string.</returns>
		string PopItemWithLowestScore();
	}
}