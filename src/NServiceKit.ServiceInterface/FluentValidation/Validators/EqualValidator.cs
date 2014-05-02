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

    /// <summary>An equal validator.</summary>
    public class EqualValidator : PropertyValidator, IComparisonValidator {
        readonly Func<object, object> func;
        readonly IEqualityComparer comparer;

        /// <summary>Initializes a new instance of the NServiceKit.FluentValidation.Validators.EqualValidator class.</summary>
        ///
        /// <param name="valueToCompare">The value to compare.</param>
        public EqualValidator(object valueToCompare) : base(() => Messages.equal_error, ValidationErrors.Equal) {
            this.ValueToCompare = valueToCompare;
        }

        /// <summary>Initializes a new instance of the NServiceKit.FluentValidation.Validators.EqualValidator class.</summary>
        ///
        /// <param name="valueToCompare">The value to compare.</param>
        /// <param name="comparer">      The comparer.</param>
        public EqualValidator(object valueToCompare, IEqualityComparer comparer)
            : base(() => Messages.equal_error, ValidationErrors.Equal)
        {
            ValueToCompare = valueToCompare;
            this.comparer = comparer;
        }

        /// <summary>Initializes a new instance of the NServiceKit.FluentValidation.Validators.EqualValidator class.</summary>
        ///
        /// <param name="comparisonProperty">The comparison property.</param>
        /// <param name="member">            The member.</param>
        public EqualValidator(Func<object, object> comparisonProperty, MemberInfo member)
            : base(() => Messages.equal_error, ValidationErrors.Equal)
        {
            func = comparisonProperty;
            MemberToCompare = member;
        }

        /// <summary>Initializes a new instance of the NServiceKit.FluentValidation.Validators.EqualValidator class.</summary>
        ///
        /// <param name="comparisonProperty">The comparison property.</param>
        /// <param name="member">            The member.</param>
        /// <param name="comparer">          The comparer.</param>
        public EqualValidator(Func<object, object> comparisonProperty, MemberInfo member, IEqualityComparer comparer)
            : base(() => Messages.equal_error, ValidationErrors.Equal)
        {
            func = comparisonProperty;
            MemberToCompare = member;
            this.comparer = comparer;
        }

        /// <summary>Query if 'context' is valid.</summary>
        ///
        /// <param name="context">The context.</param>
        ///
        /// <returns>true if valid, false if not.</returns>
        protected override bool IsValid(PropertyValidatorContext context) {
            var comparisonValue = GetComparisonValue(context);
            bool success = Compare(comparisonValue, context.PropertyValue);

            if (!success) {
                context.MessageFormatter.AppendArgument("PropertyValue", comparisonValue);
                return false;
            }

            return true;
        }

        private object GetComparisonValue(PropertyValidatorContext context) {
            if(func != null) {
                return func(context.Instance);
            }

            return ValueToCompare;
        }

        /// <summary>Gets the comparison.</summary>
        ///
        /// <value>The comparison.</value>
        public Comparison Comparison {
            get { return Comparison.Equal; }
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