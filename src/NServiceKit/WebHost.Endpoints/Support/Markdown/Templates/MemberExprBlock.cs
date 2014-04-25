using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using NServiceKit.Html;
using NServiceKit.Logging;
using NServiceKit.Markdown;

namespace NServiceKit.WebHost.Endpoints.Support.Markdown.Templates
{
    /// <summary>
    /// 
    /// </summary>
    public class MemberExprBlock : TemplateBlock
    {
        private static ILog Log = LogManager.GetLogger(typeof(MemberExprBlock));

        private string memberExpr;
        private readonly string modelMemberExpr;
        private readonly string varName;

        private bool ReferencesSelf
        {
            get { return this.modelMemberExpr == null; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberExprBlock"/> class.
        /// </summary>
        /// <param name="memberExpr">The member expr.</param>
        public MemberExprBlock(string memberExpr)
        {
            try
            {
                this.memberExpr = memberExpr;
                this.varName = memberExpr.GetVarName();
                this.modelMemberExpr = varName != memberExpr
                    ? memberExpr.Substring(this.varName.Length + 1)
                    : null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private Func<object, string> valueFn;
        private Func<string> staticValueFn;

        protected override void OnFirstRun()
        {
            base.OnFirstRun();

            object memberExprValue;
            if (ScopeArgs.TryGetValue(this.varName, out memberExprValue))
            {
                InitializeValueFn(memberExprValue);
            }
            else
            {
                staticValueFn = DataBinder.CompileStaticAccessToString(memberExpr);
            }
        }

        private void InitializeValueFn(object memberExprValue)
        {
            valueFn = this.ReferencesSelf 
                ? Convert.ToString 
                : DataBinder.CompileToString(memberExprValue.GetType(), modelMemberExpr);
        }
        
        public override void Write(MarkdownViewBase instance, TextWriter textWriter, Dictionary<string, object> scopeArgs)
        {
            object memberExprValue;
            if (!scopeArgs.TryGetValue(this.varName, out memberExprValue))
            {
                if (staticValueFn != null)
                {
                    var strValue = this.staticValueFn();
                    textWriter.Write(HttpUtility.HtmlEncode(strValue));
                }
                else
                {
                    textWriter.Write(this.memberExpr);
                }
                return;
            }

            if (memberExprValue == null) return;

            try
            {
                if (memberExprValue is MvcHtmlString)
                {
                    textWriter.Write(memberExprValue);
                    return;
                }
                if (valueFn == null)
                {
                    InitializeValueFn(memberExprValue);
                }
                var strValue = this.ReferencesSelf
                    ? Convert.ToString(memberExprValue)
                    : valueFn(memberExprValue);

                textWriter.Write(HttpUtility.HtmlEncode(strValue));
            }
            catch (Exception ex)
            {
                Log.Error("MemberExprBlock: " + ex.Message, ex);
            }
        }
    }
}