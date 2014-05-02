using System;
using System.Collections.Generic;

namespace NServiceKit.Redis.Pipeline
{
	/// <summary>
	/// Interface to operations that allow queued commands to be completed
	/// </summary>
	public interface IRedisQueueCompletableOperation
	{
        /// <summary>Complete void queued command.</summary>
        ///
        /// <param name="voidReadCommand">The void read command.</param>
		void CompleteVoidQueuedCommand(Action voidReadCommand);

        /// <summary>Complete int queued command.</summary>
        ///
        /// <param name="intReadCommand">The int read command.</param>
		void CompleteIntQueuedCommand(Func<int> intReadCommand);

        /// <summary>Complete long queued command.</summary>
        ///
        /// <param name="longReadCommand">The long read command.</param>
		void CompleteLongQueuedCommand(Func<long> longReadCommand);

        /// <summary>Complete bytes queued command.</summary>
        ///
        /// <param name="bytesReadCommand">The bytes read command.</param>
		void CompleteBytesQueuedCommand(Func<byte[]> bytesReadCommand);

        /// <summary>Complete multi bytes queued command.</summary>
        ///
        /// <param name="multiBytesReadCommand">The multi bytes read command.</param>
		void CompleteMultiBytesQueuedCommand(Func<byte[][]> multiBytesReadCommand);

        /// <summary>Complete string queued command.</summary>
        ///
        /// <param name="stringReadCommand">The string read command.</param>
		void CompleteStringQueuedCommand(Func<string> stringReadCommand);

        /// <summary>Complete multi string queued command.</summary>
        ///
        /// <param name="multiStringReadCommand">The multi string read command.</param>
		void CompleteMultiStringQueuedCommand(Func<List<string>> multiStringReadCommand);

        /// <summary>Complete double queued command.</summary>
        ///
        /// <param name="doubleReadCommand">The double read command.</param>
		void CompleteDoubleQueuedCommand(Func<double> doubleReadCommand);
	}
}