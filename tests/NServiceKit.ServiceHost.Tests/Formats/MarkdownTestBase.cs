using System.Collections.Generic;
using System.Linq;
using System.Text;
using NServiceKit.ServiceInterface.Testing;
using NServiceKit.VirtualPath;
using NServiceKit.WebHost.Endpoints.Formats;
using NServiceKit.WebHost.Endpoints.Support.Markdown;

namespace NServiceKit.ServiceHost.Tests.Formats
{
    /// <summary>A markdown test base.</summary>
	public class MarkdownTestBase
	{
        /// <summary>Name of the template.</summary>
		public const string TemplateName = "Template";
        /// <summary>Name of the page.</summary>
		protected const string PageName = "Page";

        /// <summary>Creates a new MarkdownFormat.</summary>
        ///
        /// <param name="websiteTemplate">The website template.</param>
        /// <param name="pageTemplate">   The page template.</param>
        ///
        /// <returns>A MarkdownFormat.</returns>
		public MarkdownFormat Create(string websiteTemplate, string pageTemplate)
		{
			var markdownFormat = new MarkdownFormat {
			    VirtualPathProvider = new InMemoryVirtualPathProvider(new BasicAppHost())
            };

            markdownFormat.AddFileAndTemplate("websiteTemplate", websiteTemplate);
			markdownFormat.AddPage(
				new MarkdownPage(markdownFormat, "/path/to/tpl", PageName, pageTemplate) {
                    Template = "websiteTemplate",
				});

			return markdownFormat;
		}

        /// <summary>Creates a new MarkdownFormat.</summary>
        ///
        /// <param name="pageTemplate">The page template.</param>
        ///
        /// <returns>A MarkdownFormat.</returns>
		public MarkdownFormat Create(string pageTemplate)
		{
			var markdownFormat = new MarkdownFormat();
			markdownFormat.AddPage(
				new MarkdownPage(markdownFormat, "/path/to/tpl", PageName, pageTemplate));

			return markdownFormat;
		}

        /// <summary>Renders to HTML.</summary>
        ///
        /// <param name="pageTemplate">The page template.</param>
        /// <param name="scopeArgs">   The scope arguments.</param>
        ///
        /// <returns>A string.</returns>
		public string RenderToHtml(string pageTemplate, Dictionary<string, object> scopeArgs)
		{
			var markdown = Create(pageTemplate);
			var html = markdown.RenderDynamicPageHtml(PageName, scopeArgs);
			return html;
		}

        /// <summary>Renders to HTML.</summary>
        ///
        /// <param name="pageTemplate">   The page template.</param>
        /// <param name="scopeArgs">      The scope arguments.</param>
        /// <param name="websiteTemplate">The website template.</param>
        ///
        /// <returns>A string.</returns>
		public string RenderToHtml(string pageTemplate, Dictionary<string, object> scopeArgs, string websiteTemplate)
		{
			var markdown = Create(pageTemplate);
			var html = markdown.RenderDynamicPageHtml(PageName, scopeArgs);
			return html;
		}

        /// <summary>Renders to HTML.</summary>
        ///
        /// <param name="pageTemplate">The page template.</param>
        /// <param name="model">       The model.</param>
        ///
        /// <returns>A string.</returns>
		public string RenderToHtml(string pageTemplate, object model)
		{
			var markdown = Create(pageTemplate);
			var html = markdown.RenderDynamicPageHtml(PageName, model);
			return html;
		}
	}

    /// <summary>A markdown test extensions.</summary>
	public static class MarkdownTestExtensions
	{
        /// <summary>A string extension method that normalize new lines.</summary>
        ///
        /// <param name="text">The text to act on.</param>
        ///
        /// <returns>A string.</returns>
        public static string NormalizeNewLines(this string text)
        {
            return text.Replace("\r\n", "\n");
        }

        /// <summary>A string extension method that strip lines and whitespace.</summary>
        ///
        /// <param name="text">The text to act on.</param>
        ///
        /// <returns>A string.</returns>
        public static string StripLinesAndWhitespace(this string text)
        {
            var sb = new StringBuilder();
            text.Replace("\r\n", "\n").Split('\n').ToList().ConvertAll(x => x.Trim()).ForEach(x => sb.Append(x));
            return sb.ToString();
        }
    }
}