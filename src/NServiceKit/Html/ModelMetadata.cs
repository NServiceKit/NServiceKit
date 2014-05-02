// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NServiceKit.Markdown;

namespace NServiceKit.Html
{
    /// <summary>A model metadata.</summary>
	public class ModelMetadata
	{
        /// <summary>The default order.</summary>
		public const int DefaultOrder = 10000;

		// Explicit backing store for the things we want initialized by default, so don't have to call
		// the protected virtual setters of an auto-generated property
		private Dictionary<string, object> _additionalValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
		private readonly Type _containerType;
		private bool _convertEmptyStringToNull = true;
		private bool _isRequired;
		private object _model;
		private Func<object> _modelAccessor;
		private readonly Type _modelType;
		private int _order = DefaultOrder;
		private IEnumerable<ModelMetadata> _properties;
		private readonly string _propertyName;
		private Type _realModelType;
		private bool _requestValidationEnabled = true;
		private bool _showForDisplay = true;
		private bool _showForEdit = true;
		private string _simpleDisplayText;

        /// <summary>Initializes a new instance of the NServiceKit.Html.ModelMetadata class.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="provider">     The provider.</param>
        /// <param name="containerType">The type of the container.</param>
        /// <param name="modelAccessor">The model accessor.</param>
        /// <param name="modelType">    The type of the model.</param>
        /// <param name="propertyName"> The name of the property.</param>
		public ModelMetadata(ModelMetadataProvider provider, Type containerType, Func<object> modelAccessor, Type modelType, string propertyName)
		{
			if (provider == null)
			{
				throw new ArgumentNullException("provider");
			}
			if (modelType == null)
			{
				throw new ArgumentNullException("modelType");
			}

			Provider = provider;

			_containerType = containerType;
			_isRequired = !TypeHelpers.TypeAllowsNullValue(modelType);
			_modelAccessor = modelAccessor;
			_modelType = modelType;
			_propertyName = propertyName;
		}

        /// <summary>Gets the additional values.</summary>
        ///
        /// <value>The additional values.</value>
		public virtual Dictionary<string, object> AdditionalValues
		{
			get
			{
				return _additionalValues;
			}
		}

        /// <summary>Gets the type of the container.</summary>
        ///
        /// <value>The type of the container.</value>
		public Type ContainerType
		{
			get
			{
				return _containerType;
			}
		}

        /// <summary>Gets or sets a value indicating whether the convert empty string to null.</summary>
        ///
        /// <value>true if convert empty string to null, false if not.</value>
		public virtual bool ConvertEmptyStringToNull
		{
			get
			{
				return _convertEmptyStringToNull;
			}
			set
			{
				_convertEmptyStringToNull = value;
			}
		}

        /// <summary>Gets or sets the name of the data type.</summary>
        ///
        /// <value>The name of the data type.</value>
		public virtual string DataTypeName
		{
			get;
			set;
		}

        /// <summary>Gets or sets the description.</summary>
        ///
        /// <value>The description.</value>
		public virtual string Description
		{
			get;
			set;
		}

        /// <summary>Gets or sets the display format string.</summary>
        ///
        /// <value>The display format string.</value>
		public virtual string DisplayFormatString
		{
			get;
			set;
		}

        /// <summary>Gets or sets the name of the display.</summary>
        ///
        /// <value>The name of the display.</value>
		public virtual string DisplayName
		{
			get;
			set;
		}

        /// <summary>Gets or sets the edit format string.</summary>
        ///
        /// <value>The edit format string.</value>
		public virtual string EditFormatString
		{
			get;
			set;
		}

        /// <summary>Gets or sets a value indicating whether the surrounding HTML is hidden.</summary>
        ///
        /// <value>true if hide surrounding html, false if not.</value>
		public virtual bool HideSurroundingHtml
		{
			get;
			set;
		}

        /// <summary>Gets a value indicating whether this object is complex type.</summary>
        ///
        /// <value>true if this object is complex type, false if not.</value>
		public virtual bool IsComplexType
		{
			get
			{
				return !(TypeDescriptor.GetConverter(ModelType).CanConvertFrom(typeof(string)));
			}
		}

        /// <summary>Gets a value indicating whether this object is nullable value type.</summary>
        ///
        /// <value>true if this object is nullable value type, false if not.</value>
		public bool IsNullableValueType
		{
			get
			{
				return TypeHelpers.IsNullableValueType(ModelType);
			}
		}

        /// <summary>Gets or sets a value indicating whether this object is read only.</summary>
        ///
        /// <value>true if this object is read only, false if not.</value>
		public virtual bool IsReadOnly
		{
			get;
			set;
		}

        /// <summary>Gets or sets a value indicating whether this object is required.</summary>
        ///
        /// <value>true if this object is required, false if not.</value>
		public virtual bool IsRequired
		{
			get
			{
				return _isRequired;
			}
			set
			{
				_isRequired = value;
			}
		}

