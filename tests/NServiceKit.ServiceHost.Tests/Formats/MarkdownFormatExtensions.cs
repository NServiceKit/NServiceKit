using NServiceKit.VirtualPath;
using NServiceKit.WebHost.Endpoints.Formats;
using NServiceKit.WebHost.Endpoints.Support.Markdown;

namespace NServiceKit.ServiceHost.Tests.Formats
{
    /// <summary>A markdown format extensions.</summary>
    public static class MarkdownFormatExtensions
    {
        /// <summary>A MarkdownFormat extension method that adds a file and page to 'markdownPage'.</summary>
        ///
        /// <param name="markdown">    The markdown to act on.</param>
        /// <param name="markdownPage">The markdown page.</param>
        public static void AddFileAndPage(this MarkdownFormat markdown, MarkdownPage markdownPage)
        {
            var pathProvider = (InMemoryVirtualPathProvider)markdown.VirtualPathProvider;
            pathProvider.AddFile(markdownPage.FilePath, markdownPage.Contents);
            markdown.AddPage(markdownPage);
        }

        /// <summary>A MarkdownFormat extension method that adds a file and template.</summary>
        ///
        /// <param name="markdown">The markdown to act on.</param>
        /// <param name="filePath">Full pathname of the file.</param>
        /// <param name="contents">The contents.</param>
        public static void AddFileAndTemplate(this MarkdownFormat markdown, string filePath, string contents)
        {
            var pathProvider = (InMemoryVirtualPathProvider)markdown.VirtualPathProvider;
            pathProvider.AddFile(filePath, contents);
            markdown.AddTemplate(filePath, contents);
        }
    }
}