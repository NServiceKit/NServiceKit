using System;

namespace NServiceKit.DesignPatterns.Model
{
    /// <summary>Interface for has user session.</summary>
	public interface IHasUserSession
	{
        /// <summary>Gets the identifier of the user.</summary>
        ///
        /// <value>The identifier of the user.</value>
		Guid UserId { get; }

        /// <summary>Gets the identifier of the session.</summary>
        ///
        /// <value>The identifier of the session.</value>
		Guid SessionId { get; }
	}
}