using System.Web;

namespace NServiceKit.Html
{
    /// <summary>An URL helper.</summary>
	public class UrlHelper
	{
        /// <summary>Contents the given document.</summary>
        ///
        /// <param name="url">URL of the document.</param>
        ///
        /// <returns>A string.</returns>
		public string Content(string url)
		{
		    return VirtualPathUtility.ToAbsolute(url);
		}
	}
}