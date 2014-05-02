using System.IO;

namespace NServiceKit.ServiceHost
{
    /// <summary>Interface for file.</summary>
	public interface IFile
	{
        /// <summary>Gets the filename of the file.</summary>
        ///
        /// <value>The name of the file.</value>
		string FileName { get; }

        /// <summary>Gets the length of the content.</summary>
        ///
        /// <value>The length of the content.</value>
		long ContentLength { get; }

        /// <summary>Gets the type of the content.</summary>
        ///
        /// <value>The type of the content.</value>
		string ContentType { get; }

        /// <summary>Gets the input stream.</summary>
        ///
        /// <value>The input stream.</value>
		Stream InputStream { get; }
	}
}