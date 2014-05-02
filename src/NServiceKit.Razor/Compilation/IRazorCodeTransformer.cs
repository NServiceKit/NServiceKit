using System.CodeDom;
using System.Collections.Generic;

namespace NServiceKit.Razor.Compilation
{
    /// <summary>Interface for razor code transformer.</summary>
    public interface IRazorCodeTransformer
    {
        /// <summary>Initializes this object.</summary>
        ///
        /// <param name="razorHost"> The razor host.</param>
        /// <param name="directives">The directives.</param>
        void Initialize(IRazorHost razorHost, IDictionary<string, string> directives);

        /// <summary>Process the generated code.</summary>
        ///
        /// <param name="codeCompileUnit">   The code compile unit.</param>
        /// <param name="generatedNamespace">The generated namespace.</param>
        /// <param name="generatedClass">    The generated class.</param>
        /// <param name="executeMethod">     The execute method.</param>
        void ProcessGeneratedCode(CodeCompileUnit codeCompileUnit, CodeNamespace generatedNamespace, CodeTypeDeclaration generatedClass, CodeMemberMethod executeMethod);

        /// <summary>Process the output described by codeContent.</summary>
        ///
        /// <param name="codeContent">The code content.</param>
        ///
        /// <returns>A string.</returns>
        string ProcessOutput(string codeContent);
    }
}
