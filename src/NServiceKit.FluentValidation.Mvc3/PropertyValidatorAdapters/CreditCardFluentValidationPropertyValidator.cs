using NServiceKit.FluentValidation.Internal;
using NServiceKit.FluentValidation.Validators;

namespace FluentValidation.Mvc
{
	using System.Collections.Generic;
	using System.Web.Mvc;

	internal class CreditCardFluentValidationPropertyValidator : FluentValidationPropertyValidator
	{
        /// <summary>Initializes a new instance of the FluentValidation.Mvc.CreditCardFluentValidationPropertyValidator class.</summary>
        ///
        /// <param name="metadata">         The metadata.</param>
        /// <param name="controllerContext">Context for the controller.</param>
        /// <param name="rule">             The rule.</param>
        /// <param name="validator">        The validator.</param>
		public CreditCardFluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, PropertyRule rule, IPropertyValidator validator)
			: base(metadata, controllerContext, rule, validator)
		{
			ShouldValidate = false;
		}

        /// <summary>When implemented in a derived class, returns metadata for client validation.</summary>
        ///
        /// <returns>The metadata for client validation.</returns>
		public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
		{
			if (!ShouldGenerateClientSideRules()) yield break;

			var formatter = new MessageFormatter().AppendPropertyName(Rule.GetDisplayName());
			string message = formatter.BuildMessage(Validator.ErrorMessageSource.GetString());

			yield return new ModelClientValidationRule {
				ValidationType = "creditcard",
				ErrorMessage = message
			};
		}
	}
}