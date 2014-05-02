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
// The latest version of this file can be found at http://fluentvalidation.codeplex.com
#endregion

using NServiceKit.FluentValidation.Internal;

namespace FluentValidation.Mvc {
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Web.Mvc;

    /// <summary>Attribute for customize validator.</summary>
	public class CustomizeValidatorAttribute : CustomModelBinderAttribute, IModelBinder {

        /// <summary>Gets or sets the set the rule belongs to.</summary>
        ///
        /// <value>The rule set.</value>
		public string RuleSet { get; set; }

        /// <summary>Gets or sets the properties.</summary>
        ///
        /// <value>The properties.</value>
		public string Properties { get; set; }

        /// <summary>Gets or sets the interceptor.</summary>
        ///
        /// <value>The interceptor.</value>
		public Type Interceptor { get; set; }

		private const string key = "_FV_CustomizeValidator" ;

        /// <summary>Retrieves the associated model binder.</summary>
        ///
        /// <returns>A reference to an object that implements the <see cref="T:System.Web.Mvc.IModelBinder" /> interface.</returns>
		public override IModelBinder GetBinder() {
			return this;
		}

        /// <summary>Binds the model to a value by using the specified controller context and binding context.</summary>
        ///
        /// <param name="controllerContext">The controller context.</param>
        /// <param name="bindingContext">   The binding context.</param>
        ///
        /// <returns>The bound value.</returns>
		public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
			// Originally I thought about storing this inside ModelMetadata.AdditionalValues.
			// Unfortunately, DefaultModelBinder overwrites this property internally.
			// So anything added to AdditionalValues will not be passed to the ValidatorProvider.
			// This is a very poor design decision. 
			// The only piece of information that is passed all the way down to the validator is the controller context.
			// So we resort to storing the attribute in HttpContext.Items. 
			// Horrible, horrible, horrible hack. Horrible.
			controllerContext.HttpContext.Items[key] = this;

			var innerBinder = ModelBinders.Binders.GetBinder(bindingContext.ModelType);
			var result = innerBinder.BindModel(controllerContext, bindingContext);

			controllerContext.HttpContext.Items.Remove(key);

			return result;
		}

        /// <summary>Gets from controller context.</summary>
        ///
        /// <param name="context">The context.</param>
        ///
        /// <returns>The data that was read from the controller context.</returns>
		public static CustomizeValidatorAttribute GetFromControllerContext(ControllerContext context) {
			return context.HttpContext.Items[key] as CustomizeValidatorAttribute;
		}

		/// <summary>
		/// Builds a validator selector from the options specified in the attribute's properties.
		/// </summary>
		public IValidatorSelector ToValidatorSelector() {
			IValidatorSelector selector;

			if(! string.IsNullOrEmpty(RuleSet)) {
				var rulesets = RuleSet.Split(',', ';');
				selector = new RulesetValidatorSelector(rulesets);
			}
			else if(! string.IsNullOrEmpty(Properties)) {
				var properties = Properties.Split(',', ';');
				selector = new MemberNameValidatorSelector(properties);
			}
			else {
				selector = new DefaultValidatorSelector();
			}

			return selector;

		}

        /// <summary>Gets the interceptor.</summary>
        ///
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        ///
        /// <returns>The interceptor.</returns>
		public IValidatorInterceptor GetInterceptor() {
			if (Interceptor == null) return null;

			if(! typeof(IValidatorInterceptor).IsAssignableFrom(Interceptor)) {
				throw new InvalidOperationException("Type {0} is not an IValidatorInterceptor. The Interceptor property of CustomizeValidatorAttribute must implement IValidatorInterceptor.");
			}

			var instance = Activator.CreateInstance(Interceptor) as IValidatorInterceptor;

			if(instance == null) {
				throw new InvalidOperationException("Type {0} is not an IValidatorInterceptor. The Interceptor property of CustomizeValidatorAttribute must implement IValidatorInterceptor.");
			}

			return instance;
		}
	}
}