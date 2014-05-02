using System.Web;
using System.Web.Routing;
using NServiceKit.MiniProfiler;
using NServiceKit.MiniProfiler.UI;
using NServiceKit.Text;
//using IHtmlString = NServiceKit.MiniProfiler.IHtmlString;

namespace NServiceKit.Mvc.MiniProfiler
{
    /// <summary>A mini profiler route handler.</summary>
    public class MiniProfilerRouteHandler : IRouteHandler
    {
        /// <summary>Initializes a new instance of the NServiceKit.Mvc.MiniProfiler.MiniProfilerRouteHandler class.</summary>
        ///
        /// <param name="miniProfilerHandler">The mini profiler handler.</param>
    	public MiniProfilerRouteHandler(MiniProfilerHandler miniProfilerHandler)
    	{
    		MiniProfilerHandler = miniProfilerHandler;
    	}

        /// <summary>Gets or sets the mini profiler handler.</summary>
        ///
        /// <value>The mini profiler handler.</value>
    	public MiniProfilerHandler MiniProfilerHandler { get; set; }

        /// <summary>Provides the object that processes the request.</summary>
        ///
        /// <param name="requestContext">An object that encapsulates information about the request.</param>
        ///
        /// <returns>An object that processes the request.</returns>
    	public IHttpHandler GetHttpHandler(RequestContext requestContext)
    	{
    		return MiniProfilerHandler;
    	}
    }

    /// <summary>A mini profiler.</summary>
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

        /// <summary>Renders the includes.</summary>
        ///
        /// <param name="position">            The position.</param>
        /// <param name="showTrivial">         The show trivial.</param>
        /// <param name="showTimeWithChildren">The show time with children.</param>
        /// <param name="maxTracesToShow">     The maximum traces to show.</param>
        /// <param name="xhtml">               true to xhtml.</param>
        /// <param name="showControls">        The show controls.</param>
        ///
        /// <returns>A System.Web.IHtmlString.</returns>
		public static System.Web.IHtmlString RenderIncludes(RenderPosition? position = null, bool? showTrivial = null, bool? showTimeWithChildren = null, int? maxTracesToShow = null, bool xhtml = false, bool? showControls = null)
		{
			var path = VirtualPathUtility.ToAbsolute("~");
			return MiniProfilerHandler.RenderIncludes(Profiler.Current, position, showTrivial, showTimeWithChildren, maxTracesToShow, xhtml, showControls, path)
				.ToMvcHtmlString();
		}		 

        /// <summary>A NServiceKit.MiniProfiler.IHtmlString extension method that converts a htmlString to a mvc HTML string.</summary>
        ///
        /// <param name="htmlString">The HTML string.</param>
        ///
        /// <returns>htmlString as a System.Web.Mvc.MvcHtmlString.</returns>
		public static System.Web.Mvc.MvcHtmlString ToMvcHtmlString(this NServiceKit.MiniProfiler.IHtmlString htmlString)
		{
			return System.Web.Mvc.MvcHtmlString.Create(htmlString.ToString());
		}

	}
}