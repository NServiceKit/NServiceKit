using NServiceKit.Razor;
using NServiceKit.ServiceHost.Tests.AppData;

namespace NServiceKit.ServiceHost.Tests.Formats_Razor
{
	public abstract class CustomRazorBasePage<TModel> : ViewPage<TModel> where TModel : class
	{
		public FormatHelpers Fmt = new FormatHelpers();
		public NorthwindHelpers Nwnd = new NorthwindHelpers();
	}
}
