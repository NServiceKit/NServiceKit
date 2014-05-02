using System.CodeDom;

namespace NServiceKit.Razor.Compilation.CodeTransformers
{
    /// <summary>A make type partial.</summary>
    public class MakeTypePartial : RazorCodeTransformerBase
    {
        /// <summary>Process the generated code.</summary>
        ///
        /// <param name="codeCompileUnit">   The code compile unit.</param>
        /// <param name="generatedNamespace">The generated namespace.</param>
        /// <param name="generatedClass">    The generated class.</param>
        /// <param name="executeMethod">     The execute method.</param>
        public override void ProcessGeneratedCode(CodeCompileUnit codeCompileUnit, CodeNamespace generatedNamespace, CodeTypeDeclaration generatedClass, CodeMemberMethod executeMethod)
        {
            generatedClass.IsPartial = true;
        }
    }
}
