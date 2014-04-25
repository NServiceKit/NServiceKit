using System;
using System.Collections.Generic;

namespace NServiceKit.IO
{
    /// <summary>
    /// 
    /// </summary>
    public interface IVirtualPathProvider
    {
        /// <summary>
        /// Gets the root directory.
        /// </summary>
        /// <value>
        /// The root directory.
        /// </value>
		IVirtualDirectory RootDirectory { get; }

        /// <summary>
        /// Gets the virtual path separator.
        /// </summary>
        /// <value>
        /// The virtual path separator.
        /// </value>
        string VirtualPathSeparator { get; }

        /// <summary>
        /// Gets the real path separator.
        /// </summary>
        /// <value>
        /// The real path separator.
        /// </value>
        string RealPathSeparator { get; }

        /// <summary>
        /// Combines the virtual path.
        /// </summary>
        /// <param name="basePath">The base path.</param>
        /// <param name="relativePath">The relative path.</param>
        /// <returns></returns>
        string CombineVirtualPath(string basePath, string relativePath);

        /// <summary>
        /// Files the exists.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns></returns>
        bool FileExists(string virtualPath);

        /// <summary>
        /// Directories the exists.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns></returns>
        bool DirectoryExists(string virtualPath);

        /// <summary>
        /// Gets the file.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns></returns>
        IVirtualFile GetFile(string virtualPath);

        /// <summary>
        /// Gets the file hash.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns></returns>
        string GetFileHash(string virtualPath);

        /// <summary>
        /// Gets the file hash.
        /// </summary>
        /// <param name="virtualFile">The virtual file.</param>
        /// <returns></returns>
        string GetFileHash(IVirtualFile virtualFile);

        /// <summary>
        /// Gets the directory.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns></returns>
        IVirtualDirectory GetDirectory(string virtualPath);

        /// <summary>
        /// Gets all matching files.
        /// </summary>
        /// <param name="globPattern">The glob pattern.</param>
        /// <param name="maxDepth">The maximum depth.</param>
        /// <returns></returns>
        IEnumerable<IVirtualFile> GetAllMatchingFiles(string globPattern, int maxDepth = Int32.MaxValue);

        /// <summary>
        /// Determines whether [is shared file] [the specified virtual file].
        /// </summary>
        /// <param name="virtualFile">The virtual file.</param>
        /// <returns></returns>
        bool IsSharedFile(IVirtualFile virtualFile);

        /// <summary>
        /// Determines whether [is view file] [the specified virtual file].
        /// </summary>
        /// <param name="virtualFile">The virtual file.</param>
        /// <returns></returns>
        bool IsViewFile(IVirtualFile virtualFile);
    }
}
