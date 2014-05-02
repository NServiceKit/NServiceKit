using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Razor;
using System.Web.Razor.Generator;
using System.Web.Razor.Parser;
using System.Web.Razor.Parser.SyntaxTree;
using System.Web.Razor.Text;
using NServiceKit.Common.Extensions;
using NServiceKit.DataAnnotations;
using NServiceKit.Html;
using NServiceKit.IO;
using NServiceKit.Logging;
using NServiceKit.MiniProfiler;
using NServiceKit.Text;

namespace NServiceKit.Razor.Compilation
{
    /// <summary>A razor page host.</summary>
    public class RazorPageHost : RazorEngineHost, IRazorHost
    {
        private static ILog log = LogManager.GetLogger(typeof(RazorPageHost));

        private static readonly IEnumerable<string> _defaultImports = new[] {
            "System",
            "System.Collections.Generic",
            "System.IO",
            "System.Linq",
            "System.Net",
            "System.Text",
            "NServiceKit.Text",
            "NServiceKit.Html",
        };

        private readonly IRazorCodeTransformer _codeTransformer;
        private readonly CodeDomProvider _codeDomProvider;
        private readonly IDictionary<string, string> _directives;
        private string _defaultClassName;

        /// <summary>Gets the path provider.</summary>
        ///
        /// <value>The path provider.</value>
        public IVirtualPathProvider PathProvider { get; protected set; }

        /// <summary>Gets the file.</summary>
        ///
        /// <value>The file.</value>
        public IVirtualFile File { get; protected set; }

        /// <summary>Initializes a new instance of the NServiceKit.Razor.Compilation.RazorPageHost class.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="pathProvider">   The path provider.</param>
        /// <param name="file">           The file.</param>
        /// <param name="codeTransformer">The code transformer.</param>
        /// <param name="codeDomProvider">The code dom provider.</param>
        /// <param name="directives">     The directives.</param>
        public RazorPageHost(IVirtualPathProvider pathProvider,
                              IVirtualFile file,
                              IRazorCodeTransformer codeTransformer,
                              CodeDomProvider codeDomProvider,
                              IDictionary<string, string> directives)
            : base(new CSharpRazorCodeLanguage())
        {
            this.PathProvider = pathProvider;
            this.File = file;

            if (codeTransformer == null)
            {
                throw new ArgumentNullException("codeTransformer");
            }
            if (this.PathProvider == null)
            {
                throw new ArgumentNullException("pathProvider");
            }
            if (this.File == null)
            {
                throw new ArgumentNullException("file");
            }
            if (codeDomProvider == null)
            {
                throw new ArgumentNullException("codeDomProvider");
            }
            _codeTransformer = codeTransformer;
            _codeDomProvider = codeDomProvider;
            _directives = directives;
            base.DefaultNamespace = "ASP";
            EnableLinePragmas = true;

            base.GeneratedClassContext = new GeneratedClassContext(
                executeMethodName: GeneratedClassContext.DefaultExecuteMethodName,
                writeMethodName: GeneratedClassContext.DefaultWriteMethodName,
                writeLiteralMethodName: GeneratedClassContext.DefaultWriteLiteralMethodName,
                writeToMethodName: "WriteTo",
                writeLiteralToMethodName: "WriteLiteralTo",
                templateTypeName: typeof(HelperResult).FullName,
                defineSectionMethodName: "DefineSection",
                beginContextMethodName: "BeginContext",
                endContextMethodName: "EndContext"
                )
                {
                    ResolveUrlMethodName = "Href",
                };

            base.DefaultBaseClass = typeof(ViewPage).FullName;
            foreach (var import in _defaultImports)
            {
                base.NamespaceImports.Add(import);
            }
        }

        /// <summary>Gets or sets the default class name.</summary>
        ///
        /// <value>The default class name.</value>
        public override string DefaultClassName
        {
            get
            {
                return _defaultClassName ?? GetClassName();
            }
            set
            {
                if (!String.Equals(value, "__CompiledTemplate", StringComparison.OrdinalIgnoreCase))
                {
                    //  By default RazorEngineHost assigns the name __CompiledTemplate. We'll ignore this assignment
                    _defaultClassName = value;
                }
            }
        }

