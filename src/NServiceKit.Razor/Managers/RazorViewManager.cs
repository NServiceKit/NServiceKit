using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using Microsoft.CSharp;
using NServiceKit.Common.Extensions;
using NServiceKit.Common.Utils;
using NServiceKit.IO;
using NServiceKit.Logging;
using NServiceKit.Razor.Compilation;
using NServiceKit.Razor.Managers.RazorGen;
using NServiceKit.ServiceHost;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints.Extensions;

namespace NServiceKit.Razor.Managers
{
    /// <summary>
    /// This view manager is responsible for keeping track of all the 
    /// available Razor views and states of Razor pages.
    /// </summary>
    public class RazorViewManager
    {
        /// <summary>The log.</summary>
        public static ILog Log = LogManager.GetLogger(typeof(RazorViewManager));

        /// <summary>The pages.</summary>
        public Dictionary<string, RazorPage> Pages = new Dictionary<string, RazorPage>(StringComparer.InvariantCultureIgnoreCase);
        /// <summary>The view names map.</summary>
        protected Dictionary<string, string> ViewNamesMap = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>Gets or sets the configuration.</summary>
        ///
        /// <value>The configuration.</value>
        protected IRazorConfig Config { get; set; }

        /// <summary>The path provider.</summary>
        protected IVirtualPathProvider PathProvider = null;

        /// <summary>Initializes a new instance of the NServiceKit.Razor.Managers.RazorViewManager class.</summary>
        ///
        /// <param name="viewConfig">         The view configuration.</param>
        /// <param name="virtualPathProvider">The virtual path provider.</param>
        public RazorViewManager(IRazorConfig viewConfig, IVirtualPathProvider virtualPathProvider)
        {
            this.Config = viewConfig;
            this.PathProvider = virtualPathProvider;
        }

        /// <summary>Initialises this object.</summary>
        public void Init()
        {
            if (Config.WaitForPrecompilationOnStartup)
                startupPrecompilationTasks = new List<Task>();

            ScanForRazorPages();

            if (Config.WaitForPrecompilationOnStartup)
            {
                Task.WaitAll(startupPrecompilationTasks.ToArray());
                startupPrecompilationTasks = null;
            }
        }

        private void ScanForRazorPages()
        {
            var pattern = Path.ChangeExtension("*", this.Config.RazorFileExtension);

            var files = this.PathProvider.GetAllMatchingFiles(pattern)
                            .Where(IsWatchedFile);

            // you can override IsWatchedFile to filter
            files.ForEach(x => TrackPage(x));
        }

        /// <summary>Adds a page.</summary>
        ///
        /// <param name="filePath">Full pathname of the file.</param>
        ///
        /// <returns>A RazorPage.</returns>
        public virtual RazorPage AddPage(string filePath)
        {
            var newFile = GetVirutalFile(filePath);
            return AddPage(newFile);
        }

        /// <summary>Invalidate page.</summary>
        ///
        /// <param name="page">The page.</param>
        public virtual void InvalidatePage(RazorPage page)
        {
            if (page.IsValid || page.IsCompiling)
            {
                lock (page.SyncRoot)
                {
                    page.IsValid = false;
                }
            }

            if (Config.PrecompilePages)
                PrecompilePage(page);
        }

        /// <summary>Adds a page.</summary>
        ///
        /// <param name="file">The file.</param>
        ///
        /// <returns>A RazorPage.</returns>
        public virtual RazorPage AddPage(IVirtualFile file)
        {
            return IsWatchedFile(file) 
                ? TrackPage(file)
                : null;
        }

        /// <summary>Track page.</summary>
        ///
        /// <param name="file">The file.</param>
        ///
        /// <returns>A RazorPage.</returns>
        public virtual RazorPage TrackPage(IVirtualFile file)
        {
            //get the base type.
            var pageBaseType = this.Config.PageBaseType;

            var transformer = new RazorViewPageTransformer(pageBaseType);

            //create a RazorPage
            var page = new RazorPage
            {
                PageHost = new RazorPageHost(PathProvider, file, transformer, new CSharpCodeProvider(), new Dictionary<string, string>()),
                IsValid = false,
                File = file
            };

            //add it to our pages dictionary.
            AddPage(page);

            if (Config.PrecompilePages)
                PrecompilePage(page);
            
            return page;
        }

