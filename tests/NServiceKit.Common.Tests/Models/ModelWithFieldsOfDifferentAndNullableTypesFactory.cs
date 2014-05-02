namespace NServiceKit.Common.Tests.Models
{
    /// <summary>A model with fields of different and nullable types factory.</summary>
	public class ModelWithFieldsOfDifferentAndNullableTypesFactory
		: ModelFactoryBase<ModelWithFieldsOfDifferentAndNullableTypes>
	{
        /// <summary>The instance.</summary>
		public static ModelWithFieldsOfDifferentAndNullableTypesFactory Instance 
			= new ModelWithFieldsOfDifferentAndNullableTypesFactory();

        /// <summary>Assert is equal.</summary>
        ///
        /// <param name="actual">  The actual.</param>
        /// <param name="expected">The expected.</param>
		public override void AssertIsEqual(
			ModelWithFieldsOfDifferentAndNullableTypes actual, ModelWithFieldsOfDifferentAndNullableTypes expected)
		{
			ModelWithFieldsOfDifferentAndNullableTypes.AssertIsEqual(actual, expected);
		}

        /// <summary>Creates an instance.</summary>
        ///
        /// <param name="i">Zero-based index of the.</param>
        ///
        /// <returns>The new instance.</returns>
		public override ModelWithFieldsOfDifferentAndNullableTypes CreateInstance(int i)
		{
			return ModelWithFieldsOfDifferentAndNullableTypes.CreateConstant(i);
		}
	}
}