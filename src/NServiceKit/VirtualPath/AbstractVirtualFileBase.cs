using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using NServiceKit.Common;
using NServiceKit.IO;
using NServiceKit.Text;

namespace NServiceKit.VirtualPath
{
    /// <summary>An abstract virtual file base.</summary>
    public abstract class AbstractVirtualFileBase : IVirtualFile
    {
        /// <summary>Gets or sets the virtual path provider.</summary>
        ///
        /// <value>The virtual path provider.</value>
        public IVirtualPathProvider VirtualPathProvider { get; set; }

        /// <summary>Gets the extension.</summary>
        ///
        /// <value>The extension.</value>
        public string Extension
        {
            get { return Name.SplitOnLast('.').LastOrDefault(); }
        }

        /// <summary>Gets or sets the pathname of the directory.</summary>
        ///
        /// <value>The pathname of the directory.</value>
        public IVirtualDirectory Directory { get; set; }

        /// <summary>Gets the name.</summary>
        ///
        /// <value>The name.</value>
        public abstract string Name { get; }

        /// <summary>Gets the full pathname of the virtual file.</summary>
        ///
        /// <value>The full pathname of the virtual file.</value>
        public virtual string VirtualPath { get { return GetVirtualPathToRoot(); } }

        /// <summary>Gets the full pathname of the real file.</summary>
        ///
        /// <value>The full pathname of the real file.</value>
        public virtual string RealPath { get { return GetRealPathToRoot(); } }

        /// <summary>Gets a value indicating whether this object is directory.</summary>
        ///
        /// <value>true if this object is directory, false if not.</value>
        public virtual bool IsDirectory { get { return false; } }

        /// <summary>Gets the Date/Time of the last modified.</summary>
        ///
        /// <value>The last modified.</value>
        public abstract DateTime LastModified { get; }

        /// <summary>Initializes a new instance of the NServiceKit.VirtualPath.AbstractVirtualFileBase class.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="owningProvider">The owning provider.</param>
        /// <param name="directory">     Pathname of the directory.</param>
        protected AbstractVirtualFileBase(
            IVirtualPathProvider owningProvider, IVirtualDirectory directory)
        {
            if (owningProvider == null)
                throw new ArgumentNullException("owningProvider");

            if (directory == null)
                throw new ArgumentNullException("directory");

            this.VirtualPathProvider = owningProvider;
            this.Directory = directory;
        }

        /// <summary>Gets the file hash.</summary>
        ///
        /// <returns>The file hash.</returns>
        public virtual string GetFileHash()
        {
            using (var stream = OpenRead())
            {
                return stream.ToMd5Hash();
            }
        }

        /// <summary>Opens a StreamReader.</summary>
        ///
        /// <returns>A StreamReader.</returns>
        public virtual StreamReader OpenText()
        {
            return new StreamReader(OpenRead());
        }

        /// <summary>Reads all text.</summary>
        ///
        /// <returns>all text.</returns>
        public virtual string ReadAllText()
        {
            using (var reader = OpenText())
            {
                var text = reader.ReadToEnd();
				return text;
            }
        }

        /// <summary>Opens the file for reading.</summary>
        ///
        /// <returns>A Stream.</returns>
        public abstract Stream OpenRead();

        /// <summary>Gets virtual path to root.</summary>
        ///
        /// <returns>The virtual path to root.</returns>
        protected virtual String GetVirtualPathToRoot()
        {
            return GetPathToRoot(VirtualPathProvider.VirtualPathSeparator, p => p.VirtualPath);
        }

        /// <summary>Gets real path to root.</summary>
        ///
        /// <returns>The real path to root.</returns>
        protected virtual string GetRealPathToRoot()
        {
            return GetPathToRoot(VirtualPathProvider.RealPathSeparator, p => p.RealPath);
        }

        /// <summary>Gets path to root.</summary>
        ///
        /// <param name="separator">The separator.</param>
        /// <param name="pathSel">  The path selected.</param>
        ///
        /// <returns>The path to root.</returns>
        protected virtual string GetPathToRoot(string separator, Func<IVirtualDirectory, string> pathSel)
        {
            var parentPath = Directory != null ? pathSel(Directory) : string.Empty;
            if (parentPath == separator)
                parentPath = string.Empty;

            return string.Concat(parentPath, separator, Name);
        }

        /// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />.</summary>
        ///
        /// <param name="obj">The <see cref="T:System.Object" /> to compare with the current <see cref="T:System.Object" />.</param>
        ///
        /// <returns>true if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            var other = obj as AbstractVirtualFileBase;
            if (other == null)
                return false;

            return other.VirtualPath == this.VirtualPath;
        }

        /// <summary>Serves as a hash function for a particular type.</summary>
        ///
        /// <returns>A hash code for the current <see cref="T:System.Object" />.</returns>
        public override int GetHashCode()
        {
            return VirtualPath.GetHashCode();
        }

        /// <summary>Returns a string that represents the current object.</summary>
        ///
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return string.Format("{0} -> {1}", RealPath, VirtualPath);
        }
    }
}