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

namespace NServiceKit.FluentValidation.TestHelper
{
    using System;
    using System.Linq.Expressions;
    using Internal;
    using System.Linq;
    using Validators;

    /// <summary>A validation test extension.</summary>
    public static class ValidationTestExtension {

        /// <summary>An IValidator&lt;T&gt; extension method that should have validation error for.</summary>
        ///
        /// <typeparam name="T">     Generic type parameter.</typeparam>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <param name="validator"> The validator to act on.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="value">     The value.</param>
        public static void ShouldHaveValidationErrorFor<T, TValue>(this IValidator<T> validator,
                                                                   Expression<Func<T, TValue>> expression, TValue value) where T : class, new() {
            new ValidatorTester<T, TValue>(expression, validator, value).ValidateError(new T());
        }

        /// <summary>An IValidator&lt;T&gt; extension method that should have validation error for.</summary>
        ///
        /// <typeparam name="T">     Generic type parameter.</typeparam>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <param name="validator">   The validator to act on.</param>
        /// <param name="expression">  The expression.</param>
        /// <param name="objectToTest">The object to test.</param>
        public static void ShouldHaveValidationErrorFor<T, TValue>(this IValidator<T> validator, Expression<Func<T, TValue>> expression, T objectToTest) where T : class {
            var value = expression.Compile()(objectToTest);
            new ValidatorTester<T, TValue>(expression, validator, value).ValidateError(objectToTest);
        }

        /// <summary>An IValidator&lt;T&gt; extension method that should not have validation error for.</summary>
        ///
        /// <typeparam name="T">     Generic type parameter.</typeparam>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <param name="validator"> The validator to act on.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="value">     The value.</param>
        public static void ShouldNotHaveValidationErrorFor<T, TValue>(this IValidator<T> validator,
                                                                      Expression<Func<T, TValue>> expression, TValue value) where T : class, new() {
            new ValidatorTester<T, TValue>(expression, validator, value).ValidateNoError(new T());
        }

        /// <summary>An IValidator&lt;T&gt; extension method that should not have validation error for.</summary>
        ///
        /// <typeparam name="T">     Generic type parameter.</typeparam>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <param name="validator">   The validator to act on.</param>
        /// <param name="expression">  The expression.</param>
        /// <param name="objectToTest">The object to test.</param>
        public static void ShouldNotHaveValidationErrorFor<T, TValue>(this IValidator<T> validator, Expression<Func<T, TValue>> expression, T objectToTest) where T : class {
            var value = expression.Compile()(objectToTest);
            new ValidatorTester<T, TValue>(expression, validator, value).ValidateNoError(objectToTest);
        }

        /// <summary>An IValidator&lt;T&gt; extension method that should have child validator.</summary>
        ///
        /// <exception cref="ValidationTestException">Thrown when a Validation Test error condition occurs.</exception>
        ///
        /// <typeparam name="T">        Generic type parameter.</typeparam>
        /// <typeparam name="TProperty">Type of the property.</typeparam>
        /// <param name="validator">         The validator to act on.</param>
        /// <param name="expression">        The expression.</param>
        /// <param name="childValidatorType">Type of the child validator.</param>
        public static void ShouldHaveChildValidator<T, TProperty>(this IValidator<T> validator, Expression<Func<T, TProperty>> expression, Type childValidatorType) {
            var descriptor = validator.CreateDescriptor();
            var matchingValidators = descriptor.GetValidatorsForMember(expression.GetMember().Name);

            var childValidators = matchingValidators.OfType<ChildValidatorAdaptor>().Select(x => x.Validator);
            childValidators = childValidators.Concat(matchingValidators.OfType<ChildCollectionValidatorAdaptor>().Select(x => x.Validator));

            if(! childValidators.Any(x => x.GetType() == childValidatorType)) {
                throw new ValidationTestException(string.Format("Expected property '{0}' to have a child validator of type '{1}.'", expression.GetMember().Name, childValidatorType.Name));
            }
        }

    }
}