using System.Collections.Generic;
using System.IO;
using NServiceKit.Markdown;

namespace NServiceKit.WebHost.Endpoints.Support.Markdown.Templates
{
    /// <summary>
    /// 
    /// </summary>
    public class TextBlock : TemplateBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextBlock"/> class.
        /// </summary>
        /// <param name="content">The content.</param>
        public TextBlock(string content)
        {
            Content = CleanHtml(content);
        }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        public string Content { get; set; }

        /// <summary>Writes the specified instance.</summary>
        ///
        /// <param name="instance">  The instance.</param>
        /// <param name="textWriter">The text writer.</param>
        /// <param name="scopeArgs"> The scope arguments.</param>
        public override void Write(MarkdownViewBase instance, TextWriter textWriter, Dictionary<string, object> scopeArgs)
        {
            textWriter.Write(Content);
        }
    }
}