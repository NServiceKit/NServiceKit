#region License
// Copyright (c) Jeremy Skinner (http://www.jeremyskinner.co.uk)
// 
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at 
// 
// http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and 
// limitations under the License.
// 
// The latest version of this file can be found at http://www.codeplex.com/FluentValidation
#endregion

namespace NServiceKit.FluentValidation.Validators
{
    using System;
    using System.Text.RegularExpressions;
    using Attributes;
    using Internal;
    using Resources;
    using Results;

    //Email regex from http://hexillion.com/samples/#Regex
    public class EmailValidator : PropertyValidator, IRegularExpressionValidator, IEmailValidator {
        private readonly Regex regex;
        const string expression = @"^(?:[\w\!\#\$\%\&\'\*\+\-\/\=\?\^\`\{\|\}\~]+\.)*[\w\!\#\$\%\&\'\*\+\-\/\=\?\^\`\{\|\}\~]+@(?:(?:(?:[a-zA-Z0-9](?:[a-zA-Z0-9\-](?!\.)){0,61}[a-zA-Z0-9]?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9\-](?!$)){0,61}[a-zA-Z0-9]?)|(?:\[(?:(?:[01]?\d{1,2}|2[0-4]\d|25[0-5])\.){3}(?:[01]?\d{1,2}|2[0-4]\d|25[0-5])\]))$";

        /// <summary>Initializes a new instance of the NServiceKit.FluentValidation.Validators.EmailValidator class.</summary>
        public EmailValidator() : base(() => Messages.email_error, ValidationErrors.Email) {
            regex = new Regex(expression, RegexOptions.IgnoreCase);
        }

        /// <summary>Query if 'context' is valid.</summary>
        ///
        /// <param name="context">The context.</param>
        ///
        /// <returns>true if valid, false if not.</returns>
        protected override bool IsValid(PropertyValidatorContext context) {
            if (context.PropertyValue == null) return true;

            if (!regex.IsMatch((string)context.PropertyValue)) {
                return false;
            }

            return true;
        }

        /// <summary>Gets the expression.</summary>
        ///
        /// <value>The expression.</value>
        public string Expression {
            get { return expression; }
        }
    }

    /// <summary>Interface for email validator.</summary>
    public interface IEmailValidator : IRegularExpressionValidator {
        
    }
}