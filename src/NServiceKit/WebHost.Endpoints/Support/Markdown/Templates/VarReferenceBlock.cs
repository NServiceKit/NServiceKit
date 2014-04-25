using System.Collections.Generic;
using System.IO;
using NServiceKit.Markdown;

namespace NServiceKit.WebHost.Endpoints.Support.Markdown.Templates
{
    /// <summary>
    /// 
    /// </summary>
    public class VarReferenceBlock : TemplateBlock
	{
		private readonly string varName;

        /// <summary>
        /// Initializes a new instance of the <see cref="VarReferenceBlock"/> class.
        /// </summary>
        /// <param name="varName">Name of the variable.</param>
		public VarReferenceBlock(string varName)
		{
			this.varName = varName;
		}

		public override void Write(MarkdownViewBase instance, TextWriter textWriter, Dictionary<string, object> scopeArgs)
		{
			object value = null;
			scopeArgs.TryGetValue(varName, out value);

			if (value == null)
				return;

			textWriter.Write(value);
		}
	}
}