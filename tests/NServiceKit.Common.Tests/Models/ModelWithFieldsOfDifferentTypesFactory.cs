using System;

namespace NServiceKit.Common.Tests.Models
{
    /// <summary>A model with fields of different types factory.</summary>
	public class ModelWithFieldsOfDifferentTypesFactory
		: ModelFactoryBase<ModelWithFieldsOfDifferentTypes>
	{
        /// <summary>The instance.</summary>
		public static ModelWithFieldsOfDifferentTypesFactory Instance 
			= new ModelWithFieldsOfDifferentTypesFactory();

        /// <summary>Assert is equal.</summary>
        ///
        /// <param name="actual">  The actual.</param>
        /// <param name="expected">The expected.</param>
		public override void AssertIsEqual(
			ModelWithFieldsOfDifferentTypes actual, ModelWithFieldsOfDifferentTypes expected)
		{
			ModelWithFieldsOfDifferentTypes.AssertIsEqual(actual, expected);
		}

        /// <summary>Creates an instance.</summary>
        ///
        /// <param name="i">Zero-based index of the.</param>
        ///
        /// <returns>The new instance.</returns>
		public override ModelWithFieldsOfDifferentTypes CreateInstance(int i)
		{
			return ModelWithFieldsOfDifferentTypes.CreateConstant(i);
		}
	}
}