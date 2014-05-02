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
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using NServiceKit.Markdown;

namespace NServiceKit.Html
{
    /// <summary>A text area extensions.</summary>
	public static class TextAreaExtensions
	{
		// These values are similar to the defaults used by WebForms
		// when using <asp:TextBox TextMode="MultiLine"> without specifying
		// the Rows and Columns attributes.
		private const int TextAreaRows = 2;
		private const int TextAreaColumns = 20;
		private static Dictionary<string, object> implicitRowsAndColumns = new Dictionary<string, object> {
            { "rows", TextAreaRows.ToString(CultureInfo.InvariantCulture) },
            { "cols", TextAreaColumns.ToString(CultureInfo.InvariantCulture) },
        };

		private static Dictionary<string, object> GetRowsAndColumnsDictionary(int rows, int columns)
		{
			if (rows < 0)
			{
				throw new ArgumentOutOfRangeException("rows", MvcResources.HtmlHelper_TextAreaParameterOutOfRange);
			}
			if (columns < 0)
			{
				throw new ArgumentOutOfRangeException("columns", MvcResources.HtmlHelper_TextAreaParameterOutOfRange);
			}

			var result = new Dictionary<string, object>();
			if (rows > 0)
			{
				result.Add("rows", rows.ToString(CultureInfo.InvariantCulture));
			}
			if (columns > 0)
			{
				result.Add("cols", columns.ToString(CultureInfo.InvariantCulture));
			}

			return result;
		}

        /// <summary>A HtmlHelper extension method that text area.</summary>
        ///
        /// <param name="htmlHelper">The htmlHelper to act on.</param>
        /// <param name="name">      The name.</param>
        ///
        /// <returns>A MvcHtmlString.</returns>
		public static MvcHtmlString TextArea(this HtmlHelper htmlHelper, string name)
		{
			return TextArea(htmlHelper, name, null /* value */, null /* htmlAttributes */);
		}

