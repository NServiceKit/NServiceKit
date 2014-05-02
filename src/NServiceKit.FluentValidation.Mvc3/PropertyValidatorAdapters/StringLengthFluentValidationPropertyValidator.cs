using NServiceKit.FluentValidation.Internal;
using NServiceKit.FluentValidation.Resources;
using NServiceKit.FluentValidation.Validators;

namespace FluentValidation.Mvc {
	using System.Collections.Generic;
	using System.Web.Mvc;

	internal class StringLengthFluentValidationPropertyValidator : FluentValidationPropertyValidator {
		private ILengthValidator LengthValidator {
			get { return (ILengthValidator)Validator; }
		}

        /// <summary>Initializes a new instance of the FluentValidation.Mvc.StringLengthFluentValidationPropertyValidator class.</summary>
        ///
        /// <param name="metadata">         The metadata.</param>
        /// <param name="controllerContext">Context for the controller.</param>
        /// <param name="rule">             The rule.</param>
        /// <param name="validator">        The validator.</param>
		public StringLengthFluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, PropertyRule rule, IPropertyValidator validator)
			: base(metadata, controllerContext, rule, validator) {
			ShouldValidate = false;
		}

        /// <summary>When implemented in a derived class, returns metadata for client validation.</summary>
        ///
        /// <returns>The metadata for client validation.</returns>
		public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
			if(!ShouldGenerateClientSideRules()) yield break;

			var formatter = new MessageFormatter()
				.AppendPropertyName(Rule.GetDisplayName())
				.AppendArgument("MinLength", LengthValidator.Min)
				.AppendArgument("MaxLength", LengthValidator.Max);

			string message = LengthValidator.ErrorMessageSource.GetString();

			if(LengthValidator.ErrorMessageSource.ResourceType == typeof(Messages)) {
				// If we're using the default resources then the mesage for length errors will have two parts, eg:
				// '{PropertyName}' must be between {MinLength} and {MaxLength} characters. You entered {TotalLength} characters.
				// We can't include the "TotalLength" part of the message because this information isn't available at the time the message is constructed.
				// Instead, we'll just strip this off by finding the index of the period that separates the two parts of the message.

				message = message.Substring(0, message.IndexOf(".") + 1);
			}

			message = formatter.BuildMessage(message);

			yield return new ModelClientValidationStringLengthRule(message, LengthValidator.Min, LengthValidator.Max) ;
		}
	}
}