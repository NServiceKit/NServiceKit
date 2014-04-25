using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using NServiceKit.Html;
using NServiceKit.Markdown;
using NServiceKit.Text;

namespace NServiceKit.WebHost.Endpoints.Support.Markdown.Templates
{
    /// <summary>
    /// 
    /// </summary>
    public class MethodStatementExprBlock : EvalExprStatementBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MethodStatementExprBlock"/> class.
        /// </summary>
        /// <param name="methodExpr">The method expr.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="statement">The statement.</param>
        public MethodStatementExprBlock(string methodExpr, string condition, string statement)
            : base(condition, statement)
        {
            this.methodExpr = methodExpr;
        }

        private readonly string methodExpr;
        protected override void OnFirstRun()
        {
            Prepare(Page);
            base.OnFirstRun();
        }

        /// <summary>
        /// Gets the name of the dependent page.
        /// </summary>
        /// <value>
        /// The name of the dependent page.
        /// </value>
        public string DependentPageName { get; private set; }

        private void Prepare(MarkdownPage markdownPage)
        {
            var rawMethodExpr = methodExpr.Replace("Html.", "");
            if (rawMethodExpr == "Partial")
            {
                this.DependentPageName = this.Condition.ExtractContents("\"", "\"");
            }
            this.WriteRawHtml = rawMethodExpr == "Raw";

            var parts = methodExpr.Split('.');
            if (parts.Length > 2)
                throw new ArgumentException("Unable to resolve method: " + methodExpr);

            var usesBaseType = parts.Length == 1;
            var typePropertyName = parts[0];
            var methodName = usesBaseType ? parts[0] : parts[1];

            Type type = null;
            if (typePropertyName == "Html")
            {
                type = markdownPage.ExecutionContext.BaseType.HasGenericType()
                    ? typeof(HtmlHelper<>)
                    : typeof(HtmlHelper);
            }
            if (type == null)
            {
                type = usesBaseType
                    ? markdownPage.ExecutionContext.BaseType
                    : markdownPage.Markdown.MarkdownGlobalHelpers.TryGetValue(typePropertyName, out type) ? type : null;
            }

            if (type == null)
                throw new InvalidDataException(string.Format(
                    "Unable to resolve type '{0}'. Check type exists in Config.MarkdownBaseType or Page.Markdown.MarkdownGlobalHelpers",
                    typePropertyName));

            var mi = methodName == "Partial" 
                ? type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(m => m.GetParameters().Length == 2 && m.Name == methodName)
                : type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);

            if (mi == null)
            {
                mi = HtmlHelper.GetMethod(methodName);
                if (mi == null)
                    throw new ArgumentException("Unable to resolve method '" + methodExpr + "' on type " + type.Name);
            }

            base.ReturnType = mi.ReturnType;

            var isMemberExpr = Condition.IndexOf('(') != -1;
            if (!isMemberExpr || this.WriteRawHtml)
            {
                base.Condition = methodExpr + "(" + Condition + ")";
            }
        }

        /// <summary>
        /// Writes the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="textWriter">The text writer.</param>
        /// <param name="scopeArgs">The scope arguments.</param>
        public override void Write(MarkdownViewBase instance, TextWriter textWriter, Dictionary<string, object> scopeArgs)
        {
            var paramValues = GetParamValues(scopeArgs);
            var result = Evaluator.Evaluate(instance, CodeGenMethodName, paramValues.ToArray());
            if (result == null) return;

            string strResult;

            var mvcString = result as MvcHtmlString;
            if (mvcString != null)
            {
                WriteRawHtml = true;
                strResult = mvcString.ToHtmlString();
            }
            else
            {
                strResult = result as string ?? Convert.ToString(result);
            }

            if (!WriteRawHtml)
                strResult = HttpUtility.HtmlEncode(strResult);

            textWriter.Write(strResult);
        }
    }
}