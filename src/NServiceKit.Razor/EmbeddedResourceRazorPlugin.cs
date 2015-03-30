using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NServiceKit.Common;
using NServiceKit.IO;
using NServiceKit.VirtualPath;
using NServiceKit.WebHost.Endpoints;

namespace NServiceKit.Razor
{
    /// <summary>
    /// Wraps the official RazorFormat plug-in with the ability to parse out files from embedded resources.
    /// All .cshtml files embedded in the provided assemblies will be exposed to razor.
    /// </summary>
    public class EmbeddedResourceRazorPlugin : IPlugin
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblies"></param>
        public EmbeddedResourceRazorPlugin(params Assembly[] assemblies)
        {
            _assemblies = new List<Assembly> { typeof(EmbeddedResourceRazorPlugin).Assembly };

            if (assemblies != null)
            {
                _assemblies.AddRange(assemblies);
            }
        }

        private List<Assembly> _assemblies;

        /// <summary>
        /// Add a list of assemblies to the list of assemblies to load into the razor engine.
        /// </summary>
        /// <param name="assemblies"></param>
        public void AddViewContainingAssemblies(params Assembly[] assemblies)
        {
            this._assemblies.AddRange(assemblies);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appHost"></param>
        public void Register(IAppHost appHost)
        {
            var inMemoryProvider = new RecursiveInMemoryVirtualPathProvider(appHost);
            // Create a "Views" directory for dumping all our discovered views in
            var viewsDir = new InMemoryVirtualDirectory(inMemoryProvider, inMemoryProvider.rootDirectory)
            {
                DirName = "Views",
                files = new List<InMemoryVirtualFile>(),
                dirs = new List<InMemoryVirtualDirectory>()
            };
            inMemoryProvider.rootDirectory.dirs.Add(viewsDir);

            foreach (var asm in _assemblies)
            {
                foreach (string resource in asm.GetManifestResourceNames())
                {
                        // Get just the file name. Internally, ServiceStack dumps all these in a giant "/Views" directory anyway, so let's just do that.
                        string path = string.Join(".", resource.Split('.').Reverse().Take(2).Reverse());
                        viewsDir.AddFile(path, new StreamReader(asm.GetManifestResourceStream(resource)).ReadToEnd());
                }
            }

            var razorPlugin = new RazorFormat
            {
                VirtualPathProvider = inMemoryProvider,
                EnableLiveReload = false
            };

            appHost.LoadPlugin(razorPlugin);
        }

        private class RecursiveInMemoryVirtualPathProvider : InMemoryVirtualPathProvider
        {
            public RecursiveInMemoryVirtualPathProvider(IAppHost appHost)
                : base(appHost)
            {
            }

            public override IEnumerable<IVirtualFile> GetAllMatchingFiles(string globPattern, int maxDepth = 2147483647)
            {
                var matches = new List<IVirtualFile>();
                ResolveDirectoryRecursive(rootDirectory, globPattern, matches);
                return matches;
            }

            private void ResolveDirectoryRecursive(IVirtualDirectory directory, string glob, List<IVirtualFile> matchedFiles)
            {
                matchedFiles.AddRange(directory.Files.Where(file => file.VirtualPath.Glob(glob)));

                foreach (var dir in directory.Directories)
                {
                    ResolveDirectoryRecursive(dir, glob, matchedFiles);
                }
            }
        }
    }
}
