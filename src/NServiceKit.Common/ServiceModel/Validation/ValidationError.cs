using System;
using System.Collections.Generic;
using System.Text;
using NServiceKit.Common.Extensions;
using NServiceKit.Common.Utils;
using NServiceKit.ServiceInterface.ServiceModel;

namespace NServiceKit.Validation
{
    /// <summary>
    /// The exception which is thrown when a validation error occurred.
    /// This validation is serialized in a extra clean and human-readable way by NServiceKit.
    /// </summary>
    public class ValidationError : ArgumentException, IResponseStatusConvertible
    {
        private readonly string errorCode;

        /// <summary>Gets a message describing the error.</summary>
        ///
        /// <value>A message describing the error.</value>
        public string ErrorMessage { get; private set; }

        /// <summary>Initializes a new instance of the NServiceKit.Validation.ValidationError class.</summary>
        ///
        /// <param name="errorCode">The error code.</param>
        public ValidationError(string errorCode)
            : this(errorCode, errorCode.SplitCamelCase())
        {
        }

        /// <summary>Initializes a new instance of the NServiceKit.Validation.ValidationError class.</summary>
        ///
        /// <param name="validationResult">The validation result.</param>
        public ValidationError(ValidationErrorResult validationResult)
            : base(validationResult.ErrorMessage)
        {
            this.errorCode = validationResult.ErrorCode;
            this.ErrorMessage = validationResult.ErrorMessage;
            this.Violations = validationResult.Errors;
        }

        /// <summary>Initializes a new instance of the NServiceKit.Validation.ValidationError class.</summary>
        ///
        /// <param name="validationError">The validation error.</param>
        public ValidationError(ValidationErrorField validationError)
            : this(validationError.ErrorCode, validationError.ErrorMessage)
        {
            this.Violations.Add(validationError);
        }

        /// <summary>Initializes a new instance of the NServiceKit.Validation.ValidationError class.</summary>
        ///
        /// <param name="errorCode">   The error code.</param>
        /// <param name="errorMessage">Message describing the error.</param>
        public ValidationError(string errorCode, string errorMessage)
            : base(errorMessage)
        {
            this.errorCode = errorCode;
            this.ErrorMessage = errorMessage;
            this.Violations = new List<ValidationErrorField>();
        }

        /// <summary>
        /// Returns the first error code
        /// </summary>
        /// <value>The error code.</value>
        public string ErrorCode
        {
            get
            {
                return this.errorCode;
            }
        }

        /// <summary>Gets the error message and the parameter name, or only the error message if no parameter name is set.</summary>
        ///
        /// <value>
        /// A text string describing the details of the exception. The value of this property takes one of two forms: Condition Value The paramName is a null reference (Nothing in
        /// Visual Basic) or of zero length. The message string passed to the constructor. The paramName is not null reference (Nothing in Visual Basic) and it has a
        /// length greater than zero. The message string appended with the name of the invalid parameter.
        /// </value>
        public override string Message
        {
            get
            {
                //If there is only 1 validation error than we just show the error message
                if (this.Violations.Count == 0)
                    return this.ErrorMessage;

                if (this.Violations.Count == 1)
                    return this.ErrorMessage ?? this.Violations[0].ErrorMessage;

                var sb = new StringBuilder(this.ErrorMessage).AppendLine();
                foreach (var error in this.Violations)
                {
                    if (!string.IsNullOrEmpty(error.ErrorMessage))
                    {
                        var fieldLabel = error.FieldName != null ? string.Format(" [{0}]", error.FieldName) : null;
                        sb.AppendFormat("\n  - {0}{1}", error.ErrorMessage, fieldLabel);
                    }
                    else
                    {
                        var fieldLabel = error.FieldName != null ? ": " + error.FieldName : null;
                        sb.AppendFormat("\n  - {0}{1}", error.ErrorCode, fieldLabel);
                    }
                }
                return sb.ToString();
            }
        }

        /// <summary>Gets the violations.</summary>
        ///
        /// <value>The violations.</value>
        public IList<ValidationErrorField> Violations { get; private set; }

