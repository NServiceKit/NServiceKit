using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NServiceKit.Common;
using NServiceKit.WebHost.Endpoints.Support.Markdown.Templates;

namespace NServiceKit.WebHost.Endpoints.Support.Markdown
{
    /// <summary>
    /// Extension methods for working with markdown templates.
    /// </summary>
	public static class TemplateExtensions
	{
		private const string UnwantedPrefix = "<p>";
		private const string UnwantedSuffix = "</p>";
		private const char EscapeChar = '\\';
		private const char QuoteChar = '"';

        /// <summary>
        /// 
        /// </summary>
		public const char StatementPlaceholderChar = '^';

		private const char BeginStatementChar = '{';
		private const char EndStatementChar = '}';
		private const char BeginMethodChar = '(';
		private const char EndMethodChar = ')';

        static readonly char[] LineEndChars = new[] { '\r', '\n' };
        static readonly char[] WhiteSpaceChars = new[] { ' ', '\t', '\r', '\n' };
        static readonly char[] WhiteSpaceAndSymbolChars = new[] {
			' ', '\t', '\r', '\n', '(', ')', '!', '+', '-'
		};

		static readonly bool[] AlphaNumericFlags = new bool[128];
		static readonly bool[] MemberExprFlags = new bool[128];
		static readonly bool[] StatementFlags = new bool[128];
		static readonly bool[] WhiteSpaceFlags = new bool[128];

		static TemplateExtensions()
		{
			for (int i = 'A'; i <= 'Z'; i++)
			{
				AlphaNumericFlags[i] = true;
				MemberExprFlags[i] = true;
				StatementFlags[i] = true;
			}
			for (int i = 'a'; i <= 'z'; i++)
			{
				AlphaNumericFlags[i] = true;
				MemberExprFlags[i] = true;
				StatementFlags[i] = true;
			}
			for (int i = '0'; i <= '9'; i++)
			{
				AlphaNumericFlags[i] = true;
				MemberExprFlags[i] = true;
				StatementFlags[i] = true;
			}

			var exprChars = new[] { '.' }; //removed: '[', ']' 
			var tokenChars = new[] { '(', ')', '{', '}' };

			foreach (var exprChar in exprChars)
			{
				MemberExprFlags[exprChar] = true;
				StatementFlags[exprChar] = true;
			}
			foreach (var whitespaceChar in WhiteSpaceChars)
			{
				StatementFlags[whitespaceChar] = true;
				WhiteSpaceFlags[whitespaceChar] = true;
			}
			foreach (var tokenChar in tokenChars)
			{
				StatementFlags[tokenChar] = true;
			}
		}

        /// <summary>
        /// Splits the string on white space.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
		public static string[] SplitOnWhiteSpace(this string text)
		{
			return text.SplitAndTrimOn(WhiteSpaceChars);
		}

        /// <summary>
        /// Splits the string on white space and symbols.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
		public static string[] SplitOnWhiteSpaceAndSymbols(this string text)
		{
			return text.SplitAndTrimOn(WhiteSpaceAndSymbolChars);
		}

        /// <summary>
        /// Splits on the given characters and trims each chunk.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="chars">The chars.</param>
        /// <returns></returns>
		public static string[] SplitAndTrimOn(this string text, char[] chars)
		{
			if (text == null) return new string[0];
			var parts = text.Split(chars);
			var results = new List<string>();

			foreach (var part in parts)
			{
				var val = part.Trim();
				if (string.IsNullOrEmpty(val)) continue;

				results.Add(part);
			}

			return results.ToArray();
		}

        /// <summary>
        /// Renders to markdown.
        /// </summary>
        /// <param name="markdownPage">The markdown page.</param>
        /// <param name="scopeArgs">The scope arguments.</param>
        /// <returns></returns>
		public static string RenderToMarkdown(this MarkdownPage markdownPage, Dictionary<string, object> scopeArgs)
		{
			return RenderToString(markdownPage, scopeArgs, false);
		}

