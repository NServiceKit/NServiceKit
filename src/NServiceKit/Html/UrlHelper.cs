using System.Web;

namespace NServiceKit.Html
{
	public class UrlHelper
	{
		public string Content(string url)
		{
		    return VirtualPathUtility.ToAbsolute(url);
		}
	}
}