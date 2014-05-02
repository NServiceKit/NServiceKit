using System;
using System.Collections.Generic;
using System.Text;

namespace NServiceKit.WebHost.Endpoints.Support.Templates
{
    internal class TableTemplate
    {
        /// <summary>Gets or sets the title.</summary>
        ///
        /// <value>The title.</value>
        public string Title { get; set; }

        /// <summary>Gets or sets the items.</summary>
        ///
        /// <value>The items.</value>
        public IList<string> Items { get; set; }

        /// <summary>Gets or sets the items map.</summary>
        ///
        /// <value>The items map.</value>
        public IDictionary<string, string> ItemsMap { get; set; }

        /// <summary>Gets or sets the items int map.</summary>
        ///
        /// <value>The items int map.</value>
		public IDictionary<int, string> ItemsIntMap { get; set; }

        /// <summary>Gets or sets for each item.</summary>
        ///
        /// <value>for each item.</value>
		public Func<string, string> ForEachItem { get; set; }

        /// <summary>Gets or sets the item template.</summary>
        ///
        /// <value>The item template.</value>
        public string ItemTemplate { get; set; }

        /// <summary>Returns a string that represents the current object.</summary>
        ///
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(Title))
            {
                sb.AppendFormat("<h3>{0}</h3>", Title);
            }

            sb.Append("<table>");
            sb.Append("<tbody>");
        	if (ItemTemplate != null)
			{
				if (Items != null)
				{
					foreach (var item in Items)
					{
						sb.AppendFormat(ItemTemplate, item, item);
					}
				}
				if (ItemsMap != null)
				{
					foreach (var listItem in ItemsMap)
					{
						sb.AppendFormat(ItemTemplate, listItem.Key, listItem.Value);
					}
				}
				if (ItemsIntMap != null)
				{
					foreach (var listItem in ItemsIntMap)
					{
						sb.AppendFormat(ItemTemplate, listItem.Key, listItem.Value);
					}
				}
			}
			if (this.ForEachItem != null)
			{
				if (Items != null)
				{
					foreach (var item in Items)
					{
						sb.Append(ForEachItem(item));
					}
				}
			}
            sb.Append("</tbody>");
            sb.Append("</table>");

            return sb.ToString();
        }
    }
}