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
    using Attributes;
    using Resources;

    /// <summary>An inclusive between validator.</summary>
    public class InclusiveBetweenValidator : PropertyValidator, IBetweenValidator {

        /// <summary>Initializes a new instance of the NServiceKit.FluentValidation.Validators.InclusiveBetweenValidator class.</summary>
        ///
        /// <exception cref="ArgumentOutOfRangeException">Thrown when one or more arguments are outside the required range.</exception>
        ///
        /// <param name="from">from.</param>
        /// <param name="to">  to.</param>
        public InclusiveBetweenValidator(IComparable from, IComparable to) : base(() => Messages.inclusivebetween_error, ValidationErrors.InclusiveBetween) {
            To = to;
            From = from;

            if (to.CompareTo(from) == -1) {
                throw new ArgumentOutOfRangeException("to", "To should be larger than from.");
            }

        }

        /// <summary>Gets the source for the.</summary>
        ///
        /// <value>from.</value>
        public IComparable From { get; private set; }

        /// <summary>Gets to.</summary>
        ///
        /// <value>to.</value>
        public IComparable To { get; private set; }

        /// <summary>Query if 'context' is valid.</summary>
        ///
        /// <param name="context">The context.</param>
        ///
        /// <returns>true if valid, false if not.</returns>
        protected override bool IsValid(PropertyValidatorContext context) {
            var propertyValue = (IComparable)context.PropertyValue;

            // If the value is null then we abort and assume success.
            // This should not be a failure condition - only a NotNull/NotEmpty should cause a null to fail.
            if (propertyValue == null) return true;

            if (propertyValue.CompareTo(From) < 0 || propertyValue.CompareTo(To) > 0) {

                context.MessageFormatter
                    .AppendArgument("From", From)
                    .AppendArgument("To", To)
                    .AppendArgument("Value", context.PropertyValue);

                return false;
            }
            return true;
        }
    }

    /// <summary>Interface for between validator.</summary>
    public interface IBetweenValidator : IPropertyValidator {

        /// <summary>Gets the source for the.</summary>
        ///
        /// <value>from.</value>
        IComparable From { get; }

        /// <summary>Gets to.</summary>
        ///
        /// <value>to.</value>
        IComparable To { get; }
    }
}