        /// <summary>Gets or sets the parser.</summary>
        ///
        /// <value>The parser.</value>
        public ParserBase Parser { get; set; }

        /// <summary>Gets or sets the code generator.</summary>
        ///
        /// <value>The code generator.</value>
        public RazorCodeGenerator CodeGenerator { get; set; }

        /// <summary>Gets or sets a value indicating whether the line pragmas is enabled.</summary>
        ///
        /// <value>true if enable line pragmas, false if not.</value>
        public bool EnableLinePragmas { get; set; }

        /// <summary>Gets the generate.</summary>
        ///
        /// <exception cref="HttpParseException">Thrown when a HTTP Parse error condition occurs.</exception>
        ///
        /// <returns>The GeneratorResults.</returns>
        public GeneratorResults Generate()
        {
            lock (this)
            {
                _codeTransformer.Initialize(this, _directives);

                // Create the engine
                var engine = new RazorTemplateEngine(this);

                // Generate code 
                GeneratorResults results = null;
                try
                {
                    using (var stream = File.OpenRead())
                    using (var reader = new StreamReader(stream, Encoding.Default, detectEncodingFromByteOrderMarks: true))
                    {
                        results = engine.GenerateCode(reader, className: DefaultClassName, rootNamespace: DefaultNamespace, sourceFileName: this.File.RealPath);
                    }
                }
                catch (Exception e)
                {
                    throw new HttpParseException(e.Message, e, this.File.VirtualPath, null, 1);
                }

                //Throw the first parser message to generate the YSOD
                //TODO: Is there a way to output all errors at once?
                if  (results.ParserErrors.Count > 0)
                {
                    var error = results.ParserErrors[0];
                    throw new HttpParseException(error.Message, null, this.File.VirtualPath, null, error.Location.LineIndex + 1);
                }

                return results;
            }
        }

        /// <summary>The debug source files.</summary>
        public Dictionary<string, string> DebugSourceFiles = new Dictionary<string, string>();

        /// <summary>Gets the compile.</summary>
        ///
        /// <exception cref="HttpCompileException">Thrown when a HTTP Compile error condition occurs.</exception>
        ///
        /// <returns>A Type.</returns>
        public Type Compile()
        {
            Type forceLoadOfRuntimeBinder = typeof(Microsoft.CSharp.RuntimeBinder.Binder);
            if (forceLoadOfRuntimeBinder == null)
            {
                log.Warn("Force load of .NET 4.0+ RuntimeBinder in Microsoft.CSharp.dll");
            }

            var razorResults = Generate();

            var @params = new CompilerParameters
                {
                    GenerateInMemory = true,
                    GenerateExecutable = false,
                    IncludeDebugInformation = false,
                    CompilerOptions = "/target:library /optimize",
                    TempFiles = { KeepFiles = true }
                };

            var assemblies = CompilerServices
                .GetLoadedAssemblies()
                .Where(a => !a.IsDynamic)
                .Select(a => a.Location)
                .ToArray();

            @params.ReferencedAssemblies.AddRange(assemblies);

            //Compile the code
            var results = _codeDomProvider.CompileAssemblyFromDom(@params, razorResults.GeneratedCode);

            var tempFilesMarkedForDeletion = new TempFileCollection(null); 
            @params.TempFiles
                   .OfType<string>()
                   .ForEach(file => tempFilesMarkedForDeletion.AddFile(file, false));

            using (tempFilesMarkedForDeletion)
            {
                if (results.Errors != null && results.Errors.HasErrors)
                {
                    //check if source file exists, read it.
                    //HttpCompileException is sealed by MS. So, we'll
                    //just add a property instead of inheriting from it.
                    var sourceFile = results.Errors
                                            .OfType<CompilerError>()
                                            .First(ce => !ce.IsWarning)
                                            .FileName;

                    var sourceCode = "";
                    if (!string.IsNullOrEmpty(sourceFile) && System.IO.File.Exists(sourceFile))
                    {
                        sourceCode = System.IO.File.ReadAllText(sourceFile);
                    }
                    else
                    {
                        foreach (string tempFile in @params.TempFiles)
                        {
                            if (tempFile.EndsWith(".cs"))
                            {
                                sourceCode = System.IO.File.ReadAllText(tempFile);
                            }
                        }
                    }
                    throw new HttpCompileException(results, sourceCode);
                }

#if DEBUG
                foreach (string tempFile in @params.TempFiles)
                {
                    if (tempFile.EndsWith(".cs"))
                    {
                        var sourceCode = System.IO.File.ReadAllText(tempFile);
                        //sourceCode.Print();
                    }
                }
#endif

                return results.CompiledAssembly.GetTypes().First();
            }
        }

