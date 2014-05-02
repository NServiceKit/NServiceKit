using System;

namespace NServiceKit.DesignPatterns.Model
{
    /// <summary>Interface for has user identifier.</summary>
	public interface IHasUserId
	{
        /// <summary>Gets the identifier of the user.</summary>
        ///
        /// <value>The identifier of the user.</value>
		Guid UserId { get; }
	}
}