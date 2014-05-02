// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

namespace NServiceKit.Html
{
    /// <summary>A select list item.</summary>
    public class SelectListItem
    {
        /// <summary>Gets or sets a value indicating whether the selected.</summary>
        ///
        /// <value>true if selected, false if not.</value>
        public bool Selected { get; set; }

        /// <summary>Gets or sets the text.</summary>
        ///
        /// <value>The text.</value>
        public string Text { get; set; }

        /// <summary>Gets or sets the value.</summary>
        ///
        /// <value>The value.</value>
        public string Value { get; set; }
    }
}
