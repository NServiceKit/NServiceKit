using System;
using System.Collections.Generic;

namespace NServiceKit.Html
{
    /// <summary>A poco metadata provider.</summary>
	public class PocoMetadataProvider : ModelMetadataProvider
	{
        /// <summary>Creates a metadata.</summary>
        ///
        /// <param name="attributes">   The attributes.</param>
        /// <param name="containerType">Type of the container.</param>
        /// <param name="modelAccessor">The model accessor.</param>
        /// <param name="modelType">    Type of the model.</param>
        /// <param name="propertyName"> Name of the property.</param>
        ///
        /// <returns>The new metadata.</returns>
		protected virtual ModelMetadata CreateMetadata(IEnumerable<Attribute> attributes, Type containerType, Func<object> modelAccessor, Type modelType, string propertyName)
		{
			return new ModelMetadata(this, containerType, modelAccessor, modelType, propertyName);
		}

        /// <summary>Gets the metadata for properties in this collection.</summary>
        ///
        /// <param name="container">    The container.</param>
        /// <param name="containerType">Type of the container.</param>
        ///
        /// <returns>An enumerator that allows foreach to be used to process the metadata for properties in this collection.</returns>
		public override IEnumerable<ModelMetadata> GetMetadataForProperties(object container, Type containerType)
		{
			return new List<ModelMetadata>();
		}

        /// <summary>Gets metadata for property.</summary>
        ///
        /// <param name="modelAccessor">The model accessor.</param>
        /// <param name="containerType">Type of the container.</param>
        /// <param name="propertyName"> Name of the property.</param>
        ///
        /// <returns>The metadata for property.</returns>
		public override ModelMetadata GetMetadataForProperty(Func<object> modelAccessor, Type containerType, string propertyName)
		{
			var modelType = containerType; //FIX?
			return new ModelMetadata(this, containerType, modelAccessor, modelType, propertyName);
		}

        /// <summary>Gets metadata for type.</summary>
        ///
        /// <param name="modelAccessor">The model accessor.</param>
        /// <param name="modelType">    Type of the model.</param>
        ///
        /// <returns>The metadata for type.</returns>
		public override ModelMetadata GetMetadataForType(Func<object> modelAccessor, Type modelType)
		{
			return new ModelMetadata(this, null, modelAccessor, modelType, null);
		}
	}
}