        /// <summary>
        /// Renders to HTML.
        /// </summary>
        /// <param name="markdownPage">The markdown page.</param>
        /// <param name="scopeArgs">The scope arguments.</param>
        /// <returns></returns>
		public static string RenderToHtml(this MarkdownPage markdownPage, Dictionary<string, object> scopeArgs)
		{
			return RenderToString(markdownPage, scopeArgs, true);
		}

        /// <summary>
        /// Renders to string.
        /// </summary>
        /// <param name="markdownPage">The markdown page.</param>
        /// <param name="scopeArgs">The scope arguments.</param>
        /// <param name="renderHtml">if set to <c>true</c> [render HTML].</param>
        /// <returns></returns>
		public static string RenderToString(this MarkdownPage markdownPage, Dictionary<string, object> scopeArgs, bool renderHtml)
		{
			var sb = new StringBuilder();
			using (var writer = new StringWriter(sb))
			{
				var pageContext = new PageContext(markdownPage, scopeArgs, renderHtml);
				markdownPage.Write(writer, pageContext);
			}
			return sb.ToString();
		}

        /// <summary>
        /// Renders to string.
        /// </summary>
        /// <param name="templateWriter">The template writer.</param>
        /// <param name="scopeArgs">The scope arguments.</param>
        /// <returns></returns>
		public static string RenderToString(this ITemplateWriter templateWriter, Dictionary<string, object> scopeArgs)
		{
			var sb = new StringBuilder();
			using (var writer = new StringWriter(sb))
			{
				templateWriter.Write(null, writer, scopeArgs);
			}
			return sb.ToString();
		}

        /// <summary>
        /// Renders to string.
        /// </summary>
        /// <param name="templateWriters">The template writers.</param>
        /// <param name="scopeArgs">The scope arguments.</param>
        /// <returns></returns>
		public static string RenderToString(this IEnumerable<ITemplateWriter> templateWriters, Dictionary<string, object> scopeArgs)
		{
			var sb = new StringBuilder();
			using (var writer = new StringWriter(sb))
			{
				foreach (var templateWriter in templateWriters)
				{
					templateWriter.Write(null, writer, scopeArgs);
				}
			}
			return sb.ToString();
		}

        /// <summary>
        /// Splits the string into blocks on the given placeholder string.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="onPlaceHolder">The on place holder.</param>
        /// <returns></returns>
		public static List<TemplateBlock> SplitIntoBlocks(this string content, string onPlaceHolder)
		{
			var blocks = new List<TemplateBlock>();
			if (content.IsNullOrEmpty()) return blocks;

			var pos = 0;
			var lastPos = 0;
			while ((pos = content.IndexOf(onPlaceHolder, lastPos)) != -1)
			{
				var contentBlock = content.Substring(lastPos, pos - lastPos);

				blocks.Add(new TextBlock(contentBlock));

				lastPos = pos + onPlaceHolder.Length;
			}

			if (lastPos != content.Length - 1)
			{
				var lastBlock = content.Substring(lastPos);
				blocks.Add(new TextBlock(lastBlock));
			}

			return blocks;
		}

        /// <summary>
        /// Removes the given text from the end of the block, if it exists.
        /// </summary>
        /// <param name="textBlock">The text block.</param>
        /// <param name="text">The text.</param>
		public static void RemoveIfEndingWith(this TextBlock textBlock, string text)
		{
			if (textBlock == null) return;
			if (textBlock.Content.EndsWith(text))
			{
				textBlock.Content = textBlock.Content.Substring(
					0, textBlock.Content.Length - text.Length);
			}
		}

        /// <summary>
        /// Trims the given text from the end of the string, if it exists.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="text">The text.</param>
        /// <returns></returns>
		public static string TrimIfEndingWith(this string content, string text)
		{
			if (content == null || !content.EndsWith(text)) return content;
			return content.Substring(0, content.Length - text.Length);
		}

