using NServiceKit.FluentValidation.Internal;
using NServiceKit.FluentValidation.Validators;

namespace FluentValidation.Mvc {
	using System.Collections.Generic;
	using System.Web.Mvc;

	internal class RequiredFluentValidationPropertyValidator : FluentValidationPropertyValidator {

        /// <summary>Initializes a new instance of the FluentValidation.Mvc.RequiredFluentValidationPropertyValidator class.</summary>
        ///
        /// <param name="metadata">         The metadata.</param>
        /// <param name="controllerContext">Context for the controller.</param>
        /// <param name="rule">             The rule.</param>
        /// <param name="validator">        The validator.</param>
		public RequiredFluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, PropertyRule rule, IPropertyValidator validator) : base(metadata, controllerContext, rule, validator) {
			bool isNonNullableValueType = !TypeAllowsNullValue(metadata.ModelType);
			bool nullWasSpecified = metadata.Model == null;

			ShouldValidate = isNonNullableValueType && nullWasSpecified;
		}

        /// <summary>When implemented in a derived class, returns metadata for client validation.</summary>
        ///
        /// <returns>The metadata for client validation.</returns>
		public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
			if (!ShouldGenerateClientSideRules()) yield break;

			var formatter = new MessageFormatter().AppendPropertyName(Rule.GetDisplayName());
			var message = formatter.BuildMessage(Validator.ErrorMessageSource.GetString());
			yield return new ModelClientValidationRequiredRule(message);
		}

        /// <summary>Gets a value that indicates whether a model property is required.</summary>
        ///
        /// <value>true if the model property is required; otherwise, false.</value>
		public override bool IsRequired {
			get { return true; }
		}
	}
}