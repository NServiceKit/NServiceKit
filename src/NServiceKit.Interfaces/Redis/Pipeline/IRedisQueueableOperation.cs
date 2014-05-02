using System;
using System.Collections.Generic;

namespace NServiceKit.Redis.Pipeline
{
	/// <summary>
	/// interface to operation that can queue commands
	/// </summary>
	public interface IRedisQueueableOperation
	{
        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">The command.</param>
		void QueueCommand(Action<IRedisClient> command);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">          The command.</param>
        /// <param name="onSuccessCallback">The on success callback.</param>
		void QueueCommand(Action<IRedisClient> command, Action onSuccessCallback);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">          The command.</param>
        /// <param name="onSuccessCallback">The on success callback.</param>
        /// <param name="onErrorCallback">  The on error callback.</param>
		void QueueCommand(Action<IRedisClient> command, Action onSuccessCallback, Action<Exception> onErrorCallback);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">The command.</param>
		void QueueCommand(Func<IRedisClient, int> command);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">          The command.</param>
        /// <param name="onSuccessCallback">The on success callback.</param>
		void QueueCommand(Func<IRedisClient, int> command, Action<int> onSuccessCallback);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">          The command.</param>
        /// <param name="onSuccessCallback">The on success callback.</param>
        /// <param name="onErrorCallback">  The on error callback.</param>
		void QueueCommand(Func<IRedisClient, int> command, Action<int> onSuccessCallback, Action<Exception> onErrorCallback);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">The command.</param>
		void QueueCommand(Func<IRedisClient, long> command);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">          The command.</param>
        /// <param name="onSuccessCallback">The on success callback.</param>
		void QueueCommand(Func<IRedisClient, long> command, Action<long> onSuccessCallback);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">          The command.</param>
        /// <param name="onSuccessCallback">The on success callback.</param>
        /// <param name="onErrorCallback">  The on error callback.</param>
		void QueueCommand(Func<IRedisClient, long> command, Action<long> onSuccessCallback, Action<Exception> onErrorCallback);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">The command.</param>
		void QueueCommand(Func<IRedisClient, bool> command);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">          The command.</param>
        /// <param name="onSuccessCallback">The on success callback.</param>
		void QueueCommand(Func<IRedisClient, bool> command, Action<bool> onSuccessCallback);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">          The command.</param>
        /// <param name="onSuccessCallback">The on success callback.</param>
        /// <param name="onErrorCallback">  The on error callback.</param>
		void QueueCommand(Func<IRedisClient, bool> command, Action<bool> onSuccessCallback, Action<Exception> onErrorCallback);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">The command.</param>
		void QueueCommand(Func<IRedisClient, double> command);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">          The command.</param>
        /// <param name="onSuccessCallback">The on success callback.</param>
		void QueueCommand(Func<IRedisClient, double> command, Action<double> onSuccessCallback);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">          The command.</param>
        /// <param name="onSuccessCallback">The on success callback.</param>
        /// <param name="onErrorCallback">  The on error callback.</param>
		void QueueCommand(Func<IRedisClient, double> command, Action<double> onSuccessCallback, Action<Exception> onErrorCallback);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">The command.</param>
		void QueueCommand(Func<IRedisClient, byte[]> command);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">          The command.</param>
        /// <param name="onSuccessCallback">The on success callback.</param>
		void QueueCommand(Func<IRedisClient, byte[]> command, Action<byte[]> onSuccessCallback);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">          The command.</param>
        /// <param name="onSuccessCallback">The on success callback.</param>
        /// <param name="onErrorCallback">  The on error callback.</param>
		void QueueCommand(Func<IRedisClient, byte[]> command, Action<byte[]> onSuccessCallback, Action<Exception> onErrorCallback);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">The command.</param>
        void QueueCommand(Func<IRedisClient, byte[][]> command);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">          The command.</param>
        /// <param name="onSuccessCallback">The on success callback.</param>
        void QueueCommand(Func<IRedisClient, byte[][]> command, Action<byte[][]> onSuccessCallback);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">          The command.</param>
        /// <param name="onSuccessCallback">The on success callback.</param>
        /// <param name="onErrorCallback">  The on error callback.</param>
        void QueueCommand(Func<IRedisClient, byte[][]> command, Action<byte[][]> onSuccessCallback, Action<Exception> onErrorCallback);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">The command.</param>
		void QueueCommand(Func<IRedisClient, string> command);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">          The command.</param>
        /// <param name="onSuccessCallback">The on success callback.</param>
		void QueueCommand(Func<IRedisClient, string> command, Action<string> onSuccessCallback);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">          The command.</param>
        /// <param name="onSuccessCallback">The on success callback.</param>
        /// <param name="onErrorCallback">  The on error callback.</param>
		void QueueCommand(Func<IRedisClient, string> command, Action<string> onSuccessCallback, Action<Exception> onErrorCallback);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">The command.</param>
		void QueueCommand(Func<IRedisClient, List<string>> command);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">          The command.</param>
        /// <param name="onSuccessCallback">The on success callback.</param>
		void QueueCommand(Func<IRedisClient, List<string>> command, Action<List<string>> onSuccessCallback);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">          The command.</param>
        /// <param name="onSuccessCallback">The on success callback.</param>
        /// <param name="onErrorCallback">  The on error callback.</param>
		void QueueCommand(Func<IRedisClient, List<string>> command, Action<List<string>> onSuccessCallback, Action<Exception> onErrorCallback);
		
	}
}