        /// <summary>Adds a page.</summary>
        ///
        /// <param name="page">The page.</param>
        ///
        /// <returns>A RazorPage.</returns>
        protected virtual RazorPage AddPage(RazorPage page)
        {
            var pagePath = GetDictionaryPagePath(page.PageHost.File);

            this.Pages[pagePath] = page;

            //Views should be uniquely named and stored in any deep folder structure
            if (pagePath.StartsWithIgnoreCase("/views/"))
            {
                var viewName = pagePath.SplitOnLast('.').First().SplitOnLast('/').Last();
                ViewNamesMap[viewName] = pagePath;
            }

            return page;
        }

        /// <summary>Gets a page.</summary>
        ///
        /// <param name="absolutePath">Full pathname of the absolute file.</param>
        ///
        /// <returns>The page.</returns>
        public virtual RazorPage GetPage(string absolutePath)
        {
            RazorPage page;
            this.Pages.TryGetValue(absolutePath, out page);
            return page;
        }

        /// <summary>Gets page by path information.</summary>
        ///
        /// <param name="pathInfo">Information describing the path.</param>
        ///
        /// <returns>The page by path information.</returns>
        public virtual RazorPage GetPageByPathInfo(string pathInfo)
        {
            RazorPage page;
            if (this.Pages.TryGetValue(pathInfo, out page))
                return page;

            if (this.Pages.TryGetValue(Path.ChangeExtension(pathInfo, Config.RazorFileExtension), out page))
                return page;
            
            if (this.Pages.TryGetValue(CombinePaths(pathInfo, Config.DefaultPageName), out page))
                return page;

            return null;
        }

        /// <summary>Gets a page.</summary>
        ///
        /// <param name="request">The request.</param>
        /// <param name="dto">    The dto.</param>
        ///
        /// <returns>The page.</returns>
        public virtual RazorPage GetPage(IHttpRequest request, object dto)
        {
            var normalizePath = NormalizePath(request, dto);
            return GetPage(normalizePath);
        }

        /// <summary>Gets page by name.</summary>
        ///
        /// <param name="pageName">Name of the page.</param>
        ///
        /// <returns>The page by name.</returns>
        public virtual RazorPage GetPageByName(string pageName)
        {
            return GetPageByName(pageName, null, null);
        }

        private static string CombinePaths(params string[] paths)
        {
            var combinedPath = PathUtils.CombinePaths(paths);
            if (!combinedPath.StartsWith("/"))
                combinedPath = "/" + combinedPath;
            return combinedPath;
        }

        /// <summary>Gets page by name.</summary>
        ///
        /// <param name="pageName">Name of the page.</param>
        /// <param name="request"> The request.</param>
        /// <param name="dto">     The dto.</param>
        ///
        /// <returns>The page by name.</returns>
        public virtual RazorPage GetPageByName(string pageName, IHttpRequest request, object dto)
        {
            RazorPage page = null;
            var htmlPageName = Path.ChangeExtension(pageName, Config.RazorFileExtension);

            if (request != null)
            {
                var contextRelativePath = NormalizePath(request, dto) ?? "/views/";

                string contextParentDir = contextRelativePath;
                do
                {
                    contextParentDir = (contextParentDir ?? "").SplitOnLast('/').First();

                    var relativePath = CombinePaths(contextParentDir, htmlPageName);
                    if (this.Pages.TryGetValue(relativePath, out page))
                        return page;

                } while (!string.IsNullOrEmpty(contextParentDir));
            }

            //var sharedPath = "/view/shared/{0}".Fmt(htmlPageName);
            //if (this.Pages.TryGetValue(sharedPath, out page))
            //    return page;

            string viewPath;
            if (ViewNamesMap.TryGetValue(pageName, out viewPath))
                this.Pages.TryGetValue(viewPath, out page);

            return page;
        }

        static char[] InvalidFileChars = new[]{'<','>','`'}; //Anonymous or Generic type names
        private string NormalizePath(IHttpRequest request, object dto)
        {
            if (dto != null && !(dto is DynamicRequestObject)) // this is for a view inside /views
            {
                //if we have a view name, use it.
                var viewName = request.GetView();

                if (string.IsNullOrWhiteSpace(viewName))
                {
                    //use the response DTO name
                    viewName = dto.GetType().Name;
                }
                if (string.IsNullOrWhiteSpace(viewName))
                {
                    //the request use the request DTO name.
                    viewName = request.OperationName;
                }

                var isInvalidName = viewName.IndexOfAny(InvalidFileChars) >= 0;
                if (!isInvalidName)
                {
                    return CombinePaths("views", Path.ChangeExtension(viewName, Config.RazorFileExtension));
                }
            }

            // path/to/dir/default.cshtml
            var path = request.PathInfo;
            var defaultIndex = CombinePaths(path, Config.DefaultPageName);
            if (Pages.ContainsKey(defaultIndex))
                return defaultIndex;

            return Path.ChangeExtension(path, Config.RazorFileExtension);
        }

