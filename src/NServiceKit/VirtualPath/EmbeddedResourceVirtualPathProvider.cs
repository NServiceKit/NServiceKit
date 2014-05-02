using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NServiceKit.IO;
using NServiceKit.WebHost.Endpoints;

namespace NServiceKit.VirtualPath
{
    /// <summary>
    /// <see cref="IVirtualPathProvider" /> implementation which looks at embedded resources.
    /// Resources will have the assembly name stripped off and subsequent namespace levels will be treated as directories.
    /// For example, if you embed A.B.C.D.txt in an assembly named A, it will be provided as: /B/C/D.txt
    /// When used with the RazorFormat plug-in, this lets you embed your views as resources within
    /// the assembly and distribute a self-contained DLL.
    /// </summary>
    /// <example>
    /// To use this with the RazorFormat plug-in, do something like the following:
    ///        this.LoadPlugin(new RazorFormat
    ///        {
    ///            WatchForModifiedPages = false,
    ///            VirtualPathProvider = new EmbeddedResourceVirtualPathProvider(this).IncludeAssemblies(typeof(Program).Assembly)
    ///        });
    /// </example>
    public class EmbeddedResourceVirtualPathProvider : AbstractVirtualPathProviderBase
    {
        private readonly List<Assembly> _assemblies;
        private InMemoryVirtualDirectory _root;
        private bool _initialized;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedResourceVirtualPathProvider"/> class.
        /// </summary>
        /// <param name="appHost"></param>
        public EmbeddedResourceVirtualPathProvider(IAppHost appHost) : base(appHost)
        {
            _root = new InMemoryVirtualDirectory(this){FlattenFileEnumeration = false};
            _assemblies = new List<Assembly>();
        }

        /// <summary>
        /// Adds to the list of assemblies that will be scanned for embedded resources.
        /// </summary>
        /// <param name="assemblies"></param>
        public EmbeddedResourceVirtualPathProvider IncludeAssemblies(params Assembly[] assemblies)
        {
            _assemblies.AddRange(assemblies);
            _initialized = false;
            return this;
        }

        /// <summary>
        /// If set, will run on all discovered resources. Returning true will cause the resource to be excluded.
        /// </summary>
        public Func<IVirtualFile, bool> FileExcluder { get; set; }

        /// <summary>Gets the pathname of the root directory.</summary>
        ///
        /// <value>The pathname of the root directory.</value>
        public override IVirtualDirectory RootDirectory
        {
            get
            {
                if (!_initialized)
                {
                    Initialize();
                    _initialized = true;
                }
                return _root;
            }
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

        /// <summary>
        /// </summary>
        protected override void Initialize()
        {
            PopulateFromEmbeddedResources();
        }

        /// <summary>
        /// Populates the root directory from embedded resources.
        /// </summary>
        public void PopulateFromEmbeddedResources()
        {
            _root = new InMemoryVirtualDirectory(this) { FlattenFileEnumeration = false };
            foreach (var assembly in _assemblies)
            {
                string baseName = assembly.GetName().Name + ".";

                foreach (var resource in assembly.GetManifestResourceNames())
                {
                    // Most embedded resources will start with the assembly name (e.g. Foo.txt in Bar.dll will be named Bar.Foo.txt by default)
                    // Strip that assembly name off if it's set
                    string relativeName = resource;
                    if (relativeName.StartsWith(baseName))
                    {
                        relativeName = relativeName.Remove(0, baseName.Length);
                    }

                    string fileName;
                    string[] directoryStructure;
                    // Figure out which portion of the path represents the file name, and what directory structure it's in (if any)
                    InferFileNameAndDirectoryPath(relativeName, out fileName, out directoryStructure);

                    // Loop over the directory structure to figure out which directory this file is supposed to go in
                    InMemoryVirtualDirectory destinationDirectory = _root;
                    foreach (var directory in directoryStructure)
                    {
                        var nextLevel = (InMemoryVirtualDirectory) destinationDirectory.GetDirectory(directory);
                        // If our expected directory doesn't exist, add it
                        if (nextLevel == null)
                        {
                            nextLevel = new InMemoryVirtualDirectory(this, destinationDirectory) {FlattenFileEnumeration = false, DirName = directory};
                            destinationDirectory.dirs.Add(nextLevel);
                        }
                        destinationDirectory = nextLevel;
                    }
                    var file = new StreamBasedVirtualFile(this, destinationDirectory, assembly.GetManifestResourceStream(resource), fileName, DateTime.Now);

                    // Give people the opportunity to exclude this file
                    if (FileExcluder != null && FileExcluder(file))
                    {
                        continue;
                    }

                    destinationDirectory.files.Add(file);
                }
            }
        }

        /// <summary>
        /// Takes in a relative path and figures out which portions represent the file name and any directories in between.
        /// e.g. A.B.C.txt would produce "C.txt" and ["A", "B"]
        /// </summary>
        /// <param name="relativeName"></param>
        /// <param name="fileName"></param>
        /// <param name="directoryPath"></param>
        internal static void InferFileNameAndDirectoryPath(string relativeName, out string fileName, out string[] directoryPath)
        {
            string[] pieces = relativeName.Split(new []{'.'}, StringSplitOptions.RemoveEmptyEntries);
            // If there are no dots, treat this as a file in the root
            if (pieces.Length == 1)
            {
                fileName = relativeName;
                directoryPath = new string[0];
                return;
            }

            int length = pieces.Length;
            fileName = pieces[length - 2] + "." + pieces[length - 1];

            // Only 2 parts means a file name, but no directories
            if (pieces.Length == 2)
            {
                directoryPath = new string[0];
                return;
            }

            // Copy any other pieces verbatim, except for the last two parts used to compose the file
            directoryPath = new string[length - 2];
            Array.Copy(pieces, directoryPath, directoryPath.Length);
        }

        /// <summary>
        /// Virtual file that wraps a stream. Doesn't get unwrapped until it's resolved.
        /// </summary>
        private class StreamBasedVirtualFile : InMemoryVirtualFile
        {
            private readonly Stream _contents;
            private readonly string _name;
            private readonly DateTime _lastModified;

            /// <summary>Initializes a new instance of the NServiceKit.VirtualPath.EmbeddedResourceVirtualPathProvider.StreamBasedVirtualFile class.</summary>
            ///
            /// <param name="owningProvider">The owning provider.</param>
            /// <param name="directory">     Pathname of the directory.</param>
            /// <param name="contents">      The contents.</param>
            /// <param name="name">          The name.</param>
            /// <param name="lastModified">  The last modified.</param>
            public StreamBasedVirtualFile(IVirtualPathProvider owningProvider, IVirtualDirectory directory, Stream contents, string name, DateTime lastModified) : base(owningProvider, directory)
            {
                _contents = contents;
                _name = name;
                _lastModified = lastModified;
                FilePath = name;
            }

            /// <summary>Gets the name.</summary>
            ///
            /// <value>The name.</value>
            public override string Name
            {
                get { return _name; }
            }

            /// <summary>Gets the Date/Time of the last modified.</summary>
            ///
            /// <value>The last modified.</value>
            public override DateTime LastModified
            {
                get { return _lastModified; }
            }

            /// <summary>Opens the file for reading.</summary>
            ///
            /// <returns>A Stream.</returns>
            public override Stream OpenRead()
            {
                return _contents;
            }
        }
    }
}
