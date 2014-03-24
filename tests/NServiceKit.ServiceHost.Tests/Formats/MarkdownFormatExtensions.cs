using NServiceKit.VirtualPath;
using NServiceKit.WebHost.Endpoints.Formats;
using NServiceKit.WebHost.Endpoints.Support.Markdown;

namespace NServiceKit.ServiceHost.Tests.Formats
{
    public static class MarkdownFormatExtensions
    {
        public static void AddFileAndPage(this MarkdownFormat markdown, MarkdownPage markdownPage)
        {
            var pathProvider = (InMemoryVirtualPathProvider)markdown.VirtualPathProvider;
            pathProvider.AddFile(markdownPage.FilePath, markdownPage.Contents);
            markdown.AddPage(markdownPage);
        }

        public static void AddFileAndTemplate(this MarkdownFormat markdown, string filePath, string contents)
        {
            var pathProvider = (InMemoryVirtualPathProvider)markdown.VirtualPathProvider;
            pathProvider.AddFile(filePath, contents);
            markdown.AddTemplate(filePath, contents);
        }
    }
}