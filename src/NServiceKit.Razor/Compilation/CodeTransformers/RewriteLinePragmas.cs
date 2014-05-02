namespace NServiceKit.Razor.Compilation.CodeTransformers
{
    internal sealed class RewriteLinePragmas : RazorCodeTransformerBase
    {
        private string _binRelativePath;
        private string _fullPath;

        /// <summary>Initializes this object.</summary>
        ///
        /// <param name="razorHost"> The razor host.</param>
        /// <param name="directives">The directives.</param>
        public override void Initialize(RazorPageHost razorHost, System.Collections.Generic.IDictionary<string, string> directives)
        {
            _binRelativePath = @"..\.." + razorHost.File.VirtualPath;
            _fullPath = razorHost.File.RealPath;
        }

        /// <summary>Process the output described by codeContent.</summary>
        ///
        /// <param name="codeContent">The code content.</param>
        ///
        /// <returns>A string.</returns>
        public override string ProcessOutput(string codeContent)
        {
            return codeContent.Replace("\"" + _fullPath + "\"", "\"" + _binRelativePath + "\"");
        }
    }
}
