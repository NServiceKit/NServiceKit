using System;

namespace NServiceKit.ServiceInterface.ServiceModel
{
    /// <summary>Interface for cache by date modified.</summary>
	public interface ICacheByDateModified
	{
        /// <summary>Gets the Date/Time of the last modified.</summary>
        ///
        /// <value>The last modified.</value>
		DateTime? LastModified { get; }
	}
}