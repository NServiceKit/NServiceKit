using System.Collections.Generic;

namespace NServiceKit.ServiceHost
{
    /// <summary>Interface for has options.</summary>
	public interface IHasOptions
	{
        /// <summary>Gets options for controlling the operation.</summary>
        ///
        /// <value>The options.</value>
		IDictionary<string, string> Options { get; }
	}
}