using System;
using System.Collections.Generic;

namespace NServiceKit.ServiceHost
{
    /// <summary>Interface for rest path.</summary>
	public interface IRestPath
	{
        /// <summary>Gets a value indicating whether this object is wild card path.</summary>
        ///
        /// <value>true if this object is wild card path, false if not.</value>
        bool IsWildCardPath { get; }

        /// <summary>Gets the type of the request.</summary>
        ///
        /// <value>The type of the request.</value>
		Type RequestType { get; }

        /// <summary>Creates a request.</summary>
        ///
        /// <param name="pathInfo">              Information describing the path.</param>
        /// <param name="queryStringAndFormData">Information describing the query string and form.</param>
        /// <param name="fromInstance">          from instance.</param>
        ///
        /// <returns>The new request.</returns>
		object CreateRequest(string pathInfo, Dictionary<string, string> queryStringAndFormData, object fromInstance);
	}
}