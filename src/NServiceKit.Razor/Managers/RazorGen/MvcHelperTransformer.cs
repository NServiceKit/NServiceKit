using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using NServiceKit.Razor.Compilation;
using NServiceKit.Razor.Compilation.CodeTransformers;

namespace NServiceKit.Razor.Managers.RazorGen
{
//    [Export("MvcHelper", typeof(IRazorCodeTransformer))]
    /// <summary>A mvc helper transformer.</summary>
    public class MvcHelperTransformer : AggregateCodeTransformer
    {
        private const string WriteToMethodName = "WriteTo";
        private const string WriteLiteralToMethodName = "WriteLiteralTo";
        private readonly RazorCodeTransformerBase[] _transformers = new RazorCodeTransformerBase[] {
            new SetImports(MvcViewTransformer.MvcNamespaces, replaceExisting: false),
            new AddGeneratedClassAttribute(),
            new DirectivesBasedTransformers(),
            new MakeTypeHelper(),
            new RemoveLineHiddenPragmas(),
            new MvcWebConfigTransformer(),
        };

        /// <summary>Gets the code transformers.</summary>
        ///
        /// <value>The code transformers.</value>
        protected override IEnumerable<RazorCodeTransformerBase> CodeTransformers
        {
            get { return _transformers; }
        }

        /// <summary>Initializes this object.</summary>
        ///
        /// <param name="razorHost"> The razor host.</param>
        /// <param name="directives">The directives.</param>
        public override void Initialize(RazorPageHost razorHost, IDictionary<string, string> directives)
        {
            base.Initialize(razorHost, directives);
            //razorHost.DefaultBaseClass = typeof(System.Web.WebPages.HelperPage).FullName;

            //razorHost.GeneratedClassContext = new GeneratedClassContext(
            //        executeMethodName: GeneratedClassContext.DefaultExecuteMethodName,
            //        writeMethodName: GeneratedClassContext.DefaultWriteMethodName,
            //        writeLiteralMethodName: GeneratedClassContext.DefaultWriteLiteralMethodName,
            //        writeToMethodName: WriteToMethodName,
            //        writeLiteralToMethodName: WriteLiteralToMethodName,
            //        templateTypeName: typeof(System.Web.WebPages.HelperResult).FullName,
            //        defineSectionMethodName: "DefineSection"
            //);
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

            // Run the base processing
            base.ProcessGeneratedCode(codeCompileUnit, generatedNamespace, generatedClass, executeMethod);

            // Remove the constructor 
            generatedClass.Members.Remove(generatedClass.Members.OfType<CodeConstructor>().SingleOrDefault());
        }
    }
}
