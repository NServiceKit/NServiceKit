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

namespace NServiceKit.Redis
{
    /// <summary>Values that represent RedisKeyType.</summary>
	public enum RedisKeyType
	{
        /// <summary>An enum constant representing the none option.</summary>
		None, 

        /// <summary>An enum constant representing the string option.</summary>
		String, 

        /// <summary>An enum constant representing the list option.</summary>
		List, 

        /// <summary>An enum constant representing the set option.</summary>
		Set,

        /// <summary>An enum constant representing the sorted set option.</summary>
		SortedSet,

        /// <summary>An enum constant representing the hash option.</summary>
		Hash
	}
}