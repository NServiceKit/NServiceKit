using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web;

namespace NServiceKit.Html
{
    /// <summary>A tag builder.</summary>
	public class TagBuilder
	{
        /// <summary>The identifier attribute dot replacement.</summary>
		public const string IdAttributeDotReplacement = "_";

		private const string AttributeFormat = @" {0}=""{1}""";
		private const string ElementFormatEndTag = "</{0}>";
		private const string ElementFormatNormal = "<{0}{1}>{2}</{0}>";
		private const string ElementFormatSelfClosing = "<{0}{1} />";
		private const string ElementFormatStartTag = "<{0}{1}>";

		private string innerHtml;

        /// <summary>Initializes a new instance of the NServiceKit.Html.TagBuilder class.</summary>
        ///
        /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or illegal values.</exception>
        ///
        /// <param name="tagName">The name of the tag.</param>
		public TagBuilder(string tagName)
		{
			if (String.IsNullOrEmpty(tagName))
			{
				throw new ArgumentException(MvcResources.Common_NullOrEmpty, "tagName");
			}

			TagName = tagName;
			Attributes = new SortedDictionary<string, string>(StringComparer.Ordinal);
		}

        /// <summary>Gets the attributes.</summary>
        ///
        /// <value>The attributes.</value>
		public IDictionary<string, string> Attributes { get; private set; }

        /// <summary>Gets or sets the inner HTML.</summary>
        ///
        /// <value>The inner HTML.</value>
		public string InnerHtml
		{
			get
			{
				return innerHtml ?? String.Empty;
			}
			set
			{
				innerHtml = value;
			}
		}

        /// <summary>Gets the name of the tag.</summary>
        ///
        /// <value>The name of the tag.</value>
		public string TagName { get; private set; }

        /// <summary>Adds the CSS class.</summary>
        ///
        /// <param name="value">The value.</param>
		public void AddCssClass(string value)
		{
			string currentValue;

			if (Attributes.TryGetValue("class", out currentValue))
			{
				Attributes["class"] = value + " " + currentValue;
			}
			else
			{
				Attributes["class"] = value;
			}
		}

        /// <summary>Creates sanitized identifier.</summary>
        ///
        /// <param name="originalId">Identifier for the original.</param>
        ///
        /// <returns>The new sanitized identifier.</returns>
		public static string CreateSanitizedId(string originalId)
		{
			return CreateSanitizedId(originalId, TagBuilder.IdAttributeDotReplacement);
		}

		internal static string CreateSanitizedId(string originalId, string invalidCharReplacement)
		{
			if (String.IsNullOrEmpty(originalId))
			{
				return null;
			}

			if (invalidCharReplacement == null) {
				throw new ArgumentNullException("invalidCharReplacement");
			}

			char firstChar = originalId[0];
			if (!Html401IdUtil.IsLetter(firstChar))
			{
				// the first character must be a letter
				return null;
			}

			var sb = new StringBuilder(originalId.Length);
			sb.Append(firstChar);

			for (int i = 1; i < originalId.Length; i++)
			{
				char thisChar = originalId[i];
				if (Html401IdUtil.IsValidIdCharacter(thisChar))
				{
					sb.Append(thisChar);
				}
				else
				{
					sb.Append(invalidCharReplacement);
				}
			}

			return sb.ToString();
		}

        /// <summary>Generates an identifier.</summary>
        ///
        /// <param name="name">The name.</param>
		public void GenerateId(string name)
		{
			if (!Attributes.ContainsKey("id")) {
				string sanitizedId = CreateSanitizedId(name, IdAttributeDotReplacement);
				if (!String.IsNullOrEmpty(sanitizedId)) {
					Attributes["id"] = sanitizedId;
				}
			}
		}
		
		private string GetAttributesString()
		{
			var sb = new StringBuilder();
			foreach (var attribute in Attributes)
			{
				string key = attribute.Key;
				if (String.Equals(key, "id", StringComparison.Ordinal /* case-sensitive */) && String.IsNullOrEmpty(attribute.Value))
				{
					continue; // DevDiv Bugs #227595: don't output empty IDs
				}
				string value = HttpUtility.HtmlAttributeEncode(attribute.Value);
				sb.AppendFormat(CultureInfo.InvariantCulture, AttributeFormat, key, value);
			}
			return sb.ToString();
		}

        private void AppendAttributes(StringBuilder sb)
        {
            foreach (var attribute in Attributes) {
                string key = attribute.Key;
                if (String.Equals(key, "id", StringComparison.Ordinal /* case-sensitive */) && String.IsNullOrEmpty(attribute.Value)) {
                    continue; // DevDiv Bugs #227595: don't output empty IDs
                }
                string value = HttpUtility.HtmlAttributeEncode(attribute.Value);
                sb.Append(' ')
                    .Append(key)
                    .Append("=\"")
                    .Append(value)
                    .Append('"');
            }
        }

