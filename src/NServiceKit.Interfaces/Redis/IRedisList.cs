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

namespace NServiceKit.Redis
{
    /// <summary>Interface for redis list.</summary>
	public interface IRedisList
		: IList<string>, IHasStringId
	{
        /// <summary>Gets all.</summary>
        ///
        /// <returns>all.</returns>
		List<string> GetAll();

        /// <summary>Finds the range of the given arguments.</summary>
        ///
        /// <param name="startingFrom">The starting from.</param>
        /// <param name="endingAt">    The ending at.</param>
        ///
        /// <returns>The calculated range.</returns>
		List<string> GetRange(int startingFrom, int endingAt);

        /// <summary>Gets range from sorted list.</summary>
        ///
        /// <param name="startingFrom">The starting from.</param>
        /// <param name="endingAt">    The ending at.</param>
        ///
        /// <returns>The range from sorted list.</returns>
		List<string> GetRangeFromSortedList(int startingFrom, int endingAt);
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
		long RemoveValue(string value);

        /// <summary>Removes the value.</summary>
        ///
        /// <param name="value">      The value to push.</param>
        /// <param name="noOfMatches">The no of matches.</param>
        ///
        /// <returns>A long.</returns>
		long RemoveValue(string value, int noOfMatches);

        /// <summary>Prepends a value.</summary>
        ///
        /// <param name="value">The value to push.</param>
		void Prepend(string value);

        /// <summary>Appends a value.</summary>
        ///
        /// <param name="value">The value to push.</param>
		void Append(string value);

        /// <summary>Removes the start.</summary>
        ///
        /// <returns>A string.</returns>
		string RemoveStart();

        /// <summary>Blocking remove start.</summary>
        ///
        /// <param name="timeOut">The time out.</param>
        ///
        /// <returns>A string.</returns>
		string BlockingRemoveStart(TimeSpan? timeOut);

        /// <summary>Removes the end.</summary>
        ///
        /// <returns>A string.</returns>
		string RemoveEnd();

        /// <summary>Adds an object onto the end of this queue.</summary>
        ///
        /// <param name="value">The value to push.</param>
		void Enqueue(string value);

        /// <summary>Removes the head object from this queue.</summary>
        ///
        /// <returns>The head object from this queue.</returns>
		string Dequeue();

        /// <summary>Blocking dequeue.</summary>
        ///
        /// <param name="timeOut">The time out.</param>
        ///
        /// <returns>A string.</returns>
		string BlockingDequeue(TimeSpan? timeOut);

        /// <summary>Pushes an object onto this stack.</summary>
        ///
        /// <param name="value">The value to push.</param>
		void Push(string value);

        /// <summary>Removes and returns the top-of-stack object.</summary>
        ///
        /// <returns>The previous top-of-stack object.</returns>
		string Pop();

        /// <summary>Blocking pop.</summary>
        ///
        /// <param name="timeOut">The time out.</param>
        ///
        /// <returns>A string.</returns>
		string BlockingPop(TimeSpan? timeOut);

        /// <summary>Pops the and push described by toList.</summary>
        ///
        /// <param name="toList">List of toes.</param>
        ///
        /// <returns>A string.</returns>
		string PopAndPush(IRedisList toList);
	}
}