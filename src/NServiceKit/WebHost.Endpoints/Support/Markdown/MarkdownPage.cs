using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using NServiceKit.Common;
using NServiceKit.Logging;
using NServiceKit.Markdown;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints.Formats;
using NServiceKit.WebHost.Endpoints.Support.Markdown.Templates;

namespace NServiceKit.WebHost.Endpoints.Support.Markdown
{
    /// <summary>
    /// 
    /// </summary>
	public class MarkdownPage : IExpirable, IViewPage
	{
	    private static readonly ILog Log = LogManager.GetLogger(typeof (MarkdownPage));

        /// <summary>
        /// The model name
        /// </summary>
		public const string ModelName = "Model";

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkdownPage"/> class.
        /// </summary>
		public MarkdownPage()
		{
			this.ExecutionContext = new EvaluatorExecutionContext();
			this.Dependents = new List<IExpirable>();
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkdownPage"/> class.
        /// </summary>
        /// <param name="markdown">The markdown.</param>
        /// <param name="fullPath">The full path.</param>
        /// <param name="name">The name.</param>
        /// <param name="contents">The contents.</param>
		public MarkdownPage(MarkdownFormat markdown, string fullPath, string name, string contents)
			: this(markdown, fullPath, name, contents, MarkdownPageType.ViewPage)
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkdownPage"/> class.
        /// </summary>
        /// <param name="markdown">The markdown.</param>
        /// <param name="fullPath">The full path.</param>
        /// <param name="name">The name.</param>
        /// <param name="contents">The contents.</param>
        /// <param name="pageType">Type of the page.</param>
		public MarkdownPage(MarkdownFormat markdown, string fullPath, string name, string contents, MarkdownPageType pageType)
			: this()
		{
			Markdown = markdown;
			FilePath = fullPath;
			Name = name;
			Contents = contents;
			PageType = pageType;
		}

        /// <summary>
        /// Gets or sets the markdown.
        /// </summary>
        /// <value>
        /// The markdown.
        /// </value>
		public MarkdownFormat Markdown { get; set; }

		private int timesRun;
		private bool hasCompletedFirstRun;

        /// <summary>
        /// Gets or sets the type of the page.
        /// </summary>
        /// <value>
        /// The type of the page.
        /// </value>
		public MarkdownPageType PageType { get; set; }
        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        /// <value>
        /// The file path.
        /// </value>
		public string FilePath { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
		public string Name { get; set; }
        /// <summary>
        /// Gets or sets the contents.
        /// </summary>
        /// <value>
        /// The contents.
        /// </value>
		public string Contents { get; set; }
        /// <summary>
        /// Gets or sets the HTML contents.
        /// </summary>
        /// <value>
        /// The HTML contents.
        /// </value>
		public string HtmlContents { get; set; }
        /// <summary>
        /// Gets or sets the template.
        /// </summary>
        /// <value>
        /// The template.
        /// </value>
		public string Template { get; set; }
        /// <summary>
        /// Gets or sets the directive template.
        /// </summary>
        /// <value>
        /// The directive template.
        /// </value>
        public string DirectiveTemplate { get; set; }
        /// <summary>
        /// Gets the execution context.
        /// </summary>
        /// <value>
        /// The execution context.
        /// </value>
		public EvaluatorExecutionContext ExecutionContext { get; private set; }

        /// <summary>
        /// Gets or sets the last modified.
        /// </summary>
        /// <value>
        /// The last modified.
        /// </value>
		public DateTime? LastModified { get; set; }
        /// <summary>
        /// Gets the dependents.
        /// </summary>
        /// <value>
        /// The dependents.
        /// </value>
		public List<IExpirable> Dependents { get; private set; }

        /// <summary>
        /// Gets the last modified.
        /// </summary>
        /// <returns></returns>
		public DateTime? GetLastModified()
		{
			if (!hasCompletedFirstRun) return null;
			var lastModified = this.LastModified;
			foreach (var expirable in this.Dependents)
			{
				if (!expirable.LastModified.HasValue) continue;
				if (!lastModified.HasValue || expirable.LastModified > lastModified)
				{
					lastModified = expirable.LastModified;
				}
			}
			return lastModified;
		}

        /// <summary>
        /// Gets the template path.
        /// </summary>
        /// <returns></returns>
		public string GetTemplatePath()
		{
			return this.DirectiveTemplate ?? this.Template;
		}

		private Evaluator evaluator;
        /// <summary>
        /// Gets the evaluator.
        /// </summary>
        /// <value>
        /// The evaluator.
        /// </value>
        /// <exception cref="System.InvalidOperationException">evaluator not ready</exception>
		public Evaluator Evaluator
		{
			get
			{
				if (evaluator == null)
					throw new InvalidOperationException("evaluator not ready");

				return evaluator;
			}
		}

		private int exprSeq;

        /// <summary>
        /// Gets the next identifier.
        /// </summary>
        /// <returns></returns>
		public int GetNextId()
		{
			return exprSeq++;
		}

        /// <summary>
        /// Gets or sets the markdown blocks.
        /// </summary>
        /// <value>
        /// The markdown blocks.
        /// </value>
		public TemplateBlock[] MarkdownBlocks { get; set; }
        /// <summary>
        /// Gets or sets the HTML blocks.
        /// </summary>
        /// <value>
        /// The HTML blocks.
        /// </value>
		public TemplateBlock[] HtmlBlocks { get; set; }

		private Exception initException;
		readonly object readWriteLock = new object();
		private bool isBusy;
        /// <summary>
        /// Reloads the specified contents.
        /// </summary>
        /// <param name="contents">The contents.</param>
        /// <param name="lastModified">The last modified.</param>
		public void Reload(string contents, DateTime lastModified)
		{
			lock (readWriteLock)
			{
				try
				{
					isBusy = true;

					this.Contents = contents;
					this.LastModified = lastModified;

					initException = null;
					exprSeq = 0;
					timesRun = 0;
					ExecutionContext = new EvaluatorExecutionContext();
					Compile(true);
				}
				catch (Exception ex)
				{
					initException = ex;
				}				
				isBusy = false;
				Monitor.PulseAll(readWriteLock);
			}
		}

        /// <summary>
        /// Gets or sets a value indicating whether this instance is compiled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is compiled; otherwise, <c>false</c>.
        /// </value>
        public bool IsCompiled { get; set; }

        /// <summary>
        /// Compiles the specified force.
        /// </summary>
        /// <param name="force">if set to <c>true</c> [force].</param>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">Config.MarkdownBaseType must inherit from MarkdownViewBase</exception>
	    public void Compile(bool force=false)
	    {
            if (this.IsCompiled && !force) return;

	        var sw = Stopwatch.StartNew();

            try
            {
                if (!typeof(MarkdownViewBase).IsAssignableFrom(this.Markdown.MarkdownBaseType))
                {
                    throw new ConfigurationErrorsException(
                        "Config.MarkdownBaseType must inherit from MarkdownViewBase");
                }

                if (this.Contents.IsNullOrEmpty()) return;

                var markdownStatements = new List<StatementExprBlock>();

                var markdownContents = StatementExprBlock.Extract(this.Contents, markdownStatements);

                this.MarkdownBlocks = markdownContents.CreateTemplateBlocks(markdownStatements).ToArray();

                var htmlStatements = new List<StatementExprBlock>();
                var htmlContents = StatementExprBlock.Extract(this.Contents, htmlStatements);

                this.HtmlContents = Markdown.Transform(htmlContents);
                this.HtmlBlocks = this.HtmlContents.CreateTemplateBlocks(htmlStatements).ToArray();

                SetTemplateDirectivePath();

                this.IsCompiled = true;
                Log.InfoFormat("Compiled {0} in {1}ms", this.FilePath, sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                Log.Error("Error compiling {0}".Fmt(this.FilePath), ex);
                throw;
            }
        }

		private void SetTemplateDirectivePath()
		{
			var templateDirective = this.HtmlBlocks.FirstOrDefault(
				x => x is DirectiveBlock && ((DirectiveBlock)x).TemplatePath != null);
			if (templateDirective == null) return;

			this.DirectiveTemplate = ((DirectiveBlock)templateDirective).TemplatePath;
		}

        /// <summary>
        /// Writes the specified text writer.
        /// </summary>
        /// <param name="textWriter">The text writer.</param>
        /// <param name="pageContext">The page context.</param>
        /// <exception cref="System.ArgumentNullException">textWriter</exception>
		public void Write(TextWriter textWriter, PageContext pageContext)
		{
			if (textWriter == null)
				throw new ArgumentNullException("textWriter");

			if (pageContext == null)
				pageContext = new PageContext(this, new Dictionary<string, object>(), true);

			var blocks = pageContext.RenderHtml ? this.HtmlBlocks : this.MarkdownBlocks;

			if (Interlocked.Increment(ref timesRun) == 1)
			{
				lock (readWriteLock)
				{
					try
					{
						isBusy = true;

						this.ExecutionContext.BaseType = Markdown.MarkdownBaseType;
						this.ExecutionContext.TypeProperties = Markdown.MarkdownGlobalHelpers;

						pageContext.MarkdownPage = this;
						var initHtmlContext = pageContext.Create(this, true);
						var initMarkdownContext = pageContext.Create(this, false);

						foreach (var block in this.HtmlBlocks)
						{
						    lastBlockProcessed = block;
                            block.DoFirstRun(initHtmlContext);
                        }
						foreach (var block in this.MarkdownBlocks)
						{
                            lastBlockProcessed = block;
                            block.DoFirstRun(initMarkdownContext);
						}

						this.evaluator = this.ExecutionContext.Build();

						foreach (var block in this.HtmlBlocks)
						{
                            lastBlockProcessed = block;
                            block.AfterFirstRun(evaluator);
						}
						foreach (var block in this.MarkdownBlocks)
						{
                            lastBlockProcessed = block;
                            block.AfterFirstRun(evaluator);
                        }

						AddDependentPages(blocks);

                        lastBlockProcessed = null;
                        initException = null;
						hasCompletedFirstRun = true;
					}
					catch (Exception ex)
					{
						initException = ex;
						throw;
					}
					finally
					{
						isBusy = false;
					}
				}
			}

			lock (readWriteLock)
			{
				while (isBusy)
					Monitor.Wait(readWriteLock);
			}

			if (initException != null)
			{
				timesRun = 0;
				throw initException;
			}

			MarkdownViewBase instance = null;
			if (this.evaluator != null)
			{
				instance = (MarkdownViewBase)this.evaluator.CreateInstance();

				object model;
				pageContext.ScopeArgs.TryGetValue(ModelName, out model);

				instance.Init(Markdown.AppHost, this, pageContext.ScopeArgs, model, pageContext.RenderHtml);
			    instance.ViewEngine = Markdown;
			}

			foreach (var block in blocks)
			{
				block.Write(instance, textWriter, pageContext.ScopeArgs);
			}

			if (instance != null)
			{
				instance.OnLoad();
			}
		}

		private void AddDependentPages(IEnumerable<TemplateBlock> blocks)
		{
			foreach (var block in blocks)
			{
				var exprBlock = block as MethodStatementExprBlock;
				if (exprBlock == null || exprBlock.DependentPageName == null) continue;
				var page = Markdown.GetViewPage(exprBlock.DependentPageName);
				if (page != null)
					Dependents.Add(page);
			}

			MarkdownTemplate template;
			if (this.DirectiveTemplate != null
				&& Markdown.MasterPageTemplates.TryGetValue(this.DirectiveTemplate, out template))
			{
				this.Dependents.Add(template);
			}
			if (this.Template != null
				&& Markdown.MasterPageTemplates.TryGetValue(this.Template, out template))
			{
				this.Dependents.Add(template);
			}

		}
	}
}