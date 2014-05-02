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
    using System.Linq.Expressions;
    using Attributes;
    using Resources;

    /// <summary>A length validator.</summary>
    public class LengthValidator : PropertyValidator, ILengthValidator {

        /// <summary>Gets the minimum.</summary>
        ///
        /// <value>The minimum value.</value>
        public int Min { get; private set; }

        /// <summary>Gets the maximum.</summary>
        ///
        /// <value>The maximum value.</value>
        public int Max { get; private set; }

        /// <summary>Initializes a new instance of the NServiceKit.FluentValidation.Validators.LengthValidator class.</summary>
        ///
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        public LengthValidator(int min, int max) : this(min, max, () => Messages.length_error) {
        }

        /// <summary>Initializes a new instance of the NServiceKit.FluentValidation.Validators.LengthValidator class.</summary>
        ///
        /// <exception cref="ArgumentOutOfRangeException">Thrown when one or more arguments are outside the required range.</exception>
        ///
        /// <param name="min">                         The minimum.</param>
        /// <param name="max">                         The maximum.</param>
        /// <param name="errorMessageResourceSelector">The error message resource selector.</param>
        public LengthValidator(int min, int max, Expression<Func<string>> errorMessageResourceSelector) : base(errorMessageResourceSelector, ValidationErrors.Length) {
            Max = max;
            Min = min;

            if (max < min) {
                throw new ArgumentOutOfRangeException("max", "Max should be larger than min.");
            }

        }

        /// <summary>Query if 'context' is valid.</summary>
        ///
        /// <param name="context">The context.</param>
        ///
        /// <returns>true if valid, false if not.</returns>
        protected override bool IsValid(PropertyValidatorContext context) {
            int length = context.PropertyValue == null ? 0 : context.PropertyValue.ToString().Length;

            if (length < Min || length > Max) {
                context.MessageFormatter
                    .AppendArgument("MinLength", Min)
                    .AppendArgument("MaxLength", Max)
                    .AppendArgument("TotalLength", length);

                return false;
            }

            return true;
        }
    }

    /// <summary>An exact length validator.</summary>
    public class ExactLengthValidator : LengthValidator {

        /// <summary>Initializes a new instance of the NServiceKit.FluentValidation.Validators.ExactLengthValidator class.</summary>
        ///
        /// <param name="length">The length.</param>
        public ExactLengthValidator(int length) : base(length,length, () => Messages.exact_length_error) {
            
        }
    }

    /// <summary>Interface for length validator.</summary>
    public interface ILengthValidator : IPropertyValidator {

        /// <summary>Gets the minimum.</summary>
        ///
        /// <value>The minimum value.</value>
        int Min { get; }

        /// <summary>Gets the maximum.</summary>
        ///
        /// <value>The maximum value.</value>
        int Max { get; }
    }
}