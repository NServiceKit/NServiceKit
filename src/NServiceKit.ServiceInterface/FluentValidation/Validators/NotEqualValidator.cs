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
    using System.Collections;
    using System.Reflection;
    using Attributes;
    using Internal;
    using Resources;

    /// <summary>A not equal validator.</summary>
    public class NotEqualValidator : PropertyValidator, IComparisonValidator {
        readonly IEqualityComparer comparer;
        readonly Func<object, object> func;

        /// <summary>Initializes a new instance of the NServiceKit.FluentValidation.Validators.NotEqualValidator class.</summary>
        ///
        /// <param name="func">           The function.</param>
        /// <param name="memberToCompare">The member to compare.</param>
        public NotEqualValidator(Func<object, object> func, MemberInfo memberToCompare)
            : base(() => Messages.notequal_error, ValidationErrors.NotEqual) {
            this.func = func;
            MemberToCompare = memberToCompare;
        }

        /// <summary>Initializes a new instance of the NServiceKit.FluentValidation.Validators.NotEqualValidator class.</summary>
        ///
        /// <param name="func">            The function.</param>
        /// <param name="memberToCompare"> The member to compare.</param>
        /// <param name="equalityComparer">The equality comparer.</param>
        public NotEqualValidator(Func<object, object> func, MemberInfo memberToCompare, IEqualityComparer equalityComparer)
            : base(() => Messages.notequal_error, ValidationErrors.NotEqual)
        {
            this.func = func;
            this.comparer = equalityComparer;
            MemberToCompare = memberToCompare;
        }

        /// <summary>Initializes a new instance of the NServiceKit.FluentValidation.Validators.NotEqualValidator class.</summary>
        ///
        /// <param name="comparisonValue">Object to be compared.</param>
        public NotEqualValidator(object comparisonValue)
            : base(() => Messages.notequal_error, ValidationErrors.NotEqual)
        {
            ValueToCompare = comparisonValue;
        }

        /// <summary>Initializes a new instance of the NServiceKit.FluentValidation.Validators.NotEqualValidator class.</summary>
        ///
        /// <param name="comparisonValue"> Object to be compared.</param>
        /// <param name="equalityComparer">The equality comparer.</param>
        public NotEqualValidator(object comparisonValue, IEqualityComparer equalityComparer)
            : base(() => Messages.notequal_error, ValidationErrors.NotEqual)
        {
            ValueToCompare = comparisonValue;
            comparer = equalityComparer;
        }

        /// <summary>Query if 'context' is valid.</summary>
        ///
        /// <param name="context">The context.</param>
        ///
        /// <returns>true if valid, false if not.</returns>
        protected override bool IsValid(PropertyValidatorContext context) {
            var comparisonValue = GetComparisonValue(context);
            bool success = !Compare(comparisonValue, context.PropertyValue);

            if (!success) {
                context.MessageFormatter.AppendArgument("PropertyValue", context.PropertyValue);
                return false;
            }

            return true;
        }

        private object GetComparisonValue(PropertyValidatorContext context) {
            if (func != null) {
                return func(context.Instance);
            }

            return ValueToCompare;
        }

        /// <summary>Gets the comparison.</summary>
        ///
        /// <value>The comparison.</value>
        public Comparison Comparison {
            get { return Comparison.NotEqual; }
        }

        /// <summary>Gets the member to compare.</summary>
        ///
        /// <value>The member to compare.</value>
        public MemberInfo MemberToCompare { get; private set; }

        /// <summary>Gets the value to compare.</summary>
        ///
        /// <value>The value to compare.</value>
        public object ValueToCompare { get; private set; }

        /// <summary>Compares two object objects to determine their relative ordering.</summary>
        ///
        /// <param name="comparisonValue">Object to be compared.</param>
        /// <param name="propertyValue">  Object to be compared.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        protected bool Compare(object comparisonValue, object propertyValue) {
            if(comparer != null) {
                return comparer.Equals(comparisonValue, propertyValue);
            }
            return Equals(comparisonValue, propertyValue);
        }
    }
}