using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using NServiceKit.Text;

namespace NServiceKit.Html
{
    /// <summary>Dictionary of view data.</summary>
	public class ViewDataDictionary : IDictionary<string, object>
	{
		private readonly Dictionary<string, object> innerDictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
		private object model;
		private ModelMetadata modelMetadata;
		private ModelStateDictionary modelState;
        private TemplateInfo _templateMetadata;

        /// <summary>Initializes a new instance of the NServiceKit.Html.ViewDataDictionary class.</summary>
		public ViewDataDictionary() : this((object)null) { }

        /// <summary>Initializes a new instance of the NServiceKit.Html.ViewDataDictionary class.</summary>
        ///
        /// <param name="model">The model.</param>
		public ViewDataDictionary(object model)
		{
			Model = model;
		}

        /// <summary>Initializes a new instance of the NServiceKit.Html.ViewDataDictionary class.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="dictionary">The dictionary.</param>
		public ViewDataDictionary(ViewDataDictionary dictionary)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException("dictionary");
			}

			foreach (var entry in dictionary)
			{
				innerDictionary.Add(entry.Key, entry.Value);
			}
			foreach (var entry in dictionary.ModelState)
			{
				ModelState.Add(entry.Key, entry.Value);
			}

			Model = dictionary.Model;

