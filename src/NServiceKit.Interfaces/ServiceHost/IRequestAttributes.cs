using System;

namespace NServiceKit.ServiceHost
{
    /// <summary>Interface for request attributes.</summary>
	public interface IRequestAttributes
	{
        /// <summary>Gets a value indicating whether the accepts gzip.</summary>
        ///
        /// <value>true if accepts gzip, false if not.</value>
		bool AcceptsGzip { get; }

        /// <summary>Gets a value indicating whether the accepts deflate.</summary>
        ///
        /// <value>true if accepts deflate, false if not.</value>
		bool AcceptsDeflate { get; }
	}
}