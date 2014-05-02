using NServiceKit.Razor;
using NServiceKit.ServiceHost.Tests.AppData;

namespace NServiceKit.ServiceHost.Tests.Formats_Razor
{
    /// <summary>A custom razor base page.</summary>
    /// <typeparam name="TModel">Type of the model.</typeparam>
	public abstract class CustomRazorBasePage<TModel> : ViewPage<TModel> where TModel : class
	{
        /// <summary>Describes the format to use.</summary>
		public FormatHelpers Fmt = new FormatHelpers();
        /// <summary>The nwnd.</summary>
		public NorthwindHelpers Nwnd = new NorthwindHelpers();
	}
}