        /// <summary>
        /// Increments pos past text if it's the next chunk in content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="pos">The position.</param>
        /// <param name="text">The text.</param>
		public static void SkipIfNextIs(this string content, ref int pos, string text)
		{
			if (content == null || text == null) return;

			if (content.Length < pos + text.Length) return;

			var test = content.Substring(pos, text.Length);
			if (test != text) return;

			pos += text.Length;
		}

        /// <summary>
        /// Trims the line if it only has whitespace.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
		public static string TrimLineIfOnlyHasWhitespace(this string text)
		{
			var pos = text.LastIndexOfAny(LineEndChars) + 1; //after \n or at start if not found
			if (pos == text.Length) return text;
			var startPos = pos;
			text.EatWhitespace(ref pos);
			if (pos == text.Length) return text.Substring(0, startPos);
			return text;
		}

        /// <summary>
        /// Creates the template blocks.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="statementBlocks">The statement blocks.</param>
        /// <returns></returns>
		public static List<TemplateBlock> CreateTemplateBlocks(this string content, List<StatementExprBlock> statementBlocks)
		{
			var blocks = new List<TemplateBlock>();
			if (content.IsNullOrEmpty()) return blocks;

			int pos;
			var lastPos = 0;
			while ((pos = content.IndexOf('@', lastPos)) != -1)
			{
				var contentBlock = content.Substring(lastPos, pos - lastPos);
				var prevTextBlock = new TextBlock(contentBlock);
				blocks.Add(prevTextBlock);

				pos++; //@

				if (content[pos] == '@')
				{
					prevTextBlock.Content += "@";
					pos++;
				}
				else if (content[pos] == StatementPlaceholderChar)
				{
					pos++; //^
					var index = content.GetNextAlphaNumericExpr(ref pos);
					int statementNo;
					if (int.TryParse(index, out statementNo))
					{
						var statementIndex = statementNo - 1;
						if (statementIndex >= statementBlocks.Count)
							throw new ArgumentOutOfRangeException(
								"Expected < " + statementBlocks.Count + " but was " + statementIndex);

						var statement = statementBlocks[statementIndex];
						blocks.Add(statement);
					}

					//Strip everything but @^1 in <p>@^1</p>\n
					prevTextBlock.RemoveIfEndingWith(UnwantedPrefix);
					content.SkipIfNextIs(ref pos, UnwantedSuffix);
                    content.SkipIfNextIs(ref pos, "\r");
                    content.SkipIfNextIs(ref pos, "\n");
				}
				else
				{
					//ignore email addresses with @ in it
					var charBeforeAtSymbol = content.SafePeekAt(pos - 3);
					if (!charBeforeAtSymbol.IsAlphaNumeric())
					{
						var memberExpr = content.GetNextMemberExpr(ref pos);
						if (memberExpr != null)
						{
							blocks.Add(new MemberExprBlock(memberExpr));
						}
					}
					else
					{
						prevTextBlock.Content += "@";
					}
				}

				lastPos = pos;
			}

			if (lastPos != content.Length)
			{
				var lastBlock = lastPos == 0 ? content : content.Substring(lastPos);
				blocks.Add(new TextBlock(lastBlock));
			}

			return blocks;
		}

