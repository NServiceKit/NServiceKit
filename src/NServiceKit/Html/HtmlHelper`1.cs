
namespace NServiceKit.Html
{
	public class HtmlHelper<TModel> : HtmlHelper
	{
		public new ViewDataDictionary<TModel> ViewData
		{
			get 
            { 
                return base.ViewData as ViewDataDictionary<TModel> 
                    ?? new ViewDataDictionary<TModel>((TModel)base.ViewData.Model); 
            }
		}
	}
}
