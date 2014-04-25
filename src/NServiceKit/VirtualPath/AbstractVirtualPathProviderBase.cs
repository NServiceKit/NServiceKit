using System;
using System.Collections.Generic;
using NServiceKit.IO;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints;

namespace NServiceKit.VirtualPath
{
    /// <summary>
    /// Abstract base class for all <see cref="IVirtualPathProvider">VirtualPathProviders</see>.
    /// </summary>
    public abstract class AbstractVirtualPathProviderBase : IVirtualPathProvider
    {
        /// <summary>
        /// 
        /// </summary>
        public IAppHost AppHost { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public abstract IVirtualDirectory RootDirectory { get; }

        /// <summary>
        /// 
        /// </summary>
        public abstract string VirtualPathSeparator { get; }

        /// <summary>
        /// 
        /// </summary>
        public abstract string RealPathSeparator { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appHost"></param>
        protected AbstractVirtualPathProviderBase(IAppHost appHost)
        {
            if (appHost == null)
                throw new ArgumentNullException("appHost");

            AppHost = appHost;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="basePath"></param>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public virtual string CombineVirtualPath(string basePath, string relativePath)
        {
            return String.Concat(basePath, VirtualPathSeparator, relativePath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        public virtual bool FileExists(string virtualPath)
        {
            return GetFile(virtualPath) != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        public virtual bool DirectoryExists(string virtualPath)
        {
            return GetDirectory(virtualPath) != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        public virtual IVirtualFile GetFile(string virtualPath)
        {
            return RootDirectory.GetFile(virtualPath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        public virtual string GetFileHash(string virtualPath)
        {
            var f = GetFile(virtualPath);
            return GetFileHash(f);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="virtualFile"></param>
        /// <returns></returns>
        public virtual string GetFileHash(IVirtualFile virtualFile)
        {
            return virtualFile == null ? string.Empty : virtualFile.GetFileHash();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        public virtual IVirtualDirectory GetDirectory(string virtualPath)
        {
            return RootDirectory.GetDirectory(virtualPath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="globPattern"></param>
        /// <param name="maxDepth"></param>
        /// <returns></returns>
        public virtual IEnumerable<IVirtualFile> GetAllMatchingFiles(string globPattern, int maxDepth = Int32.MaxValue)
        {
            return RootDirectory.GetAllMatchingFiles(globPattern, maxDepth);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="virtualFile"></param>
        /// <returns></returns>
        public virtual bool IsSharedFile(IVirtualFile virtualFile)
        {
            return virtualFile.RealPath != null
                && virtualFile.RealPath.Contains("{0}{1}".Fmt(RealPathSeparator, "Shared"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="virtualFile"></param>
        /// <returns></returns>
        public virtual bool IsViewFile(IVirtualFile virtualFile)
        {
            return virtualFile.RealPath != null
                && virtualFile.RealPath.Contains("{0}{1}".Fmt(RealPathSeparator, "Views"));
        }

        /// <summary>
        /// 
        /// </summary>
        protected abstract void Initialize();
    }
}