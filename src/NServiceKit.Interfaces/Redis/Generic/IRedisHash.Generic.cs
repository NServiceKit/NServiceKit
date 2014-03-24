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
	public interface IRedisHash<TKey, TValue> : IDictionary<TKey, TValue>, IHasStringId
	{
		Dictionary<TKey, TValue> GetAll();
	}

}
