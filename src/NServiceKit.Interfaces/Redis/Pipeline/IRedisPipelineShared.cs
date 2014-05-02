using System;

namespace NServiceKit.Redis.Pipeline
{
	/// <summary>
	/// Pipeline interface shared by typed and non-typed pipelines
	/// </summary>
	public interface IRedisPipelineShared : IDisposable, IRedisQueueCompletableOperation
	{
        /// <summary>Flushes this object.</summary>
		void Flush();

        /// <summary>Replays this object.</summary>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		bool Replay();
	}
}