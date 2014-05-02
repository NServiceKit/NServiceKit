using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using NServiceKit.Razor.Compilation.CodeTransformers;

namespace NServiceKit.Razor.Managers.RazorGen
{
//    [Export("Template", typeof(IRazorCodeTransformer))]
    /// <summary>A template code transformer.</summary>
    public class TemplateCodeTransformer : AggregateCodeTransformer
    {
        private const string GenerationEnvironmentPropertyName = "GenerationEnvironment";
        private static readonly IEnumerable<string> _defaultImports = new[] {
            "System",
            "System.Collections.Generic",
            "System.Linq",
            "System.Text"
        };
        private readonly RazorCodeTransformerBase[] _codeTransforms = new RazorCodeTransformerBase[] {
            new SetImports(_defaultImports, replaceExisting: true),
            new AddGeneratedClassAttribute(),
            new DirectivesBasedTransformers(),
            new SetBaseType("RazorGenerator.Templating.RazorTemplateBase"),
        };

        /// <summary>Gets the code transformers.</summary>
        ///
        /// <value>The code transformers.</value>
        protected override IEnumerable<RazorCodeTransformerBase> CodeTransformers
        {
            get { return _codeTransforms; }
        }

        /// <summary>Process the generated code.</summary>
        ///
        /// <param name="codeCompileUnit">   The code compile unit.</param>
        /// <param name="generatedNamespace">The generated namespace.</param>
        /// <param name="generatedClass">    The generated class.</param>
        /// <param name="executeMethod">     The execute method.</param>
        public override void ProcessGeneratedCode(CodeCompileUnit codeCompileUnit, CodeNamespace generatedNamespace, CodeTypeDeclaration generatedClass, CodeMemberMethod executeMethod)
        {
            base.ProcessGeneratedCode(codeCompileUnit, generatedNamespace, generatedClass, executeMethod);
            generatedClass.IsPartial = true;
            // The generated class has a constructor in there by default.
            generatedClass.Members.Remove(generatedClass.Members.OfType<CodeConstructor>().SingleOrDefault());
        }
    }
}
