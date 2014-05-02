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
    using Internal;

    /// <summary>A validation context.</summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    public class ValidationContext<T> : ValidationContext {

        /// <summary>Initializes a new instance of the NServiceKit.FluentValidation.ValidationContext&lt;T&gt; class.</summary>
        ///
        /// <param name="instanceToValidate">The instance to validate.</param>
        public ValidationContext(T instanceToValidate) : this(instanceToValidate, new PropertyChain(), new DefaultValidatorSelector()) {
            
        }

        /// <summary>Initializes a new instance of the NServiceKit.FluentValidation.ValidationContext&lt;T&gt; class.</summary>
        ///
        /// <param name="instanceToValidate">The instance to validate.</param>
        /// <param name="propertyChain">     The property chain.</param>
        /// <param name="validatorSelector"> The validator selector.</param>
        public ValidationContext(T instanceToValidate, PropertyChain propertyChain, IValidatorSelector validatorSelector)
            : base(instanceToValidate, propertyChain, validatorSelector) {

            InstanceToValidate = instanceToValidate;
        }

        /// <summary>Gets the instance to validate.</summary>
        ///
        /// <value>The instance to validate.</value>
        public new T InstanceToValidate { get; private set; }
    }

    /// <summary>A validation context.</summary>
    public class ValidationContext {

        /// <summary>Initializes a new instance of the NServiceKit.FluentValidation.ValidationContext class.</summary>
        ///
        /// <param name="instanceToValidate">The instance to validate.</param>
        public ValidationContext(object instanceToValidate)
         : this (instanceToValidate, new PropertyChain(), new DefaultValidatorSelector()){
            
        }

        /// <summary>Initializes a new instance of the NServiceKit.FluentValidation.ValidationContext class.</summary>
        ///
        /// <param name="instanceToValidate">The instance to validate.</param>
        /// <param name="propertyChain">     The property chain.</param>
        /// <param name="validatorSelector"> The validator selector.</param>
        public ValidationContext(object instanceToValidate, PropertyChain propertyChain, IValidatorSelector validatorSelector) {
            PropertyChain = new PropertyChain(propertyChain);
            InstanceToValidate = instanceToValidate;
            Selector = validatorSelector;
        }

        /// <summary>Gets the property chain.</summary>
        ///
        /// <value>The property chain.</value>
        public PropertyChain PropertyChain { get; private set; }

        /// <summary>Gets the instance to validate.</summary>
        ///
        /// <value>The instance to validate.</value>
        public object InstanceToValidate { get; private set; }

        /// <summary>Gets the selector.</summary>
        ///
        /// <value>The selector.</value>
        public IValidatorSelector Selector { get; private set; }

        /// <summary>Gets a value indicating whether this object is child context.</summary>
        ///
        /// <value>true if this object is child context, false if not.</value>
        public bool IsChildContext { get; internal set; }

        /// <summary>Makes a deep copy of this object.</summary>
        ///
        /// <param name="chain">             The chain.</param>
        /// <param name="instanceToValidate">The instance to validate.</param>
        /// <param name="selector">          The selector.</param>
        ///
        /// <returns>A copy of this object.</returns>
        public ValidationContext Clone(PropertyChain chain = null, object instanceToValidate = null, IValidatorSelector selector = null) {
            return new ValidationContext(instanceToValidate ?? this.InstanceToValidate, chain ?? this.PropertyChain, selector ?? this.Selector);
        }

        internal ValidationContext CloneForChildValidator(object instanceToValidate) {
            return new ValidationContext(instanceToValidate, PropertyChain, Selector) {
                IsChildContext = true
            };
        }
    }
}