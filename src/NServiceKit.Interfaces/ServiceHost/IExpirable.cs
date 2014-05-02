using System;

namespace NServiceKit.WebHost.Endpoints.Support.Markdown
{
    /// <summary>Interface for expirable.</summary>
	public interface IExpirable
	{
        /// <summary>Gets the Date/Time of the last modified.</summary>
        ///
        /// <value>The last modified.</value>
		DateTime? LastModified { get; }
	}
}