using NServiceKit.FluentValidation.Internal;
using NServiceKit.FluentValidation.Validators;

namespace FluentValidation.Mvc {
	using System.Collections.Generic;
	using System.Reflection;
	using System.Web.Mvc;

	internal class EqualToFluentValidationPropertyValidator : FluentValidationPropertyValidator {
		EqualValidator EqualValidator {
			get { return (EqualValidator)Validator; }
		}

        /// <summary>Initializes a new instance of the FluentValidation.Mvc.EqualToFluentValidationPropertyValidator class.</summary>
        ///
        /// <param name="metadata">         The metadata.</param>
        /// <param name="controllerContext">Context for the controller.</param>
        /// <param name="rule">             The rule.</param>
        /// <param name="validator">        The validator.</param>
		public EqualToFluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, PropertyRule rule, IPropertyValidator validator) : base(metadata, controllerContext, rule, validator) {
			ShouldValidate = false;
		}

        /// <summary>When implemented in a derived class, returns metadata for client validation.</summary>
        ///
        /// <returns>The metadata for client validation.</returns>
		public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
			if (!ShouldGenerateClientSideRules()) yield break;

			var propertyToCompare = EqualValidator.MemberToCompare as PropertyInfo;
			if(propertyToCompare != null) {
				// If propertyToCompare is not null then we're comparing to another property.
				// If propertyToCompare is null then we're either comparing against a literal value, a field or a method call.
				// We only care about property comparisons in this case.

				var formatter = new MessageFormatter()
					.AppendPropertyName(Rule.GetDisplayName())
					.AppendArgument("PropertyValue", propertyToCompare.Name);


				string message = formatter.BuildMessage(EqualValidator.ErrorMessageSource.GetString());
				yield return new ModelClientValidationEqualToRule(message, CompareAttribute.FormatPropertyForClientValidation(propertyToCompare.Name)) ;
			}
		}
	}
}