using System;
using System.CodeDom;

namespace NServiceKit.Razor.Compilation.CodeTransformers
{
    /// <summary>A suffix file name transformer.</summary>
    public class SuffixFileNameTransformer : RazorCodeTransformerBase
    {
        private readonly string _suffix;

        /// <summary>Initializes a new instance of the NServiceKit.Razor.Compilation.CodeTransformers.SuffixFileNameTransformer class.</summary>
        ///
        /// <param name="suffix">The suffix.</param>
        public SuffixFileNameTransformer(string suffix)
        {
            _suffix = suffix;
        }

        /// <summary>Process the generated code.</summary>
        ///
        /// <param name="codeCompileUnit">   The code compile unit.</param>
        /// <param name="generatedNamespace">The generated namespace.</param>
        /// <param name="generatedClass">    The generated class.</param>
        /// <param name="executeMethod">     The execute method.</param>
        public override void ProcessGeneratedCode(CodeCompileUnit codeCompileUnit, CodeNamespace generatedNamespace, CodeTypeDeclaration generatedClass, CodeMemberMethod executeMethod)
        {
            if (!String.IsNullOrEmpty(_suffix))
            {
                generatedClass.Name += _suffix;
            }
        }
    }
}
