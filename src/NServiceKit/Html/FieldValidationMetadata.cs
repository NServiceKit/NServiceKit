// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NServiceKit.Html
{
    /// <summary>A field validation metadata.</summary>
    public class FieldValidationMetadata
    {
        private readonly Collection<ModelClientValidationRule> _validationRules = new Collection<ModelClientValidationRule>();
        private string _fieldName;

        /// <summary>Gets or sets the name of the field.</summary>
        ///
        /// <value>The name of the field.</value>
        public string FieldName
        {
            get { return _fieldName ?? String.Empty; }
            set { _fieldName = value; }
        }

        /// <summary>Gets or sets a value indicating whether the replace validation message contents.</summary>
        ///
        /// <value>true if replace validation message contents, false if not.</value>
        public bool ReplaceValidationMessageContents { get; set; }

        /// <summary>Gets or sets the identifier of the validation message.</summary>
        ///
        /// <value>The identifier of the validation message.</value>
        public string ValidationMessageId { get; set; }

        /// <summary>Gets the validation rules.</summary>
        ///
        /// <value>The validation rules.</value>
        public ICollection<ModelClientValidationRule> ValidationRules
        {
            get { return _validationRules; }
        }
    }
}
