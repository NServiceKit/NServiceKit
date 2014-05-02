using NServiceKit.FluentValidation;
using NServiceKit.FluentValidation.Internal;
using NServiceKit.FluentValidation.Results;

namespace FluentValidation.Mvc {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web.Mvc;

	/// <summary>
	/// ModelValidator implementation that uses FluentValidation.
	/// </summary>
	internal class FluentValidationModelValidator : ModelValidator {
		readonly IValidator validator;
		readonly CustomizeValidatorAttribute customizations;

        /// <summary>Initializes a new instance of the FluentValidation.Mvc.FluentValidationModelValidator class.</summary>
        ///
        /// <param name="metadata">         The metadata.</param>
        /// <param name="controllerContext">Context for the controller.</param>
        /// <param name="validator">        The validator.</param>
		public FluentValidationModelValidator(ModelMetadata metadata, ControllerContext controllerContext, IValidator validator)
			: base(metadata, controllerContext) {
			this.validator = validator;
			
			this.customizations = CustomizeValidatorAttribute.GetFromControllerContext(controllerContext) 
				?? new CustomizeValidatorAttribute();
		}

        /// <summary>When implemented in a derived class, validates the object.</summary>
        ///
        /// <param name="container">The container.</param>
        ///
        /// <returns>A list of validation results.</returns>
		public override IEnumerable<ModelValidationResult> Validate(object container) {
			if (Metadata.Model != null) {
				var selector = customizations.ToValidatorSelector();
				var interceptor = customizations.GetInterceptor() ?? (validator as IValidatorInterceptor);
				var context = new ValidationContext(Metadata.Model, new PropertyChain(), selector);

				if(interceptor != null) {
					// Allow the user to provide a customized context
					// However, if they return null then just use the original context.
					context = interceptor.BeforeMvcValidation(ControllerContext, context) ?? context;
				}

				var result = validator.Validate(context);

				if(interceptor != null) {
					// allow the user to provice a custom collection of failures, which could be empty.
					// However, if they return null then use the original collection of failures. 
					result = interceptor.AfterMvcValidation(ControllerContext, context, result) ?? result;
				}

				if (!result.IsValid) {
					return ConvertValidationResultToModelValidationResults(result);
				}
			}
			return Enumerable.Empty<ModelValidationResult>();
		}

        /// <summary>Enumerates convert validation result to model validation results in this collection.</summary>
        ///
        /// <param name="result">The result.</param>
        ///
        /// <returns>An enumerator that allows foreach to be used to process convert validation result to model validation results in this collection.</returns>
		protected virtual IEnumerable<ModelValidationResult> ConvertValidationResultToModelValidationResults(ValidationResult result) {
			return result.Errors.Select(x => new ModelValidationResult {
				MemberName = x.PropertyName,
				Message = x.ErrorMessage
			});
		}
	}
}