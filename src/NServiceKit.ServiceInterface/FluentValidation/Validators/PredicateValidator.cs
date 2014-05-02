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
    using Internal;
    using Resources;
    using Results;

    /// <summary>A predicate validator.</summary>
    public class PredicateValidator : PropertyValidator, IPredicateValidator {

        /// <summary>Predicates.</summary>
        ///
        /// <param name="instanceToValidate">      The instance to validate.</param>
        /// <param name="propertyValue">           The property value.</param>
        /// <param name="propertyValidatorContext">Context for the property validator.</param>
        ///
        /// <returns>A bool.</returns>
        public delegate bool Predicate(object instanceToValidate, object propertyValue, PropertyValidatorContext propertyValidatorContext);

        private readonly Predicate predicate;

        /// <summary>Initializes a new instance of the NServiceKit.FluentValidation.Validators.PredicateValidator class.</summary>
        ///
        /// <param name="predicate">The predicate.</param>
        public PredicateValidator(Predicate predicate) : base(() => Messages.predicate_error, ValidationErrors.Predicate) {
            predicate.Guard("A predicate must be specified.");
            this.predicate = predicate;
        }

        /// <summary>Query if 'context' is valid.</summary>
        ///
        /// <param name="context">The context.</param>
        ///
        /// <returns>true if valid, false if not.</returns>
        protected override bool IsValid(PropertyValidatorContext context) {
            if (!predicate(context.Instance, context.PropertyValue, context)) {
                return false;
            }

            return true;
        }
    }

    /// <summary>Interface for predicate validator.</summary>
    public interface IPredicateValidator : IPropertyValidator { }
}