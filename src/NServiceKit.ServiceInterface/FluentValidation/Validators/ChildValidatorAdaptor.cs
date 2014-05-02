namespace NServiceKit.FluentValidation.Validators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Internal;
    using Results;

    /// <summary>A child validator adaptor.</summary>
    public class ChildValidatorAdaptor : NoopPropertyValidator {
        readonly IValidator validator;

        /// <summary>Gets the validator.</summary>
        ///
        /// <value>The validator.</value>
        public IValidator Validator {
            get { return validator; }
        }

        /// <summary>Initializes a new instance of the NServiceKit.FluentValidation.Validators.ChildValidatorAdaptor class.</summary>
        ///
        /// <param name="validator">The validator.</param>
        public ChildValidatorAdaptor(IValidator validator) {
            this.validator = validator;
        }

        /// <summary>Enumerates validate in this collection.</summary>
        ///
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        ///
        /// <param name="context">The context.</param>
        ///
        /// <returns>An enumerator that allows foreach to be used to process validate in this collection.</returns>
        public override IEnumerable<ValidationFailure> Validate(PropertyValidatorContext context) {
            if (context.Rule.Member == null) {
                throw new InvalidOperationException(string.Format("Nested validators can only be used with Member Expressions."));
            }

            var instanceToValidate = context.PropertyValue;

            if (instanceToValidate == null) {
                return Enumerable.Empty<ValidationFailure>();
            }

            var validator = GetValidator(context);

            if(validator == null) {
                return Enumerable.Empty<ValidationFailure>();
            }

            var newContext = CreateNewValidationContextForChildValidator(instanceToValidate, context);
            var results = validator.Validate(newContext).Errors;

            return results;
        }

        /// <summary>Gets a validator.</summary>
        ///
        /// <param name="context">The context.</param>
        ///
        /// <returns>The validator.</returns>
        protected virtual IValidator GetValidator(PropertyValidatorContext context) {
            return Validator;
        }

        /// <summary>Creates new validation context for child validator.</summary>
        ///
        /// <param name="instanceToValidate">The instance to validate.</param>
        /// <param name="context">           The context.</param>
        ///
        /// <returns>The new new validation context for child validator.</returns>
        protected ValidationContext CreateNewValidationContextForChildValidator(object instanceToValidate, PropertyValidatorContext context) {
            var newContext = context.ParentContext.CloneForChildValidator(instanceToValidate);
            newContext.PropertyChain.Add(context.Rule.Member);
            return newContext;
        }
    }
}