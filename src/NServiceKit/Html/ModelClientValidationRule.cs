// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NServiceKit.Html
{
#if NET_4_0
    [TypeForwardedFrom("System.Web.Mvc, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35")]
#endif
    /// <summary>A model client validation rule.</summary>
    public class ModelClientValidationRule
    {
        private readonly Dictionary<string, object> _validationParameters = new Dictionary<string, object>();
        private string _validationType;

        /// <summary>Gets or sets a message describing the error.</summary>
        ///
        /// <value>A message describing the error.</value>
        public string ErrorMessage { get; set; }

        /// <summary>Gets options for controlling the validation.</summary>
        ///
        /// <value>Options that control the validation.</value>
        public IDictionary<string, object> ValidationParameters
        {
            get { return _validationParameters; }
        }

        /// <summary>Gets or sets the type of the validation.</summary>
        ///
        /// <value>The type of the validation.</value>
        public string ValidationType
        {
            get { return _validationType ?? String.Empty; }
            set { _validationType = value; }
        }
    }
}
