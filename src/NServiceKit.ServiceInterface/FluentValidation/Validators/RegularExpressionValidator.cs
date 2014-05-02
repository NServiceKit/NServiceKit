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

    /// <summary>A regular expression validator.</summary>
    public class RegularExpressionValidator : PropertyValidator, IRegularExpressionValidator {
        readonly string expression;
        readonly Regex regex;

        /// <summary>Initializes a new instance of the NServiceKit.FluentValidation.Validators.RegularExpressionValidator class.</summary>
        ///
        /// <param name="expression">The expression.</param>
        public RegularExpressionValidator(string expression) : base(() => Messages.regex_error, ValidationErrors.RegularExpression) {
            this.expression = expression;
            regex = new Regex(expression);

        }

        /// <summary>Query if 'context' is valid.</summary>
        ///
        /// <param name="context">The validator context.</param>
        ///
        /// <returns>true if valid, false if not.</returns>
        protected override bool IsValid(PropertyValidatorContext context) {
            if (context.PropertyValue != null && !regex.IsMatch((string)context.PropertyValue)) {
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

    /// <summary>Interface for regular expression validator.</summary>
    public interface IRegularExpressionValidator : IPropertyValidator {

        /// <summary>Gets the expression.</summary>
        ///
        /// <value>The expression.</value>
        string Expression { get; }
    }
}