        /// <summary>Merge attribute.</summary>
        ///
        /// <param name="key">  The key.</param>
        /// <param name="value">The value.</param>
		public void MergeAttribute(string key, string value)
		{
			MergeAttribute(key, value, false /* replaceExisting */);
		}

        /// <summary>Merge attribute.</summary>
        ///
        /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or illegal values.</exception>
        ///
        /// <param name="key">            The key.</param>
        /// <param name="value">          The value.</param>
        /// <param name="replaceExisting">true to replace existing.</param>
		public void MergeAttribute(string key, string value, bool replaceExisting)
		{
			if (String.IsNullOrEmpty(key))
			{
				throw new ArgumentException(MvcResources.Common_NullOrEmpty, "key");
			}

			if (replaceExisting || !Attributes.ContainsKey(key))
			{
				Attributes[key] = value;
			}
		}

        /// <summary>Merge attributes.</summary>
        ///
        /// <typeparam name="TKey">  Type of the key.</typeparam>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <param name="attributes">The attributes.</param>
		public void MergeAttributes<TKey, TValue>(IDictionary<TKey, TValue> attributes)
		{
			MergeAttributes(attributes, false /* replaceExisting */);
		}

        /// <summary>Merge attributes.</summary>
        ///
        /// <typeparam name="TKey">  Type of the key.</typeparam>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <param name="attributes">     The attributes.</param>
        /// <param name="replaceExisting">true to replace existing.</param>
		public void MergeAttributes<TKey, TValue>(IDictionary<TKey, TValue> attributes, bool replaceExisting)
		{
			if (attributes != null)
			{
				foreach (var entry in attributes)
				{
					string key = Convert.ToString(entry.Key, CultureInfo.InvariantCulture);
					string value = Convert.ToString(entry.Value, CultureInfo.InvariantCulture);
					MergeAttribute(key, value, replaceExisting);
				}
			}
		}

        /// <summary>Sets inner text.</summary>
        ///
        /// <param name="innerText">The inner text.</param>
		public void SetInnerText(string innerText)
		{
			InnerHtml = HttpUtility.HtmlEncode(innerText);
		}

        internal MvcHtmlString ToMvcHtmlString(TagRenderMode renderMode)
        {
            return ToHtmlString(renderMode);
        }

		internal MvcHtmlString ToHtmlString(TagRenderMode renderMode)
		{
			return MvcHtmlString.Create(ToString(renderMode));
		}

        /// <summary>Returns a string that represents the current object.</summary>
        ///
        /// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			return ToString(TagRenderMode.Normal);
		}

        /// <summary>Convert this object into a string representation.</summary>
        ///
        /// <param name="renderMode">The render mode.</param>
        ///
        /// <returns>A string that represents this object.</returns>
		public string ToString(TagRenderMode renderMode)
		{
            StringBuilder sb = new StringBuilder();
            switch (renderMode) {
                case TagRenderMode.StartTag:
                    sb.Append('<')
                        .Append(TagName);
                    AppendAttributes(sb);
                    sb.Append('>');
                    break;
                case TagRenderMode.EndTag:
                    sb.Append("</")
                        .Append(TagName)
                        .Append('>');
                    break;
                case TagRenderMode.SelfClosing:
                    sb.Append('<')
                        .Append(TagName);
                    AppendAttributes(sb);
                    sb.Append(" />");
                    break;
                default:
                    sb.Append('<')
                        .Append(TagName);
                    AppendAttributes(sb);
                    sb.Append('>')
                        .Append(InnerHtml)
                        .Append("</")
                        .Append(TagName)
                        .Append('>');
                    break;
            }
            return sb.ToString();
        }

		// Valid IDs are defined in http://www.w3.org/TR/html401/types.html#type-id
		private static class Html401IdUtil
		{
			private static bool IsAllowableSpecialCharacter(char c)
			{
				switch (c)
				{
					case '-':
					case '_':
					case ':':
						// note that we're specifically excluding the '.' character
						return true;

					default:
						return false;
				}
			}

			private static bool IsDigit(char c)
			{
				return ('0' <= c && c <= '9');
			}

            /// <summary>Query if 'c' is letter.</summary>
            ///
            /// <param name="c">The character.</param>
            ///
            /// <returns>true if letter, false if not.</returns>
			public static bool IsLetter(char c)
			{
				return (('A' <= c && c <= 'Z') || ('a' <= c && c <= 'z'));
			}

            /// <summary>Query if 'c' is valid identifier character.</summary>
            ///
            /// <param name="c">The character.</param>
            ///
            /// <returns>true if valid identifier character, false if not.</returns>
			public static bool IsValidIdCharacter(char c)
			{
				return (IsLetter(c) || IsDigit(c) || IsAllowableSpecialCharacter(c));
			}
		}

	}
}