using System;
using System.Collections.Generic;
using System.Text;

namespace NServiceKit.WebHost.Endpoints.Support.Templates
{
    internal class ListTemplate
    {
        /// <summary>Gets or sets the title.</summary>
        ///
        /// <value>The title.</value>
        public string Title { get; set; }

        /// <summary>Gets or sets the list items.</summary>
        ///
        /// <value>The list items.</value>
        public IList<string> ListItems { get; set; }

        /// <summary>Gets or sets the list items map.</summary>
        ///
        /// <value>The list items map.</value>
        public IDictionary<string, string> ListItemsMap { get; set; }

        /// <summary>Gets or sets the list items int map.</summary>
        ///
        /// <value>The list items int map.</value>
		public IDictionary<int, string> ListItemsIntMap { get; set; }

        /// <summary>Gets or sets for each list item.</summary>
        ///
        /// <value>for each list item.</value>
		public Func<string, string> ForEachListItem { get; set; }

        /// <summary>Gets or sets the list item template.</summary>
        ///
        /// <value>The list item template.</value>
        public string ListItemTemplate { get; set; }

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
            sb.Append("<ul>");
        	if (ListItemTemplate != null)
			{
				if (ListItems != null)
				{
					foreach (var item in ListItems)
					{
						sb.AppendFormat(ListItemTemplate, item, item);
					}
				}
				if (ListItemsMap != null)
				{
					foreach (var listItem in ListItemsMap)
					{
						sb.AppendFormat(ListItemTemplate, listItem.Key, listItem.Value);
					}
				}
				if (ListItemsIntMap != null)
				{
					foreach (var listItem in ListItemsIntMap)
					{
						sb.AppendFormat(ListItemTemplate, listItem.Key, listItem.Value);
					}
				}
			}
			if (this.ForEachListItem != null)
			{
				if (ListItems != null)
				{
					foreach (var item in ListItems)
					{
						sb.Append(ForEachListItem(item));
					}
				}
			}
			sb.Append("</ul>");
            return sb.ToString();
        }
    }
}