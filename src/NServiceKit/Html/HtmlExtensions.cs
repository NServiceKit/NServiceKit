using NServiceKit.Text;

namespace NServiceKit.Html
{
    /// <summary>A HTML extensions.</summary>
    public static class HtmlExtensions
    {
        /// <summary>A T extension method that converts a model to a raw JSON.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="model">The model to act on.</param>
        ///
        /// <returns>A MvcHtmlString.</returns>
        public static MvcHtmlString AsRawJson<T>(this T model)
        {
            return MvcHtmlString.Create(model.ToJson());
        }

        /// <summary>A T extension method that converts a model to a raw.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="model">The model to act on.</param>
        ///
        /// <returns>A MvcHtmlString.</returns>
        public static MvcHtmlString AsRaw<T>(this T model)
        {
            return MvcHtmlString.Create(
                (model != null ? model : default(T)).ToString());
        }
    }
}