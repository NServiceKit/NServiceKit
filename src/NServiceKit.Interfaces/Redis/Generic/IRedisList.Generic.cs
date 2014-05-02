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

using System;
using System.Collections.Generic;
using NServiceKit.DesignPatterns.Model;

namespace NServiceKit.Redis.Generic
{
	/// <summary>
	/// Wrap the common redis list operations under a IList[string] interface.
	/// </summary>

	public interface IRedisList<T> 
		: IList<T>, IHasStringId
	{
        /// <summary>Gets all.</summary>
        ///
        /// <returns>all.</returns>
		List<T> GetAll();

        /// <summary>Finds the range of the given arguments.</summary>
        ///
        /// <param name="startingFrom">The starting from.</param>
        /// <param name="endingAt">    The ending at.</param>
        ///
        /// <returns>The calculated range.</returns>
		List<T> GetRange(int startingFrom, int endingAt);

        /// <summary>Gets range from sorted list.</summary>
        ///
        /// <param name="startingFrom">The starting from.</param>
        /// <param name="endingAt">    The ending at.</param>
        ///
        /// <returns>The range from sorted list.</returns>
		List<T> GetRangeFromSortedList(int startingFrom, int endingAt);
        /// <summary>Removes all.</summary>
		void RemoveAll();

        /// <summary>Trims.</summary>
        ///
        /// <param name="keepStartingFrom">The keep starting from.</param>
        /// <param name="keepEndingAt">    The keep ending at.</param>
		void Trim(int keepStartingFrom, int keepEndingAt);

        /// <summary>Removes the value.</summary>
        ///
        /// <param name="value">The value to push.</param>
        ///
        /// <returns>A long.</returns>
		long RemoveValue(T value);

        /// <summary>Removes the value.</summary>
        ///
        /// <param name="value">      The value to push.</param>
        /// <param name="noOfMatches">The no of matches.</param>
        ///
        /// <returns>A long.</returns>
		long RemoveValue(T value, int noOfMatches);

        /// <summary>Adds a range.</summary>
        ///
        /// <param name="values">An IEnumerable&lt;T&gt; of items to append to this.</param>
		void AddRange(IEnumerable<T> values);

        /// <summary>Appends a value.</summary>
        ///
        /// <param name="value">The value to push.</param>
		void Append(T value);

        /// <summary>Prepends a value.</summary>
        ///
        /// <param name="value">The value to push.</param>
		void Prepend(T value);

        /// <summary>Removes the start.</summary>
        ///
        /// <returns>A T.</returns>
		T RemoveStart();

        /// <summary>Blocking remove start.</summary>
        ///
        /// <param name="timeOut">The time out.</param>
        ///
        /// <returns>A T.</returns>
		T BlockingRemoveStart(TimeSpan? timeOut);

        /// <summary>Removes the end.</summary>
        ///
        /// <returns>A T.</returns>
		T RemoveEnd();

        /// <summary>Adds an object onto the end of this queue.</summary>
        ///
        /// <param name="value">The value to push.</param>
		void Enqueue(T value);

        /// <summary>Removes the head object from this queue.</summary>
        ///
        /// <returns>The head object from this queue.</returns>
		T Dequeue();

        /// <summary>Blocking dequeue.</summary>
        ///
        /// <param name="timeOut">The time out.</param>
        ///
        /// <returns>A T.</returns>
		T BlockingDequeue(TimeSpan? timeOut);

        /// <summary>Pushes an object onto this stack.</summary>
        ///
        /// <param name="value">The value to push.</param>
		void Push(T value);

        /// <summary>Removes and returns the top-of-stack object.</summary>
        ///
        /// <returns>The previous top-of-stack object.</returns>
		T Pop();

        /// <summary>Blocking pop.</summary>
        ///
        /// <param name="timeOut">The time out.</param>
        ///
        /// <returns>A T.</returns>
		T BlockingPop(TimeSpan? timeOut);

        /// <summary>Pops the and push described by toList.</summary>
        ///
        /// <param name="toList">List of toes.</param>
        ///
        /// <returns>A T.</returns>
		T PopAndPush(IRedisList<T> toList);
	}
}