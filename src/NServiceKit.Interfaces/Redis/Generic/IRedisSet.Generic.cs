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

namespace NServiceKit.Redis.Generic
{
    /// <summary>Interface for redis set.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
	public interface IRedisSet<T> : ICollection<T>, IHasStringId
	{
        /// <summary>Sorts.</summary>
        ///
        /// <param name="startingFrom">The starting from.</param>
        /// <param name="endingAt">    The ending at.</param>
        ///
        /// <returns>The sorted values.</returns>
		List<T> Sort(int startingFrom, int endingAt);

        /// <summary>Gets all.</summary>
        ///
        /// <returns>all.</returns>
		HashSet<T> GetAll();

        /// <summary>Pops the random item.</summary>
        ///
        /// <returns>A T.</returns>
		T PopRandomItem();

        /// <summary>Gets random item.</summary>
        ///
        /// <returns>The random item.</returns>
		T GetRandomItem();

        /// <summary>Move to.</summary>
        ///
        /// <param name="item"> The item.</param>
        /// <param name="toSet">Set to belongs to.</param>
		void MoveTo(T item, IRedisSet<T> toSet);

        /// <summary>Populate with intersect of the given sets.</summary>
        ///
        /// <param name="sets">A variable-length parameters list containing sets.</param>
		void PopulateWithIntersectOf(params IRedisSet<T>[] sets);

        /// <summary>Populate with union of the given sets.</summary>
        ///
        /// <param name="sets">A variable-length parameters list containing sets.</param>
		void PopulateWithUnionOf(params IRedisSet<T>[] sets);

        /// <summary>Gets the differences.</summary>
        ///
        /// <param name="withSets">Sets the with belongs to.</param>
		void GetDifferences(params IRedisSet<T>[] withSets);

        /// <summary>Populate with differences of.</summary>
        ///
        /// <param name="fromSet"> Set from belongs to.</param>
        /// <param name="withSets">Sets the with belongs to.</param>
		void PopulateWithDifferencesOf(IRedisSet<T> fromSet, params IRedisSet<T>[] withSets);
	}

}
