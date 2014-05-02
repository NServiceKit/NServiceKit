using System.CodeDom;
using System.Diagnostics.CodeAnalysis;

namespace NServiceKit.Razor.Compilation.CodeTransformers
{
    /// <summary>An exclude from code coverage transformer.</summary>
    public class ExcludeFromCodeCoverageTransformer : RazorCodeTransformerBase
    {
        /// <summary>Process the generated code.</summary>
        ///
        /// <param name="codeCompileUnit">   The code compile unit.</param>
        /// <param name="generatedNamespace">The generated namespace.</param>
        /// <param name="generatedClass">    The generated class.</param>
        /// <param name="executeMethod">     The execute method.</param>
        public override void ProcessGeneratedCode(CodeCompileUnit codeCompileUnit, CodeNamespace generatedNamespace, CodeTypeDeclaration generatedClass, CodeMemberMethod executeMethod)
        {
            var codeTypeReference = new CodeTypeReference(typeof(ExcludeFromCodeCoverageAttribute));
            generatedClass.CustomAttributes.Add(new CodeAttributeDeclaration(codeTypeReference));
        }
    }
}
