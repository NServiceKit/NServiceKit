using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using NServiceKit.Service;
using NServiceKit.ServiceHost;

namespace NServiceKit.Common.Web
{
    /// <summary>Encapsulates the result of a compressed.</summary>
    public class CompressedResult
        : IStreamWriter, IHttpResult
    {
        /// <summary>Length of the adler 32 checksum.</summary>
        public const int Adler32ChecksumLength = 4;

        /// <summary>The default content type.</summary>
        public const string DefaultContentType = MimeTypes.Xml;

        /// <summary>Gets the contents.</summary>
        ///
        /// <value>The contents.</value>
        public byte[] Contents { get; private set; }

        /// <summary>The HTTP Response ContentType.</summary>
        ///
        /// <value>The type of the content.</value>
        public string ContentType { get; set; }

        /// <summary>Additional HTTP Headers.</summary>
        ///
        /// <value>The headers.</value>
        public Dictionary<string, string> Headers { get; private set; }

        /// <summary>The HTTP Response Status.</summary>
        ///
        /// <value>The status.</value>
        public int Status { get; set; }

        /// <summary>The HTTP Response Status Code.</summary>
        ///
        /// <value>The status code.</value>
        public HttpStatusCode StatusCode
        {
            get { return (HttpStatusCode)Status; }
            set { Status = (int)value; }
        }

        /// <summary>The HTTP Status Description.</summary>
        ///
        /// <value>Information describing the status.</value>
        public string StatusDescription { get; set; }

        /// <summary>Response DTO.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <value>The response.</value>
        public object Response
        {
            get { return this.Contents; }
            set { throw new NotImplementedException(); }
        }

        /// <summary>if not provided, get's injected by NServiceKit.</summary>
        ///
        /// <value>The response filter.</value>
        public IContentTypeWriter ResponseFilter { get; set; }

        /// <summary>Holds the request call context.</summary>
        ///
        /// <value>The request context.</value>
        public IRequestContext RequestContext { get; set; }

        /// <summary>Gets options for controlling the operation.</summary>
        ///
        /// <value>The options.</value>
        public IDictionary<string, string> Options
        {
            get { return this.Headers; }
        }

        /// <summary>Initializes a new instance of the NServiceKit.Common.Web.CompressedResult class.</summary>
        ///
        /// <param name="contents">The contents.</param>
        public CompressedResult(byte[] contents)
            : this(contents, CompressionTypes.Deflate) { }

        /// <summary>Initializes a new instance of the NServiceKit.Common.Web.CompressedResult class.</summary>
        ///
        /// <param name="contents">       The contents.</param>
        /// <param name="compressionType">Type of the compression.</param>
        public CompressedResult(byte[] contents, string compressionType)
            : this(contents, compressionType, DefaultContentType) { }

        /// <summary>Initializes a new instance of the NServiceKit.Common.Web.CompressedResult class.</summary>
        ///
        /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or illegal values.</exception>
        ///
        /// <param name="contents">       The contents.</param>
        /// <param name="compressionType">Type of the compression.</param>
        /// <param name="contentMimeType">Type of the content mime.</param>
        public CompressedResult(byte[] contents, string compressionType, string contentMimeType)
        {
            if (!CompressionTypes.IsValid(compressionType))
            {
                throw new ArgumentException("Must be either 'deflate' or 'gzip'", compressionType);
            }

            this.StatusCode = HttpStatusCode.OK;
            this.ContentType = contentMimeType;

            this.Contents = contents;
            this.Headers = new Dictionary<string, string> {
                { HttpHeaders.ContentEncoding, compressionType },
            };
        }

        /// <summary>Writes to.</summary>
        ///
        /// <param name="responseStream">The response stream.</param>
        public void WriteTo(Stream responseStream)
        {
            responseStream.Write(this.Contents, 0, this.Contents.Length);
            //stream.Write(this.Contents, Adler32ChecksumLength, this.Contents.Length - Adler32ChecksumLength);
        }

    }
}