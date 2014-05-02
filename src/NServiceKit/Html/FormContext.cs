// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using NServiceKit.Text;

namespace NServiceKit.Html
{
    /// <summary>A form context.</summary>
    public class FormContext
    {
        private readonly Dictionary<string, FieldValidationMetadata> _fieldValidators = new Dictionary<string, FieldValidationMetadata>();
        private readonly Dictionary<string, bool> _renderedFields = new Dictionary<string, bool>();

        /// <summary>Gets the field validators.</summary>
        ///
        /// <value>The field validators.</value>
        public IDictionary<string, FieldValidationMetadata> FieldValidators
        {
            get { return _fieldValidators; }
        }

        /// <summary>Gets or sets the identifier of the form.</summary>
        ///
        /// <value>The identifier of the form.</value>
        public string FormId { get; set; }

        /// <summary>Gets or sets a value indicating whether the replace validation summary.</summary>
        ///
        /// <value>true if replace validation summary, false if not.</value>
        public bool ReplaceValidationSummary { get; set; }

        /// <summary>Gets or sets the identifier of the validation summary.</summary>
        ///
        /// <value>The identifier of the validation summary.</value>
        public string ValidationSummaryId { get; set; }

        /// <summary>Gets JSON validation metadata.</summary>
        ///
        /// <returns>The JSON validation metadata.</returns>
        public string GetJsonValidationMetadata()
        {
            SortedDictionary<string, object> dict = new SortedDictionary<string, object>()
            {
                { "Fields", FieldValidators.Values },
                { "FormId", FormId }
            };
            if (!String.IsNullOrEmpty(ValidationSummaryId)) {
                dict["ValidationSummaryId"] = ValidationSummaryId;
            }
            dict["ReplaceValidationSummary"] = ReplaceValidationSummary;

            return JsonSerializer.SerializeToString(dict);
        }

        /// <summary>Gets validation metadata for field.</summary>
        ///
        /// <param name="fieldName">Name of the field.</param>
        ///
        /// <returns>The validation metadata for field.</returns>
        public FieldValidationMetadata GetValidationMetadataForField(string fieldName)
        {
            return GetValidationMetadataForField(fieldName, false /* createIfNotFound */);
        }

        /// <summary>Gets validation metadata for field.</summary>
        ///
        /// <exception>Thrown when a parameter cannot be null or empty error condition occurs.</exception>
        ///
        /// <param name="fieldName">       Name of the field.</param>
        /// <param name="createIfNotFound">true to create if not found.</param>
        ///
        /// <returns>The validation metadata for field.</returns>
        public FieldValidationMetadata GetValidationMetadataForField(string fieldName, bool createIfNotFound)
        {
            if (String.IsNullOrEmpty(fieldName)) {
                throw Error.ParameterCannotBeNullOrEmpty("fieldName");
            }

            FieldValidationMetadata metadata;
            if (!FieldValidators.TryGetValue(fieldName, out metadata)) {
                if (createIfNotFound) {
                    metadata = new FieldValidationMetadata()
                    {
                        FieldName = fieldName
                    };
                    FieldValidators[fieldName] = metadata;
                }
            }
            return metadata;
        }

        /// <summary>Rendered field.</summary>
        ///
        /// <param name="fieldName">Name of the field.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        public bool RenderedField(string fieldName)
        {
            bool result;
            _renderedFields.TryGetValue(fieldName, out result);
            return result;
        }

        /// <summary>Rendered field.</summary>
        ///
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="value">    true to value.</param>
        public void RenderedField(string fieldName, bool value)
        {
            _renderedFields[fieldName] = value;
        }
    }
}
