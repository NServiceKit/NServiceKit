using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Web;

namespace NServiceKit.Razor.Compilation.CodeTransformers
{
    /// <summary>Attribute for virtual path.</summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class VirtualPathAttribute : Attribute
    {
        /// <summary>Initializes a new instance of the NServiceKit.Razor.Compilation.CodeTransformers.VirtualPathAttribute class.</summary>
        ///
        /// <param name="virtualPath">The full pathname of the virtual file.</param>
        public VirtualPathAttribute( string virtualPath )
        {
            VirtualPath = virtualPath;
        }

        /// <summary>Gets the full pathname of the virtual file.</summary>
        ///
        /// <value>The full pathname of the virtual file.</value>
        public string VirtualPath { get; private set; }
    }
    /// <summary>Attribute for add page virtual path.</summary>
    public class AddPageVirtualPathAttribute : RazorCodeTransformerBase
    {
        private const string VirtualPathDirectiveKey = "VirtualPath";
        private string _projectRelativePath;
        private string _overriddenVirtualPath;

        /// <summary>Initializes this object.</summary>
        ///
        /// <param name="razorHost"> The razor host.</param>
        /// <param name="directives">The directives.</param>
        public override void Initialize(RazorPageHost razorHost, IDictionary<string, string> directives)
        {
            _projectRelativePath = razorHost.File.VirtualPath;
            directives.TryGetValue(VirtualPathDirectiveKey, out _overriddenVirtualPath);
        }

        /// <summary>Process the generated code.</summary>
        ///
        /// <param name="codeCompileUnit">   The code compile unit.</param>
        /// <param name="generatedNamespace">The generated namespace.</param>
        /// <param name="generatedClass">    The generated class.</param>
        /// <param name="executeMethod">     The execute method.</param>
        public override void ProcessGeneratedCode(CodeCompileUnit codeCompileUnit, CodeNamespace generatedNamespace, CodeTypeDeclaration generatedClass, CodeMemberMethod executeMethod)
        {
            Debug.Assert(_projectRelativePath != null, "Initialize has to be called before we get here.");
            var virtualPath = _overriddenVirtualPath ?? VirtualPathUtility.ToAppRelative("~/" + _projectRelativePath.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));

            generatedClass.CustomAttributes.Add(
                new CodeAttributeDeclaration(typeof(VirtualPathAttribute).FullName,
                new CodeAttributeArgument(new CodePrimitiveExpression(virtualPath))));
        }
    }
}
