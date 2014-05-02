using System.Collections.Generic;

namespace NServiceKit.CacheAccess
{
    /// <summary>Interface for cache clearable.</summary>
	public interface ICacheClearable
	{
        /// <summary>Clears this object to its blank/initial state.</summary>
        ///
        /// <param name="cacheKeys">A variable-length parameters list containing cache keys.</param>
		void Clear(IEnumerable<string> cacheKeys);

        /// <summary>Clears this object to its blank/initial state.</summary>
        ///
        /// <param name="cacheKeys">A variable-length parameters list containing cache keys.</param>
		void Clear(params string[] cacheKeys);
	}
}