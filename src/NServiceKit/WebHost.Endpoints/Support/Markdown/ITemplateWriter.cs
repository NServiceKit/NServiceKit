using System.Collections.Generic;
using System.IO;
using NServiceKit.Markdown;

namespace NServiceKit.WebHost.Endpoints.Support.Markdown
{
    /// <summary>Interface for template writer.</summary>
	public interface ITemplateWriter
	{
        /// <summary>Writes.</summary>
        ///
        /// <param name="instance">  The instance.</param>
        /// <param name="textWriter">The text writer.</param>
        /// <param name="scopeArgs"> The scope arguments.</param>
		void Write(MarkdownViewBase instance, TextWriter textWriter, Dictionary<string, object> scopeArgs);
	}
}