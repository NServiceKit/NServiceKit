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

namespace NServiceKit.FluentValidation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Internal;
    using Validators;

    /// <summary>
    /// Used for providing metadata about a validator.
    /// </summary>
    public class ValidatorDescriptor<T> : IValidatorDescriptor {

        /// <summary>Gets the rules.</summary>
        ///
        /// <value>The rules.</value>
        protected IEnumerable<IValidationRule> Rules { get; private set; }

        /// <summary>Initializes a new instance of the NServiceKit.FluentValidation.ValidatorDescriptor&lt;T&gt; class.</summary>
        ///
        /// <param name="ruleBuilders">The rule builders.</param>
        public ValidatorDescriptor(IEnumerable<IValidationRule> ruleBuilders) {
            Rules = ruleBuilders;
        }

        /// <summary>Gets the name display name for a property.</summary>
        ///
        /// <param name="property">The property.</param>
        ///
        /// <returns>The name.</returns>
        public virtual string GetName(string property) {
            var nameUsed = Rules
                .OfType<PropertyRule>()
                .Where(x => x.Member.Name == property)
                .Select(x => x.GetDisplayName()).FirstOrDefault();

            return nameUsed;
        }

        /// <summary>Gets a collection of validators grouped by property.</summary>
        ///
        /// <returns>The members with validators.</returns>
        public virtual ILookup<string, IPropertyValidator> GetMembersWithValidators() {
            var query = from rule in Rules.OfType<PropertyRule>()
                        where rule.Member != null
                        from validator in rule.Validators
                        select new { memberName = rule.Member.Name, validator };

            return query.ToLookup(x => x.memberName, x => x.validator);
        }

        /// <summary>Gets validators for a particular property.</summary>
        ///
        /// <param name="name">The name.</param>
        ///
        /// <returns>An enumerator that allows foreach to be used to process the validators for members in this collection.</returns>
        public IEnumerable<IPropertyValidator> GetValidatorsForMember(string name) {
            return GetMembersWithValidators()[name];
        }

        /// <summary>Gets rules for a property.</summary>
        ///
        /// <param name="name">The name.</param>
        ///
        /// <returns>An enumerator that allows foreach to be used to process the rules for members in this collection.</returns>
        public IEnumerable<IValidationRule> GetRulesForMember(string name) {
            var query = from rule in Rules.OfType<PropertyRule>()
                        where rule.Member.Name == name
                        select (IValidationRule)rule;

            return query.ToList();
        }

        /// <summary>Gets a name.</summary>
        ///
        /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or illegal values.</exception>
        ///
        /// <param name="propertyExpression">The property expression.</param>
        ///
        /// <returns>The name.</returns>
        public virtual string GetName(Expression<Func<T, object>> propertyExpression) {
            var member = propertyExpression.GetMember();

            if (member == null) {
                throw new ArgumentException(string.Format("Cannot retrieve name as expression '{0}' as it does not specify a property.", propertyExpression));
            }

            return GetName(member.Name);
        }
    }
}