        /// <summary>
        /// Gets the next statement expr.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="fromPos">From position.</param>
        /// <returns></returns>
        /// <exception cref="System.IO.InvalidDataException"></exception>
		public static StatementExprBlock GetNextStatementExpr(this string content, ref int fromPos)
		{
			var varExpr = content.GetNextAlphaNumericExpr(ref fromPos);
			if (varExpr != null)
			{
				if (varExpr == "foreach" || varExpr == "if" || varExpr == "section")
				{
					var conditionEndPos = content.IndexOf(BeginStatementChar, fromPos);
					if (conditionEndPos == -1)
						throw new InvalidDataException(varExpr + " at index: " + fromPos);

					var condition = content.Substring(fromPos, conditionEndPos - fromPos)
						.Trim('(', ')', ' ');

					fromPos = conditionEndPos;
					var statement = content.EatStatementExpr(ref fromPos);
					statement = statement.TrimLineIfOnlyHasWhitespace();

					if (varExpr == "foreach")
						return new ForEachStatementExprBlock(condition, statement);

					if (varExpr == "if")
					{
						string elseStatement = null;
						var nextWord = content.PeekWordAfterWhitespace(fromPos);
						if (nextWord == "else")
						{
							//Skip past else
							content.EatWhitespace(ref fromPos);
							content.GetNextAlphaNumericExpr(ref fromPos);

							elseStatement = content.EatStatementExpr(ref fromPos);
						}

						return new IfStatementExprBlock(condition, statement, elseStatement);
					}

					if (varExpr == "section")
						return new SectionStatementExprBlock(condition, statement);
				}
				else if (varExpr == "var"
					|| varExpr == "model"
					|| varExpr == "inherits"
					|| varExpr == "helper"
					|| varExpr == "template" || varExpr == "Layout")
				{
                    var pos = content.IndexOfAny(LineEndChars, fromPos);
					var restOfLine = content.Substring(fromPos, pos - fromPos);
					fromPos = pos;

					if (varExpr == "var")
						return new VarStatementExprBlock(varExpr, restOfLine);

					return new DirectiveBlock(varExpr, restOfLine);
				}
			}

			var nextToken = PeekAfterWhitespace(content, fromPos);
			if (nextToken == BeginMethodChar)
			{
				var methodParamsExpr = content.EatMethodExpr(ref fromPos);
				return new MethodStatementExprBlock(varExpr, methodParamsExpr, null);
			}

			return null;
		}

        /// <summary>
        /// Peeks the next character after whitespace.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
		public static char PeekAfterWhitespace(this string content, int index)
		{
			int c;
			for (; index < content.Length; index++)
			{
				c = content[index];
				if (c >= WhiteSpaceFlags.Length || !WhiteSpaceFlags[c])
				{
					break;
				}
			}
			return index == content.Length ? '\0' : content[index];
		}

        /// <summary>
        /// Peeks the next word after whitespace.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
		public static string PeekWordAfterWhitespace(this string content, int index)
		{
			content.EatWhitespace(ref index);
			var word = content.GetNextAlphaNumericExpr(ref index);
			return word;
		}

        /// <summary>
        /// Increments index until it runs out of whitespace.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="index">The index.</param>
		public static void EatWhitespace(this string content, ref int index)
		{
			int c;
			for (; index < content.Length; index++)
			{
				c = content[index];
				if (c >= WhiteSpaceFlags.Length || !WhiteSpaceFlags[c])
				{
					break;
				}
			}
		}

        /// <summary>
        /// Eats the rest of the line.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="index">The index.</param>
		public static void EatRestOfLine(this string content, ref int index)
		{
			int c;
			for (; index < content.Length; index++)
			{
				c = content[index];
				if (c >= WhiteSpaceFlags.Length || !WhiteSpaceFlags[c])
				{
					return;
				}
				if (c != '\r' && c != '\n') continue;

                if (c == '\r') index++;
				index++;
				return;
			}
		}

