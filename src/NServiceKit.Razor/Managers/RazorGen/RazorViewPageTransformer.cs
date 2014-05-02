using System;
using System.Collections.Generic;
using System.Web.Razor.Generator;
using NServiceKit.Razor.Compilation;
using NServiceKit.Razor.Compilation.CodeTransformers;

namespace NServiceKit.Razor.Managers.RazorGen
{
    /// <summary>A razor view page transformer.</summary>
    public class RazorViewPageTransformer : AggregateCodeTransformer
    {
        /// <summary>Initializes a new instance of the NServiceKit.Razor.Managers.RazorGen.RazorViewPageTransformer class.</summary>
        ///
        /// <param name="pageBaseType">Type of the page base.</param>
        public RazorViewPageTransformer(Type pageBaseType)
        {
            this.codeTransformers.Add(new SetBaseType(pageBaseType));
        }

        private static readonly HashSet<string> namespaces = new HashSet<string>()
            {
                "System",
            };

        private readonly List<RazorCodeTransformerBase> codeTransformers = new List<RazorCodeTransformerBase>
            {
                new AddGeneratedClassAttribute(),
                new AddPageVirtualPathAttribute(),
                new SetImports( namespaces, replaceExisting: false ),
                new RemoveLineHiddenPragmas(),
                new MakeTypePartial(),
                new WebConfigTransformer()
            };

        /// <summary>Gets the code transformers.</summary>
        ///
        /// <value>The code transformers.</value>
        protected override IEnumerable<RazorCodeTransformerBase> CodeTransformers
        {
            get { return this.codeTransformers; }
        }

        /// <summary>Initializes this object.</summary>
        ///
        /// <param name="razorHost"> The razor host.</param>
        /// <param name="directives">The directives.</param>
        public override void Initialize(RazorPageHost razorHost, IDictionary<string, string> directives)
        {
            base.Initialize(razorHost, directives);

            var path = razorHost.EnableLinePragmas ? razorHost.File.RealPath : string.Empty;
            razorHost.CodeGenerator = new NServiceKitCSharpRazorCodeGenerator(razorHost.DefaultClassName, razorHost.DefaultNamespace, path, razorHost)
                {
                    GenerateLinePragmas = razorHost.EnableLinePragmas
                };

        }

        /// <summary>Process the generated code.</summary>
        ///
        /// <param name="codeCompileUnit">   The code compile unit.</param>
        /// <param name="generatedNamespace">The generated namespace.</param>
        /// <param name="generatedClass">    The generated class.</param>
        /// <param name="executeMethod">     The execute method.</param>
        public override void ProcessGeneratedCode(System.CodeDom.CodeCompileUnit codeCompileUnit, System.CodeDom.CodeNamespace generatedNamespace, System.CodeDom.CodeTypeDeclaration generatedClass, System.CodeDom.CodeMemberMethod executeMethod)
        {
            base.ProcessGeneratedCode(codeCompileUnit, generatedNamespace, generatedClass, executeMethod);
        }
    }
}