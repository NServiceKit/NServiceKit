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
    using System.Collections.Generic;
    using System.Linq;
    using Resources;
    using Results;

    /// <summary>A delegating validator.</summary>
    public class DelegatingValidator : IPropertyValidator, IDelegatingValidator {
        private readonly Func<object, bool> condition;

        /// <summary>Gets the inner validator.</summary>
        ///
        /// <value>The inner validator.</value>
        public IPropertyValidator InnerValidator { get; private set; }

        /// <summary>Initializes a new instance of the NServiceKit.FluentValidation.Validators.DelegatingValidator class.</summary>
        ///
        /// <param name="condition">     The condition.</param>
        /// <param name="innerValidator">The inner validator.</param>
        public DelegatingValidator(Func<object, bool> condition, IPropertyValidator innerValidator) {
            this.condition = condition;
            InnerValidator = innerValidator;
        }

        /// <summary>Gets or sets the error message source.</summary>
        ///
        /// <value>The error message source.</value>
        public IStringSource ErrorMessageSource {
            get { return InnerValidator.ErrorMessageSource; }
            set { InnerValidator.ErrorMessageSource = value; }
        }

        /// <summary>Gets or sets the error code.</summary>
        ///
        /// <value>The error code.</value>
        public string ErrorCode
        {
            get { return InnerValidator.ErrorCode; }
            set { InnerValidator.ErrorCode = value; }
        }

        /// <summary>Enumerates validate in this collection.</summary>
        ///
        /// <param name="context">The context.</param>
        ///
        /// <returns>An enumerator that allows foreach to be used to process validate in this collection.</returns>
        public IEnumerable<ValidationFailure> Validate(PropertyValidatorContext context) {
            if (condition(context.Instance)) {
                return InnerValidator.Validate(context);
            }
            return Enumerable.Empty<ValidationFailure>();
        }

        /// <summary>Gets the custom message format arguments.</summary>
        ///
        /// <value>The custom message format arguments.</value>
        public ICollection<Func<object, object>> CustomMessageFormatArguments {
            get { return InnerValidator.CustomMessageFormatArguments; }
        }

        /// <summary>Gets a value indicating whether the supports standalone validation.</summary>
        ///
        /// <value>true if supports standalone validation, false if not.</value>
        public bool SupportsStandaloneValidation {
            get { return false; }
        }

        /// <summary>Gets or sets the custom state provider.</summary>
        ///
        /// <value>The custom state provider.</value>
        public Func<object, object> CustomStateProvider {
            get { return InnerValidator.CustomStateProvider; }
            set { InnerValidator.CustomStateProvider = value; }
        }

        IPropertyValidator IDelegatingValidator.InnerValidator {
            get { return InnerValidator; }
        }
    }

    /// <summary>Interface for delegating validator.</summary>
    public interface IDelegatingValidator : IPropertyValidator {

        /// <summary>Gets the inner validator.</summary>
        ///
        /// <value>The inner validator.</value>
        IPropertyValidator InnerValidator { get; }
    }
}