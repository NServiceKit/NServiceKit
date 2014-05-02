using System.CodeDom;
using System.Collections.Generic;

namespace NServiceKit.Razor.Compilation.CodeTransformers
{
    /// <summary>A razor code transformer base.</summary>
    public class RazorCodeTransformerBase : IRazorCodeTransformer
    {
        void IRazorCodeTransformer.Initialize(IRazorHost razorHost, IDictionary<string, string> directives)
        {
            Initialize((RazorPageHost)razorHost, directives);
        }

        /// <summary>Initializes this object.</summary>
        ///
        /// <param name="razorHost"> The razor host.</param>
        /// <param name="directives">The directives.</param>
        public virtual void Initialize(RazorPageHost razorHost, IDictionary<string, string> directives)
        {
            // do nothing
        }

        /// <summary>Process the generated code.</summary>
        ///
        /// <param name="codeCompileUnit">   The code compile unit.</param>
        /// <param name="generatedNamespace">The generated namespace.</param>
        /// <param name="generatedClass">    The generated class.</param>
        /// <param name="executeMethod">     The execute method.</param>
        public virtual void ProcessGeneratedCode(CodeCompileUnit codeCompileUnit, CodeNamespace generatedNamespace, CodeTypeDeclaration generatedClass, CodeMemberMethod executeMethod)
        {
            // do nothing.
        }

        /// <summary>Process the output described by codeContent.</summary>
        ///
        /// <param name="codeContent">The code content.</param>
        ///
        /// <returns>A string.</returns>
        public virtual string ProcessOutput(string codeContent)
        {
            return codeContent;
        }
    }
}
