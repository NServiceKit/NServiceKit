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
#if WINDOWS_PHONE
using NServiceKit.Text.WP;
#endif

namespace NServiceKit.Redis
{
    /// <summary>Interface for redis set.</summary>
	public interface IRedisSet
		: ICollection<string>, IHasStringId
	{
        /// <summary>Gets range from sorted set.</summary>
        ///
        /// <param name="startingFrom">The starting from.</param>
        /// <param name="endingAt">    The ending at.</param>
        ///
        /// <returns>The range from sorted set.</returns>
		List<string> GetRangeFromSortedSet(int startingFrom, int endingAt);

        /// <summary>Gets all.</summary>
        ///
        /// <returns>all.</returns>
		HashSet<string> GetAll();

        /// <summary>Removes and returns the top-of-stack object.</summary>
        ///
        /// <returns>The previous top-of-stack object.</returns>
		string Pop();

        /// <summary>Moves.</summary>
        ///
        /// <param name="value">The value.</param>
        /// <param name="toSet">Set to belongs to.</param>
		void Move(string value, IRedisSet toSet);

        /// <summary>Intersects the given with sets.</summary>
        ///
        /// <param name="withSets">Sets the with belongs to.</param>
        ///
        /// <returns>A HashSet&lt;string&gt;</returns>
		HashSet<string> Intersect(params IRedisSet[] withSets);

        /// <summary>Stores an intersect.</summary>
        ///
        /// <param name="withSets">Sets the with belongs to.</param>
		void StoreIntersect(params IRedisSet[] withSets);

        /// <summary>Unions the given with sets.</summary>
        ///
        /// <param name="withSets">Sets the with belongs to.</param>
        ///
        /// <returns>A HashSet&lt;string&gt;</returns>
		HashSet<string> Union(params IRedisSet[] withSets);

        /// <summary>Stores an union.</summary>
        ///
        /// <param name="withSets">Sets the with belongs to.</param>
		void StoreUnion(params IRedisSet[] withSets);

        /// <summary>Compares this IRedisSet[] object to another to determine their relative ordering.</summary>
        ///
        /// <param name="withSets">Sets the with belongs to.</param>
        ///
        /// <returns>A HashSet&lt;string&gt;</returns>
		HashSet<string> Diff(IRedisSet[] withSets);

        /// <summary>Stores a difference.</summary>
        ///
        /// <param name="fromSet"> Set from belongs to.</param>
        /// <param name="withSets">Sets the with belongs to.</param>
		void StoreDiff(IRedisSet fromSet, params IRedisSet[] withSets);

        /// <summary>Gets random entry.</summary>
        ///
        /// <returns>The random entry.</returns>
		string GetRandomEntry();
	}
}