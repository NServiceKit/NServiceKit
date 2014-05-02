using System;

namespace NServiceKit.Html
{
    /// <summary>A model state.</summary>
    [Serializable]
    public class ModelState
    {
        private readonly ModelErrorCollection errors = new ModelErrorCollection();

        /// <summary>Gets or sets the value.</summary>
        ///
        /// <value>The value.</value>
        public ValueProviderResult Value { get; set; }

        /// <summary>Gets the errors.</summary>
        ///
        /// <value>The errors.</value>
        public ModelErrorCollection Errors { get { return errors; } }
    }
}
