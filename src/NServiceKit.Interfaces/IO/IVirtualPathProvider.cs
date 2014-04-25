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
        /// 
        /// </summary>
		IVirtualDirectory RootDirectory { get; }

        /// <summary>
        /// 
        /// </summary>
        string VirtualPathSeparator { get; }

        /// <summary>
        /// 
        /// </summary>
        string RealPathSeparator { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="basePath"></param>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        string CombineVirtualPath(string basePath, string relativePath);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        bool FileExists(string virtualPath);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        bool DirectoryExists(string virtualPath);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        IVirtualFile GetFile(string virtualPath);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        string GetFileHash(string virtualPath);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="virtualFile"></param>
        /// <returns></returns>
        string GetFileHash(IVirtualFile virtualFile);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        IVirtualDirectory GetDirectory(string virtualPath);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="globPattern"></param>
        /// <param name="maxDepth"></param>
        /// <returns></returns>
        IEnumerable<IVirtualFile> GetAllMatchingFiles(string globPattern, int maxDepth = Int32.MaxValue);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="virtualFile"></param>
        /// <returns></returns>
        bool IsSharedFile(IVirtualFile virtualFile);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="virtualFile"></param>
        /// <returns></returns>
        bool IsViewFile(IVirtualFile virtualFile);
    }
}
