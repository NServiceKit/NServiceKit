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
    using System.Linq.Expressions;
    using Resources;
    using Results;

    /// <summary>A property validator.</summary>
    public abstract class PropertyValidator : IPropertyValidator {
        private readonly List<Func<object, object>> customFormatArgs = new List<Func<object, object>>();
        private IStringSource errorSource;
        private string errorCode;

        /// <summary>Gets or sets the custom state provider.</summary>
        ///
        /// <value>The custom state provider.</value>
        public Func<object, object> CustomStateProvider { get; set; }

        /// <summary>Gets the custom message format arguments.</summary>
        ///
        /// <value>The custom message format arguments.</value>
        public ICollection<Func<object, object>> CustomMessageFormatArguments {
            get { return customFormatArgs; }
        }

        /// <summary>Initializes a new instance of the NServiceKit.FluentValidation.Validators.PropertyValidator class.</summary>
        ///
        /// <param name="errorMessageResourceName">Name of the error message resource.</param>
        /// <param name="errorMessageResourceType">Type of the error message resource.</param>
        /// <param name="errorCode">               The error code.</param>
        protected PropertyValidator(string errorMessageResourceName, Type errorMessageResourceType, string errorCode) {
            this.errorSource = new LocalizedStringSource(errorMessageResourceType, errorMessageResourceName, new FallbackAwareResourceAccessorBuilder());
            this.errorCode = errorCode;
        }

        /// <summary>Initializes a new instance of the NServiceKit.FluentValidation.Validators.PropertyValidator class.</summary>
        ///
        /// <param name="errorMessage">Message describing the error.</param>
        /// <param name="errorCode">   The error code.</param>
        protected PropertyValidator(string errorMessage, string errorCode) {
            this.errorSource = new StaticStringSource(errorMessage);
            this.errorCode = errorCode;
        }

        /// <summary>Initializes a new instance of the NServiceKit.FluentValidation.Validators.PropertyValidator class.</summary>
        ///
        /// <param name="errorMessageResourceSelector">The error message resource selector.</param>
        /// <param name="errorCode">                   The error code.</param>
        protected PropertyValidator(Expression<Func<string>> errorMessageResourceSelector, string errorCode) {
            this.errorSource = LocalizedStringSource.CreateFromExpression(errorMessageResourceSelector, new FallbackAwareResourceAccessorBuilder());
            this.errorCode = errorCode;
        }

        /// <summary>Gets or sets the error message source.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <value>The error message source.</value>
        public IStringSource ErrorMessageSource {
            get { return errorSource; }
            set {
                if (value == null) {
                    throw new ArgumentNullException("value");
                }
                errorSource = value;
            }
        }

        /// <summary>Gets or sets the error code.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <value>The error code.</value>
        public string ErrorCode
        {
            get { return errorCode; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                errorCode = value;
            }
        }

        /// <summary>Enumerates validate in this collection.</summary>
        ///
        /// <param name="context">The context.</param>
        ///
        /// <returns>An enumerator that allows foreach to be used to process validate in this collection.</returns>
        public virtual IEnumerable<ValidationFailure> Validate(PropertyValidatorContext context) {
            context.MessageFormatter.AppendPropertyName(context.PropertyDescription);

            if (!IsValid(context)) {
                return new[] { CreateValidationError(context) };
            }

            return Enumerable.Empty<ValidationFailure>();
        }

        /// <summary>Query if 'context' is valid.</summary>
        ///
        /// <param name="context">The validator context.</param>
        ///
        /// <returns>true if valid, false if not.</returns>
        protected abstract bool IsValid(PropertyValidatorContext context);

        /// <summary>
        /// Creates an error validation result for this validator.
        /// </summary>
        /// <param name="context">The validator context</param>
        /// <returns>Returns an error validation result.</returns>
        protected virtual ValidationFailure CreateValidationError(PropertyValidatorContext context) {
            context.MessageFormatter.AppendAdditionalArguments(
                customFormatArgs.Select(func => func(context.Instance)).ToArray()
            );

            string error = context.MessageFormatter.BuildMessage(errorSource.GetString());

            var failure = new ValidationFailure(context.PropertyName, error, errorCode, context.PropertyValue);

            if (CustomStateProvider != null) {
                failure.CustomState = CustomStateProvider(context.Instance);
            }

            return failure;
        }
    }
}