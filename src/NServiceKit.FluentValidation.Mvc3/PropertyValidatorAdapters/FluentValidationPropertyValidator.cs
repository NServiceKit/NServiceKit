using NServiceKit.FluentValidation;
using NServiceKit.FluentValidation.Internal;
using NServiceKit.FluentValidation.Validators;

namespace FluentValidation.Mvc {
	using System;
	using System.Collections.Generic;
	using System.Web.Mvc;
	using System.Linq;

    /// <summary>A fluent validation property validator.</summary>
	public class FluentValidationPropertyValidator : ModelValidator {

        /// <summary>Gets the validator.</summary>
        ///
        /// <value>The validator.</value>
		public IPropertyValidator Validator { get; private set; }

        /// <summary>Gets the rule.</summary>
        ///
        /// <value>The rule.</value>
		public PropertyRule Rule { get; private set; }


		/*
		 This might seem a bit strange, but we do *not* want to do any work in these validators.
		 They should only be used for metadata purposes.
		 This is so that the validation can be left to the actual FluentValidationModelValidator.
		 The exception to this is the Required validator - these *do* need to run standalone
		 in order to bypass MVC's "A value is required" message which cannot be turned off.
		 Basically, this is all just to bypass the bad design in ASP.NET MVC. Boo, hiss. 
		*/

        /// <summary>Gets or sets a value indicating whether we should validate.</summary>
        ///
        /// <value>true if we should validate, false if not.</value>
		protected bool ShouldValidate { get; set; }

        /// <summary>Initializes a new instance of the FluentValidation.Mvc.FluentValidationPropertyValidator class.</summary>
        ///
        /// <param name="metadata">         The metadata.</param>
        /// <param name="controllerContext">Context for the controller.</param>
        /// <param name="rule">             The rule.</param>
        /// <param name="validator">        The validator.</param>
		public FluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, PropertyRule rule, IPropertyValidator validator) : base(metadata, controllerContext) {
			this.Validator = validator;

			// Build a new rule instead of the one passed in.
			// We do this as the rule passed in will not have the correct properties defined for standalone validation.
			// We also want to ensure we copy across the CustomPropertyName and RuleSet, if specified. 
			Rule = new PropertyRule(null, x => metadata.Model, null, null, metadata.ModelType, null) {
				PropertyName = metadata.PropertyName,
				DisplayName = rule == null ? null : rule.DisplayName,
				RuleSet = rule == null ? null : rule.RuleSet
			};
		}

        /// <summary>When implemented in a derived class, validates the object.</summary>
        ///
        /// <param name="container">The container.</param>
        ///
        /// <returns>A list of validation results.</returns>
		public override IEnumerable<ModelValidationResult> Validate(object container) {
			if (ShouldValidate) {
				var fakeRule = new PropertyRule(null, x => Metadata.Model, null, null, Metadata.ModelType, null) {
					PropertyName = Metadata.PropertyName,
					DisplayName = Rule == null ? null : Rule.DisplayName,
				};

				var fakeParentContext = new ValidationContext(container);
				var context = new PropertyValidatorContext(fakeParentContext, fakeRule, Metadata.PropertyName);
				var result = Validator.Validate(context);

				foreach (var failure in result) {
					yield return new ModelValidationResult { Message = failure.ErrorMessage };
				}
			}
		}

        /// <summary>Type allows null value.</summary>
        ///
        /// <param name="type">The type.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		protected bool TypeAllowsNullValue(Type type) {
			return (!type.IsValueType || Nullable.GetUnderlyingType(type) != null);
		}

        /// <summary>Determine if we should generate client side rules.</summary>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		protected virtual bool ShouldGenerateClientSideRules() {
			var ruleSetToGenerateClientSideRules = RuleSetForClientSideMessagesAttribute.GetRuleSetsForClientValidation(ControllerContext.HttpContext);
			return ruleSetToGenerateClientSideRules.Contains(Rule.RuleSet);
		}

        /// <summary>When implemented in a derived class, returns metadata for client validation.</summary>
        ///
        /// <returns>The metadata for client validation.</returns>
		public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
			if (!ShouldGenerateClientSideRules()) return Enumerable.Empty<ModelClientValidationRule>();

			var supportsClientValidation = Validator as System.Web.Mvc.IClientValidatable;
			
			if(supportsClientValidation != null) {
				return supportsClientValidation.GetClientValidationRules(Metadata, ControllerContext);
			}

			return Enumerable.Empty<ModelClientValidationRule>();
		}
	}
}