        /// <summary>Generates a source code.</summary>
        ///
        /// <returns>The source code.</returns>
        public string GenerateSourceCode()
        {
            var razorResults = Generate();

            using (var writer = new StringWriter())
            {
                var options = new CodeGeneratorOptions
                    {
                        BlankLinesBetweenMembers = false,
                        BracingStyle = "C"
                    };

                //Generate the code
                writer.WriteLine("#pragma warning disable 1591");
                _codeDomProvider.GenerateCodeFromCompileUnit(razorResults.GeneratedCode, writer, options);
                writer.WriteLine("#pragma warning restore 1591");

                writer.Flush();

                // Perform output transformations and return
                string codeContent = writer.ToString();
                codeContent = _codeTransformer.ProcessOutput(codeContent);
                return codeContent;
            }
        }

        /// <summary>Posts the process generated code.</summary>
        ///
        /// <param name="context">The context.</param>
        public override void PostProcessGeneratedCode(CodeGeneratorContext context)
        {
            _codeTransformer.ProcessGeneratedCode(context.CompileUnit, context.Namespace, context.GeneratedClass, context.TargetMethod);
        }

        /// <summary>Decorate code generator.</summary>
        ///
        /// <param name="incomingCodeGenerator">The incoming code generator.</param>
        ///
        /// <returns>A RazorCodeGenerator.</returns>
        public override RazorCodeGenerator DecorateCodeGenerator(RazorCodeGenerator incomingCodeGenerator)
        {
            var codeGenerator = CodeGenerator ?? base.DecorateCodeGenerator(incomingCodeGenerator);
            codeGenerator.GenerateLinePragmas = EnableLinePragmas;
            return codeGenerator;
        }

        /// <summary>Gets class name.</summary>
        ///
        /// <returns>The class name.</returns>
        protected virtual string GetClassName()
        {
            string filename = Path.GetFileNameWithoutExtension(this.File.VirtualPath);
            return "__" + ParserHelpers.SanitizeClassName(filename);
        }

        /// <summary>Decorate code parser.</summary>
        ///
        /// <param name="incomingCodeParser">The incoming code parser.</param>
        ///
        /// <returns>A ParserBase.</returns>
        public override ParserBase DecorateCodeParser(ParserBase incomingCodeParser)
        {
            if (incomingCodeParser is System.Web.Razor.Parser.CSharpCodeParser)
                return new NServiceKitCSharpCodeParser();

            return base.DecorateCodeParser(incomingCodeParser);
        }
    }

    /// <summary>A service kit c sharp razor code generator.</summary>
    public class NServiceKitCSharpRazorCodeGenerator : CSharpRazorCodeGenerator
    {
        private const string DefaultModelTypeName = "dynamic";
        private const string HiddenLinePragma = "#line hidden";

        /// <summary>Initializes a new instance of the NServiceKit.Razor.Compilation.NServiceKitCSharpRazorCodeGenerator class.</summary>
        ///
        /// <param name="className">        Name of the class.</param>
        /// <param name="rootNamespaceName">Name of the root namespace.</param>
        /// <param name="sourceFileName">   Filename of the source file.</param>
        /// <param name="host">             The host.</param>
        public NServiceKitCSharpRazorCodeGenerator(string className, string rootNamespaceName, string sourceFileName, RazorEngineHost host)
            : base(className, rootNamespaceName, sourceFileName, host)
        {
        }

