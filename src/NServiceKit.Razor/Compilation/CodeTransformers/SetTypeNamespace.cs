using System.Collections.Generic;

namespace NServiceKit.Razor.Compilation.CodeTransformers
{
    /// <summary>A set type namespace.</summary>
    public class SetTypeNamespace : RazorCodeTransformerBase
    {
        private readonly string _namespace;

        /// <summary>Initializes a new instance of the NServiceKit.Razor.Compilation.CodeTransformers.SetTypeNamespace class.</summary>
        ///
        /// <param name="namespace">The namespace.</param>
        public SetTypeNamespace(string @namespace)
        {
            _namespace = @namespace;
        }

        /// <summary>Initializes this object.</summary>
        ///
        /// <param name="razorHost"> The razor host.</param>
        /// <param name="directives">The directives.</param>
        public override void Initialize(RazorPageHost razorHost, IDictionary<string, string> directives)
        {
            razorHost.DefaultNamespace = _namespace;
        }
    }
}
