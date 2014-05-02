// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Linq.Expressions;

namespace NServiceKit.Html
{
    /// <summary>A display text extensions.</summary>
	public static class DisplayTextExtensions
	{
        /// <summary>A HtmlHelper extension method that displays a text.</summary>
        ///
        /// <param name="html">The HTML to act on.</param>
        /// <param name="name">The name.</param>
        ///
        /// <returns>A MvcHtmlString.</returns>
		public static MvcHtmlString DisplayText(this HtmlHelper html, string name)
		{
			return DisplayTextHelper(ModelMetadata.FromStringExpression(name, html.ViewData));
		}

        /// <summary>A HtmlHelper&lt;TModel&gt; extension method that displays a text for.</summary>
        ///
        /// <typeparam name="TModel"> Type of the model.</typeparam>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="html">      The HTML to act on.</param>
        /// <param name="expression">The expression.</param>
        ///
        /// <returns>A MvcHtmlString.</returns>
		public static MvcHtmlString DisplayTextFor<TModel, TResult>(this HtmlHelper<TModel> html, Expression<Func<TModel, TResult>> expression)
		{
			return DisplayTextHelper(ModelMetadata.FromLambdaExpression(expression, html.ViewData));
		}

		private static MvcHtmlString DisplayTextHelper(ModelMetadata metadata)
		{
			return MvcHtmlString.Create(metadata.SimpleDisplayText);
		}
	}
}
