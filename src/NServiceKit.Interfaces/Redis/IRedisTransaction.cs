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
using NServiceKit.Redis.Pipeline;

namespace NServiceKit.Redis
{
    /// <summary>
    /// Interface to redis transaction
    /// </summary>
	public interface IRedisTransaction
        : IRedisTransactionBase, IRedisQueueableOperation, IDisposable
	{
        /// <summary>Commits this object.</summary>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		bool Commit();
        /// <summary>Rollbacks this object.</summary>
		void Rollback();
	}
}