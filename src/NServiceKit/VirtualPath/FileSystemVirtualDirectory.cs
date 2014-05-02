using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NServiceKit.IO;

namespace NServiceKit.VirtualPath
{
    /// <summary>A file system virtual directory.</summary>
    public class FileSystemVirtualDirectory : AbstractVirtualDirectoryBase
    {
        /// <summary>Information describing the backing dir.</summary>
        protected DirectoryInfo BackingDirInfo;

        /// <summary>Gets the files.</summary>
        ///
        /// <value>The files.</value>
        public override IEnumerable<IVirtualFile> Files
        {
            get { return this.Where(n => n.IsDirectory == false).Cast<IVirtualFile>(); }
        }

        /// <summary>Gets the directories.</summary>
        ///
        /// <value>The directories.</value>
        public override IEnumerable<IVirtualDirectory> Directories
        {
            get { return this.Where(n => n.IsDirectory).Cast<IVirtualDirectory>(); }
        }

        /// <summary>Gets the name.</summary>
        ///
        /// <value>The name.</value>
        public override string Name
        {
            get { return BackingDirInfo.Name; }
        }

        /// <summary>Gets the last modified.</summary>
        ///
        /// <value>The last modified.</value>
        public override DateTime LastModified
        {
            get { return BackingDirInfo.LastWriteTime; }
        }

        /// <summary>Gets the real path.</summary>
        ///
        /// <value>The real path.</value>
        public override string RealPath
        {
            get { return BackingDirInfo.FullName; }
        }

        /// <summary>Initializes a new instance of the NServiceKit.VirtualPath.FileSystemVirtualDirectory class.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="owningProvider"> The owning provider.</param>
        /// <param name="parentDirectory">Pathname of the parent directory.</param>
        /// <param name="dInfo">          The information.</param>
        public FileSystemVirtualDirectory(IVirtualPathProvider owningProvider, IVirtualDirectory parentDirectory, DirectoryInfo dInfo)
            : base(owningProvider, parentDirectory)
        {
            if (dInfo == null)
                throw new ArgumentNullException("dInfo");

            this.BackingDirInfo = dInfo;
        }

        /// <summary>Gets the enumerator.</summary>
        ///
        /// <returns>The enumerator.</returns>
        public override IEnumerator<IVirtualNode> GetEnumerator()
        {
            var directoryNodes = BackingDirInfo.GetDirectories()
                .Select(dInfo => new FileSystemVirtualDirectory(VirtualPathProvider, this, dInfo));

            var fileNodes = BackingDirInfo.GetFiles()
                .Select(fInfo => new FileSystemVirtualFile(VirtualPathProvider, this, fInfo));

            return directoryNodes.Cast<IVirtualNode>()
                .Union<IVirtualNode>(fileNodes.Cast<IVirtualNode>())
                .GetEnumerator();
        }

        /// <summary>Gets the file from backing directory or default.</summary>
        ///
        /// <param name="fName">Name of the file.</param>
        ///
        /// <returns>The file from backing directory or default.</returns>
        protected override IVirtualFile GetFileFromBackingDirectoryOrDefault(string fName)
        {
            var fInfo = EnumerateFiles(fName).FirstOrDefault();

            return fInfo != null
                ? new FileSystemVirtualFile(VirtualPathProvider, this, fInfo)
                : null;
        }

        /// <summary>Gets the matching files in dir.</summary>
        ///
        /// <param name="globPattern">The glob pattern.</param>
        ///
        /// <returns>An enumerator that allows foreach to be used to process the matching files in dirs in this collection.</returns>
        protected override IEnumerable<IVirtualFile> GetMatchingFilesInDir(string globPattern)
        {
            var matchingFilesInBackingDir = EnumerateFiles(globPattern)
                .Select(fInfo => (IVirtualFile)new FileSystemVirtualFile(VirtualPathProvider, this, fInfo));
            
            return matchingFilesInBackingDir;
        }

        /// <summary>Gets the directory from backing directory or default.</summary>
        ///
        /// <param name="dName">Name of the directory.</param>
        ///
        /// <returns>The directory from backing directory or default.</returns>
        protected override IVirtualDirectory GetDirectoryFromBackingDirectoryOrDefault(string dName)
        {
            var dInfo = EnumerateDirectories(dName)
                .FirstOrDefault();

            return dInfo != null
                ? new FileSystemVirtualDirectory(VirtualPathProvider, this, dInfo)
                : null;
        }

        /// <summary>Enumerates the files in this collection.</summary>
        ///
        /// <param name="pattern">Specifies the pattern.</param>
        ///
        /// <returns>An enumerator that allows foreach to be used to process the files in this collection.</returns>
        public IEnumerable<FileInfo> EnumerateFiles(string pattern)
        {
            return BackingDirInfo.GetFiles(pattern, SearchOption.TopDirectoryOnly);
        }

        /// <summary>Enumerates the directories in this collection.</summary>
        ///
        /// <param name="dirName">Pathname of the directory.</param>
        ///
        /// <returns>An enumerator that allows foreach to be used to process the directories in this collection.</returns>
        public IEnumerable<DirectoryInfo> EnumerateDirectories(string dirName)
        {
            return BackingDirInfo.GetDirectories(dirName, SearchOption.TopDirectoryOnly);
        }
    }
}
