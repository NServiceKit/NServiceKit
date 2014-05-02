using System.Collections.Generic;
using System.IO;
using System.Text;
using NServiceKit.Html;
using NServiceKit.Markdown;

namespace NServiceKit.WebHost.Endpoints.Support.Markdown.Templates
{
    /// <summary>
    /// 
    /// </summary>
    public class SectionStatementExprBlock : StatementExprBlock
    {
        /// <summary>
        /// Gets or sets the name of the section.
        /// </summary>
        /// <value>
        /// The name of the section.
        /// </value>
        public string SectionName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SectionStatementExprBlock"/> class.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <param name="statement">The statement.</param>
        public SectionStatementExprBlock(string condition, string statement)
            : base(condition, statement)
        {
            Prepare();
        }

        /// <summary>
        /// Prepares this instance.
        /// </summary>
        public void Prepare()
        {
            this.SectionName = Condition.Trim();
        }

        /// <summary>Writes the specified instance.</summary>
        ///
        /// <param name="instance">  The instance.</param>
        /// <param name="textWriter">The text writer.</param>
        /// <param name="scopeArgs"> The scope arguments.</param>
        public override void Write(MarkdownViewBase instance, TextWriter textWriter, Dictionary<string, object> scopeArgs)
        {
            //Don't output anything, capture all output and store it in scopeArgs[SectionName]
            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
            {
                base.Write(instance, sw, scopeArgs);
            }

            var markdown = sb.ToString();
            var renderedMarkup = Transform(markdown);
            scopeArgs[SectionName] = MvcHtmlString.Create(renderedMarkup);
        }
    }
}