        /// <summary>A HtmlHelper extension method that text area.</summary>
        ///
        /// <param name="htmlHelper">    The htmlHelper to act on.</param>
        /// <param name="name">          The name.</param>
        /// <param name="htmlAttributes">The HTML attributes.</param>
        ///
        /// <returns>A MvcHtmlString.</returns>
		public static MvcHtmlString TextArea(this HtmlHelper htmlHelper, string name, object htmlAttributes)
		{
			return TextArea(htmlHelper, name, null /* value */, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
		}

        /// <summary>A HtmlHelper extension method that text area.</summary>
        ///
        /// <param name="htmlHelper">    The htmlHelper to act on.</param>
        /// <param name="name">          The name.</param>
        /// <param name="htmlAttributes">The HTML attributes.</param>
        ///
        /// <returns>A MvcHtmlString.</returns>
		public static MvcHtmlString TextArea(this HtmlHelper htmlHelper, string name, IDictionary<string, object> htmlAttributes)
		{
			return TextArea(htmlHelper, name, null /* value */, htmlAttributes);
		}

        /// <summary>A HtmlHelper extension method that text area.</summary>
        ///
        /// <param name="htmlHelper">The htmlHelper to act on.</param>
        /// <param name="name">      The name.</param>
        /// <param name="value">     The value.</param>
        ///
        /// <returns>A MvcHtmlString.</returns>
		public static MvcHtmlString TextArea(this HtmlHelper htmlHelper, string name, string value)
		{
			return TextArea(htmlHelper, name, value, null /* htmlAttributes */);
		}

        /// <summary>A HtmlHelper extension method that text area.</summary>
        ///
        /// <param name="htmlHelper">    The htmlHelper to act on.</param>
        /// <param name="name">          The name.</param>
        /// <param name="value">         The value.</param>
        /// <param name="htmlAttributes">The HTML attributes.</param>
        ///
        /// <returns>A MvcHtmlString.</returns>
		public static MvcHtmlString TextArea(this HtmlHelper htmlHelper, string name, string value, object htmlAttributes)
		{
			return TextArea(htmlHelper, name, value, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
		}

        /// <summary>A HtmlHelper extension method that text area.</summary>
        ///
        /// <param name="htmlHelper">    The htmlHelper to act on.</param>
        /// <param name="name">          The name.</param>
        /// <param name="value">         The value.</param>
        /// <param name="htmlAttributes">The HTML attributes.</param>
        ///
        /// <returns>A MvcHtmlString.</returns>
		public static MvcHtmlString TextArea(this HtmlHelper htmlHelper, string name, string value, IDictionary<string, object> htmlAttributes)
		{
			ModelMetadata metadata = ModelMetadata.FromStringExpression(name, htmlHelper.ViewData);
			if (value != null)
			{
				metadata.Model = value;
			}

			return TextAreaHelper(htmlHelper, metadata, name, implicitRowsAndColumns, htmlAttributes);
		}

        /// <summary>A HtmlHelper extension method that text area.</summary>
        ///
        /// <param name="htmlHelper">    The htmlHelper to act on.</param>
        /// <param name="name">          The name.</param>
        /// <param name="value">         The value.</param>
        /// <param name="rows">          The rows.</param>
        /// <param name="columns">       The columns.</param>
        /// <param name="htmlAttributes">The HTML attributes.</param>
        ///
        /// <returns>A MvcHtmlString.</returns>
		public static MvcHtmlString TextArea(this HtmlHelper htmlHelper, string name, string value, int rows, int columns, object htmlAttributes)
		{
			return TextArea(htmlHelper, name, value, rows, columns, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
		}

        /// <summary>A HtmlHelper extension method that text area.</summary>
        ///
        /// <param name="htmlHelper">    The htmlHelper to act on.</param>
        /// <param name="name">          The name.</param>
        /// <param name="value">         The value.</param>
        /// <param name="rows">          The rows.</param>
        /// <param name="columns">       The columns.</param>
        /// <param name="htmlAttributes">The HTML attributes.</param>
        ///
        /// <returns>A MvcHtmlString.</returns>
		public static MvcHtmlString TextArea(this HtmlHelper htmlHelper, string name, string value, int rows, int columns, IDictionary<string, object> htmlAttributes)
		{
			var metadata = ModelMetadata.FromStringExpression(name, htmlHelper.ViewData);
			if (value != null)
			{
				metadata.Model = value;
			}

			return TextAreaHelper(htmlHelper, metadata, name, GetRowsAndColumnsDictionary(rows, columns), htmlAttributes);
		}

        /// <summary>A HtmlHelper&lt;TModel&gt; extension method that text area for.</summary>
        ///
        /// <typeparam name="TModel">   Type of the model.</typeparam>
        /// <typeparam name="TProperty">Type of the property.</typeparam>
        /// <param name="htmlHelper">The htmlHelper to act on.</param>
        /// <param name="expression">The expression.</param>
        ///
        /// <returns>A MvcHtmlString.</returns>
		public static MvcHtmlString TextAreaFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
		{
			return TextAreaFor(htmlHelper, expression, (IDictionary<string, object>)null);
		}

        /// <summary>A HtmlHelper&lt;TModel&gt; extension method that text area for.</summary>
        ///
        /// <typeparam name="TModel">   Type of the model.</typeparam>
        /// <typeparam name="TProperty">Type of the property.</typeparam>
        /// <param name="htmlHelper">    The htmlHelper to act on.</param>
        /// <param name="expression">    The expression.</param>
        /// <param name="htmlAttributes">The HTML attributes.</param>
        ///
        /// <returns>A MvcHtmlString.</returns>
		public static MvcHtmlString TextAreaFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
		{
			return TextAreaFor(htmlHelper, expression, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
		}

        /// <summary>A HtmlHelper&lt;TModel&gt; extension method that text area for.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <typeparam name="TModel">   Type of the model.</typeparam>
        /// <typeparam name="TProperty">Type of the property.</typeparam>
        /// <param name="htmlHelper">    The htmlHelper to act on.</param>
        /// <param name="expression">    The expression.</param>
        /// <param name="htmlAttributes">The HTML attributes.</param>
        ///
        /// <returns>A MvcHtmlString.</returns>
		public static MvcHtmlString TextAreaFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
		{
			if (expression == null)
			{
				throw new ArgumentNullException("expression");
			}

			return TextAreaHelper(htmlHelper,
								  ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData),
								  ExpressionHelper.GetExpressionText(expression),
								  implicitRowsAndColumns,
								  htmlAttributes);
		}

        /// <summary>A HtmlHelper&lt;TModel&gt; extension method that text area for.</summary>
        ///
        /// <typeparam name="TModel">   Type of the model.</typeparam>
        /// <typeparam name="TProperty">Type of the property.</typeparam>
        /// <param name="htmlHelper">    The htmlHelper to act on.</param>
        /// <param name="expression">    The expression.</param>
        /// <param name="rows">          The rows.</param>
        /// <param name="columns">       The columns.</param>
        /// <param name="htmlAttributes">The HTML attributes.</param>
        ///
        /// <returns>A MvcHtmlString.</returns>
		public static MvcHtmlString TextAreaFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, int rows, int columns, object htmlAttributes)
		{
			return TextAreaFor(htmlHelper, expression, rows, columns, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
		}

        /// <summary>A HtmlHelper&lt;TModel&gt; extension method that text area for.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <typeparam name="TModel">   Type of the model.</typeparam>
        /// <typeparam name="TProperty">Type of the property.</typeparam>
        /// <param name="htmlHelper">    The htmlHelper to act on.</param>
        /// <param name="expression">    The expression.</param>
        /// <param name="rows">          The rows.</param>
        /// <param name="columns">       The columns.</param>
        /// <param name="htmlAttributes">The HTML attributes.</param>
        ///
        /// <returns>A MvcHtmlString.</returns>
		public static MvcHtmlString TextAreaFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, int rows, int columns, IDictionary<string, object> htmlAttributes)
		{
			if (expression == null)
			{
				throw new ArgumentNullException("expression");
			}

			return TextAreaHelper(htmlHelper,
								  ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData),
								  ExpressionHelper.GetExpressionText(expression),
								  GetRowsAndColumnsDictionary(rows, columns),
								  htmlAttributes);
		}

		private static MvcHtmlString TextAreaHelper(HtmlHelper htmlHelper, ModelMetadata modelMetadata, string name, IDictionary<string, object> rowsAndColumns, IDictionary<string, object> htmlAttributes)
		{
			string fullName = name;
			if (String.IsNullOrEmpty(fullName))
			{
				throw new ArgumentException(MvcResources.Common_NullOrEmpty, "name");
			}

			TagBuilder tagBuilder = new TagBuilder("textarea");
            tagBuilder.GenerateId(fullName);
			tagBuilder.MergeAttributes(htmlAttributes, true);
			tagBuilder.MergeAttributes(rowsAndColumns, rowsAndColumns != implicitRowsAndColumns);  // Only force explicit rows/cols
			tagBuilder.MergeAttribute("name", fullName, true);

			// If there are any errors for a named field, we add the CSS attribute.
			ModelState modelState;
            if (htmlHelper.ViewData.ModelState.TryGetValue(fullName, out modelState) && modelState.Errors.Count > 0) {
                tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
            }

			string value;
			if (modelState != null && modelState.Value != null)
			{
				value = modelState.Value.AttemptedValue;
			}
			else if (modelMetadata.Model != null)
			{
				value = modelMetadata.Model.ToString();
			}
			else
			{
				value = String.Empty;
			}

			// The first newline is always trimmed when a TextArea is rendered, so we add an extra one
			// in case the value being rendered is something like "\r\nHello".
			tagBuilder.SetInnerText(Environment.NewLine + value);

			return tagBuilder.ToHtmlString(TagRenderMode.Normal);
		}
	}
}
