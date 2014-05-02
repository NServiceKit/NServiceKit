using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web;

namespace NServiceKit.MiniProfiler.Helpers
{
    // http://haacked.com/archive/2009/01/04/fun-with-named-formats-string-parsing-and-edge-cases.aspx
    internal static class HaackFormatter
    {
        /// <summary>A string extension method that formats.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="format">The format to act on.</param>
        /// <param name="source">Source for the.</param>
        ///
        /// <returns>The formatted value.</returns>
        public static string Format(this string format, object source)
        {

            if (format == null)
            {
                throw new ArgumentNullException("format");
            }

            var formattedStrings = (from expression in SplitFormat(format)
                                    select expression.Eval(source)).ToArray();
            return String.Join("", formattedStrings);
        }

        private static IEnumerable<ITextExpression> SplitFormat(string format)
        {
            int exprEndIndex = -1;
            int expStartIndex;

            do
            {
                expStartIndex = format.IndexOfExpressionStart(exprEndIndex + 1);
                if (expStartIndex < 0)
                {
                    //everything after last end brace index.
                    if (exprEndIndex + 1 < format.Length)
                    {
                        yield return new LiteralFormat(
                            format.Substring(exprEndIndex + 1));
                    }
                    break;
                }

                if (expStartIndex - exprEndIndex - 1 > 0)
                {
                    //everything up to next start brace index
                    yield return new LiteralFormat(format.Substring(exprEndIndex + 1
                      , expStartIndex - exprEndIndex - 1));
                }

                int endBraceIndex = format.IndexOfExpressionEnd(expStartIndex + 1);
                if (endBraceIndex < 0)
                {
                    //rest of string, no end brace (could be invalid expression)
                    yield return new FormatExpression(format.Substring(expStartIndex));
                }
                else
                {
                    exprEndIndex = endBraceIndex;
                    //everything from start to end brace.
                    yield return new FormatExpression(format.Substring(expStartIndex
                      , endBraceIndex - expStartIndex + 1));

                }
            } while (expStartIndex > -1);
        }

        static int IndexOfExpressionStart(this string format, int startIndex)
        {
            int index = format.IndexOf('{', startIndex);
            if (index == -1)
            {
                return index;
            }

            //peek ahead.
            if (index + 1 < format.Length)
            {
                char nextChar = format[index + 1];
                if (nextChar == '{')
                {
                    return IndexOfExpressionStart(format, index + 2);
                }
            }

            return index;
        }

        static int IndexOfExpressionEnd(this string format, int startIndex)
        {
            int endBraceIndex = format.IndexOf('}', startIndex);
            if (endBraceIndex == -1)
            {
                return endBraceIndex;
            }
            //start peeking ahead until there are no more braces...
            // }}}}
            int braceCount = 0;
            for (int i = endBraceIndex + 1; i < format.Length; i++)
            {
                if (format[i] == '}')
                {
                    braceCount++;
                }
                else
                {
                    break;
                }
            }
            if (braceCount % 2 == 1)
            {
                return IndexOfExpressionEnd(format, endBraceIndex + braceCount + 1);
            }

            return endBraceIndex;
        }

        /// <summary>A format expression.</summary>
        public class FormatExpression : ITextExpression
        {
            bool _invalidExpression = false;

            /// <summary>Initializes a new instance of the NServiceKit.MiniProfiler.Helpers.HaackFormatter.FormatExpression class.</summary>
            ///
            /// <param name="expression">The expression.</param>
            public FormatExpression(string expression)
            {
                if (!expression.StartsWith("{") || !expression.EndsWith("}"))
                {
                    _invalidExpression = true;
                    Expression = expression;
                    return;
                }

                string expressionWithoutBraces = expression.Substring(1
                    , expression.Length - 2);
                int colonIndex = expressionWithoutBraces.IndexOf(':');
                if (colonIndex < 0)
                {
                    Expression = expressionWithoutBraces;
                }
                else
                {
                    Expression = expressionWithoutBraces.Substring(0, colonIndex);
                    Format = expressionWithoutBraces.Substring(colonIndex + 1);
                }
            }

            /// <summary>Gets the expression.</summary>
            ///
            /// <value>The expression.</value>
            public string Expression
            {
                get;
                private set;
            }

            /// <summary>Gets the format to use.</summary>
            ///
            /// <value>The format.</value>
            public string Format
            {
                get;
                private set;
            }

            /// <summary>Evals the given o.</summary>
            ///
            /// <exception cref="FormatException">Thrown when the format of the ? is incorrect.</exception>
            ///
            /// <param name="o">The object to process.</param>
            ///
            /// <returns>A string.</returns>
            public string Eval(object o)
            {
                if (_invalidExpression)
                {
                    throw new FormatException("Invalid expression");
                }
                try
                {
                    if (String.IsNullOrEmpty(Format))
                    {
                        return (DataBinder.Eval(o, Expression) ?? string.Empty).ToString();
                    }
                    return (DataBinder.Eval(o, Expression, "{0:" + Format + "}") ??
                      string.Empty).ToString();
                }
                catch (ArgumentException)
                {
                    throw new FormatException();
                }
                catch (HttpException)
                {
                    throw new FormatException();
                }
            }
        }

        /// <summary>A literal format.</summary>
        public class LiteralFormat : ITextExpression
        {
            /// <summary>Initializes a new instance of the NServiceKit.MiniProfiler.Helpers.HaackFormatter.LiteralFormat class.</summary>
            ///
            /// <param name="literalText">The literal text.</param>
            public LiteralFormat(string literalText)
            {
                LiteralText = literalText;
            }

            /// <summary>Gets the literal text.</summary>
            ///
            /// <value>The literal text.</value>
            public string LiteralText
            {
                get;
                private set;
            }

            /// <summary>Evals the given o.</summary>
            ///
            /// <param name="o">The object to process.</param>
            ///
            /// <returns>A string.</returns>
            public string Eval(object o)
            {
                string literalText = LiteralText
                    .Replace("{{", "{")
                    .Replace("}}", "}");
                return literalText;
            }
        }

        /// <summary>Interface for text expression.</summary>
        public interface ITextExpression
        {
            string Eval(object o);
        }
    }
}