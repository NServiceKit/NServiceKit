using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NServiceKit.Markdown;

namespace NServiceKit.WebHost.Endpoints.Support.Markdown.Templates
{
    /// <summary>
    /// 
    /// </summary>
    public class ForEachStatementExprBlock : StatementExprBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ForEachStatementExprBlock"/> class.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <param name="statement">The statement.</param>
        public ForEachStatementExprBlock(string condition, string statement)
            : base(condition, statement)
        {
            Prepare();
        }

        /// <summary>
        /// Gets or sets the name of the enumerator.
        /// </summary>
        /// <value>
        /// The name of the enumerator.
        /// </value>
        public string EnumeratorName { get; set; }

        /// <summary>
        /// Gets or sets the member expr.
        /// </summary>
        /// <value>
        /// The member expr.
        /// </value>
        public string MemberExpr { get; set; }

        /// <summary>
        /// Gets or sets the name of the member variable.
        /// </summary>
        /// <value>
        /// The name of the member variable.
        /// </value>
        public string MemberVarName { get; set; }

        private void Prepare()
        {
            var parts = Condition.SplitOnWhiteSpace();
            if (parts.Length < 3)
                throw new InvalidDataException("Invalid foreach condition: " + Condition);

            var i = parts[0] == "var" ? 1 : 0;
            this.EnumeratorName = parts[i++];
            if (parts[i++] != "in")
                throw new InvalidDataException("Invalid foreach 'in' condition: " + Condition);

            this.MemberExpr = parts[i++];
            this.MemberVarName = this.MemberExpr.GetVarName();
        }

        private object GetModel(Dictionary<string, object> scopeArgs)
        {
            object model;
            if (!scopeArgs.TryGetValue(this.MemberVarName, out model))
                throw new ArgumentException(this.MemberVarName + " does not exist");

            return model;
        }

        private IEnumerable GetMemberExprEnumerator(object model)
        {
            var memberExprEnumerator = getMemberFn(model) as IEnumerable;
            if (memberExprEnumerator == null)
                throw new ArgumentException(this.MemberExpr + " is not an IEnumerable");
            return memberExprEnumerator;
        }

        private Func<object, object> getMemberFn;
        /// <summary>Called when [first run].</summary>
        protected override void OnFirstRun()
        {
            base.OnFirstRun(false);
            var model = GetModel(ScopeArgs);

            getMemberFn = DataBinder.Compile(model.GetType(), MemberExpr);
            var memberExprEnumerator = GetMemberExprEnumerator(model);

            var pageContext = CreatePageContext();
            foreach (var item in memberExprEnumerator)
            {
                ScopeArgs[this.EnumeratorName] = item;
                foreach (var templateBlock in ChildBlocks)
                {
                    templateBlock.DoFirstRun(pageContext);
                }
            }
        }

        /// <summary>Writes the specified instance.</summary>
        ///
        /// <param name="instance">  The instance.</param>
        /// <param name="textWriter">The text writer.</param>
        /// <param name="scopeArgs"> The scope arguments.</param>
        public override void Write(MarkdownViewBase instance, TextWriter textWriter, Dictionary<string, object> scopeArgs)
        {
            var model = GetModel(scopeArgs);
            var memberExprEnumerator = GetMemberExprEnumerator(model);

            if (IsNested)
            {
                //Write Markdown
                foreach (var item in memberExprEnumerator)
                {
                    scopeArgs[this.EnumeratorName] = item;
                    base.Write(instance, textWriter, scopeArgs);
                }
            }
            else
            {
                //Buffer Markdown output before converting and writing HTML
                var sb = new StringBuilder();
                using (var sw = new StringWriter(sb))
                {
                    foreach (var item in memberExprEnumerator)
                    {
                        scopeArgs[this.EnumeratorName] = item;
                        base.Write(instance, sw, scopeArgs);
                    }
                }

                var markdown = sb.ToString();
                var renderedMarkup = Transform(markdown);
                textWriter.Write(renderedMarkup);
            }
        }
    }
}