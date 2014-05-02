using System;
using System.CodeDom;
using System.Reflection;

namespace NServiceKit.Razor.Compilation.CodeTransformers
{
    /// <summary>A set type visibility.</summary>
    public class SetTypeVisibility : RazorCodeTransformerBase
    {
        private readonly string _visibility;

        /// <summary>Initializes a new instance of the NServiceKit.Razor.Compilation.CodeTransformers.SetTypeVisibility class.</summary>
        ///
        /// <param name="visibility">The visibility.</param>
        public SetTypeVisibility(string visibility)
        {
            _visibility = visibility;
        }

        /// <summary>Process the generated code.</summary>
        ///
        /// <param name="codeCompileUnit">   The code compile unit.</param>
        /// <param name="generatedNamespace">The generated namespace.</param>
        /// <param name="generatedClass">    The generated class.</param>
        /// <param name="executeMethod">     The execute method.</param>
        public override void ProcessGeneratedCode(CodeCompileUnit codeCompileUnit, CodeNamespace generatedNamespace, CodeTypeDeclaration generatedClass, CodeMemberMethod executeMethod)
        {
            if (_visibility.Equals("Public", StringComparison.OrdinalIgnoreCase))
            {
                generatedClass.TypeAttributes = generatedClass.TypeAttributes & ~TypeAttributes.VisibilityMask | TypeAttributes.Public;
            }
            else if (_visibility.Equals("Internal", StringComparison.OrdinalIgnoreCase))
            {
                generatedClass.TypeAttributes = generatedClass.TypeAttributes & ~TypeAttributes.VisibilityMask | TypeAttributes.NestedFamANDAssem;
            }
        }
    }
}