        /// <summary>Gets or sets the model.</summary>
        ///
        /// <value>The model.</value>
		public object Model
		{
			get
			{
				if (_modelAccessor != null)
				{
					_model = _modelAccessor();
					_modelAccessor = null;
				}
				return _model;
			}
			set
			{
				_model = value;
				_modelAccessor = null;
				_properties = null;
				_realModelType = null;
			}
		}

        /// <summary>Gets the type of the model.</summary>
        ///
        /// <value>The type of the model.</value>
		public Type ModelType
		{
			get
			{
				return _modelType;
			}
		}

        /// <summary>Gets or sets the null display text.</summary>
        ///
        /// <value>The null display text.</value>
		public virtual string NullDisplayText
		{
			get;
			set;
		}

        /// <summary>Gets or sets the order.</summary>
        ///
        /// <value>The order.</value>
		public virtual int Order
		{
			get
			{
				return _order;
			}
			set
			{
				_order = value;
			}
		}

        /// <summary>Gets the properties.</summary>
        ///
        /// <value>The properties.</value>
		public virtual IEnumerable<ModelMetadata> Properties
		{
			get
			{
				if (_properties == null)
				{
					_properties = Provider.GetMetadataForProperties(Model, RealModelType).OrderBy(m => m.Order);
				}
				return _properties;
			}
		}

        /// <summary>Gets the name of the property.</summary>
        ///
        /// <value>The name of the property.</value>
		public string PropertyName
		{
			get
			{
				return _propertyName;
			}
		}

        /// <summary>Gets or sets the provider.</summary>
        ///
        /// <value>The provider.</value>
		protected ModelMetadataProvider Provider
		{
			get;
			set;
		}

		internal Type RealModelType
		{
			get
			{
				if (_realModelType == null)
				{
					_realModelType = ModelType;

					// Don't call GetType() if the model is Nullable<T>, because it will
					// turn Nullable<T> into T for non-null values
					if (Model != null && !TypeHelpers.IsNullableValueType(ModelType))
					{
						_realModelType = Model.GetType();
					}
				}

				return _realModelType;
			}
		}

        /// <summary>Gets or sets a value indicating whether the request validation is enabled.</summary>
        ///
        /// <value>true if request validation enabled, false if not.</value>
		public virtual bool RequestValidationEnabled
		{
			get
			{
				return _requestValidationEnabled;
			}
			set
			{
				_requestValidationEnabled = value;
			}
		}

        /// <summary>Gets or sets the name of the short display.</summary>
        ///
        /// <value>The name of the short display.</value>
		public virtual string ShortDisplayName
		{
			get;
			set;
		}

        /// <summary>Gets or sets a value indicating whether for display is shown.</summary>
        ///
        /// <value>true if show for display, false if not.</value>
		public virtual bool ShowForDisplay
		{
			get
			{
				return _showForDisplay;
			}
			set
			{
				_showForDisplay = value;
			}
		}

        /// <summary>Gets or sets a value indicating whether for edit is shown.</summary>
        ///
        /// <value>true if show for edit, false if not.</value>
		public virtual bool ShowForEdit
		{
			get
			{
				return _showForEdit;
			}
			set
			{
				_showForEdit = value;
			}
		}

        /// <summary>Gets or sets the simple display text.</summary>
        ///
        /// <value>The simple display text.</value>
		public virtual string SimpleDisplayText
		{
			get
			{
				if (_simpleDisplayText == null)
				{
					_simpleDisplayText = GetSimpleDisplayText();
				}
				return _simpleDisplayText;
			}
			set
			{
				_simpleDisplayText = value;
			}
		}

        /// <summary>Gets or sets the template hint.</summary>
        ///
        /// <value>The template hint.</value>
		public virtual string TemplateHint
		{
			get;
			set;
		}

        /// <summary>Gets or sets the watermark.</summary>
        ///
        /// <value>The watermark.</value>
		public virtual string Watermark
		{
			get;
			set;
		}

