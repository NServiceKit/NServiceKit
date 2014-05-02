using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Razor.Generator;
using NServiceKit.Text;

namespace NServiceKit.Razor.Compilation.CodeTransformers
{
    /// <summary>Attribute for add generated class.</summary>
    public class AddGeneratedClassAttribute : RazorCodeTransformerBase
    {
        /// <summary>Process the generated code.</summary>
        ///
        /// <param name="codeCompileUnit">   The code compile unit.</param>
        /// <param name="generatedNamespace">The generated namespace.</param>
        /// <param name="generatedClass">    The generated class.</param>
        /// <param name="executeMethod">     The execute method.</param>
        public override void ProcessGeneratedCode(CodeCompileUnit codeCompileUnit, CodeNamespace generatedNamespace, CodeTypeDeclaration generatedClass, CodeMemberMethod executeMethod)
        {
            string tool = "RazorGenerator";
            Version version = GetType().Assembly.GetName().Version;
            generatedClass.CustomAttributes.Add(
                new CodeAttributeDeclaration(typeof(System.CodeDom.Compiler.GeneratedCodeAttribute).FullName,
                        new CodeAttributeArgument(new CodePrimitiveExpression(tool)),
                        new CodeAttributeArgument(new CodePrimitiveExpression(version.ToString()))
            ));
        }
    }

    /// <summary>A set imports.</summary>
    public class SetImports : RazorCodeTransformerBase
    {
        private readonly IEnumerable<string> _imports;
        private readonly bool _replaceExisting;

        /// <summary>Initializes a new instance of the NServiceKit.Razor.Compilation.CodeTransformers.SetImports class.</summary>
        ///
        /// <param name="imports">        The imports.</param>
        /// <param name="replaceExisting">true to replace existing.</param>
        public SetImports(IEnumerable<string> imports, bool replaceExisting = false)
        {
            _imports = imports;
            _replaceExisting = replaceExisting;
        }

        /// <summary>Initializes this object.</summary>
        ///
        /// <param name="razorHost"> The razor host.</param>
        /// <param name="directives">The directives.</param>
        public override void Initialize(RazorPageHost razorHost, IDictionary<string, string> directives)
        {
            if (_replaceExisting)
            {
                razorHost.NamespaceImports.Clear();
            }
            foreach (var import in _imports)
            {
                razorHost.NamespaceImports.Add(import);
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
            // Sort imports.
            var imports = new List<CodeNamespaceImport>(generatedNamespace.Imports.OfType<CodeNamespaceImport>());
            generatedNamespace.Imports.Clear();
            generatedNamespace.Imports.AddRange(imports.OrderBy(c => c.Namespace, NamespaceComparer.Instance).ToArray());
        }

        private class NamespaceComparer : IComparer<string>
        {
            /// <summary>The instance.</summary>
            public static readonly NamespaceComparer Instance = new NamespaceComparer();

            /// <summary>Compares two string objects to determine their relative ordering.</summary>
            ///
            /// <param name="x">String to be compared.</param>
            /// <param name="y">String to be compared.</param>
            ///
            /// <returns>Negative if 'x' is less than 'y', 0 if they are equal, or positive if it is greater.</returns>
            public int Compare(string x, string y)
            {
                if (x == null || y == null)
                {
                    return StringComparer.OrdinalIgnoreCase.Compare(x, y);
                }
                bool xIsSystem = x.StartsWith("System", StringComparison.OrdinalIgnoreCase);
                bool yIsSystem = y.StartsWith("System", StringComparison.OrdinalIgnoreCase);

                if (!(xIsSystem ^ yIsSystem))
                {
                    return x.CompareTo(y);
                }
                else if (xIsSystem)
                {
                    return -1;
                }
                return 1;
            }
        }
    }

    /// <summary>A make type static.</summary>
    public class MakeTypeStatic : RazorCodeTransformerBase
    {
        /// <summary>Process the output described by codeContent.</summary>
        ///
        /// <param name="codeContent">The code content.</param>
        ///
        /// <returns>A string.</returns>
        public override string ProcessOutput(string codeContent)
        {
            return codeContent.Replace("public class", "public static class");
        }
    }

    /// <summary>A set base type.</summary>
    public class SetBaseType : RazorCodeTransformerBase
    {
        private const string DefaultModelTypeName = "dynamic";
        private readonly bool isGenericType;

        private readonly string _typeName;

        /// <summary>Initializes a new instance of the NServiceKit.Razor.Compilation.CodeTransformers.SetBaseType class.</summary>
        ///
        /// <param name="typeName">     Name of the type.</param>
        /// <param name="isGenericType">true if this object is generic type.</param>
        public SetBaseType(string typeName, bool isGenericType=true)
        {
            _typeName = typeName.SplitOnLast("`")[0]; //get clean generic name without 'GenericType`1' n args suffix
            this.isGenericType = isGenericType;
        }

        /// <summary>Initializes a new instance of the NServiceKit.Razor.Compilation.CodeTransformers.SetBaseType class.</summary>
        ///
        /// <param name="type">The type.</param>
        public SetBaseType(Type type)
            : this(type.FullName, type.IsGenericType)
        {
        }

        /// <summary>Initializes this object.</summary>
        ///
        /// <param name="razorHost"> The razor host.</param>
        /// <param name="directives">The directives.</param>
        public override void Initialize(RazorPageHost razorHost, IDictionary<string, string> directives)
        {
            base.Initialize(razorHost, directives);

            //string baseClass = razorHost.DefaultBaseClass;
            razorHost.DefaultBaseClass = _typeName;

            // The CSharpRazorCodeGenerator decides to generate line pragmas based on if the file path is available. 
            //Set it to an empty string if we do not want to generate them.

            //var path = razorHost.EnableLinePragmas ? razorHost.File.RealPath : String.Empty;
            //razorHost.CodeGenerator = new CSharpRazorCodeGenerator(razorHost.DefaultClassName, razorHost.DefaultNamespace, path, razorHost)
            //{
            //    GenerateLinePragmas = razorHost.EnableLinePragmas
            //};
            //razorHost.Parser = new NServiceKitCSharpCodeParser();
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
            if (generatedClass.BaseTypes.Count > 0)
            {
                var codeTypeReference = generatedClass.BaseTypes[0];
                if (!codeTypeReference.BaseType.Contains('<') && isGenericType)
                {
                    // Use the default model if it wasn't specified by the user.
                    codeTypeReference.BaseType += '<' + DefaultModelTypeName + '>';
                }
            }
        }
    }

    /// <summary>A make type helper.</summary>
    public class MakeTypeHelper : RazorCodeTransformerBase
    {
        /// <summary>Initializes this object.</summary>
        ///
        /// <param name="razorHost"> The razor host.</param>
        /// <param name="directives">The directives.</param>
        public override void Initialize(RazorPageHost razorHost, IDictionary<string, string> directives)
        {
            razorHost.StaticHelpers = true;
        }

        /// <summary>Process the generated code.</summary>
        ///
        /// <param name="codeCompileUnit">   The code compile unit.</param>
        /// <param name="generatedNamespace">The generated namespace.</param>
        /// <param name="generatedClass">    The generated class.</param>
        /// <param name="executeMethod">     The execute method.</param>
        public override void ProcessGeneratedCode(CodeCompileUnit codeCompileUnit, CodeNamespace generatedNamespace, CodeTypeDeclaration generatedClass, CodeMemberMethod executeMethod)
        {
            generatedClass.Members.Remove(executeMethod);
        }
    }
}
