using System;
using System.Collections.Generic;
using System.IO;
using NServiceKit.Common.Extensions;
using NServiceKit.Configuration;
using NServiceKit.Service;
using NServiceKit.ServiceHost;
using NServiceKit.Text;
#if NETFX_CORE
using System.Net.Http.Headers;
#endif

namespace NServiceKit.Common.Web
{
    /// <summary>Encapsulates the result of a compressed file.</summary>
    public class CompressedFileResult
        : IStreamWriter, IHasOptions
    {
        /// <summary>Length of the adler 32 checksum.</summary>
        public const int Adler32ChecksumLength = 4;

        /// <summary>The default content type.</summary>
        public const string DefaultContentType = MimeTypes.Xml;

        /// <summary>Gets the full pathname of the file.</summary>
        ///
        /// <value>The full pathname of the file.</value>
        public string FilePath { get; private set; }

        /// <summary>Gets the headers.</summary>
        ///
        /// <value>The headers.</value>
        public Dictionary<string, string> Headers { get; private set; }

        /// <summary>Gets options for controlling the operation.</summary>
        ///
        /// <value>The options.</value>
        public IDictionary<string, string> Options
        {
            get { return this.Headers; }
        }

        /// <summary>Initializes a new instance of the NServiceKit.Common.Web.CompressedFileResult class.</summary>
        ///
        /// <param name="filePath">Full pathname of the file.</param>
        public CompressedFileResult(string filePath)
            : this(filePath, CompressionTypes.Deflate) { }

        /// <summary>Initializes a new instance of the NServiceKit.Common.Web.CompressedFileResult class.</summary>
        ///
        /// <param name="filePath">       Full pathname of the file.</param>
        /// <param name="compressionType">Type of the compression.</param>
        public CompressedFileResult(string filePath, string compressionType)
            : this(filePath, compressionType, DefaultContentType) { }

        /// <summary>Initializes a new instance of the NServiceKit.Common.Web.CompressedFileResult class.</summary>
        ///
        /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or illegal values.</exception>
        ///
        /// <param name="filePath">       Full pathname of the file.</param>
        /// <param name="compressionType">Type of the compression.</param>
        /// <param name="contentMimeType">Type of the content mime.</param>
        public CompressedFileResult(string filePath, string compressionType, string contentMimeType)
        {
            if (!CompressionTypes.IsValid(compressionType))
            {
                throw new ArgumentException("Must be either 'deflate' or 'gzip'", compressionType);
            }

            this.FilePath = filePath;
            this.Headers = new Dictionary<string, string> {
                { HttpHeaders.ContentType, contentMimeType },
                { HttpHeaders.ContentEncoding, compressionType },
            };
        }

#if NETFX_CORE
        public async void WriteTo(Stream responseStream)
        {
            var file = await Windows.Storage.StorageFile.GetFileFromPathAsync(this.FilePath);
            using (var fs = await file.OpenStreamForWriteAsync())
            {
                fs.Position = Adler32ChecksumLength;

                fs.WriteTo(responseStream);
                responseStream.Flush();
            }
        }
#else

        /// <summary>Writes to.</summary>
        ///
        /// <param name="responseStream">The response stream.</param>
        public void WriteTo(Stream responseStream)
        {
            using (var fs = new FileStream(this.FilePath, FileMode.Open, FileAccess.Read))
            {
                fs.Position = Adler32ChecksumLength;

                fs.WriteTo(responseStream);
                responseStream.Flush();
            }
        }
#endif

    }
}