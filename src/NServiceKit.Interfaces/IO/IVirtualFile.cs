using System.IO;

namespace NServiceKit.IO
{
    /// <summary>
    /// 
    /// </summary>
    public interface IVirtualFile : IVirtualNode
    {
        /// <summary>
        /// Gets the virtual path provider.
        /// </summary>
        /// <value>
        /// The virtual path provider.
        /// </value>
        IVirtualPathProvider VirtualPathProvider { get; }

        /// <summary>
        /// Gets the extension.
        /// </summary>
        /// <value>
        /// The extension.
        /// </value>
        string Extension { get; }

        /// <summary>
        /// Gets the file hash.
        /// </summary>
        /// <returns></returns>
        string GetFileHash();

        /// <summary>
        /// Opens the file for reading.
        /// </summary>
        /// <returns></returns>
        Stream OpenRead();

        /// <summary>
        /// Opens a StreamReader.
        /// </summary>
        /// <returns></returns>
        StreamReader OpenText();

        /// <summary>
        /// Reads all text.
        /// </summary>
        /// <returns></returns>
        string ReadAllText();
    }
}
