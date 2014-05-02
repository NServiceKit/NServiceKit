using System;
using System.Globalization;
using System.IO;

namespace NServiceKit.MiniProfiler
{
    /// <summary>Interface for HTML string.</summary>
	public interface IHtmlString
	{
        /// <summary>Converts this object to a HTML string.</summary>
        ///
        /// <returns>This object as a string.</returns>
		string ToHtmlString();
	}

    /// <summary>A HTML string.</summary>
	public class HtmlString : IHtmlString
	{
		private string value;

        /// <summary>Initializes a new instance of the NServiceKit.MiniProfiler.HtmlString class.</summary>
        ///
        /// <param name="value">The value.</param>
		public HtmlString(string value)
		{
			this.value = value;
		}

        /// <summary>Returns a string that represents the current object.</summary>
        ///
        /// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			return value;
		}

        /// <summary>Converts this object to a HTML string.</summary>
        ///
        /// <returns>This object as a string.</returns>
		public string ToHtmlString()
		{
			return this.ToString();
		}
	}

    /// <summary>Encapsulates the result of a helper.</summary>
	public class HelperResult : IHtmlString
	{
		private readonly Action<TextWriter> action;

        /// <summary>Initializes a new instance of the NServiceKit.MiniProfiler.HelperResult class.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="action">The action.</param>
		public HelperResult(Action<TextWriter> action)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action", "The action parameter cannot be null.");
			}

			this.action = action;
		}

        /// <summary>Converts this object to a HTML string.</summary>
        ///
        /// <returns>This object as a string.</returns>
		public string ToHtmlString()
		{
			return this.ToString();
		}

        /// <summary>Returns a string that represents the current object.</summary>
        ///
        /// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			using (var stringWriter = new StringWriter(CultureInfo.InvariantCulture))
			{
				this.action(stringWriter);
				return stringWriter.ToString();
			}
		}

        /// <summary>Writes to.</summary>
        ///
        /// <param name="writer">The writer.</param>
		public void WriteTo(TextWriter writer)
		{
			this.action(writer);
		}
	}
}