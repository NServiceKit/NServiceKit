using System;
using System.IO;
using NServiceKit.IO;

namespace NServiceKit.VirtualPath
{
    /// <summary>A file system virtual file.</summary>
    public class FileSystemVirtualFile : AbstractVirtualFileBase
    {
        /// <summary>The backing file.</summary>
        protected FileInfo BackingFile;

        /// <summary>Gets the name.</summary>
        ///
        /// <value>The name.</value>
        public override string Name
        {
            get { return BackingFile.Name; }
        }

        /// <summary>Gets the full pathname of the real file.</summary>
        ///
        /// <value>The full pathname of the real file.</value>
        public override string RealPath
        {
            get { return BackingFile.FullName; }
        }

        /// <summary>Gets the Date/Time of the last modified.</summary>
        ///
        /// <value>The last modified.</value>
        public override DateTime LastModified
        {
            get { return BackingFile.LastWriteTime; }
        }

        /// <summary>Initializes a new instance of the NServiceKit.VirtualPath.FileSystemVirtualFile class.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="owningProvider">The owning provider.</param>
        /// <param name="directory">     Pathname of the directory.</param>
        /// <param name="fInfo">         The information.</param>
        public FileSystemVirtualFile(IVirtualPathProvider owningProvider, IVirtualDirectory directory, FileInfo fInfo) 
            : base(owningProvider, directory)
        {
            if (fInfo == null)
                throw new ArgumentNullException("fInfo");

            this.BackingFile = fInfo;
        }

        /// <summary>Opens the file for reading.</summary>
        ///
        /// <returns>A Stream.</returns>
        public override Stream OpenRead()
        {
            return BackingFile.OpenRead();
        }
    }
}
