using System;
using System.Collections.Generic;
using System.IO;
using NServiceKit.Markdown;

namespace NServiceKit.WebHost.Endpoints.Support.Markdown.Templates
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class TemplateBlock : ITemplateWriter
    {
        /// <summary>
        /// Gets or sets the page.
        /// </summary>
        /// <value>
        /// The page.
        /// </value>
        protected MarkdownPage Page { get; set; }

        /// <summary>
        /// Gets or sets the evaluator.
        /// </summary>
        /// <value>
        /// The evaluator.
        /// </value>
        protected Evaluator Evaluator { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is nested.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is nested; otherwise, <c>false</c>.
        /// </value>
        public bool IsNested { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [write raw HTML].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [write raw HTML]; otherwise, <c>false</c>.
        /// </value>
        protected bool WriteRawHtml { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [render HTML].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [render HTML]; otherwise, <c>false</c>.
        /// </value>
        protected bool RenderHtml { get; set; }

        /// <summary>
        /// Gets or sets the scope arguments.
        /// </summary>
        /// <value>
        /// The scope arguments.
        /// </value>
        protected Dictionary<string, object> ScopeArgs { get; set; }

        /// <summary>
        /// Creates the page context.
        /// </summary>
        /// <returns></returns>
        protected PageContext CreatePageContext()
        {
            return new PageContext(Page, ScopeArgs, RenderHtml);
        }

        /// <summary>
        /// Does the first run.
        /// </summary>
        /// <param name="pageContext">The page context.</param>
        public void DoFirstRun(PageContext pageContext)
        {
            //this.PageContext = pageContext;
            this.Page = pageContext.MarkdownPage;
            this.RenderHtml = pageContext.RenderHtml;
            this.ScopeArgs = pageContext.ScopeArgs;

            OnFirstRun();
        }

        /// <summary>
        /// Afters the first run.
        /// </summary>
        /// <param name="evaluator">The evaluator.</param>
        public void AfterFirstRun(Evaluator evaluator)
        {
            this.Evaluator = evaluator;

            OnAfterFirstRun();
        }

        /// <summary>
        /// Called when [first run].
        /// </summary>
        protected virtual void OnFirstRun() { }
        /// <summary>
        /// Called when [after first run].
        /// </summary>
        protected virtual void OnAfterFirstRun() { }

        /// <summary>
        /// Adds the eval item.
        /// </summary>
        /// <param name="evalItem">The eval item.</param>
        public void AddEvalItem(EvaluatorItem evalItem)
        {
            this.Page.ExecutionContext.Items.Add(evalItem);
        }

        private const string EscapedStartTagArtefact = "<p>^";

        /// <summary>
        /// Transforms the HTML.
        /// </summary>
        /// <param name="markdownText">The markdown text.</param>
        /// <returns></returns>
        public string TransformHtml(string markdownText)
        {
            var html = Page.Markdown.Transform(markdownText);

            return CleanHtml(html);
        }

        /// <summary>
        /// Cleans the HTML.
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <returns></returns>
        public static string CleanHtml(string html)
        {
            // ^ is added before ^<html></html> tags to trick Markdown into not thinking its a HTML
            // Start tag so it doesn't skip it and encodes the inner body as normal.
            // We need to Un markdown encode the result i.e. <p>^<div id="searchresults"></p>

            var pos = 0;
            var hasEscapedTags = false;
            while ((pos = html.IndexOf(EscapedStartTagArtefact, pos, StringComparison.CurrentCulture)) != -1)
            {
                hasEscapedTags = true;

                var endPos = html.IndexOf("</p>", pos, StringComparison.CurrentCulture);
                if (endPos == -1) return html; //Unexpected Error so skip

                html = html.Substring(0, endPos) + html.Substring(endPos + 4);

                pos = endPos;
            }

            if (hasEscapedTags) html = html.Replace(EscapedStartTagArtefact, "");

            return html;
        }

        /// <summary>
        /// Transforms the specified markdown text.
        /// </summary>
        /// <param name="markdownText">The markdown text.</param>
        /// <returns></returns>
        public string Transform(string markdownText)
        {
            return this.RenderHtml ? TransformHtml(markdownText) : markdownText;
        }

        /// <summary>
        /// Writes the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="textWriter">The text writer.</param>
        /// <param name="scopeArgs">The scope arguments.</param>
        public abstract void Write(MarkdownViewBase instance, TextWriter textWriter, Dictionary<string, object> scopeArgs);
    }
}