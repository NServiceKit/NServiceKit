using System;
using System.Runtime.Serialization;

namespace NServiceKit.Service
{
    /// <summary>Interface for response base.</summary>
    ///
    /// <typeparam name="TData">          Type of the data.</typeparam>
    /// <typeparam name="TResponseStatus">Type of the response status.</typeparam>
	public interface IResponseBase<TData, TResponseStatus> 
	{
        /// <summary>Gets or sets the version.</summary>
        ///
        /// <value>The version.</value>
		int Version { get; set; }

        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
		TResponseStatus ResponseStatus { get; set; }

        /// <summary>Gets or sets information describing the response.</summary>
        ///
        /// <value>Information describing the response.</value>
		TData ResponseData { get; set; }
	}
}