        /// <summary>Initializes this object.</summary>
        ///
        /// <param name="context">The context.</param>
        protected override void Initialize(CodeGeneratorContext context)
        {
            base.Initialize(context);

            context.GeneratedClass.Members.Insert(0, new CodeSnippetTypeMember(HiddenLinePragma));
        }
    }

    /// <summary>A service kit c sharp code parser.</summary>
    public class NServiceKitCSharpCodeParser : System.Web.Razor.Parser.CSharpCodeParser
    {
        private const string ModelKeyword = "model";
        private const string GenericTypeFormatString = "{0}<{1}>";
        private SourceLocation? _endInheritsLocation;
        private bool _modelStatementFound;

        /// <summary>Initializes a new instance of the NServiceKit.Razor.Compilation.NServiceKitCSharpCodeParser class.</summary>
        public NServiceKitCSharpCodeParser()
        {
            MapDirectives(ModelDirective, ModelKeyword);
        }

        /// <summary>Inherits directive.</summary>
        protected override void InheritsDirective()
        {
            // Verify we're on the right keyword and accept
            AssertDirective(SyntaxConstants.CSharp.InheritsKeyword);
            AcceptAndMoveNext();
            _endInheritsLocation = CurrentLocation;

            InheritsDirectiveCore();
            CheckForInheritsAndModelStatements();
        }

        private void CheckForInheritsAndModelStatements()
        {
            if (_modelStatementFound && _endInheritsLocation.HasValue)
            {
                Context.OnError(_endInheritsLocation.Value, String.Format(CultureInfo.CurrentCulture, MvcResources.MvcRazorCodeParser_CannotHaveModelAndInheritsKeyword, ModelKeyword));
            }
        }

        /// <summary>Model directive.</summary>
        protected virtual void ModelDirective()
        {
            // Verify we're on the right keyword and accept
            AssertDirective(ModelKeyword);
            AcceptAndMoveNext();

            SourceLocation endModelLocation = CurrentLocation;

            BaseTypeDirective(string.Format(CultureInfo.CurrentCulture,
                              MvcResources.MvcRazorCodeParser_ModelKeywordMustBeFollowedByTypeName, ModelKeyword),
                CreateModelCodeGenerator);

            if (_modelStatementFound)
            {
                Context.OnError(endModelLocation, String.Format(CultureInfo.CurrentCulture,
                    MvcResources.MvcRazorCodeParser_OnlyOneModelStatementIsAllowed, ModelKeyword));
            }

            _modelStatementFound = true;

            CheckForInheritsAndModelStatements();
        }

        private SpanCodeGenerator CreateModelCodeGenerator(string model)
        {
            return new SetModelTypeCodeGenerator(model, GenericTypeFormatString);
        }

        /// <summary>Layout directive.</summary>
        protected override void LayoutDirective()
        {
            AssertDirective(SyntaxConstants.CSharp.LayoutKeyword);
            AcceptAndMoveNext();
            BaseTypeDirective(MvcResources.MvcRazorCodeParser_OnlyOneModelStatementIsAllowed.Fmt("layout"), CreateLayoutCodeGenerator);
        }

        private SpanCodeGenerator CreateLayoutCodeGenerator(string layoutPath)
        {
            return new SetLayoutCodeGenerator(layoutPath);
        }

        /// <summary>A set layout code generator.</summary>
        public class SetLayoutCodeGenerator : SpanCodeGenerator
        {
            /// <summary>Initializes a new instance of the NServiceKit.Razor.Compilation.NServiceKitCSharpCodeParser.SetLayoutCodeGenerator class.</summary>
            ///
            /// <param name="layoutPath">The full pathname of the layout file.</param>
            public SetLayoutCodeGenerator(string layoutPath)
            {
                LayoutPath = layoutPath != null ? layoutPath.Trim(' ', '"') : null;
            }

