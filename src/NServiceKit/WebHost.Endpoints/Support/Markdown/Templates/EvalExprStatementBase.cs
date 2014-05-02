using System;
using System.Collections.Generic;
using NServiceKit.Text;

namespace NServiceKit.WebHost.Endpoints.Support.Markdown.Templates
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class EvalExprStatementBase : StatementExprBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EvalExprStatementBase"/> class.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <param name="statement">The statement.</param>
        protected EvalExprStatementBase(string condition, string statement)
            : base(condition, statement)
        {
        }

        /// <summary>Type of the return.</summary>
        protected Type ReturnType = typeof(string);
        private string[] paramNames;

        /// <summary>
        /// Gets or sets the name of the code gen method.
        /// </summary>
        /// <value>
        /// The name of the code gen method.
        /// </value>
        protected string CodeGenMethodName { get; set; }

        /// <summary>
        /// Gets the parameter names.
        /// </summary>
        /// <param name="scopeArgs">The scope arguments.</param>
        /// <returns></returns>
        public string[] GetParamNames(Dictionary<string, object> scopeArgs)
        {
            return this.paramNames ?? (this.paramNames = scopeArgs.Keys.ToArray());
        }

        /// <summary>Called when [first run].</summary>
        protected override void OnFirstRun()
        {
            base.OnFirstRun();

            CodeGenMethodName = "EvalExpr_" + this.Id;

            var exprParams = GetExprParams();
            var evalItem = new EvaluatorItem(ReturnType, CodeGenMethodName, Condition, exprParams);

            AddEvalItem(evalItem);
        }

        /// <summary>
        /// Gets the expr parameters.
        /// </summary>
        /// <returns></returns>
        protected Dictionary<string, Type> GetExprParams()
        {
            var exprParams = new Dictionary<string, Type>();
            paramNames = GetParamNames(ScopeArgs);
            var paramValues = GetParamValues(ScopeArgs);
            for (var i = 0; i < paramNames.Length; i++)
            {
                var paramName = paramNames[i];
                var paramValue = paramValues[i];

                exprParams[paramName] = paramValue != null ? paramValue.GetType() : typeof(object);
            }
            return exprParams;
        }

        /// <summary>
        /// Gets the parameter values.
        /// </summary>
        /// <param name="scopeArgs">The scope arguments.</param>
        /// <param name="defaultToNullValues">if set to <c>true</c> [default to null values].</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Unresolved param  + paramName +  in  + Condition</exception>
        protected List<object> GetParamValues(IDictionary<string, object> scopeArgs, bool defaultToNullValues)
        {
            var results = new List<object>();
            foreach (var paramName in paramNames)
            {
                object paramValue;
                if (!scopeArgs.TryGetValue(paramName, out paramValue) && !defaultToNullValues)
                    throw new ArgumentException("Unresolved param " + paramName + " in " + Condition);

                results.Add(paramValue);
            }
            return results;
        }

        /// <summary>
        /// Gets the parameter values.
        /// </summary>
        /// <param name="scopeArgs">The scope arguments.</param>
        /// <returns></returns>
        protected List<object> GetParamValues(IDictionary<string, object> scopeArgs)
        {
            return GetParamValues(scopeArgs, false);
        }

        /// <summary>
        /// Evaluates the specified scope arguments.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scopeArgs">The scope arguments.</param>
        /// <param name="defaultToNullValues">if set to <c>true</c> [default to null values].</param>
        /// <returns></returns>
        public T Evaluate<T>(Dictionary<string, object> scopeArgs, bool defaultToNullValues)
        {
            var paramValues = GetParamValues(scopeArgs, defaultToNullValues);
            return (T)Evaluator.Evaluate(CodeGenMethodName, paramValues.ToArray());
        }

        /// <summary>
        /// Evaluates the specified scope arguments.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scopeArgs">The scope arguments.</param>
        /// <returns></returns>
        public T Evaluate<T>(Dictionary<string, object> scopeArgs)
        {
            return Evaluate<T>(scopeArgs, true);
        }
    }
}