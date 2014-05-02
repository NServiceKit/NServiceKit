using NServiceKit.FluentValidation.Internal;
using NServiceKit.FluentValidation.Resources;
using NServiceKit.FluentValidation.Validators;

namespace FluentValidation.Mvc {
	using System.Collections.Generic;
	using System.Web.Mvc;

	internal class RangeFluentValidationPropertyValidator : FluentValidationPropertyValidator {
		InclusiveBetweenValidator RangeValidator {
			get { return (InclusiveBetweenValidator)Validator; }
		}

        /// <summary>Initializes a new instance of the FluentValidation.Mvc.RangeFluentValidationPropertyValidator class.</summary>
        ///
        /// <param name="metadata">           The metadata.</param>
        /// <param name="controllerContext">  Context for the controller.</param>
        /// <param name="propertyDescription">Information describing the property.</param>
        /// <param name="validator">          The validator.</param>
		public RangeFluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, PropertyRule propertyDescription, IPropertyValidator validator) : base(metadata, controllerContext, propertyDescription, validator) {
			ShouldValidate=false;
		}

        /// <summary>When implemented in a derived class, returns metadata for client validation.</summary>
        ///
        /// <returns>The metadata for client validation.</returns>
		public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
			if (!ShouldGenerateClientSideRules()) yield break;

			var formatter = new MessageFormatter()
				.AppendPropertyName(Rule.GetDisplayName())
				.AppendArgument("From", RangeValidator.From)
				.AppendArgument("To", RangeValidator.To);

			string message = RangeValidator.ErrorMessageSource.GetString();

			if (RangeValidator.ErrorMessageSource.ResourceType == typeof(Messages)) {
				// If we're using the default resources then the mesage for length errors will have two parts, eg:
				// '{PropertyName}' must be between {From} and {To}. You entered {Value}.
				// We can't include the "Value" part of the message because this information isn't available at the time the message is constructed.
				// Instead, we'll just strip this off by finding the index of the period that separates the two parts of the message.

				message = message.Substring(0, message.IndexOf(".") + 1);
			}

			message = formatter.BuildMessage(message);

			yield return new ModelClientValidationRangeRule(message, RangeValidator.From, RangeValidator.To);
		}
	}
}