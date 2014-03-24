using System.Collections.Generic;
using System.IO;
using NServiceKit.Markdown;

namespace NServiceKit.WebHost.Endpoints.Support.Markdown
{
	public interface ITemplateWriter
	{
		void Write(MarkdownViewBase instance, TextWriter textWriter, Dictionary<string, object> scopeArgs);
	}
}