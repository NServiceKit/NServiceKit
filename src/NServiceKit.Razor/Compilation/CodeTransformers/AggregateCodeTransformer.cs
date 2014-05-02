using System.CodeDom;
using System.Collections.Generic;

namespace NServiceKit.Razor.Compilation.CodeTransformers
{
    /// <summary>An aggregate code transformer.</summary>
    public abstract class AggregateCodeTransformer : RazorCodeTransformerBase
    {
        /// <summary>Gets the code transformers.</summary>
        ///
        /// <value>The code transformers.</value>
        protected abstract IEnumerable<RazorCodeTransformerBase> CodeTransformers
        {
            get;
        }

        /// <summary>Initializes this object.</summary>
        ///
        /// <param name="razorHost"> The razor host.</param>
        /// <param name="directives">The directives.</param>
        public override void Initialize(RazorPageHost razorHost, IDictionary<string, string> directives)
        {
            foreach (var transformer in CodeTransformers)
            {
                transformer.Initialize(razorHost, directives);
            }
        }

        /// <summary>Process the generated code.</summary>
        ///
        /// <param name="codeCompileUnit">   The code compile unit.</param>
        /// <param name="generatedNamespace">The generated namespace.</param>
        /// <param name="generatedClass">    The generated class.</param>
        /// <param name="executeMethod">     The execute method.</param>
        public override void ProcessGeneratedCode(CodeCompileUnit codeCompileUnit, CodeNamespace generatedNamespace, CodeTypeDeclaration generatedClass, CodeMemberMethod executeMethod)
        {
            foreach (var transformer in CodeTransformers)
            {
                transformer.ProcessGeneratedCode(codeCompileUnit, generatedNamespace, generatedClass, executeMethod);
            }
        }

        /// <summary>Process the output described by codeContent.</summary>
        ///
        /// <param name="codeContent">The code content.</param>
        ///
        /// <returns>A string.</returns>
        public override string ProcessOutput(string codeContent)
        {
            foreach (var transformer in CodeTransformers)
            {
                codeContent = transformer.ProcessOutput(codeContent);
            }
            return codeContent;
        }
    }
}
