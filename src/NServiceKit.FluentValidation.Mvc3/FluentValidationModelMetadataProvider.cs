#region License
// Copyright (c) Jeremy Skinner (http://www.jeremyskinner.co.uk)
// 
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at 
// 
// http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and 
// limitations under the License.
// 
// The latest version of this file can be found at http://www.codeplex.com/FluentValidation
#endregion

using NServiceKit.FluentValidation;
using NServiceKit.FluentValidation.Resources;
using NServiceKit.FluentValidation.Results;
using NServiceKit.FluentValidation.Validators;

namespace FluentValidation.Mvc {
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.ComponentModel.DataAnnotations;
	using System.Linq;
	using System.Web.Mvc;

    /// <summary>A fluent validation model metadata provider.</summary>
	public class FluentValidationModelMetadataProvider : DataAnnotationsModelMetadataProvider {
		readonly IValidatorFactory factory;

        /// <summary>Initializes a new instance of the FluentValidation.Mvc.FluentValidationModelMetadataProvider class.</summary>
        ///
        /// <param name="factory">The factory.</param>
		public FluentValidationModelMetadataProvider(IValidatorFactory factory) {
			this.factory = factory;
		}

        /// <summary>Returns the metadata for the specified property using the container type and property descriptor.</summary>
        ///
        /// <param name="modelAccessor">     The model accessor.</param>
        /// <param name="containerType">     The type of the container.</param>
        /// <param name="propertyDescriptor">The property descriptor.</param>
        ///
        /// <returns>The metadata for the specified property using the container type and property descriptor.</returns>
		protected override ModelMetadata GetMetadataForProperty(Func<object> modelAccessor, Type containerType, PropertyDescriptor propertyDescriptor) {
			var attributes = ConvertFVMetaDataToAttributes(containerType, propertyDescriptor.Name);
			return CreateMetadata(attributes, containerType, modelAccessor, propertyDescriptor.PropertyType, propertyDescriptor.Name);
		}

		IEnumerable<Attribute> ConvertFVMetaDataToAttributes(Type type, string name) {
			var validator = factory.GetValidator(type);

			if (validator == null) {
				return Enumerable.Empty<Attribute>();
			}

			IEnumerable<IPropertyValidator> validators;

//			if (name == null) {
				//validators = validator.CreateDescriptor().GetMembersWithValidators().SelectMany(x => x);
//				validators = Enumerable.Empty<IPropertyValidator>();
//			}
//			else {
				validators = validator.CreateDescriptor().GetValidatorsForMember(name);
//			}

			var attributes = validators.OfType<IAttributeMetadataValidator>()
				.Select(x => x.ToAttribute())
				.Concat(SpecialCaseValidatorConversions(validators));



			return attributes.ToList();
		}

		IEnumerable<Attribute> SpecialCaseValidatorConversions(IEnumerable<IPropertyValidator> validators) {

			//Email Validator should be convertible to DataType EmailAddress.
			var emailValidators = validators
				.OfType<IEmailValidator>()
				.Select(x => new DataTypeAttribute(DataType.EmailAddress))
				.Cast<Attribute>();

			var requiredValidators = validators.OfType<INotNullValidator>().Cast<IPropertyValidator>()
				.Concat(validators.OfType<INotEmptyValidator>().Cast<IPropertyValidator>())
				.Select(x => new RequiredAttribute())
				.Cast<Attribute>();

			return requiredValidators.Concat(emailValidators);
		}

		/*IEnumerable<Attribute> ConvertFVMetaDataToAttributes(Type type) {
			return ConvertFVMetaDataToAttributes(type, null);
		}*/

		/*public override ModelMetadata GetMetadataForType(Func<object> modelAccessor, Type modelType) {
			var attributes = ConvertFVMetaDataToAttributes(modelType);
			return CreateMetadata(attributes, null /* containerType ?1?, modelAccessor, modelType, null /* propertyName ?1?);
		}*/

	}

    /// <summary>Interface for attribute metadata validator.</summary>
    public interface IAttributeMetadataValidator : IPropertyValidator
    {
        /// <summary>Converts this object to an attribute.</summary>
        ///
        /// <returns>This object as an Attribute.</returns>
        Attribute ToAttribute();
    }

    internal class AttributeMetadataValidator : IAttributeMetadataValidator
    {
        readonly Attribute attribute;

