using System.Collections.Generic;
using NServiceKit.Common.Extensions;
using NServiceKit.WebHost.Endpoints;

namespace NServiceKit.Razor.Compilation.CodeTransformers
{
    /// <summary>A web configuration transformer.</summary>
    public class WebConfigTransformer : AggregateCodeTransformer
    {
        private readonly string DefaultBaseType = typeof(ViewPage).FullName;
        private const string RazorWebPagesSectionName = "system.web.webPages.razor/pages";
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
            //read the base type here from the web.config here

            EndpointHostConfig.RazorNamespaces
                              .ForEach(ns => razorHost.NamespaceImports.Add(ns));

            base.Initialize(razorHost, directives);
        }
    }
}