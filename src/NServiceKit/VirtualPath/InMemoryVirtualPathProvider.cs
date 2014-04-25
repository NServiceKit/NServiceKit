using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NServiceKit.Common;
using NServiceKit.IO;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints;

namespace NServiceKit.VirtualPath
{
    /// <summary>
    /// 
    /// </summary>
    public interface IWriteableVirtualPathProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="contents"></param>
        void AddFile(string filePath, string contents);
    }

    /// <summary>
    /// In Memory repository for files. Useful for testing.
    /// </summary>
    public class InMemoryVirtualPathProvider : AbstractVirtualPathProviderBase, IWriteableVirtualPathProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="appHost"></param>
        public InMemoryVirtualPathProvider(IAppHost appHost)
            : base(appHost)
        {
            this.rootDirectory = new InMemoryVirtualDirectory(this);
        }

        /// <summary>
        /// 
        /// </summary>
        public InMemoryVirtualDirectory rootDirectory;

        public override IVirtualDirectory RootDirectory
        {
            get { return rootDirectory; }
        }

        public override string VirtualPathSeparator
        {
            get { return "/"; }
        }

        public override string RealPathSeparator
        {
            get { return "/"; }
        }

        protected override void Initialize()
        {
        }

        public void AddFile(string filePath, string contents)
        {
            rootDirectory.AddFile(filePath, contents);
        }

        public override IVirtualFile GetFile(string virtualPath)
        {
            return rootDirectory.GetFile(virtualPath)
                ?? base.GetFile(virtualPath);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class InMemoryVirtualDirectory : AbstractVirtualDirectoryBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="owningProvider"></param>
        public InMemoryVirtualDirectory(IVirtualPathProvider owningProvider) 
            : base(owningProvider)
        {
            this.files = new List<InMemoryVirtualFile>();
            this.dirs = new List<InMemoryVirtualDirectory>();
            this.DirLastModified = DateTime.MinValue;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="owningProvider"></param>
        /// <param name="parentDirectory"></param>
        public InMemoryVirtualDirectory(IVirtualPathProvider owningProvider, IVirtualDirectory parentDirectory) 
            : base(owningProvider, parentDirectory) {}

        /// <summary>
        /// 
        /// </summary>
        public DateTime DirLastModified { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public override DateTime LastModified
        {
            get { return DirLastModified; }
        }

        /// <summary>
        /// 
        /// </summary>
        public List<InMemoryVirtualFile> files;

        /// <summary>
        /// 
        /// </summary>
        public override IEnumerable<IVirtualFile> Files
        {
            get { return files.Cast<IVirtualFile>(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public List<InMemoryVirtualDirectory> dirs;

        /// <summary>
        /// 
        /// </summary>
        public override IEnumerable<IVirtualDirectory> Directories
        {
            get { return dirs.Cast<IVirtualDirectory>(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string DirName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public override string Name
        {
            get { return DirName; }
        }

        /// <summary>
        /// Gets the file.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns></returns>
        public override IVirtualFile GetFile(string virtualPath)
        {
            virtualPath = StripBeginningDirectorySeparator(virtualPath);
            return files.FirstOrDefault(x => x.FilePath == virtualPath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IEnumerator<IVirtualNode> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the file from backing directory or default.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        protected override IVirtualFile GetFileFromBackingDirectoryOrDefault(string fileName)
        {
            return GetFile(fileName);
        }

        /// <summary>
        /// Gets the matching files in dir.
        /// </summary>
        /// <param name="globPattern">The glob pattern.</param>
        /// <returns></returns>
        protected override IEnumerable<IVirtualFile> GetMatchingFilesInDir(string globPattern)
        {
            var matchingFilesInBackingDir = EnumerateFiles(globPattern).Cast<IVirtualFile>();
            return matchingFilesInBackingDir;
        }

        /// <summary>
        /// Enumerates the files.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <returns></returns>
        public IEnumerable<InMemoryVirtualFile> EnumerateFiles(string pattern)
        {
            foreach (var file in files.Where(f => f.Name.Glob(pattern)))
            {
                yield return file;
            }
            foreach (var file in dirs.SelectMany(d => d.EnumerateFiles(pattern)))
            {
                yield return file;
            }
        }

        /// <summary>
        /// Gets the directory from backing directory or default.
        /// </summary>
        /// <param name="directoryName">Name of the directory.</param>
        /// <returns></returns>
        protected override IVirtualDirectory GetDirectoryFromBackingDirectoryOrDefault(string directoryName)
        {
            return null;
        }

        static readonly char[] DirSeps = new[] { '\\', '/' };

        /// <summary>
        /// Adds the file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="contents">The contents.</param>
        public void AddFile(string filePath, string contents)
        {
            filePath = StripBeginningDirectorySeparator(filePath);
            this.files.Add(new InMemoryVirtualFile(VirtualPathProvider, this) {
                FilePath = filePath,
                FileName = filePath.Split(DirSeps).Last(),
                TextContents = contents,
            });
        }

        /// <summary>
        /// Strips the beginning directory separator.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        private static string StripBeginningDirectorySeparator(string filePath)
        {
            if (String.IsNullOrEmpty(filePath))
                return filePath;

            if (DirSeps.Any(d => filePath[0] == d))
                 return filePath.Substring(1);

            return filePath;
        }
    }
    
    public class InMemoryVirtualFile : AbstractVirtualFileBase
    {
        public InMemoryVirtualFile(IVirtualPathProvider owningProvider, IVirtualDirectory directory) 
            : base(owningProvider, directory)
        {
            this.FileLastModified = DateTime.MinValue;            
        }

        public string FilePath { get; set; }

        public string FileName { get; set; }
        public override string Name
        {
            get { return FilePath; }
        }

        public DateTime FileLastModified { get; set; }
        public override DateTime LastModified
        {
            get { return FileLastModified; }
        }

        public string TextContents { get; set; }

        public byte[] ByteContents { get; set; }

        public override Stream OpenRead()
        {
            return new MemoryStream(ByteContents ?? (TextContents ?? "").ToUtf8Bytes());
        }
    }


}