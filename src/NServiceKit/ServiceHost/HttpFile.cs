using System.IO;

namespace NServiceKit.ServiceHost
{
    /// <summary>A HTTP file.</summary>
	public class HttpFile : IFile
	{
        /// <summary>Gets or sets the filename of the file.</summary>
        ///
        /// <value>The name of the file.</value>
		public string FileName { get; set; }

        /// <summary>Gets or sets the length of the content.</summary>
        ///
        /// <value>The length of the content.</value>
		public long ContentLength { get; set; }

        /// <summary>Gets or sets the type of the content.</summary>
        ///
        /// <value>The type of the content.</value>
		public string ContentType { get; set; }

        /// <summary>Gets or sets the input stream.</summary>
        ///
        /// <value>The input stream.</value>
		public Stream InputStream { get; set; }
	}
}