        /// <summary>From lambda expression.</summary>
        ///
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        ///
        /// <typeparam name="TParameter">Type of the parameter.</typeparam>
        /// <typeparam name="TValue">    Type of the value.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <param name="viewData">  Information describing the view.</param>
        ///
        /// <returns>A ModelMetadata.</returns>
		public static ModelMetadata FromLambdaExpression<TParameter, TValue>(
			Expression<Func<TParameter, TValue>> expression, ViewDataDictionary<TParameter> viewData)
		{
			if (expression == null)
			{
				throw new ArgumentNullException("expression");
			}
			if (viewData == null)
			{
				throw new ArgumentNullException("viewData");
			}

			string propertyName = null;
			Type containerType = null;
			bool legalExpression = false;

			// Need to verify the expression is valid; it needs to at least end in something
			// that we can convert to a meaningful string for model binding purposes

			switch (expression.Body.NodeType)
			{
				// ArrayIndex always means a single-dimensional indexer; multi-dimensional indexer is a method call to Get()
				case ExpressionType.ArrayIndex:
					legalExpression = true;
					break;

				// Only legal method call is a single argument indexer/DefaultMember call
				case ExpressionType.Call:
					legalExpression = ExpressionHelper.IsSingleArgumentIndexer(expression.Body);
					break;

				// Property/field access is always legal
				case ExpressionType.MemberAccess:
					var memberExpression = (MemberExpression)expression.Body;
					propertyName = memberExpression.Member is PropertyInfo ? memberExpression.Member.Name : null;
					containerType = memberExpression.Expression.Type;
					legalExpression = true;
					break;

				// Parameter expression means "model => model", so we delegate to FromModel
				case ExpressionType.Parameter:
					return FromModel(viewData);
			}

			if (!legalExpression)
			{
				throw new InvalidOperationException(MvcResources.TemplateHelpers_TemplateLimitations);
			}

			TParameter container = viewData.Model;
			Func<object> modelAccessor = () => {
				try
				{
					return CachedExpressionCompiler.Process(expression)(container);
				}
				catch (NullReferenceException)
				{
					return null;
				}
			};

			return GetMetadataFromProvider(modelAccessor, typeof(TValue), propertyName, containerType);
		}

		private static ModelMetadata FromModel(ViewDataDictionary viewData)
		{
			return viewData.ModelMetadata ?? GetMetadataFromProvider(null, typeof(string), null, null);
		}

        /// <summary>From string expression.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="expression">The expression.</param>
        /// <param name="viewData">  Information describing the view.</param>
        ///
        /// <returns>A ModelMetadata.</returns>
		public static ModelMetadata FromStringExpression(string expression, ViewDataDictionary viewData)
		{
			if (expression == null)
			{
				throw new ArgumentNullException("expression");
			}
			if (viewData == null)
			{
				throw new ArgumentNullException("viewData");
			}
			if (expression.Length == 0)
			{    // Empty string really means "model metadata for the current model"
				return FromModel(viewData);
			}

			var vdi = viewData.GetViewDataInfo(expression);
			Type containerType = null;
			Type modelType = null;
			Func<object> modelAccessor = null;
			string propertyName = null;

			if (vdi != null)
			{
				if (vdi.Container != null)
				{
					containerType = vdi.Container.GetType();
				}

				modelAccessor = () => vdi.Value;

				if (vdi.PropertyDescriptor != null)
				{
					propertyName = vdi.PropertyDescriptor.Name;
					modelType = vdi.PropertyDescriptor.PropertyType;
				}
				else if (vdi.Value != null)
				{  // We only need to delay accessing properties (for LINQ to SQL)
					modelType = vdi.Value.GetType();
				}
			}
			//  Try getting a property from ModelMetadata if we couldn't find an answer in ViewData
			else if (viewData.ModelMetadata != null)
			{
				ModelMetadata propertyMetadata = viewData.ModelMetadata.Properties.Where(p => p.PropertyName == expression).FirstOrDefault();
				if (propertyMetadata != null)
				{
					return propertyMetadata;
				}
			}


			return GetMetadataFromProvider(modelAccessor, modelType ?? typeof(string), propertyName, containerType);
		}

        /// <summary>Gets display name.</summary>
        ///
        /// <returns>The display name.</returns>
		public string GetDisplayName()
		{
			return DisplayName ?? PropertyName ?? ModelType.Name;
		}

		private static ModelMetadata GetMetadataFromProvider(Func<object> modelAccessor, Type modelType, string propertyName, Type containerType)
		{
			if (containerType != null && !String.IsNullOrEmpty(propertyName))
			{
				return ModelMetadataProviders.Current.GetMetadataForProperty(modelAccessor, containerType, propertyName);
			}
			return ModelMetadataProviders.Current.GetMetadataForType(modelAccessor, modelType);
		}

        /// <summary>Gets simple display text.</summary>
        ///
        /// <returns>The simple display text.</returns>
		protected virtual string GetSimpleDisplayText()
		{
			if (Model == null)
			{
				return NullDisplayText;
			}

			string toStringResult = Convert.ToString(Model, CultureInfo.CurrentCulture);
			if (!toStringResult.Equals(Model.GetType().FullName, StringComparison.Ordinal))
			{
				return toStringResult;
			}

			ModelMetadata firstProperty = Properties.FirstOrDefault();
			if (firstProperty == null)
			{
				return String.Empty;
			}

			if (firstProperty.Model == null)
			{
				return firstProperty.NullDisplayText;
			}

			return Convert.ToString(firstProperty.Model, CultureInfo.CurrentCulture);
		}

	}
}
