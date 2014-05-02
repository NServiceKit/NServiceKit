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
    /// <summary>Interface for redis hash.</summary>
    ///
    /// <typeparam name="TKey">  Type of the key.</typeparam>
    /// <typeparam name="TValue">Type of the value.</typeparam>
	public interface IRedisHash<TKey, TValue> : IDictionary<TKey, TValue>, IHasStringId
	{
        /// <summary>Gets all.</summary>
        ///
        /// <returns>all.</returns>
		Dictionary<TKey, TValue> GetAll();
	}

}
