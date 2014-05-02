using System.IO;
using NServiceKit.Html;
using NServiceKit.Text;

namespace NServiceKit.Razor
{
    /// <summary>A razor page extensions.</summary>
    public static class RazorPageExtensions
    {
         /// <summary>An IRazorView extension method that renders the section to HTML.</summary>
         ///
         /// <param name="razorView">  The razorView to act on.</param>
         /// <param name="sectionName">Name of the section.</param>
         ///
         /// <returns>A string.</returns>
         public static string RenderSectionToHtml(this IRazorView razorView, string sectionName)
         {
             using (var ms = new MemoryStream())
             using (var writer = new StreamWriter(ms))
             {
                 razorView.RenderSection(sectionName, writer);
                 writer.Flush();
                 return ms.ToArray().FromUtf8Bytes();
             }
         }
    }
}