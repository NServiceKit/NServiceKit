// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System.Collections;

namespace NServiceKit.Html
{
    /// <summary>List of selects.</summary>
    public class SelectList : MultiSelectList
    {
        /// <summary>Initializes a new instance of the NServiceKit.Html.SelectList class.</summary>
        ///
        /// <param name="items">The items.</param>
        public SelectList(IEnumerable items)
            : this(items, null /* selectedValue */)
        {
        }

        /// <summary>Initializes a new instance of the NServiceKit.Html.SelectList class.</summary>
        ///
        /// <param name="items">        The items.</param>
        /// <param name="selectedValue">The selected value.</param>
        public SelectList(IEnumerable items, object selectedValue)
            : this(items, null /* dataValuefield */, null /* dataTextField */, selectedValue)
        {
        }

        /// <summary>Initializes a new instance of the NServiceKit.Html.SelectList class.</summary>
        ///
        /// <param name="items">         The items.</param>
        /// <param name="dataValueField">The data value field.</param>
        /// <param name="dataTextField"> The data text field.</param>
        public SelectList(IEnumerable items, string dataValueField, string dataTextField)
            : this(items, dataValueField, dataTextField, null /* selectedValue */)
        {
        }

        /// <summary>Initializes a new instance of the NServiceKit.Html.SelectList class.</summary>
        ///
        /// <param name="items">         The items.</param>
        /// <param name="dataValueField">The data value field.</param>
        /// <param name="dataTextField"> The data text field.</param>
        /// <param name="selectedValue"> The selected value.</param>
        public SelectList(IEnumerable items, string dataValueField, string dataTextField, object selectedValue)
            : base(items, dataValueField, dataTextField, ToEnumerable(selectedValue))
        {
            SelectedValue = selectedValue;
        }

        /// <summary>Gets the selected value.</summary>
        ///
        /// <value>The selected value.</value>
        public object SelectedValue { get; private set; }

        private static IEnumerable ToEnumerable(object selectedValue)
        {
            return (selectedValue != null) ? new object[] { selectedValue } : null;
        }
    }
}