        /// <summary>Initializes a new instance of the FluentValidation.Mvc.AttributeMetadataValidator class.</summary>
        ///
        /// <param name="attributeConverter">The attribute converter.</param>
        public AttributeMetadataValidator(Attribute attributeConverter)
        {
            attribute = attributeConverter;
        }

        /// <summary>Gets or sets the error message source.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <value>The error message source.</value>
        public IStringSource ErrorMessageSource
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        /// <summary>Gets or sets the error code.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <value>The error code.</value>
    	public string ErrorCode
    	{
    		get { throw new NotImplementedException(); }
    		set { throw new NotImplementedException(); }
    	}

        /// <summary>Enumerates validate in this collection.</summary>
        ///
        /// <param name="context">The context.</param>
        ///
        /// <returns>An enumerator that allows foreach to be used to process validate in this collection.</returns>
    	public IEnumerable<ValidationFailure> Validate(PropertyValidatorContext context)
        {
            return Enumerable.Empty<ValidationFailure>();
        }

        /// <summary>Gets or sets the error message template.</summary>
        ///
        /// <value>The error message template.</value>
        public string ErrorMessageTemplate
        {
            get { return null; }
            set { }
        }

        /// <summary>Gets the custom message format arguments.</summary>
        ///
        /// <value>The custom message format arguments.</value>
        public ICollection<Func<object, object>> CustomMessageFormatArguments
        {
            get { return null; }
        }

        /// <summary>Gets a value indicating whether the supports standalone validation.</summary>
        ///
        /// <value>true if supports standalone validation, false if not.</value>
        public bool SupportsStandaloneValidation
        {
            get { return false; }
        }

        /// <summary>Gets or sets the custom state provider.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <value>The custom state provider.</value>
        public Func<object, object> CustomStateProvider
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        /// <summary>Converts this object to an attribute.</summary>
        ///
        /// <returns>This object as an Attribute.</returns>
        public Attribute ToAttribute()
        {
            return attribute;
        }
    }
}

