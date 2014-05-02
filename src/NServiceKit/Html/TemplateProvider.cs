using System;
using System.Collections.Generic;
using System.Threading;
using NServiceKit.Common;
using NServiceKit.Common.Net30;
using NServiceKit.IO;
using NServiceKit.Logging;
using NServiceKit.Text;
using NServiceKit.VirtualPath;

namespace NServiceKit.Html
{
    /// <summary>A template provider.</summary>
    public class TemplateProvider
    {
        /// <summary>Gets or sets the compile in parallel with no of threads.</summary>
        ///
        /// <value>The compile in parallel with no of threads.</value>
        public int? CompileInParallelWithNoOfThreads { get; set; }

        private static readonly ILog Log = LogManager.GetLogger(typeof(TemplateProvider));

        AutoResetEvent waiter = new AutoResetEvent(false);

        readonly string defaultTemplateName;

        /// <summary>Initializes a new instance of the NServiceKit.Html.TemplateProvider class.</summary>
        ///
        /// <param name="defaultTemplateName">The default template name.</param>
        public TemplateProvider(string defaultTemplateName)
        {
            this.defaultTemplateName = defaultTemplateName;
        }

        readonly Dictionary<string, IVirtualFile> templatePathsFound = new Dictionary<string, IVirtualFile>(StringComparer.InvariantCultureIgnoreCase);
        readonly HashSet<string> templatePathsNotFound = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>Gets template path.</summary>
        ///
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        ///
        /// <param name="fileDir">The file dir.</param>
        ///
        /// <returns>The template path.</returns>
        public string GetTemplatePath(IVirtualDirectory fileDir)
        {
            try
            {
                if (templatePathsNotFound.Contains(fileDir.VirtualPath)) return null;

                var templateDir = fileDir;
                IVirtualFile templateFile;
                while (templateDir != null && templateDir.GetFile(defaultTemplateName) == null)
                {
                    if (templatePathsFound.TryGetValue(templateDir.VirtualPath, out templateFile))
                        return templateFile.RealPath;

                    templateDir = templateDir.ParentDirectory;
                }

                if (templateDir != null)
                {
                    templateFile = templateDir.GetFile(defaultTemplateName);
                    templatePathsFound[templateDir.VirtualPath] = templateFile;
                    return templateFile.VirtualPath;
                }

                templatePathsNotFound.Add(fileDir.VirtualPath);
                return null;

            }
            catch (Exception ex)
            {
                ex.Message.Print();
                throw;
            }
        }

        private readonly ConcurrentQueue<IViewPage> compilePages = new ConcurrentQueue<IViewPage>();
        private readonly ConcurrentQueue<IViewPage> priorityCompilePages = new ConcurrentQueue<IViewPage>();

        /// <summary>Queue page to compile.</summary>
        ///
        /// <param name="pageToCompile">The page to compile.</param>
        public void QueuePageToCompile(IViewPage pageToCompile)
        {
            waiter.Reset();
            compilePages.Enqueue(pageToCompile);
        }

        private int runningThreads;
        /// <summary>Compile queued pages.</summary>
        public void CompileQueuedPages()
        {
            var compileInParallel = CompileInParallelWithNoOfThreads > 0;

            Log.InfoFormat("Starting to compile {0}/{1} pages, {2}",
                compilePages.Count, priorityCompilePages.Count,
                compileInParallel ? "In Parallel" : "Sequentially");

            if (compileInParallel)
            {
                var threadsToRun = Math.Min(CompileInParallelWithNoOfThreads.GetValueOrDefault(), compilePages.Count);
                if (threadsToRun <= runningThreads) return;

                Log.InfoFormat("Starting {0} threads..", threadsToRun);

                threadsToRun.Times(x => {
                    ThreadPool.QueueUserWorkItem(waitHandle => { try { CompileAllPages(); } catch { } });
                });
            }
            else
            {
                CompileAllPages();
            }
        }

        private void CompileAllPages()
        {
            try
            {
                Interlocked.Increment(ref runningThreads);
                while (!compilePages.IsEmpty || !priorityCompilePages.IsEmpty)
                {
                    IViewPage viewPage;
                    if (compilePages.TryDequeue(out viewPage))
                    {
                        viewPage.Compile();
                    }
                }
            }
            finally
            {
                Interlocked.Decrement(ref runningThreads);
                Log.InfoFormat("Compilation threads remaining {0}...", runningThreads);
                waiter.Set();
            }
        }

        /// <summary>Ensures that all compiled.</summary>
        public void EnsureAllCompiled()
        {
            if (compilePages.IsEmpty && priorityCompilePages.IsEmpty) return;
            waiter.WaitOne(60 * 1000);
        }
    }

}

