using System;
using System.Collections.ObjectModel;

namespace NServiceKit.Html
{
    /// <summary>Collection of model errors.</summary>
	[Serializable]
	public class ModelErrorCollection : Collection<ModelError>
	{
        /// <summary>Adds errorMessage.</summary>
        ///
        /// <param name="exception">The exception to add.</param>
		public void Add(Exception exception)
		{
			Add(new ModelError(exception));
		}

        /// <summary>Adds errorMessage.</summary>
        ///
        /// <param name="errorMessage">The error message to add.</param>
		public void Add(string errorMessage)
		{
			Add(new ModelError(errorMessage));
		}
	}
}
