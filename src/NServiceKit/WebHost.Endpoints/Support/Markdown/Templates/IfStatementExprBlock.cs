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
    public class IfStatementExprBlock : EvalExprStatementBase
    {
        /// <summary>
        /// Gets or sets the else statement.
        /// </summary>
        /// <value>
        /// The else statement.
        /// </value>
        public string ElseStatement { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IfStatementExprBlock"/> class.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <param name="statement">The statement.</param>
        /// <param name="elseStatement">The else statement.</param>
        public IfStatementExprBlock(string condition, string statement, string elseStatement)
            : base(condition, statement)
        {
            this.ReturnType = typeof(bool);
            this.ElseStatement = elseStatement;
            this.ElseChildBlocks = new TemplateBlock[0];
        }

        /// <summary>
        /// Gets or sets the else child blocks.
        /// </summary>
        /// <value>
        /// The else child blocks.
        /// </value>
        public TemplateBlock[] ElseChildBlocks { get; set; }

        /// <summary>
        /// Prepares the specified all statements.
        /// </summary>
        /// <param name="allStatements">All statements.</param>
        protected override void Prepare(List<StatementExprBlock> allStatements)
        {
            base.Prepare(allStatements);

            if (this.ElseStatement.IsNullOrEmpty()) return;

            var parsedStatement = Extract(this.ElseStatement, allStatements);

            var elseChildBlocks = parsedStatement.CreateTemplateBlocks(allStatements);
            elseChildBlocks.ForEach(x => x.IsNested = true);

            RemoveTrailingNewLineIfProceedsStatement(elseChildBlocks);
			
            this.ElseChildBlocks = elseChildBlocks.ToArray();
        }

        public override void Write(MarkdownViewBase instance, TextWriter textWriter, Dictionary<string, object> scopeArgs)
        {
            var resultCondition = Evaluate<bool>(scopeArgs);
            if (resultCondition)
            {
                WriteStatement(instance, textWriter, scopeArgs);
            }
            else
            {
                if (ElseStatement != null && this.ElseChildBlocks.Length > 0)
                {
                    WriteElseStatement(instance, textWriter, scopeArgs);
                }
            }
        }

        protected override void OnFirstRun()
        {
            base.OnFirstRun();

            var pageContext = CreatePageContext();
            foreach (var templateBlock in ElseChildBlocks)
            {
                templateBlock.DoFirstRun(pageContext);
            }
        }

        /// <summary>
        /// Writes the else statement.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="textWriter">The text writer.</param>
        /// <param name="scopeArgs">The scope arguments.</param>
        protected void WriteElseStatement(MarkdownViewBase instance, TextWriter textWriter, Dictionary<string, object> scopeArgs)
        {
            //TODO: DRY IT
            if (IsNested)
            {
                //Write Markdown
                foreach (var templateBlock in ElseChildBlocks)
                {
                    templateBlock.Write(instance, textWriter, scopeArgs);
                }
            }
            else
            {
                //Buffer Markdown output before converting and writing HTML
                var sb = new StringBuilder();
                using (var sw = new StringWriter(sb))
                {
                    foreach (var templateBlock in ElseChildBlocks)
                    {
                        templateBlock.Write(instance, sw, scopeArgs);
                    }
                }

                var markdown = sb.ToString();
                var html = Transform(markdown);
                textWriter.Write(html);
            }
        }
    }
}