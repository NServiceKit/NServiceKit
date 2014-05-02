
namespace NServiceKit.Html
{
    /// <summary>A HTML helper.</summary>
    /// <typeparam name="TModel">Type of the model.</typeparam>
	public class HtmlHelper<TModel> : HtmlHelper
	{
        /// <summary>Gets information describing the view.</summary>
        ///
        /// <value>Information describing the view.</value>
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
