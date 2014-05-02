using System;
using System.Collections.Generic;

namespace NServiceKit.Html
{
    /// <summary>A model metadata provider.</summary>
	public abstract class ModelMetadataProvider
	{
        /// <summary>Gets the metadata for properties in this collection.</summary>
        ///
        /// <param name="container">    The container.</param>
        /// <param name="containerType">Type of the container.</param>
        ///
        /// <returns>An enumerator that allows foreach to be used to process the metadata for properties in this collection.</returns>
		public abstract IEnumerable<ModelMetadata> GetMetadataForProperties(object container, Type containerType);

        /// <summary>Gets metadata for property.</summary>
        ///
        /// <param name="modelAccessor">The model accessor.</param>
        /// <param name="containerType">Type of the container.</param>
        /// <param name="propertyName"> Name of the property.</param>
        ///
        /// <returns>The metadata for property.</returns>
		public abstract ModelMetadata GetMetadataForProperty(Func<object> modelAccessor, Type containerType, string propertyName);

        /// <summary>Gets metadata for type.</summary>
        ///
        /// <param name="modelAccessor">The model accessor.</param>
        /// <param name="modelType">    Type of the model.</param>
        ///
        /// <returns>The metadata for type.</returns>
		public abstract ModelMetadata GetMetadataForType(Func<object> modelAccessor, Type modelType);
	}
}
