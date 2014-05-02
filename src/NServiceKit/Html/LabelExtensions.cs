/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. All rights reserved.
 *
 * This software is subject to the Microsoft Public License (Ms-PL). 
 * A copy of the license can be found in the license.htm file included 
 * in this distribution.
 *
 * You must not remove this notice, or any other, from this software.
 *
 * ***************************************************************************/

using System;
using System.Linq;
using System.Linq.Expressions;
using NServiceKit.Markdown;

namespace NServiceKit.Html
{
    /// <summary>A label extensions.</summary>
	public static class LabelExtensions
	{
        /// <summary>A HtmlHelper extension method that labels.</summary>
        ///
        /// <param name="html">      The HTML to act on.</param>
        /// <param name="expression">The expression.</param>
        ///
        /// <returns>A MvcHtmlString.</returns>
		public static MvcHtmlString Label(this HtmlHelper html, string expression)
		{
			return Label(html, expression, null);
		}

        /// <summary>A HtmlHelper extension method that labels.</summary>
        ///
        /// <param name="html">      The HTML to act on.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="labelText"> The label text.</param>
        ///
        /// <returns>A MvcHtmlString.</returns>
		public static MvcHtmlString Label(this HtmlHelper html, string expression, string labelText)
		{
			return LabelHelper(html,
				ModelMetadata.FromStringExpression(expression, html.ViewData),
				expression,
				labelText);
		}

        /// <summary>A HtmlHelper&lt;TModel&gt; extension method that label for.</summary>
        ///
        /// <typeparam name="TModel">Type of the model.</typeparam>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <param name="html">      The HTML to act on.</param>
        /// <param name="expression">The expression.</param>
        ///
        /// <returns>A MvcHtmlString.</returns>
		public static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
		{
			return LabelFor(html, expression, null);
		}

        /// <summary>A HtmlHelper&lt;TModel&gt; extension method that label for.</summary>
        ///
        /// <typeparam name="TModel">Type of the model.</typeparam>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <param name="html">      The HTML to act on.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="labelText"> The label text.</param>
        ///
        /// <returns>A MvcHtmlString.</returns>
		public static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string labelText)
		{
			return LabelHelper(html,
				ModelMetadata.FromLambdaExpression(expression, html.ViewData),
				ExpressionHelper.GetExpressionText(expression),
				labelText);
		}

        /// <summary>A HtmlHelper extension method that label for model.</summary>
        ///
        /// <param name="html">The HTML to act on.</param>
        ///
        /// <returns>A MvcHtmlString.</returns>
		public static MvcHtmlString LabelForModel(this HtmlHelper html)
		{
			return LabelForModel(html, null);
		}

        /// <summary>A HtmlHelper extension method that label for model.</summary>
        ///
        /// <param name="html">     The HTML to act on.</param>
        /// <param name="labelText">The label text.</param>
        ///
        /// <returns>A MvcHtmlString.</returns>
		public static MvcHtmlString LabelForModel(this HtmlHelper html, string labelText)
		{
			return LabelHelper(html, html.ViewData.ModelMetadata, String.Empty, labelText);
		}

		internal static MvcHtmlString LabelHelper(HtmlHelper html, ModelMetadata metadata, string htmlFieldName, string labelText = null)
		{
			var resolvedLabelText = labelText ?? metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();
			if (String.IsNullOrEmpty(resolvedLabelText))
			{
				return MvcHtmlString.Empty;
			}

			var tag = new TagBuilder("label");
			tag.Attributes.Add("for", TagBuilder.CreateSanitizedId(htmlFieldName));
			tag.SetInnerText(resolvedLabelText);
			return tag.ToHtmlString(TagRenderMode.Normal);
		}
	}
}
