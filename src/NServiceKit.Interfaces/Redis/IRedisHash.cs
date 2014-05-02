using System.Collections.Generic;
using NServiceKit.DesignPatterns.Model;

namespace NServiceKit.Redis
{
    /// <summary>Interface for redis hash.</summary>
	public interface IRedisHash
		: IDictionary<string, string>, IHasStringId
	{
        /// <summary>Adds if not exists.</summary>
        ///
        /// <param name="item">The item.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		bool AddIfNotExists(KeyValuePair<string, string> item);

        /// <summary>Adds a range.</summary>
        ///
        /// <param name="items">An IEnumerable&lt;KeyValuePair&lt;string,string&gt;&gt; of items to append to this.</param>
		void AddRange(IEnumerable<KeyValuePair<string, string>> items);

        /// <summary>Increment value.</summary>
        ///
        /// <param name="key">        The key.</param>
        /// <param name="incrementBy">Amount to increment by.</param>
        ///
        /// <returns>A long.</returns>
		long IncrementValue(string key, int incrementBy);
	}
}