		private static string EatBlockExpr(this string content, ref int fromPos, char beginChar, char endChar, bool allowDoubleEscaping)
		{
			content.EatWhitespace(ref fromPos);
			if (content[fromPos++] != beginChar)
				throw new InvalidDataException("Expected " + beginChar + " at: " + fromPos);
			content.EatRestOfLine(ref fromPos);

			var startPos = fromPos;

			var hasDoubleEscaping = false;
			var withinQuotes = false;
			var endsToEat = 1;
			while (fromPos < content.Length && endsToEat > 0)
			{
				var c = content[fromPos];

				if (c == QuoteChar
					&& content[fromPos - 1] != EscapeChar)
					withinQuotes = !withinQuotes;

				if (!withinQuotes)
				{
					var nextChar = content.SafePeekAt(fromPos);
					var isEscaped = allowDoubleEscaping
						&& (c == beginChar && nextChar == beginChar 
						    || c == endChar && nextChar == endChar);
					if (isEscaped)
					{
						hasDoubleEscaping = true;
						fromPos++;
					}
					else
					{
						if (c == beginChar)
							endsToEat++;

						if (c == endChar)
							endsToEat--;
					}
				}

				fromPos++;
			}

			var result = content.Substring(startPos, fromPos - startPos - 1);
			if (!hasDoubleEscaping) return result;

			return result.Replace(beginChar.ToString() + beginChar, beginChar.ToString())
				.Replace(endChar.ToString() + endChar, endChar.ToString());
		}

        /// <summary>
        /// Safes peeks at the specified index. Returns \0 if it doesn't exist.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="fromPos">From position.</param>
        /// <returns></returns>
		public static char SafePeekAt(this string content, int fromPos)
		{
			return fromPos + 1 >= content.Length ? '\0' : content[fromPos + 1];
		}

        /// <summary>
        /// Determines whether the given character is alphanumeric.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns></returns>
		public static bool IsAlphaNumeric(this char c)
		{
			return c < AlphaNumericFlags.Length && AlphaNumericFlags[c];
		}

		private static string EatStatementExpr(this string content, ref int fromPos)
		{
			return EatBlockExpr(content, ref fromPos, BeginStatementChar, EndStatementChar, true);
		}

		private static string EatMethodExpr(this string content, ref int fromPos)
		{
			return EatBlockExpr(content, ref fromPos, BeginMethodChar, EndMethodChar, false);
		}

        /// <summary>
        /// Gets the next alpha numeric expression.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="fromPos">From position.</param>
        /// <returns></returns>
		public static string GetNextAlphaNumericExpr(this string content, ref int fromPos)
		{
			var startPos = fromPos;
			for (; fromPos < content.Length; fromPos++)
			{
				var exprChar = content[fromPos];
				if (exprChar >= MemberExprFlags.Length) return null;
				if (!MemberExprFlags[exprChar]) break;
			}
			return content.Substring(startPos, fromPos - startPos);
		}

        /// <summary>
        /// Gets the next member expr.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="fromPos">From position.</param>
        /// <returns></returns>
		public static string GetNextMemberExpr(this string content, ref int fromPos)
		{
			var startPos = fromPos;
			for (; fromPos < content.Length; fromPos++)
			{
				var exprChar = content[fromPos];
				if (exprChar >= MemberExprFlags.Length) return null;
				if (!MemberExprFlags[exprChar]) break;
			}

			if (fromPos == startPos) return null;

			//Remove trailing '.' if any
			var memberExpr = content.Substring(startPos, fromPos - startPos);
			if (memberExpr[memberExpr.Length-1] == '.')
			{
				memberExpr = memberExpr.Substring(0, memberExpr.Length - 1);
				fromPos--;
			}
			return memberExpr;
		}

        /// <summary>
        /// Removes all white space.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns></returns>
		public static string RemoveAllWhiteSpace(this string content)
		{
			return content.RemoveCharFlags(WhiteSpaceFlags);
		}

        /// <summary>
        /// Gets the name of the variable.
        /// </summary>
        /// <param name="memberExpr">The member expr.</param>
        /// <returns></returns>
		public static string GetVarName(this string memberExpr)
		{
			if (memberExpr == null) return null;

			for (var i = 0; i < memberExpr.Length; i++)
			{
				var exprChar = memberExpr[i];
				if (exprChar >= AlphaNumericFlags.Length) return null;
				if (!AlphaNumericFlags[exprChar]) return memberExpr.Substring(0, i);
			}

			return memberExpr;
		}

	}
}