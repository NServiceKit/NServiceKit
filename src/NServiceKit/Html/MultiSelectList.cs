// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI;

namespace NServiceKit.Html
{
    /// <summary>List of multi selects.</summary>
    public class MultiSelectList : IEnumerable<SelectListItem>
    {
        /// <summary>Initializes a new instance of the NServiceKit.Html.MultiSelectList class.</summary>
        ///
        /// <param name="items">The items.</param>
        public MultiSelectList(IEnumerable items)
            : this(items, null /* selectedValues */)
        {
        }

        /// <summary>Initializes a new instance of the NServiceKit.Html.MultiSelectList class.</summary>
        ///
        /// <param name="items">         The items.</param>
        /// <param name="selectedValues">The selected values.</param>
        public MultiSelectList(IEnumerable items, IEnumerable selectedValues)
            : this(items, null /* dataValuefield */, null /* dataTextField */, selectedValues)
        {
        }

        /// <summary>Initializes a new instance of the NServiceKit.Html.MultiSelectList class.</summary>
        ///
        /// <param name="items">         The items.</param>
        /// <param name="dataValueField">The data value field.</param>
        /// <param name="dataTextField"> The data text field.</param>
        public MultiSelectList(IEnumerable items, string dataValueField, string dataTextField)
            : this(items, dataValueField, dataTextField, null /* selectedValues */)
        {
        }

        /// <summary>Initializes a new instance of the NServiceKit.Html.MultiSelectList class.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="items">         The items.</param>
        /// <param name="dataValueField">The data value field.</param>
        /// <param name="dataTextField"> The data text field.</param>
        /// <param name="selectedValues">The selected values.</param>
        public MultiSelectList(IEnumerable items, string dataValueField, string dataTextField, IEnumerable selectedValues)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            Items = items;
            DataValueField = dataValueField;
            DataTextField = dataTextField;
            SelectedValues = selectedValues;
        }

        /// <summary>Gets the data text field.</summary>
        ///
        /// <value>The data text field.</value>
        public string DataTextField { get; private set; }

        /// <summary>Gets the data value field.</summary>
        ///
        /// <value>The data value field.</value>
        public string DataValueField { get; private set; }

        /// <summary>Gets the items.</summary>
        ///
        /// <value>The items.</value>
        public IEnumerable Items { get; private set; }

        /// <summary>Gets the selected values.</summary>
        ///
        /// <value>The selected values.</value>
        public IEnumerable SelectedValues { get; private set; }

        /// <summary>Gets the enumerator.</summary>
        ///
        /// <returns>The enumerator.</returns>
        public virtual IEnumerator<SelectListItem> GetEnumerator()
        {
            return GetListItems().GetEnumerator();
        }

        internal IList<SelectListItem> GetListItems()
        {
            return (!String.IsNullOrEmpty(DataValueField))
                       ? GetListItemsWithValueField()
                       : GetListItemsWithoutValueField();
        }

        private IList<SelectListItem> GetListItemsWithValueField()
        {
            HashSet<string> selectedValues = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (SelectedValues != null)
            {
                selectedValues.UnionWith(from object value in SelectedValues
                                         select Convert.ToString(value, CultureInfo.CurrentCulture));
            }

            var listItems = from object item in Items
                            let value = Eval(item, DataValueField)
                            select new SelectListItem
                            {
                                Value = value,
                                Text = Eval(item, DataTextField),
                                Selected = selectedValues.Contains(value)
                            };
            return listItems.ToList();
        }

        private IList<SelectListItem> GetListItemsWithoutValueField()
        {
            HashSet<object> selectedValues = new HashSet<object>();
            if (SelectedValues != null)
            {
                selectedValues.UnionWith(SelectedValues.Cast<object>());
            }

            var listItems = from object item in Items
                            select new SelectListItem
                            {
                                Text = Eval(item, DataTextField),
                                Selected = selectedValues.Contains(item)
                            };
            return listItems.ToList();
        }

        private static string Eval(object container, string expression)
        {
            object value = container;
            if (!String.IsNullOrEmpty(expression))
            {
                value = DataBinder.Eval(container, expression);
            }
            return Convert.ToString(value, CultureInfo.CurrentCulture);
        }

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