namespace FluentValidation.Mvc.MetadataExtensions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    /// <summary>A metadata extensions.</summary>
    public static class MetadataExtensions
    {
        /// <summary>An IRuleBuilder&lt;T,TProperty&gt; extension method that hidden input.</summary>
        ///
        /// <typeparam name="T">        Generic type parameter.</typeparam>
        /// <typeparam name="TProperty">Type of the property.</typeparam>
        /// <param name="ruleBuilder">The ruleBuilder to act on.</param>
        ///
        /// <returns>An IRuleBuilder&lt;T,TProperty&gt;</returns>
        public static IRuleBuilder<T, TProperty> HiddenInput<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new AttributeMetadataValidator(new HiddenInputAttribute()));
        }

        /// <summary>An IRuleBuilder&lt;T,TProperty&gt; extension method that hidden input.</summary>
        ///
        /// <typeparam name="T">        Generic type parameter.</typeparam>
        /// <typeparam name="TProperty">Type of the property.</typeparam>
        /// <param name="ruleBuilder"> The ruleBuilder to act on.</param>
        /// <param name="displayValue">true to display value.</param>
        ///
        /// <returns>An IRuleBuilder&lt;T,TProperty&gt;</returns>
        public static IRuleBuilder<T, TProperty> HiddenInput<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, bool displayValue)
        {
            return ruleBuilder.SetValidator(new AttributeMetadataValidator(new HiddenInputAttribute { DisplayValue = displayValue }));
        }

        /// <summary>An IRuleBuilder&lt;T,TProperty&gt; extension method that user interface hints.</summary>
        ///
        /// <typeparam name="T">        Generic type parameter.</typeparam>
        /// <typeparam name="TProperty">Type of the property.</typeparam>
        /// <param name="ruleBuilder">The ruleBuilder to act on.</param>
        /// <param name="hint">       The hint.</param>
        ///
        /// <returns>An IRuleBuilder&lt;T,TProperty&gt;</returns>
        public static IRuleBuilder<T, TProperty> UIHint<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, string hint)
        {
            return ruleBuilder.SetValidator(new AttributeMetadataValidator(new UIHintAttribute(hint)));
        }

        /// <summary>An IRuleBuilder&lt;T,TProperty&gt; extension method that user interface hints.</summary>
        ///
        /// <typeparam name="T">        Generic type parameter.</typeparam>
        /// <typeparam name="TProperty">Type of the property.</typeparam>
        /// <param name="ruleBuilder">      The ruleBuilder to act on.</param>
        /// <param name="hint">             The hint.</param>
        /// <param name="presentationLayer">The presentation layer.</param>
        ///
        /// <returns>An IRuleBuilder&lt;T,TProperty&gt;</returns>
        public static IRuleBuilder<T, TProperty> UIHint<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, string hint, string presentationLayer)
        {
            return ruleBuilder.SetValidator(new AttributeMetadataValidator(new UIHintAttribute(hint, presentationLayer)));
        }

        /// <summary>An IRuleBuilder&lt;T,TProperty&gt; extension method that scaffolds.</summary>
        ///
        /// <typeparam name="T">        Generic type parameter.</typeparam>
        /// <typeparam name="TProperty">Type of the property.</typeparam>
        /// <param name="ruleBuilder">The ruleBuilder to act on.</param>
        /// <param name="scaffold">   true to scaffold.</param>
        ///
        /// <returns>An IRuleBuilder&lt;T,TProperty&gt;</returns>
        public static IRuleBuilder<T, TProperty> Scaffold<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, bool scaffold)
        {
            return ruleBuilder.SetValidator(new AttributeMetadataValidator(new ScaffoldColumnAttribute(scaffold)));
        }

        /// <summary>An IRuleBuilder&lt;T,TProperty&gt; extension method that data type.</summary>
        ///
        /// <typeparam name="T">        Generic type parameter.</typeparam>
        /// <typeparam name="TProperty">Type of the property.</typeparam>
        /// <param name="ruleBuilder">The ruleBuilder to act on.</param>
        /// <param name="dataType">   Type of the data.</param>
        ///
        /// <returns>An IRuleBuilder&lt;T,TProperty&gt;</returns>
        public static IRuleBuilder<T, TProperty> DataType<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, DataType dataType)
        {
            return ruleBuilder.SetValidator(new AttributeMetadataValidator(new DataTypeAttribute(dataType)));
        }

        /// <summary>An IRuleBuilder&lt;T,TProperty&gt; extension method that data type.</summary>
        ///
        /// <typeparam name="T">        Generic type parameter.</typeparam>
        /// <typeparam name="TProperty">Type of the property.</typeparam>
        /// <param name="ruleBuilder">   The ruleBuilder to act on.</param>
        /// <param name="customDataType">Type of the custom data.</param>
        ///
        /// <returns>An IRuleBuilder&lt;T,TProperty&gt;</returns>
        public static IRuleBuilder<T, TProperty> DataType<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, string customDataType)
        {
            return ruleBuilder.SetValidator(new AttributeMetadataValidator(new DataTypeAttribute(customDataType)));
        }

        /// <summary>An IRuleBuilder&lt;T,TProperty&gt; extension method that displays a name.</summary>
        ///
        /// <typeparam name="T">        Generic type parameter.</typeparam>
        /// <typeparam name="TProperty">Type of the property.</typeparam>
        /// <param name="ruleBuilder">The ruleBuilder to act on.</param>
        /// <param name="name">       The name.</param>
        ///
        /// <returns>An IRuleBuilder&lt;T,TProperty&gt;</returns>
        public static IRuleBuilder<T, TProperty> DisplayName<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, string name)
        {
#if NET4
            return ruleBuilder.SetValidator(new AttributeMetadataValidator(new DisplayAttribute { Name = name }));
#else
			return ruleBuilder.SetValidator(new AttributeMetadataValidator(new DisplayNameAttribute(name)));
#endif
        }

        /// <summary>An IRuleBuilder&lt;T,TProperty&gt; extension method that displays a format described by ruleBuilder.</summary>
        ///
        /// <typeparam name="T">        Generic type parameter.</typeparam>
        /// <typeparam name="TProperty">Type of the property.</typeparam>
        /// <param name="ruleBuilder">The ruleBuilder to act on.</param>
        ///
        /// <returns>An IDisplayFormatBuilder&lt;T,TProperty&gt;</returns>
        public static IDisplayFormatBuilder<T, TProperty> DisplayFormat<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return new DisplayFormatBuilder<T, TProperty>(ruleBuilder);
        }

        /// <summary>An IRuleBuilder&lt;T,TProperty&gt; extension method that reads an only.</summary>
        ///
        /// <typeparam name="T">        Generic type parameter.</typeparam>
        /// <typeparam name="TProperty">Type of the property.</typeparam>
        /// <param name="ruleBuilder">The ruleBuilder to act on.</param>
        /// <param name="readOnly">   true to read only.</param>
        ///
        /// <returns>The only.</returns>
        public static IRuleBuilder<T, TProperty> ReadOnly<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, bool readOnly)
        {
            return ruleBuilder.SetValidator(new AttributeMetadataValidator(new ReadOnlyAttribute(readOnly)));
        }

        /// <summary>Interface for display format builder.</summary>
        ///
        /// <typeparam name="T">        Generic type parameter.</typeparam>
        /// <typeparam name="TProperty">Type of the property.</typeparam>
        public interface IDisplayFormatBuilder<T, TProperty> : IRuleBuilder<T, TProperty>
        {
            IDisplayFormatBuilder<T, TProperty> NullDisplayText(string text);
            IDisplayFormatBuilder<T, TProperty> DataFormatString(string text);
            IDisplayFormatBuilder<T, TProperty> ApplyFormatInEditMode(bool apply);
            IDisplayFormatBuilder<T, TProperty> ConvertEmptyStringToNull(bool convert);
        }

        private class DisplayFormatBuilder<T, TProperty> : IDisplayFormatBuilder<T, TProperty>
        {
            readonly IRuleBuilder<T, TProperty> builder;
            readonly DisplayFormatAttribute attribute = new DisplayFormatAttribute();

            /// <summary>Initializes a new instance of the FluentValidation.Mvc.MetadataExtensions.MetadataExtensions.DisplayFormatBuilder&lt;T, TProperty&gt; class.</summary>
            ///
            /// <param name="builder">The builder.</param>
            public DisplayFormatBuilder(IRuleBuilder<T, TProperty> builder)
            {
                this.builder = builder;
                builder.SetValidator(new AttributeMetadataValidator(attribute));
            }

            /// <summary>Sets a validator.</summary>
            ///
            /// <param name="validator">The validator.</param>
            ///
            /// <returns>An IRuleBuilderOptions&lt;T,TProperty&gt;</returns>
            public IRuleBuilderOptions<T, TProperty> SetValidator(IPropertyValidator validator)
            {
                return builder.SetValidator(validator);
            }

            /// <summary>Sets a validator.</summary>
            ///
            /// <param name="validator">The validator.</param>
            ///
            /// <returns>An IRuleBuilderOptions&lt;T,TProperty&gt;</returns>
            [Obsolete]
            public IRuleBuilderOptions<T, TProperty> SetValidator(IValidator validator)
            {
                return builder.SetValidator(validator);
            }

            /// <summary>Sets a validator.</summary>
            ///
            /// <param name="validator">The validator.</param>
            ///
            /// <returns>An IRuleBuilderOptions&lt;T,TProperty&gt;</returns>
            public IRuleBuilderOptions<T, TProperty> SetValidator(IValidator<TProperty> validator)
            {
                return builder.SetValidator(validator);

            }

            /// <summary>Null display text.</summary>
            ///
            /// <param name="text">The text.</param>
            ///
            /// <returns>An IDisplayFormatBuilder&lt;T,TProperty&gt;</returns>
            public IDisplayFormatBuilder<T, TProperty> NullDisplayText(string text)
            {
                attribute.NullDisplayText = text;
                return this;
            }

            /// <summary>Data format string.</summary>
            ///
            /// <param name="text">The text.</param>
            ///
            /// <returns>An IDisplayFormatBuilder&lt;T,TProperty&gt;</returns>
            public IDisplayFormatBuilder<T, TProperty> DataFormatString(string text)
            {
                attribute.DataFormatString = text;
                return this;
            }

            /// <summary>Applies the format in edit mode described by apply.</summary>
            ///
            /// <param name="apply">true to apply.</param>
            ///
            /// <returns>An IDisplayFormatBuilder&lt;T,TProperty&gt;</returns>
            public IDisplayFormatBuilder<T, TProperty> ApplyFormatInEditMode(bool apply)
            {
                attribute.ApplyFormatInEditMode = apply;
                return this;
            }

            /// <summary>Convert empty string to null.</summary>
            ///
            /// <param name="convert">true to convert.</param>
            ///
            /// <returns>The empty converted string to null.</returns>
            public IDisplayFormatBuilder<T, TProperty> ConvertEmptyStringToNull(bool convert)
            {
                attribute.ConvertEmptyStringToNull = convert;
                return this;
            }
        }
    }
}