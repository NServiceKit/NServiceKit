using System;
using System.Collections.Generic;

namespace NServiceKit.IO
{
    /// <summary>
    /// 
    /// </summary>
    public interface IVirtualDirectory : IVirtualNode, IEnumerable<IVirtualNode>
    {
        /// <summary>
        /// Gets a value indicating whether this instance is root.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is root; otherwise, <c>false</c>.
        /// </value>
        bool IsRoot { get; }

        /// <summary>
        /// Gets the parent directory.
        /// </summary>
        /// <value>
        /// The parent directory.
        /// </value>
        IVirtualDirectory ParentDirectory { get; }

        /// <summary>
        /// Gets the files.
        /// </summary>
        /// <value>
        /// The files.
        /// </value>
        IEnumerable<IVirtualFile> Files { get; }

        /// <summary>
        /// Gets the directories.
        /// </summary>
        /// <value>
        /// The directories.
        /// </value>
        IEnumerable<IVirtualDirectory> Directories { get; }

        /// <summary>
        /// Gets the file.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns></returns>
        IVirtualFile GetFile(string virtualPath);
        /// <summary>
        /// Gets the file.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns></returns>
        IVirtualFile GetFile(Stack<string> virtualPath);

        /// <summary>
        /// Gets the directory.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns></returns>
        IVirtualDirectory GetDirectory(string virtualPath);
        /// <summary>
        /// Gets the directory.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns></returns>
        IVirtualDirectory GetDirectory(Stack<string> virtualPath);

        /// <summary>
        /// Gets all matching files.
        /// </summary>
        /// <param name="globPattern">The glob pattern.</param>
        /// <param name="maxDepth">The maximum depth.</param>
        /// <returns></returns>
        IEnumerable<IVirtualFile> GetAllMatchingFiles(string globPattern, int maxDepth = Int32.MaxValue);
    }
}
