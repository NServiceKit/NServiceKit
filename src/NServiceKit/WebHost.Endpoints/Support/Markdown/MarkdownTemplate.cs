using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using NServiceKit.Markdown;

namespace NServiceKit.WebHost.Endpoints.Support.Markdown
{
    /// <summary>
    /// 
    /// </summary>
	public class MarkdownTemplate : ITemplateWriter, IExpirable
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="MarkdownTemplate"/> class.
        /// </summary>
		public MarkdownTemplate() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkdownTemplate"/> class.
        /// </summary>
        /// <param name="fullPath">The full path.</param>
        /// <param name="name">The name.</param>
        /// <param name="contents">The contents.</param>
		public MarkdownTemplate(string fullPath, string name, string contents)
		{
			FilePath = fullPath;
			Name = name;
			Contents = contents;
		}

        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        /// <value>
        /// The file path.
        /// </value>
		public string FilePath { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
		public string Name { get; set; }
        /// <summary>
        /// Gets or sets the contents.
        /// </summary>
        /// <value>
        /// The contents.
        /// </value>
		public string Contents { get; set; }
        /// <summary>
        /// Gets or sets the last modified.
        /// </summary>
        /// <value>
        /// The last modified.
        /// </value>
		public DateTime? LastModified { get; set; }

        /// <summary>
        /// Gets or sets the text blocks.
        /// </summary>
        /// <value>
        /// The text blocks.
        /// </value>
		public string[] TextBlocks { get; set; }
        /// <summary>
        /// Gets or sets the variable reference blocks.
        /// </summary>
        /// <value>
        /// The variable reference blocks.
        /// </value>
		public string[] VarRefBlocks { get; set; }

        /// <summary>
        /// The body place holder
        /// </summary>
		public static string BodyPlaceHolder = "Body";
        /// <summary>
        /// The place holder prefix
        /// </summary>
		public static string PlaceHolderPrefix = "<!--@";
        /// <summary>
        /// The place holder suffix
        /// </summary>
		public static string PlaceHolderSuffix = "-->";

		internal Exception initException;
		readonly object readWriteLock = new object();
		private bool isBusy;

        /// <summary>
        /// Reloads the specified template contents.
        /// </summary>
        /// <param name="templateContents">The template contents.</param>
        /// <param name="lastModified">The last modified.</param>
        public void Reload(string templateContents, DateTime lastModified)
		{
			lock (readWriteLock)
			{
				try
				{
					isBusy = true;

					this.Contents = templateContents;
					this.LastModified = lastModified;
					initException = null;
					Prepare();
				}
				catch (Exception ex)
				{
					initException = ex;
				}
				isBusy = false;
				Monitor.PulseAll(readWriteLock);
			}
		}

        /// <summary>
        /// Prepares this instance.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Contents</exception>
        /// <exception cref="System.IO.InvalidDataException">Unterminated PlaceHolder expecting --></exception>
		public void Prepare()
		{
			if (this.Contents == null)
				throw new ArgumentNullException("Contents");

			var textBlocks = new List<string>();
			var variablePositions = new Dictionary<int, string>();

			int pos;
			var lastPos = 0;
			while ((pos = this.Contents.IndexOf(PlaceHolderPrefix, lastPos)) != -1)
			{
				var contentBlock = this.Contents.Substring(lastPos, pos - lastPos);

				textBlocks.Add(contentBlock);

				var endPos = this.Contents.IndexOf(PlaceHolderSuffix, pos);
				if (endPos == -1)
					throw new InvalidDataException("Unterminated PlaceHolder expecting -->");

				var varRef = this.Contents.Substring(
					pos + PlaceHolderPrefix.Length, endPos - (pos + PlaceHolderPrefix.Length));

				var index = textBlocks.Count;
				variablePositions[index] = varRef;

				lastPos = endPos + PlaceHolderSuffix.Length;
			}
			if (lastPos != this.Contents.Length - 1)
			{
				var lastBlock = this.Contents.Substring(lastPos);
				textBlocks.Add(lastBlock);
			}

			this.TextBlocks = textBlocks.ToArray();
			this.VarRefBlocks = new string[this.TextBlocks.Length];

			foreach (var varPos in variablePositions)
			{
				this.VarRefBlocks[varPos.Key] = varPos.Value;
			}
		}

        /// <summary>
        /// Renders to string.
        /// </summary>
        /// <param name="scopeArgs">The scope arguments.</param>
        /// <returns></returns>
        /// <exception cref="System.IO.InvalidDataException">Template has not been Initialized.</exception>
		public string RenderToString(Dictionary<string, object> scopeArgs)
		{
			lock (readWriteLock)
			{
				while (isBusy)
					Monitor.Wait(readWriteLock);
			}

			if (TextBlocks == null || VarRefBlocks == null)
				throw new InvalidDataException("Template has not been Initialized.");

			var sb = new StringBuilder();
			for (var i = 0; i < TextBlocks.Length; i++)
			{
				var textBlock = TextBlocks[i];
				var varName = VarRefBlocks[i];
				if (varName != null && scopeArgs != null)
				{
					object varValue;
					if (scopeArgs.TryGetValue(varName, out varValue))
					{
						sb.Append(varValue);
					}
				}
				sb.Append(textBlock);
			}
			return sb.ToString();
		}

        /// <summary>
        /// Writes the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="textWriter">The text writer.</param>
        /// <param name="scopeArgs">The scope arguments.</param>
        /// <exception cref="System.IO.InvalidDataException">Template has not been Initialized.</exception>
		public void Write(MarkdownViewBase instance, TextWriter textWriter, Dictionary<string, object> scopeArgs)
		{
			lock (readWriteLock)
			{
				while (isBusy)
					Monitor.Wait(readWriteLock);
			}

			if (TextBlocks == null || VarRefBlocks == null)
				throw new InvalidDataException("Template has not been Initialized.");

			for (var i = 0; i < TextBlocks.Length; i++)
			{
				var textBlock = TextBlocks[i];
				var varName = VarRefBlocks[i];
				if (varName != null)
				{
					object varValue;
					if (scopeArgs.TryGetValue(varName, out varValue))
					{
						textWriter.Write(varValue);
					}
				}
				textWriter.Write(textBlock);
			}
		}

	}

}