        /// <summary>
        /// Used if we need to serialize this exception to XML
        /// </summary>
        /// <returns></returns>
        public string ToXml()
        {
            var sb = new StringBuilder();
            sb.Append("<ValidationException>");
            foreach (ValidationErrorField error in this.Violations)
            {
                sb.Append("<ValidationError>")
                    .AppendFormat("<Code>{0}</Code>", error.ErrorCode)
                    .AppendFormat("<Field>{0}</Field>", error.FieldName)
                    .AppendFormat("<Message>{0}</Message>", error.ErrorMessage)
                    .Append("</ValidationError>");
            }
            sb.Append("</ValidationException>");
            return sb.ToString();
        }

        /// <summary>Creates an exception.</summary>
        ///
        /// <param name="errorCode">The error code.</param>
        ///
        /// <returns>The new exception.</returns>
        public static ValidationError CreateException(Enum errorCode)
        {
            return new ValidationError(errorCode.ToString());
        }

        /// <summary>Creates an exception.</summary>
        ///
        /// <param name="errorCode">   The error code.</param>
        /// <param name="errorMessage">Message describing the error.</param>
        ///
        /// <returns>The new exception.</returns>
        public static ValidationError CreateException(Enum errorCode, string errorMessage)
        {
            return new ValidationError(errorCode.ToString(), errorMessage);
        }

        /// <summary>Creates an exception.</summary>
        ///
        /// <param name="errorCode">   The error code.</param>
        /// <param name="errorMessage">Message describing the error.</param>
        /// <param name="fieldName">   Name of the field.</param>
        ///
        /// <returns>The new exception.</returns>
        public static ValidationError CreateException(Enum errorCode, string errorMessage, string fieldName)
        {
            return CreateException(errorCode.ToString(), errorMessage, fieldName);
        }

        /// <summary>Creates an exception.</summary>
        ///
        /// <param name="errorCode">The error code.</param>
        ///
        /// <returns>The new exception.</returns>
        public static ValidationError CreateException(string errorCode)
        {
            return new ValidationError(errorCode);
        }

        /// <summary>Creates an exception.</summary>
        ///
        /// <param name="errorCode">   The error code.</param>
        /// <param name="errorMessage">Message describing the error.</param>
        ///
        /// <returns>The new exception.</returns>
        public static ValidationError CreateException(string errorCode, string errorMessage)
        {
            return new ValidationError(errorCode, errorMessage);
        }

        /// <summary>Creates an exception.</summary>
        ///
        /// <param name="errorCode">   The error code.</param>
        /// <param name="errorMessage">Message describing the error.</param>
        /// <param name="fieldName">   Name of the field.</param>
        ///
        /// <returns>The new exception.</returns>
        public static ValidationError CreateException(string errorCode, string errorMessage, string fieldName)
        {
            var error = new ValidationErrorField(errorCode, fieldName, errorMessage);
            return new ValidationError(new ValidationErrorResult(new List<ValidationErrorField> { error }));
        }

        /// <summary>Creates an exception.</summary>
        ///
        /// <param name="error">The error.</param>
        ///
        /// <returns>The new exception.</returns>
        public static ValidationError CreateException(ValidationErrorField error)
        {
            return new ValidationError(error);
        }

        /// <summary>Throw if not valid.</summary>
        ///
        /// <exception cref="ValidationError">Thrown when a validation error error condition occurs.</exception>
        ///
        /// <param name="validationResult">The validation result.</param>
        public static void ThrowIfNotValid(ValidationErrorResult validationResult)
        {
            if (!validationResult.IsValid)
            {
                throw new ValidationError(validationResult);
            }
        }

        /// <summary>Converts this object to a response status.</summary>
        ///
        /// <returns>This object as the ResponseStatus.</returns>
        public ResponseStatus ToResponseStatus()
        {
            return ResponseStatusUtils.CreateResponseStatus(ErrorCode, Message, Violations);
        }
    }
}