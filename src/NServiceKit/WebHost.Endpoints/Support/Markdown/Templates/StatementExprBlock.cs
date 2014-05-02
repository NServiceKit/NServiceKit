using System.Collections.Generic;
using System.IO;
using System.Text;
using NServiceKit.Common;
using NServiceKit.Markdown;

namespace NServiceKit.WebHost.Endpoints.Support.Markdown.Templates
{
    /// <summary>
    /// 
    /// </summary>
    public class StatementExprBlock : TemplateBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatementExprBlock"/> class.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <param name="statement">The statement.</param>
        public StatementExprBlock(string condition, string statement)
        {
            this.Condition = condition;
            this.Statement = statement;
            this.ChildBlocks = new TemplateBlock[0];
        }

        /// <summary>
        /// Gets or sets the condition.
        /// </summary>
        /// <value>
        /// The condition.
        /// </value>
        public string Condition { get; set; }
        /// <summary>
        /// Gets or sets the statement.
        /// </summary>
        /// <value>
        /// The statement.
        /// </value>
        public string Statement { get; set; }

        /// <summary>
        /// Gets or sets the child blocks.
        /// </summary>
        /// <value>
        /// The child blocks.
        /// </value>
        public TemplateBlock[] ChildBlocks { get; set; }

        /// <summary>
        /// Prepares the specified all statements.
        /// </summary>
        /// <param name="allStatements">All statements.</param>
        protected virtual void Prepare(List<StatementExprBlock> allStatements)
        {
            if (this.Statement.IsNullOrEmpty()) return;

            var parsedStatement = Extract(this.Statement, allStatements);

            var childBlocks = parsedStatement.CreateTemplateBlocks(allStatements);
            childBlocks.ForEach(x => x.IsNested = true);

            RemoveTrailingNewLineIfProceedsStatement(childBlocks);

            this.ChildBlocks = childBlocks.ToArray();
        }

        internal static void RemoveTrailingNewLineIfProceedsStatement(List<TemplateBlock> childBlocks)
        {
            if (childBlocks.Count < 2) return;

            var lastIndex = childBlocks.Count - 1;
            if (!(childBlocks[lastIndex - 1] is StatementExprBlock)) return;

            var textBlock = childBlocks[lastIndex] as TextBlock;
            if (textBlock == null) return;

            if (textBlock.Content == "\r\n")
            {
                childBlocks.RemoveAt(lastIndex);
            }
        }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Called when [first run].
        /// </summary>
        /// <param name="applyToChildren">if set to <c>true</c> [apply to children].</param>
        protected void OnFirstRun(bool applyToChildren)
        {
            if (applyToChildren)
                this.OnFirstRun();
            else
                base.OnFirstRun();
        }

        /// <summary>Called when [first run].</summary>
        protected override void OnFirstRun()
        {
            base.OnFirstRun();

            this.Id = Page.GetNextId();

            var pageContext = CreatePageContext();
            foreach (var templateBlock in ChildBlocks)
            {
                templateBlock.DoFirstRun(pageContext);
            }
        }

        /// <summary>Called when [after first run].</summary>
        protected override void OnAfterFirstRun()
        {
            base.OnAfterFirstRun();

            foreach (var templateBlock in ChildBlocks)
            {
                templateBlock.AfterFirstRun(this.Evaluator);
            }
        }

        /// <summary>Writes the specified instance.</summary>
        ///
        /// <param name="instance">  The instance.</param>
        /// <param name="textWriter">The text writer.</param>
        /// <param name="scopeArgs"> The scope arguments.</param>
        public override void Write(MarkdownViewBase instance, TextWriter textWriter, Dictionary<string, object> scopeArgs)
        {
            WriteInternal(instance, textWriter, scopeArgs);
        }

        private void WriteInternal(MarkdownViewBase instance, TextWriter textWriter, Dictionary<string, object> scopeArgs)
        {
            foreach (var templateBlock in ChildBlocks)
            {
                templateBlock.Write(instance, textWriter, scopeArgs);
            }
        }

        /// <summary>
        /// Extracts the specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="allStatements">All statements.</param>
        /// <returns></returns>
        /// <exception cref="System.IO.InvalidDataException">Unterminated Comment at charIndex:  + pos</exception>
        public static string Extract(string content, List<StatementExprBlock> allStatements)
        {
            var sb = new StringBuilder();

            var initialCount = allStatements.Count;
            int pos;
            var lastPos = 0;
            while ((pos = content.IndexOf('@', lastPos)) != -1)
            {
                var peekChar = content.Substring(pos + 1, 1);
                var isComment = peekChar == "*";
                if (isComment)
                {
                    var endPos = content.IndexOf("*@", pos);
                    if (endPos == -1)
                        throw new InvalidDataException("Unterminated Comment at charIndex: " + pos);
                    lastPos = endPos + 2;
                    continue;
                }
                if (peekChar == "@")
                {
                    sb.Append('@');
                    pos += 2;
                    lastPos = pos;
                    continue;
                }

                var contentBlock = content.Substring(lastPos, pos - lastPos);

                var startPos = pos;
                pos++; //@

                var statementExpr = content.GetNextStatementExpr(ref pos);
                if (statementExpr != null)
                {
                    contentBlock = contentBlock.TrimLineIfOnlyHasWhitespace();
                    sb.Append(contentBlock);

                    if (statementExpr is MethodStatementExprBlock)
                        sb.Append(' '); //ensure a spacer between method blocks

                    statementExpr.Prepare(allStatements);
                    allStatements.Add(statementExpr);
                    var placeholder = "@" + TemplateExtensions.StatementPlaceholderChar + allStatements.Count;
                    sb.Append(placeholder);
                    lastPos = pos;
                }
                else
                {
                    sb.Append(contentBlock);

                    sb.Append('@');
                    lastPos = startPos + 1;
                }
            }

            if (lastPos != content.Length - 1)
            {
                var lastBlock = lastPos == 0 ? content : content.Substring(lastPos);
                sb.Append(lastBlock);
            }

            return allStatements.Count > initialCount ? sb.ToString() : content;
        }

        /// <summary>
        /// Writes the statement.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="textWriter">The text writer.</param>
        /// <param name="scopeArgs">The scope arguments.</param>
        protected void WriteStatement(MarkdownViewBase instance, TextWriter textWriter, Dictionary<string, object> scopeArgs)
        {
            if (IsNested)
            {
                //Write Markdown
                WriteInternal(instance, textWriter, scopeArgs);
            }
            else
            {
                //Buffer Markdown output before converting and writing HTML
                var sb = new StringBuilder();
                using (var sw = new StringWriter(sb))
                {
                    WriteInternal(instance, sw, scopeArgs);
                }

                var markdown = sb.ToString();
                var html = Transform(markdown);
                textWriter.Write(html);
            }
        }
    }
}