        /// <summary>Query if 'file' is watched file.</summary>
        ///
        /// <param name="file">The file.</param>
        ///
        /// <returns>true if watched file, false if not.</returns>
        public virtual bool IsWatchedFile(IVirtualFile file)
        {
            return this.Config.RazorFileExtension.EndsWithIgnoreCase(file.Extension);
        }

        /// <summary>Gets dictionary page path.</summary>
        ///
        /// <param name="relativePath">Full pathname of the relative file.</param>
        ///
        /// <returns>The dictionary page path.</returns>
        public virtual string GetDictionaryPagePath(string relativePath)
        {
            if (relativePath.ToLowerInvariant().StartsWith("/views/"))
            {
                //re-write the /views path
                //so we can uniquely get views by
                //ResponseDTO/RequestDTO type.
                //PageResolver:NormalizePath()
                //knows how to resolve DTO views.
                return "/views/" + Path.GetFileName(relativePath);
            }
            return relativePath;
        }

        /// <summary>Gets dictionary page path.</summary>
        ///
        /// <param name="file">The file.</param>
        ///
        /// <returns>The dictionary page path.</returns>
        public virtual string GetDictionaryPagePath(IVirtualFile file)
        {
            return GetDictionaryPagePath(file.VirtualPath);
        }

        private List<Task> startupPrecompilationTasks;

        /// <summary>Precompile page.</summary>
        ///
        /// <param name="page">The page.</param>
        ///
        /// <returns>A Task&lt;RazorPage&gt;</returns>
        protected virtual Task<RazorPage> PrecompilePage(RazorPage page)
        {
            page.MarkedForCompilation = true;

            var task = Task.Factory.StartNew(() =>
            {
                try
                {
                    EnsureCompiled(page);

                    if ( page.CompileException != null )
                        Log.ErrorFormat("Precompilation of Razor page '{0}' failed: {1}", page.File.Name, page.CompileException.Message);
                }
                catch (Exception ex)
                {
                    Log.ErrorFormat("Precompilation of Razor page '{0}' failed: {1}", page.File.Name, ex.Message);
                }
                return page;
            });

            if (startupPrecompilationTasks != null )
                startupPrecompilationTasks.Add(task);

            return task;
        }

        /// <summary>Ensures that compiled.</summary>
        ///
        /// <param name="page">The page.</param>
        public virtual void EnsureCompiled(RazorPage page)
        {
            if (page == null) return;
            if (page.IsValid) return;

            lock (page.SyncRoot)
            {
                if (page.IsValid) return;

                var compileTimer = System.Diagnostics.Stopwatch.StartNew();
                try
                {
                    page.IsCompiling = true;
                    page.CompileException = null;

                    var type = page.PageHost.Compile();

                    page.PageType = type;

                    page.IsValid = true;

                    compileTimer.Stop();
                    Log.DebugFormat("Compiled Razor page '{0}' in {1}ms.", page.File.Name, compileTimer.ElapsedMilliseconds);
                }
                catch (HttpCompileException ex)
                {
                    page.CompileException = ex;
                }
                finally
                {
                    page.IsCompiling = false;
                    page.MarkedForCompilation = false;
                }
            }
        }

        #region FileSystemWatcher Handlers

        /// <summary>Gets relative path.</summary>
        ///
        /// <param name="ospath">The ospath.</param>
        ///
        /// <returns>The relative path.</returns>
        public virtual string GetRelativePath(string ospath)
        {
            if (Config.ScanRootPath == null)
                return ospath;

            var relative = ospath
                .Replace(Config.ScanRootPath, "")
                .Replace(this.PathProvider.RealPathSeparator, "/");
            return relative;
        }

        /// <summary>Gets virutal file.</summary>
        ///
        /// <param name="ospath">The ospath.</param>
        ///
        /// <returns>The virutal file.</returns>
        public virtual IVirtualFile GetVirutalFile(string ospath)
        {
            var relative = GetRelativePath(ospath);
            var file = this.PathProvider.GetFile(relative);

            return file; 
        }
        #endregion
    }
}