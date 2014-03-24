using System.Collections.Generic;
using NServiceKit.WebHost.Endpoints;

namespace NServiceKit.Html
{
	public interface ITemplatePage
	{
        IViewEngine ViewEngine { get; set; }
        IAppHost AppHost { get; set; }
		T Get<T>();
		Dictionary<string, object> ScopeArgs { get; set; }
	}
}