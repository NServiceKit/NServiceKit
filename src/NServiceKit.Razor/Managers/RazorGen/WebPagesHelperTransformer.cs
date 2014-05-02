using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NServiceKit.Razor.Compilation.CodeTransformers;

namespace NServiceKit.Razor.Managers.RazorGen
{
//    [Export("WebPagesHelper", typeof(IRazorCodeTransformer))]
    /// <summary>A web pages helper transformer.</summary>
    public class WebPagesHelperTransformer : AggregateCodeTransformer
    {
        private readonly RazorCodeTransformerBase[] _codeTransformers = new RazorCodeTransformerBase[] {
            new DirectivesBasedTransformers(),
            new AddGeneratedClassAttribute(),
            new SetImports(new[] { "System.Web.WebPages.Html" }, replaceExisting: false),
//            new SetBaseType(typeof(System.Web.WebPages.HelperPage)),
            new MakeTypeHelper(),
            new RemoveLineHiddenPragmas(),
        };

        /// <summary>Gets the code transformers.</summary>
        ///
        /// <value>The code transformers.</value>
        protected override IEnumerable<RazorCodeTransformerBase> CodeTransformers
        {
            get { return _codeTransformers; }
        }

        /// <summary>Process the generated code.</summary>
        ///
        /// <param name="codeCompileUnit">   The code compile unit.</param>
        /// <param name="generatedNamespace">The generated namespace.</param>
        /// <param name="generatedClass">    The generated class.</param>
        /// <param name="executeMethod">     The execute method.</param>
        public override void ProcessGeneratedCode(CodeCompileUnit codeCompileUnit,
                                                  CodeNamespace generatedNamespace,
                                                  CodeTypeDeclaration generatedClass,
                                                  CodeMemberMethod executeMethod)
        {
            base.ProcessGeneratedCode(codeCompileUnit, generatedNamespace, generatedClass, executeMethod);

            // Make all helper methods prefixed by '_' internal
            foreach (var method in generatedClass.Members.OfType<CodeSnippetTypeMember>())
            {
                method.Text = Regex.Replace(method.Text, "public static System\\.Web\\.WebPages\\.HelperResult _",
                     "internal static System.Web.WebPages.HelperResult _");
            }
        }
    }
}
