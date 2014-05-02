using System;
using System.Collections.Generic;
using System.Net;
using NServiceKit.ServiceHost;

namespace NServiceKit.Common.Web
{
    /// <summary>A serialization context.</summary>
    public class SerializationContext : IRequestContext
    {
        /// <summary>Initializes a new instance of the NServiceKit.Common.Web.SerializationContext class.</summary>
        ///
        /// <param name="contentType">The type of the content.</param>
        public SerializationContext(string contentType)
        {
            this.ResponseContentType = this.ContentType = contentType;
        }

        /// <summary>Gets the get.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        ///
        /// <returns>A T.</returns>
        public T Get<T>() where T : class
        {
            return default(T);
        }

        /// <summary>Gets a header.</summary>
        ///
        /// <param name="headerName">Name of the header.</param>
        ///
        /// <returns>The header.</returns>
        public string GetHeader(string headerName)
        {
            return null;
        }

        /// <summary>Gets the IP address.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <value>The IP address.</value>
        public string IpAddress
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>Gets the cookies.</summary>
        ///
        /// <value>The cookies.</value>
        public IDictionary<string, System.Net.Cookie> Cookies
        {
            get { return new Dictionary<string, System.Net.Cookie>(); }
        }

        /// <summary>Gets the endpoint attributes.</summary>
        ///
        /// <value>The endpoint attributes.</value>
        public EndpointAttributes EndpointAttributes
        {
            get { return EndpointAttributes.None; }
        }

        /// <summary>Gets the request attributes.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <value>The request attributes.</value>
        public IRequestAttributes RequestAttributes
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>Gets or sets the type of the content.</summary>
        ///
        /// <value>The type of the content.</value>
        public string ContentType { get; set; }

        /// <summary>Gets or sets the type of the response content.</summary>
        ///
        /// <value>The type of the response content.</value>
        public string ResponseContentType { get; set; }

        /// <summary>Gets or sets the type of the compression.</summary>
        ///
        /// <value>The type of the compression.</value>
        public string CompressionType { get; set; }

        /// <summary>Gets URI of the absolute.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <value>The absolute URI.</value>
        public string AbsoluteUri
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>Gets information describing the path.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <value>Information describing the path.</value>
        public string PathInfo
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>Gets the files.</summary>
        ///
        /// <value>The files.</value>
        public IFile[] Files
        {
            get { return new IFile[0]; }
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
        }
    }
}