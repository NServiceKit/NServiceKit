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

        /// <summary>Gets the pathname of the root directory.</summary>
        ///
        /// <value>The pathname of the root directory.</value>
        public override IVirtualDirectory RootDirectory
        {
            get { return rootDirectory; }
        }

        /// <summary>Gets the virtual path separator.</summary>
        ///
        /// <value>The virtual path separator.</value>
        public override string VirtualPathSeparator
        {
            get { return "/"; }
        }

        /// <summary>Gets the real path separator.</summary>
        ///
        /// <value>The real path separator.</value>
        public override string RealPathSeparator
        {
            get { return "/"; }
        }

        /// <summary>Initializes this object.</summary>
        protected override void Initialize()
        {
        }

        /// <summary>Adds a file to 'contents'.</summary>
        ///
        /// <param name="filePath">.</param>
        /// <param name="contents">.</param>
        public void AddFile(string filePath, string contents)
        {
            rootDirectory.AddFile(filePath, contents);
        }

        /// <summary>Gets a file.</summary>
        ///
        /// <param name="virtualPath">.</param>
        ///
        /// <returns>The file.</returns>
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
            files = new List<InMemoryVirtualFile>();
            dirs = new List<InMemoryVirtualDirectory>();
            DirLastModified = DateTime.MinValue;
            FlattenFileEnumeration = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owningProvider"></param>
        /// <param name="parentDirectory"></param>
        public InMemoryVirtualDirectory(IVirtualPathProvider owningProvider, IVirtualDirectory parentDirectory)
            : base(owningProvider, parentDirectory)
        {
            files = new List<InMemoryVirtualFile>();
            dirs = new List<InMemoryVirtualDirectory>();
            DirLastModified = DateTime.MinValue;
            FlattenFileEnumeration = true;
        }

        /// <summary>
        /// Whether EnumerateFiles should flatten and expand all subdirectories or not.
        /// </summary>
        /// <value>
        /// When true, EnumerateFiles returns all files in all subdirectories (default). When false, only returns for this directory.
        /// </value>
        public bool FlattenFileEnumeration { get; set; }

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
            string filename = Path.GetFileName(virtualPath); 

            // Actually looks for files now. 
            foreach(IVirtualDirectory fileDir in this.Directory.Directories)
            {
                IVirtualFile returnFile = fileDir.Files.First(x => x.Name == filename); 
                return returnFile; 
            }

            return null; 
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

            if (FlattenFileEnumeration)
            {
                foreach (var file in dirs.SelectMany(d => d.EnumerateFiles(pattern)))
                {
                    yield return file;
                }
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
    
    /// <summary>An in memory virtual file.</summary>
    public class InMemoryVirtualFile : AbstractVirtualFileBase
    {
        /// <summary>Initializes a new instance of the NServiceKit.VirtualPath.InMemoryVirtualFile class.</summary>
        ///
        /// <param name="owningProvider">The owning provider.</param>
        /// <param name="directory">     Pathname of the directory.</param>
        public InMemoryVirtualFile(IVirtualPathProvider owningProvider, IVirtualDirectory directory) 
            : base(owningProvider, directory)
        {
            this.FileLastModified = DateTime.MinValue;            
        }

        /// <summary>Gets or sets the full pathname of the file.</summary>
        ///
        /// <value>The full pathname of the file.</value>
        public string FilePath { get; set; }

        /// <summary>Gets or sets the filename of the file.</summary>
        ///
        /// <value>The name of the file.</value>
        public string FileName { get; set; }

        /// <summary>Gets the name.</summary>
        ///
        /// <value>The name.</value>
        public override string Name
        {
            get { return FilePath; }
        }

        /// <summary>Gets or sets the Date/Time of the file last modified.</summary>
        ///
        /// <value>The file last modified.</value>
        public DateTime FileLastModified { get; set; }

        /// <summary>Gets the Date/Time of the last modified.</summary>
        ///
        /// <value>The last modified.</value>
        public override DateTime LastModified
        {
            get { return FileLastModified; }
        }

        /// <summary>Gets or sets the text contents.</summary>
        ///
        /// <value>The text contents.</value>
        public string TextContents { get; set; }

        /// <summary>Gets or sets the byte contents.</summary>
        ///
        /// <value>The byte contents.</value>
        public byte[] ByteContents { get; set; }

        /// <summary>Opens the file for reading.</summary>
        ///
        /// <returns>A Stream.</returns>
        public override Stream OpenRead()
        {
            return new MemoryStream(ByteContents ?? (TextContents ?? "").ToUtf8Bytes());
        }
    }


}