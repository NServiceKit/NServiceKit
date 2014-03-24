using System.IO;
using NServiceKit.Html;
using NServiceKit.Text;

namespace NServiceKit.Razor
{
    public static class RazorPageExtensions
    {
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