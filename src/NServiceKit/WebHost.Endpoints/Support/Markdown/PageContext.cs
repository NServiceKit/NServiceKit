using System.Collections.Generic;

namespace NServiceKit.WebHost.Endpoints.Support.Markdown
{
    /// <summary>
    /// 
    /// </summary>
    public class PageContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PageContext"/> class.
        /// </summary>
        public PageContext() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageContext"/> class.
        /// </summary>
        /// <param name="markdownPage">The markdown page.</param>
        /// <param name="scopeArgs">The scope arguments.</param>
        /// <param name="renderHtml">if set to <c>true</c> [render HTML].</param>
        public PageContext(MarkdownPage markdownPage, Dictionary<string, object> scopeArgs, bool renderHtml)
        {
            MarkdownPage = markdownPage;
            ScopeArgs = scopeArgs ?? new Dictionary<string, object>();
            RenderHtml = renderHtml;
        }

        /// <summary>
        /// Gets or sets the markdown page.
        /// </summary>
        /// <value>
        /// The markdown page.
        /// </value>
        public MarkdownPage MarkdownPage { get; set; }

        /// <summary>
        /// Gets or sets the scope arguments.
        /// </summary>
        /// <value>
        /// The scope arguments.
        /// </value>
        public Dictionary<string, object> ScopeArgs { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [render HTML].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [render HTML]; otherwise, <c>false</c>.
        /// </value>
        public bool RenderHtml { get; set; }

        /// <summary>
        /// Creates the specified markdown page.
        /// </summary>
        /// <param name="markdownPage">The markdown page.</param>
        /// <param name="renderHtml">if set to <c>true</c> [render HTML].</param>
        /// <returns></returns>
        public PageContext Create(MarkdownPage markdownPage, bool renderHtml)
        {
            return new PageContext(markdownPage, ScopeArgs, renderHtml);
        }
    }
}