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
	public interface IRedisSet<T> : ICollection<T>, IHasStringId
	{
		List<T> Sort(int startingFrom, int endingAt);
		HashSet<T> GetAll();
		T PopRandomItem();
		T GetRandomItem();
		void MoveTo(T item, IRedisSet<T> toSet);
		void PopulateWithIntersectOf(params IRedisSet<T>[] sets);
		void PopulateWithUnionOf(params IRedisSet<T>[] sets);
		void GetDifferences(params IRedisSet<T>[] withSets);
		void PopulateWithDifferencesOf(IRedisSet<T> fromSet, params IRedisSet<T>[] withSets);
	}

}
