using System.Web;
using System.Web.Routing;
using NServiceKit.MiniProfiler;
using NServiceKit.MiniProfiler.UI;
using NServiceKit.Text;
//using IHtmlString = NServiceKit.MiniProfiler.IHtmlString;

namespace NServiceKit.Mvc.MiniProfiler
{
    public class MiniProfilerRouteHandler : IRouteHandler
    {
    	public MiniProfilerRouteHandler(MiniProfilerHandler miniProfilerHandler)
    	{
    		MiniProfilerHandler = miniProfilerHandler;
    	}

    	public MiniProfilerHandler MiniProfilerHandler { get; set; }

    	public IHttpHandler GetHttpHandler(RequestContext requestContext)
    	{
    		return MiniProfilerHandler;
    	}
    }

	public static class MiniProfiler
	{
		internal static void RegisterRoutes()
		{
			var urls = new[] 
		    { 
		        "ssr-jquip.all", 
		        "ssr-includes.js", 
		        "ssr-includes.css", 
		        "ssr-includes.tmpl", 
		        "ssr-results"
		    };
			var routes = RouteTable.Routes;
			var handler = new MiniProfilerRouteHandler(new MiniProfilerHandler());
			var prefix = (Profiler.Settings.RouteBasePath ?? "").Replace("~/", "").WithTrailingSlash();

			using (routes.GetWriteLock())
			{
				foreach (var url in urls)
				{
					var route = new Route(prefix + url, handler) {
						// we have to specify these, so no MVC route helpers will match, e.g. @Html.ActionLink("Home", "Index", "Home")
						Defaults = new RouteValueDictionary(new { controller = "MiniProfilerHandler", action = "ProcessRequest" })
					};

					// put our routes at the beginning, like a boss
					routes.Insert(0, route);
				}
			}
		}

		public static System.Web.IHtmlString RenderIncludes(RenderPosition? position = null, bool? showTrivial = null, bool? showTimeWithChildren = null, int? maxTracesToShow = null, bool xhtml = false, bool? showControls = null)
		{
			var path = VirtualPathUtility.ToAbsolute("~");
			return MiniProfilerHandler.RenderIncludes(Profiler.Current, position, showTrivial, showTimeWithChildren, maxTracesToShow, xhtml, showControls, path)
				.ToMvcHtmlString();
		}		 

		public static System.Web.Mvc.MvcHtmlString ToMvcHtmlString(this NServiceKit.MiniProfiler.IHtmlString htmlString)
		{
			return System.Web.Mvc.MvcHtmlString.Create(htmlString.ToString());
		}

	}
}