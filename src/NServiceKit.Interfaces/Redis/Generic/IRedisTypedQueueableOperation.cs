using System;
using System.Collections.Generic;

namespace NServiceKit.Redis.Generic
{
    /// <summary>
    /// interface to queueable operation using typed redis client
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRedisTypedQueueableOperation<T>
    {
        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">The command.</param>
        void QueueCommand(Action<IRedisTypedClient<T>> command);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">          The command.</param>
        /// <param name="onSuccessCallback">The on success callback.</param>
        void QueueCommand(Action<IRedisTypedClient<T>> command, Action onSuccessCallback);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">          The command.</param>
        /// <param name="onSuccessCallback">The on success callback.</param>
        /// <param name="onErrorCallback">  The on error callback.</param>
        void QueueCommand(Action<IRedisTypedClient<T>> command, Action onSuccessCallback, Action<Exception> onErrorCallback);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">The command.</param>
        void QueueCommand(Func<IRedisTypedClient<T>, int> command);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">          The command.</param>
        /// <param name="onSuccessCallback">The on success callback.</param>
        void QueueCommand(Func<IRedisTypedClient<T>, int> command, Action<int> onSuccessCallback);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">          The command.</param>
        /// <param name="onSuccessCallback">The on success callback.</param>
        /// <param name="onErrorCallback">  The on error callback.</param>
        void QueueCommand(Func<IRedisTypedClient<T>, int> command, Action<int> onSuccessCallback, Action<Exception> onErrorCallback);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">The command.</param>
        void QueueCommand(Func<IRedisTypedClient<T>, long> command);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">          The command.</param>
        /// <param name="onSuccessCallback">The on success callback.</param>
        void QueueCommand(Func<IRedisTypedClient<T>, long> command, Action<long> onSuccessCallback);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">          The command.</param>
        /// <param name="onSuccessCallback">The on success callback.</param>
        /// <param name="onErrorCallback">  The on error callback.</param>
        void QueueCommand(Func<IRedisTypedClient<T>, long> command, Action<long> onSuccessCallback, Action<Exception> onErrorCallback);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">The command.</param>
        void QueueCommand(Func<IRedisTypedClient<T>, bool> command);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">          The command.</param>
        /// <param name="onSuccessCallback">The on success callback.</param>
        void QueueCommand(Func<IRedisTypedClient<T>, bool> command, Action<bool> onSuccessCallback);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">          The command.</param>
        /// <param name="onSuccessCallback">The on success callback.</param>
        /// <param name="onErrorCallback">  The on error callback.</param>
        void QueueCommand(Func<IRedisTypedClient<T>, bool> command, Action<bool> onSuccessCallback, Action<Exception> onErrorCallback);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">The command.</param>
        void QueueCommand(Func<IRedisTypedClient<T>, double> command);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">          The command.</param>
        /// <param name="onSuccessCallback">The on success callback.</param>
        void QueueCommand(Func<IRedisTypedClient<T>, double> command, Action<double> onSuccessCallback);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">          The command.</param>
        /// <param name="onSuccessCallback">The on success callback.</param>
        /// <param name="onErrorCallback">  The on error callback.</param>
        void QueueCommand(Func<IRedisTypedClient<T>, double> command, Action<double> onSuccessCallback, Action<Exception> onErrorCallback);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">The command.</param>
        void QueueCommand(Func<IRedisTypedClient<T>, byte[]> command);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">          The command.</param>
        /// <param name="onSuccessCallback">The on success callback.</param>
        void QueueCommand(Func<IRedisTypedClient<T>, byte[]> command, Action<byte[]> onSuccessCallback);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">          The command.</param>
        /// <param name="onSuccessCallback">The on success callback.</param>
        /// <param name="onErrorCallback">  The on error callback.</param>
        void QueueCommand(Func<IRedisTypedClient<T>, byte[]> command, Action<byte[]> onSuccessCallback, Action<Exception> onErrorCallback);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">The command.</param>
        void QueueCommand(Func<IRedisTypedClient<T>, string> command);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">          The command.</param>
        /// <param name="onSuccessCallback">The on success callback.</param>
        void QueueCommand(Func<IRedisTypedClient<T>, string> command, Action<string> onSuccessCallback);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">          The command.</param>
        /// <param name="onSuccessCallback">The on success callback.</param>
        /// <param name="onErrorCallback">  The on error callback.</param>
        void QueueCommand(Func<IRedisTypedClient<T>, string> command, Action<string> onSuccessCallback, Action<Exception> onErrorCallback);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">The command.</param>
        void QueueCommand(Func<IRedisTypedClient<T>, T> command);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">          The command.</param>
        /// <param name="onSuccessCallback">The on success callback.</param>
        void QueueCommand(Func<IRedisTypedClient<T>, T> command, Action<T> onSuccessCallback);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">          The command.</param>
        /// <param name="onSuccessCallback">The on success callback.</param>
        /// <param name="onErrorCallback">  The on error callback.</param>
        void QueueCommand(Func<IRedisTypedClient<T>, T> command, Action<T> onSuccessCallback, Action<Exception> onErrorCallback);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">The command.</param>
        void QueueCommand(Func<IRedisTypedClient<T>, List<string>> command);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">          The command.</param>
        /// <param name="onSuccessCallback">The on success callback.</param>
        void QueueCommand(Func<IRedisTypedClient<T>, List<string>> command, Action<List<string>> onSuccessCallback);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">          The command.</param>
        /// <param name="onSuccessCallback">The on success callback.</param>
        /// <param name="onErrorCallback">  The on error callback.</param>
        void QueueCommand(Func<IRedisTypedClient<T>, List<string>> command, Action<List<string>> onSuccessCallback, Action<Exception> onErrorCallback);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">The command.</param>
        void QueueCommand(Func<IRedisTypedClient<T>, List<T>> command);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">          The command.</param>
        /// <param name="onSuccessCallback">The on success callback.</param>
        void QueueCommand(Func<IRedisTypedClient<T>, List<T>> command, Action<List<T>> onSuccessCallback);

        /// <summary>Queue command.</summary>
        ///
        /// <param name="command">          The command.</param>
        /// <param name="onSuccessCallback">The on success callback.</param>
        /// <param name="onErrorCallback">  The on error callback.</param>
        void QueueCommand(Func<IRedisTypedClient<T>, List<T>> command, Action<List<T>> onSuccessCallback, Action<Exception> onErrorCallback);
		
    }
}