			// PERF: Don't unnecessarily instantiate the model metadata
			modelMetadata = dictionary.modelMetadata;
		}

        /// <summary>Gets the number of. </summary>
        ///
        /// <value>The count.</value>
		public int Count
		{
			get
			{
				return innerDictionary.Count;
			}
		}

        /// <summary>Gets a value indicating whether this object is read only.</summary>
        ///
        /// <value>true if this object is read only, false if not.</value>
		public bool IsReadOnly
		{
			get
			{
				return ((IDictionary<string, object>)innerDictionary).IsReadOnly;
			}
		}

        /// <summary>Gets the keys.</summary>
        ///
        /// <value>The keys.</value>
		public ICollection<string> Keys
		{
			get
			{
				return innerDictionary.Keys;
			}
		}

        /// <summary>Gets or sets the model.</summary>
        ///
        /// <value>The model.</value>
		public object Model
		{
			get
			{
				return model;
			}
			set
			{
				modelMetadata = null;
				SetModel(value);
			}
		}

        /// <summary>Gets or sets the model metadata.</summary>
        ///
        /// <value>The model metadata.</value>
		public virtual ModelMetadata ModelMetadata
		{
			get
			{
				if (modelMetadata == null && model != null)
				{
					modelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType());
				}
				return modelMetadata;
			}
			set
			{
				modelMetadata = value;
			}
		}

        /// <summary>Gets the state of the model.</summary>
        ///
        /// <value>The model state.</value>
		public ModelStateDictionary ModelState
		{
			get
			{
				return modelState ?? (modelState = new ModelStateDictionary());
			}
		}

	    private bool hasPopulatedModelState;

        /// <summary>Populate model state.</summary>
        ///
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
		public virtual void PopulateModelState()
		{
			if (model == null) return;
            if (hasPopulatedModelState) return;

		    lock (this)
		    {
                if (hasPopulatedModelState) return;
                
                //Skip non-poco's, i.e. List
                modelState = new ModelStateDictionary();
                var modelType = model.GetType();
                var listType = modelType.IsGenericType
                    ? modelType.GetTypeWithGenericInterfaceOf(typeof(IList<>))
                    : null;
                if (listType != null || model.GetType().IsArray) return;

                var strModel = TypeSerializer.SerializeToString(model);
                var map = TypeSerializer.DeserializeFromString<Dictionary<string, string>>(strModel);
                foreach (var kvp in map)
                {
                    var valueState = new ModelState {
                        Value = new ValueProviderResult(kvp.Value, kvp.Value, CultureInfo.CurrentCulture)
                    };
                    try
                    {
                        modelState.Add(kvp.Key, valueState);
                    }
                    catch (Exception ex)
                    {
                        ex.Message.PrintDump();
                        throw;
                    }
                }
		        hasPopulatedModelState = true;
		    }
		}

        /// <summary>Indexer to get or set items within this collection using array index syntax.</summary>
        ///
        /// <param name="key">The key.</param>
        ///
        /// <returns>The indexed item.</returns>
		public object this[string key]
		{
			get
			{
				object value;
				innerDictionary.TryGetValue(key, out value);
				return value;
			}
			set
			{
				innerDictionary[key] = value;
			}
		}

        /// <summary>Gets or sets information describing the template.</summary>
        ///
        /// <value>Information describing the template.</value>
        public TemplateInfo TemplateInfo
        {
            get
            {
                if (_templateMetadata == null) {
                    _templateMetadata = new TemplateInfo();
                }
                return _templateMetadata;
            }
            set
            {
                _templateMetadata = value;
            }
        }

        /// <summary>Gets the values.</summary>
        ///
        /// <value>The values.</value>
		public ICollection<object> Values
		{
			get
			{
				return innerDictionary.Values;
			}
		}

        /// <summary>Adds key.</summary>
        ///
        /// <param name="item">The item to remove.</param>
		public void Add(KeyValuePair<string, object> item)
		{
			((IDictionary<string, object>)innerDictionary).Add(item);
		}

        /// <summary>Adds key.</summary>
        ///
        /// <param name="key">  The key.</param>
        /// <param name="value">The value.</param>
		public void Add(string key, object value)
		{
			innerDictionary.Add(key, value);
		}

        /// <summary>Clears this object to its blank/initial state.</summary>
		public void Clear()
		{
			innerDictionary.Clear();
		}

        /// <summary>Query if this object contains the given item.</summary>
        ///
        /// <param name="item">The item to remove.</param>
        ///
        /// <returns>true if the object is in this collection, false if not.</returns>
		public bool Contains(KeyValuePair<string, object> item)
		{
			return ((IDictionary<string, object>)innerDictionary).Contains(item);
		}

        /// <summary>Query if 'key' contains key.</summary>
        ///
        /// <param name="key">The key.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		public bool ContainsKey(string key)
		{
			return innerDictionary.ContainsKey(key);
		}

        /// <summary>Copies to.</summary>
        ///
        /// <param name="array">     The array.</param>
        /// <param name="arrayIndex">Zero-based index of the array.</param>
		public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
		{
			((IDictionary<string, object>)innerDictionary).CopyTo(array, arrayIndex);
		}

        /// <summary>Evals.</summary>
        ///
        /// <param name="expression">The expression.</param>
        ///
        /// <returns>A string.</returns>
		public object Eval(string expression)
		{
			ViewDataInfo info = GetViewDataInfo(expression);
			return (info != null) ? info.Value : null;
		}

        /// <summary>Evals.</summary>
        ///
        /// <param name="expression">The expression.</param>
        /// <param name="format">    Describes the format to use.</param>
        ///
        /// <returns>A string.</returns>
		public string Eval(string expression, string format)
		{
			object value = Eval(expression);

			if (value == null)
			{
				return String.Empty;
			}

			if (String.IsNullOrEmpty(format))
			{
				return Convert.ToString(value, CultureInfo.CurrentCulture);
			}
			else
			{
				return String.Format(CultureInfo.CurrentCulture, format, value);
			}
		}

        internal static string FormatValueInternal(object value, string format)
        {
            if (value == null) {
                return String.Empty;
            }

            if (String.IsNullOrEmpty(format)) {
                return Convert.ToString(value, CultureInfo.CurrentCulture);
            } else {
                return String.Format(CultureInfo.CurrentCulture, format, value);
            }
        }

        /// <summary>Gets the enumerator.</summary>
        ///
        /// <returns>The enumerator.</returns>
		public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			return innerDictionary.GetEnumerator();
		}

        /// <summary>Gets view data information.</summary>
        ///
        /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or illegal values.</exception>
        ///
        /// <param name="expression">The expression.</param>
        ///
        /// <returns>The view data information.</returns>
		public ViewDataInfo GetViewDataInfo(string expression)
		{
			if (String.IsNullOrEmpty(expression))
			{
				throw new ArgumentException(MvcResources.Common_NullOrEmpty, "expression");
			}

			return ViewDataEvaluator.Eval(this, expression);
		}

        /// <summary>Removes the given key.</summary>
        ///
        /// <param name="item">The item to remove.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		public bool Remove(KeyValuePair<string, object> item)
		{
			return ((IDictionary<string, object>)innerDictionary).Remove(item);
		}

        /// <summary>Removes the given key.</summary>
        ///
        /// <param name="key">The key to remove.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		public bool Remove(string key)
		{
			return innerDictionary.Remove(key);
		}

        /// <summary>
        /// This method will execute before the derived type's instance constructor executes. Derived types must be aware of this and should plan accordingly. For example, the logic in SetModel() should be
        /// simple enough so as not to depend on the "this" pointer referencing a fully constructed object.
        /// </summary>
        ///
        /// <param name="value">The value.</param>
		protected virtual void SetModel(object value)
		{
			model = value;
		}

        /// <summary>Attempts to get value from the given data.</summary>
        ///
        /// <param name="key">  The key.</param>
        /// <param name="value">The value.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		public bool TryGetValue(string key, out object value)
		{
			return innerDictionary.TryGetValue(key, out value);
		}

		internal static class ViewDataEvaluator
		{
            /// <summary>Evals.</summary>
            ///
            /// <param name="vdd">       The vdd.</param>
            /// <param name="expression">The expression.</param>
            ///
            /// <returns>A ViewDataInfo.</returns>
			public static ViewDataInfo Eval(ViewDataDictionary vdd, string expression)
			{
				//Given an expression "foo.bar.baz" we look up the following (pseudocode):
				//  this["foo.bar.baz.quux"]
				//  this["foo.bar.baz"]["quux"]
				//  this["foo.bar"]["baz.quux]
				//  this["foo.bar"]["baz"]["quux"]
				//  this["foo"]["bar.baz.quux"]
				//  this["foo"]["bar.baz"]["quux"]
				//  this["foo"]["bar"]["baz.quux"]
				//  this["foo"]["bar"]["baz"]["quux"]

				ViewDataInfo evaluated = EvalComplexExpression(vdd, expression);
				return evaluated;
			}

			private static ViewDataInfo EvalComplexExpression(object indexableObject, string expression)
			{
				foreach (ExpressionPair expressionPair in GetRightToLeftExpressions(expression))
				{
					string subExpression = expressionPair.Left;
					string postExpression = expressionPair.Right;

					ViewDataInfo subTargetInfo = GetPropertyValue(indexableObject, subExpression);
					if (subTargetInfo != null)
					{
						if (String.IsNullOrEmpty(postExpression))
						{
							return subTargetInfo;
						}

						if (subTargetInfo.Value != null)
						{
							ViewDataInfo potential = EvalComplexExpression(subTargetInfo.Value, postExpression);
							if (potential != null)
							{
								return potential;
							}
						}
					}
				}
				return null;
			}

			private static IEnumerable<ExpressionPair> GetRightToLeftExpressions(string expression)
			{
				// Produces an enumeration of all the combinations of complex property names
				// given a complex expression. See the list above for an example of the result
				// of the enumeration.

				yield return new ExpressionPair(expression, String.Empty);

				int lastDot = expression.LastIndexOf('.');

				string subExpression = expression;
				string postExpression = string.Empty;

				while (lastDot > -1)
				{
					subExpression = expression.Substring(0, lastDot);
					postExpression = expression.Substring(lastDot + 1);
					yield return new ExpressionPair(subExpression, postExpression);

					lastDot = subExpression.LastIndexOf('.');
				}
			}

			private static ViewDataInfo GetIndexedPropertyValue(object indexableObject, string key)
			{
				var dict = indexableObject as IDictionary<string, object>;
				object value = null;
				bool success = false;

				if (dict != null)
				{
					success = dict.TryGetValue(key, out value);
				}
				else
				{
					var tgvDel = TypeHelpers.CreateTryGetValueDelegate(indexableObject.GetType());
					if (tgvDel != null)
					{
						success = tgvDel(indexableObject, key, out value);
					}
				}

				if (success)
				{
					return new ViewDataInfo() {
						Container = indexableObject,
						Value = value
					};
				}

				return null;
			}

			private static ViewDataInfo GetPropertyValue(object container, string propertyName)
			{
				// This method handles one "segment" of a complex property expression

				// First, we try to evaluate the property based on its indexer
				var value = GetIndexedPropertyValue(container, propertyName);
				if (value != null)
				{
					return value;
				}

				// If the indexer didn't return anything useful, continue...

				// If the container is a ViewDataDictionary then treat its Model property
				// as the container instead of the ViewDataDictionary itself.
				var vdd = container as ViewDataDictionary;
				if (vdd != null)
				{
					container = vdd.Model;
				}

				// If the container is null, we're out of options
				if (container == null)
				{
					return null;
				}

				// Second, we try to use PropertyDescriptors and treat the expression as a property name
				var descriptor = TypeDescriptor.GetProperties(container).Find(propertyName, true);
				if (descriptor == null)
				{
					return null;
				}

				return new ViewDataInfo(() => descriptor.GetValue(container)) {
					Container = container,
					PropertyDescriptor = descriptor
				};
			}

			private struct ExpressionPair
			{
                /// <summary>The left.</summary>
				public readonly string Left;
                /// <summary>The right.</summary>
				public readonly string Right;

                /// <summary>Initializes a new instance of the NServiceKit.Html.ViewDataDictionary.ViewDataEvaluator class.</summary>
                ///
                /// <param name="left"> The left.</param>
                /// <param name="right">The right.</param>
				public ExpressionPair(string left, string right)
				{
					Left = left;
					Right = right;
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)innerDictionary).GetEnumerator();
		}

        /// <summary>Converts this object to a raw JSON.</summary>
        ///
        /// <returns>A MvcHtmlString.</returns>
        public MvcHtmlString AsRawJson()
        {
            return MvcHtmlString.Create(Model.ToJson());
        }

        /// <summary>Converts this object to a raw.</summary>
        ///
        /// <returns>A MvcHtmlString.</returns>
        public MvcHtmlString AsRaw()
        {
            return MvcHtmlString.Create((Model ?? "").ToString());
        }

	}
}
