
namespace NServiceKit.Html
{
    /// <summary>Dictionary of view data.</summary>
    /// <typeparam name="TModel">Type of the model.</typeparam>
	public class ViewDataDictionary<TModel> : ViewDataDictionary
	{
        /// <summary>Initializes a new instance of the NServiceKit.Html.ViewDataDictionary&lt;TModel&gt; class.</summary>
		public ViewDataDictionary() : base(default(TModel)) {}

        /// <summary>Initializes a new instance of the NServiceKit.Html.ViewDataDictionary&lt;TModel&gt; class.</summary>
        ///
        /// <param name="model">The model.</param>
		public ViewDataDictionary(TModel model) : base(model) {}

        /// <summary>Initializes a new instance of the NServiceKit.Html.ViewDataDictionary&lt;TModel&gt; class.</summary>
        ///
        /// <param name="viewDataDictionary">Dictionary of view data.</param>
		public ViewDataDictionary(ViewDataDictionary viewDataDictionary) 
			: base(viewDataDictionary) {}

        /// <summary>Gets or sets the model.</summary>
        ///
        /// <value>The model.</value>
		public new TModel Model
		{
			get { return (TModel)base.Model; }
			set { SetModel(value); }
		}

        /// <summary>Gets or sets the model metadata.</summary>
        ///
        /// <value>The model metadata.</value>
		public override ModelMetadata ModelMetadata
		{
			get
			{
				var result = base.ModelMetadata
					?? (base.ModelMetadata = ModelMetadataProviders.Current
						.GetMetadataForType(null, typeof(TModel)));

				return result;
			}
			set { base.ModelMetadata = value; }
		}

        /// <summary>Sets a model.</summary>
        ///
        /// <exception>Thrown when an exception error condition occurs.</exception>
        ///
        /// <param name="value">The value.</param>
		protected override void SetModel(object value)
		{
			bool castWillSucceed = TypeHelpers.IsCompatibleObject<TModel>(value);

			if (castWillSucceed)
			{
				base.SetModel((TModel)value);
			}
			else
			{
				var exception = (value != null)
					? Error.ViewDataDictionary_WrongTModelType(value.GetType(), typeof(TModel))
					: Error.ViewDataDictionary_ModelCannotBeNull(typeof(TModel));
				throw exception;
			}
		}
	}
}