            /// <summary>Gets or sets the full pathname of the layout file.</summary>
            ///
            /// <value>The full pathname of the layout file.</value>
            public string LayoutPath { get; set; }

            /// <summary>Generates a code.</summary>
            ///
            /// <param name="target"> Target for the.</param>
            /// <param name="context">The context.</param>
            public override void GenerateCode(Span target, CodeGeneratorContext context)
            {
                if (!context.Host.DesignTimeMode && !String.IsNullOrEmpty(context.Host.GeneratedClassContext.LayoutPropertyName))
                {
                    context.GeneratedClass.CustomAttributes.Add(
                        new CodeAttributeDeclaration(typeof(MetaAttribute).FullName,
                                new CodeAttributeArgument(new CodePrimitiveExpression("Layout")),
                                new CodeAttributeArgument(new CodePrimitiveExpression(LayoutPath))
                    ));

                    context.TargetMethod.Statements.Add(
                        new CodeAssignStatement(
                            new CodePropertyReferenceExpression(null, context.Host.GeneratedClassContext.LayoutPropertyName),
                            new CodePrimitiveExpression(LayoutPath)));
                }
            }

            /// <summary>Returns a string that represents the current object.</summary>
            ///
            /// <returns>A string that represents the current object.</returns>
            public override string ToString()
            {
                return "Layout: " + LayoutPath;
            }

            /// <summary>Tests if this object is considered equal to another.</summary>
            ///
            /// <param name="obj">The object to compare to this object.</param>
            ///
            /// <returns>true if the objects are considered equal, false if they are not.</returns>
            public override bool Equals(object obj)
            {
                var other = obj as SetLayoutCodeGenerator;
                return other != null && String.Equals(other.LayoutPath, LayoutPath, StringComparison.Ordinal);
            }

            /// <summary>Returns a hash code for this object.</summary>
            ///
            /// <returns>A hash code for this object.</returns>
            public override int GetHashCode()
            {
                return LayoutPath.GetHashCode();
            }
        }

        internal class SetModelTypeCodeGenerator : SetBaseTypeCodeGenerator
        {
            private readonly string _genericTypeFormat;

            /// <summary>Initializes a new instance of the NServiceKit.Razor.Compilation.NServiceKitCSharpCodeParser.SetModelTypeCodeGenerator class.</summary>
            ///
            /// <param name="modelType">        Type of the model.</param>
            /// <param name="genericTypeFormat">The generic type format.</param>
            public SetModelTypeCodeGenerator(string modelType, string genericTypeFormat)
                : base(modelType)
            {
                _genericTypeFormat = genericTypeFormat;
            }

            /// <summary>Resolve type.</summary>
            ///
            /// <param name="context"> The context.</param>
            /// <param name="baseType">Type of the base.</param>
            ///
            /// <returns>A string.</returns>
            protected override string ResolveType(CodeGeneratorContext context, string baseType)
            {
                var typeString = string.Format(
                    CultureInfo.InvariantCulture, _genericTypeFormat, context.Host.DefaultBaseClass, baseType);
                return typeString;
            }

            /// <summary>Tests if this object is considered equal to another.</summary>
            ///
            /// <param name="obj">The object to compare to this object.</param>
            ///
            /// <returns>true if the objects are considered equal, false if they are not.</returns>
            public override bool Equals(object obj)
            {
                var other = obj as SetModelTypeCodeGenerator;
                return other != null &&
                       base.Equals(obj) &&
                       String.Equals(_genericTypeFormat, other._genericTypeFormat, StringComparison.Ordinal);
            }

            /// <summary>Returns a hash code for this object.</summary>
            ///
            /// <returns>A hash code for this object.</returns>
            public override int GetHashCode()
            {
                return (base.GetHashCode() + _genericTypeFormat).GetHashCode();
            }

            /// <summary>Convert this object into a string representation.</summary>
            ///
            /// <returns>A string that represents this object.</returns>
            public override string ToString()
            {
                return "Model:" + BaseType;
            }
        }
    }

}
