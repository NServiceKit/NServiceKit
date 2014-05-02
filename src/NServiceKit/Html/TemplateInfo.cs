// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace NServiceKit.Html
{
    /// <summary>Information about the template.</summary>
    public class TemplateInfo
    {
        private string _htmlFieldPrefix;
        private object _formattedModelValue;
        private HashSet<object> _visitedObjects;

        /// <summary>Gets or sets the formatted model value.</summary>
        ///
        /// <value>The formatted model value.</value>
        public object FormattedModelValue
        {
            get { return _formattedModelValue ?? String.Empty; }
            set { _formattedModelValue = value; }
        }

        /// <summary>Gets or sets the HTML field prefix.</summary>
        ///
        /// <value>The HTML field prefix.</value>
        public string HtmlFieldPrefix
        {
            get { return _htmlFieldPrefix ?? String.Empty; }
            set { _htmlFieldPrefix = value; }
        }

        /// <summary>Gets the depth of the template.</summary>
        ///
        /// <value>The depth of the template.</value>
        public int TemplateDepth
        {
            get { return VisitedObjects.Count; }
        }

        // DDB #224750 - Keep a collection of visited objects to prevent infinite recursion
        internal HashSet<object> VisitedObjects
        {
            get
            {
                if (_visitedObjects == null) {
                    _visitedObjects = new HashSet<object>();
                }
                return _visitedObjects;
            }
            set { _visitedObjects = value; }
        }

        /// <summary>Gets full HTML field identifier.</summary>
        ///
        /// <param name="partialFieldName">Name of the partial field.</param>
        ///
        /// <returns>The full HTML field identifier.</returns>
        public string GetFullHtmlFieldId(string partialFieldName)
        {
            return HtmlHelper.GenerateIdFromName(GetFullHtmlFieldName(partialFieldName));
        }

        /// <summary>Gets full HTML field name.</summary>
        ///
        /// <param name="partialFieldName">Name of the partial field.</param>
        ///
        /// <returns>The full HTML field name.</returns>
        public string GetFullHtmlFieldName(string partialFieldName)
        {
            // This uses "combine and trim" because either or both of these values might be empty
            return (HtmlFieldPrefix + "." + (partialFieldName ?? String.Empty)).Trim('.');
        }

        /// <summary>Visited the given metadata.</summary>
        ///
        /// <param name="metadata">The metadata.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        public bool Visited(ModelMetadata metadata)
        {
            return VisitedObjects.Contains(metadata.Model ?? metadata.ModelType);
        }
    }
}
