using System;
using System.IO;
using NServiceKit.IO;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints;

namespace NServiceKit.VirtualPath
{
    /// <summary>A file system virtual path provider.</summary>
    public class FileSystemVirtualPathProvider : AbstractVirtualPathProviderBase
    {
        /// <summary>Information describing the root dir.</summary>
        protected DirectoryInfo RootDirInfo;
        /// <summary>The root dir.</summary>
        protected FileSystemVirtualDirectory RootDir;

        /// <summary>Gets the pathname of the root directory.</summary>
        ///
        /// <value>The pathname of the root directory.</value>
        public override IVirtualDirectory RootDirectory { get { return RootDir; } }

        /// <summary>Gets the virtual path separator.</summary>
        ///
        /// <value>The virtual path separator.</value>
        public override String VirtualPathSeparator { get { return "/"; } }

        /// <summary>Gets the real path separator.</summary>
        ///
        /// <value>The real path separator.</value>
        public override string RealPathSeparator { get { return Convert.ToString(Path.DirectorySeparatorChar); } }

        /// <summary>Initializes a new instance of the NServiceKit.VirtualPath.FileSystemVirtualPathProvider class.</summary>
        ///
        /// <param name="appHost">          The application host.</param>
        /// <param name="rootDirectoryPath">Pathname of the root directory.</param>
        public FileSystemVirtualPathProvider(IAppHost appHost, String rootDirectoryPath)
            : this(appHost, new DirectoryInfo(rootDirectoryPath))
        { }

        /// <summary>Initializes a new instance of the NServiceKit.VirtualPath.FileSystemVirtualPathProvider class.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="appHost">    The application host.</param>
        /// <param name="rootDirInfo">Information describing the root dir.</param>
        public FileSystemVirtualPathProvider(IAppHost appHost, DirectoryInfo rootDirInfo)
            : base(appHost)
        {
            if (rootDirInfo == null)
                throw new ArgumentNullException("rootDirInfo");

            this.RootDirInfo = rootDirInfo;
            Initialize();
        }

        /// <summary>Initializes a new instance of the NServiceKit.VirtualPath.FileSystemVirtualPathProvider class.</summary>
        ///
        /// <param name="appHost">The application host.</param>
        public FileSystemVirtualPathProvider(IAppHost appHost)
            : base(appHost)
        {
            Initialize();
        }

        /// <summary>Initializes this object.</summary>
        ///
        /// <exception cref="ApplicationException">Thrown when an Application error condition occurs.</exception>
        protected override sealed void Initialize()
        {
            if (RootDirInfo == null)
                RootDirInfo = new DirectoryInfo(AppHost.Config.WebHostPhysicalPath);

            if (RootDirInfo == null || ! RootDirInfo.Exists)
                throw new ApplicationException(
                    "RootDir '{0}' for virtual path does not exist".Fmt(RootDirInfo.FullName));

            RootDir = new FileSystemVirtualDirectory(this, null, RootDirInfo);
        }
       
    }
}
