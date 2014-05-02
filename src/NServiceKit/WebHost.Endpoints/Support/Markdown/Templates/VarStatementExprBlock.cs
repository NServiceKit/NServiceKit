using System;
using System.Collections.Generic;
using System.IO;
using NServiceKit.Markdown;

namespace NServiceKit.WebHost.Endpoints.Support.Markdown.Templates
{
    /// <summary>
    /// 
    /// </summary>
    public class VarStatementExprBlock : EvalExprStatementBase
    {
        private string varName;
        private string memberExpr;

        /// <summary>
        /// Initializes a new instance of the <see cref="VarStatementExprBlock"/> class.
        /// </summary>
        /// <param name="directive">The directive.</param>
        /// <param name="line">The line.</param>
        /// <exception cref="System.ArgumentException">Expected 'var' got:  + directive</exception>
        public VarStatementExprBlock(string directive, string line)
            : base(line, null)
        {
            if (directive != "var")
                throw new ArgumentException("Expected 'var' got: " + directive);

            this.ReturnType = typeof(object);
        }

        /// <summary>Called when [first run].</summary>
        ///
        /// <exception cref="InvalidDataException">Thrown when an Invalid Data error condition occurs.</exception>
        protected override void OnFirstRun()
        {
            if (varName != null)
                return;

            var declaration = Condition.TrimEnd().TrimEnd(';');

            var parts = declaration.Split('=');
            if (parts.Length != 2)
                throw new InvalidDataException(
                    "Invalid var declaration, should be '@var varName = {MemberExpression} [, {VarDeclaration}]' was: " + declaration);

            varName = parts[0].Trim();
            memberExpr = parts[1].Trim();

            this.Condition = memberExpr;

            const string methodName = "resolveVarType";
            var exprParams = GetExprParams();
            var evaluator = new Evaluator(ReturnType, Condition, methodName, exprParams);
            var result = evaluator.Evaluate(methodName, GetParamValues(ScopeArgs).ToArray());
            ScopeArgs[varName] = result;
            if (result != null)
                this.ReturnType = result.GetType();

            base.OnFirstRun();
        }

        /// <summary>Writes the specified instance.</summary>
        ///
        /// <param name="instance">  The instance.</param>
        /// <param name="textWriter">The text writer.</param>
        /// <param name="scopeArgs"> The scope arguments.</param>
        public override void Write(MarkdownViewBase instance, TextWriter textWriter, Dictionary<string, object> scopeArgs)
        {
            //Resolve and add to ScopeArgs
            var resultCondition = Evaluate<object>(scopeArgs, true);
            scopeArgs[varName] = resultCondition;
        }
    }
}