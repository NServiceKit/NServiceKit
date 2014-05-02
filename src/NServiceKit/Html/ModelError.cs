using System;

namespace NServiceKit.Html
{
    /// <summary>A model error.</summary>
	[Serializable]
	public class ModelError
	{
        /// <summary>Initializes a new instance of the NServiceKit.Html.ModelError class.</summary>
        ///
        /// <param name="exception">The exception.</param>
		public ModelError(Exception exception)
			: this(exception, null /* errorMessage */)
		{
		}

        /// <summary>Initializes a new instance of the NServiceKit.Html.ModelError class.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="exception">   The exception.</param>
        /// <param name="errorMessage">A message describing the error.</param>
		public ModelError(Exception exception, string errorMessage)
			: this(errorMessage)
		{
			if (exception == null)
			{
				throw new ArgumentNullException("exception");
			}

			Exception = exception;
		}

        /// <summary>Initializes a new instance of the NServiceKit.Html.ModelError class.</summary>
        ///
        /// <param name="errorMessage">A message describing the error.</param>
		public ModelError(string errorMessage)
		{
			ErrorMessage = errorMessage ?? String.Empty;
		}

        /// <summary>Gets the exception.</summary>
        ///
        /// <value>The exception.</value>
		public Exception Exception
		{
			get;
			private set;
		}

        /// <summary>Gets a message describing the error.</summary>
        ///
        /// <value>A message describing the error.</value>
		public string ErrorMessage
		{
			get;
			private set;
		}
	}
}
