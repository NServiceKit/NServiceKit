using System;
using System.Collections.Generic;
using System.Net;

namespace NServiceKit.ServiceHost
{
    /// <summary>Interface for request context.</summary>
	public interface IRequestContext : IDisposable
	{
        /// <summary>Gets the get.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        ///
        /// <returns>A T.</returns>
		T Get<T>() where T : class;

        /// <summary>Gets the IP address.</summary>
        ///
        /// <value>The IP address.</value>
		string IpAddress { get; }

        /// <summary>Gets a header.</summary>
        ///
        /// <param name="headerName">Name of the header.</param>
        ///
        /// <returns>The header.</returns>
		string GetHeader(string headerName);

        /// <summary>Gets the cookies.</summary>
        ///
        /// <value>The cookies.</value>
		IDictionary<string, Cookie> Cookies { get; }

        /// <summary>Gets the endpoint attributes.</summary>
        ///
        /// <value>The endpoint attributes.</value>
		EndpointAttributes EndpointAttributes { get; }

        /// <summary>Gets the request attributes.</summary>
        ///
        /// <value>The request attributes.</value>
		IRequestAttributes RequestAttributes { get; }

        /// <summary>Gets the type of the content.</summary>
        ///
        /// <value>The type of the content.</value>
		string ContentType { get; }

        /// <summary>Gets the type of the response content.</summary>
        ///
        /// <value>The type of the response content.</value>
		string ResponseContentType { get; }

        /// <summary>Gets the type of the compression.</summary>
        ///
        /// <value>The type of the compression.</value>
		string CompressionType { get; }

        /// <summary>Gets URI of the absolute.</summary>
        ///
        /// <value>The absolute URI.</value>
		string AbsoluteUri { get; }

        /// <summary>Gets information describing the path.</summary>
        ///
        /// <value>Information describing the path.</value>
		string PathInfo { get; }

        /// <summary>Gets the files.</summary>
        ///
        /// <value>The files.</value>
		IFile[] Files { get; }
	}
}