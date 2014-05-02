using System;
using System.Collections.Generic;

namespace NServiceKit.Razor.Compilation.CodeTransformers
{
    /// <summary>The directives based transformers.</summary>
    public class DirectivesBasedTransformers : AggregateCodeTransformer
    {
        /// <summary>The type visibility key.</summary>
        public static readonly string TypeVisibilityKey = "TypeVisibility";
        /// <summary>The disable line pragmas key.</summary>
        public static readonly string DisableLinePragmasKey = "DisableLinePragmas";
        /// <summary>The trim leading underscores key.</summary>
        public static readonly string TrimLeadingUnderscoresKey = "TrimLeadingUnderscores";
        /// <summary>The generate absolute path line pragmas.</summary>
        public static readonly string GenerateAbsolutePathLinePragmas = "GenerateAbsolutePathLinePragmas";
        /// <summary>The namespace key.</summary>
        public static readonly string NamespaceKey = "Namespace";
        /// <summary>The exclude from code coverage.</summary>
        public static readonly string ExcludeFromCodeCoverage = "ExcludeFromCodeCoverage";
        /// <summary>Filename of the suffix file.</summary>
        public static readonly string SuffixFileName = "ClassSuffix";
        private readonly List<RazorCodeTransformerBase> _transformers = new List<RazorCodeTransformerBase>();

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
            string typeVisibility;
            if (directives.TryGetValue(TypeVisibilityKey, out typeVisibility))
            {
                _transformers.Add(new SetTypeVisibility(typeVisibility));
            }

            string typeNamespace;
            if (directives.TryGetValue(NamespaceKey, out typeNamespace))
            {
                _transformers.Add(new SetTypeNamespace(typeNamespace));
            }

            if (ReadSwitchValue(directives, DisableLinePragmasKey) == true)
            {
                razorHost.EnableLinePragmas = false;
            }
            else if (ReadSwitchValue(directives, GenerateAbsolutePathLinePragmas) != true)
            {
                // Rewrite line pragamas to generate bin relative paths instead of absolute paths.
                _transformers.Add(new RewriteLinePragmas());
            }

            if (ReadSwitchValue(directives, TrimLeadingUnderscoresKey) != false)
            {
                // This should in theory be a different transformer.
                razorHost.DefaultClassName = razorHost.DefaultClassName.TrimStart('_');
            }

            if (ReadSwitchValue(directives, ExcludeFromCodeCoverage) == true)
            {
                _transformers.Add(new ExcludeFromCodeCoverageTransformer());
            }

            string suffix;
            if (directives.TryGetValue(SuffixFileName, out suffix))
            {
                _transformers.Add(new SuffixFileNameTransformer(suffix));
            }

            base.Initialize(razorHost, directives);
        }

        private static bool? ReadSwitchValue(IDictionary<string, string> directives, string key)
        {
            string value;
            bool switchValue;

            if (directives.TryGetValue(key, out value) && Boolean.TryParse(value, out switchValue))
            {
                return switchValue;
            }
            return null;
        }
    }
}
