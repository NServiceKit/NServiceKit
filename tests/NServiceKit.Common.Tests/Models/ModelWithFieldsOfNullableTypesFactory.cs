namespace NServiceKit.Common.Tests.Models
{
    /// <summary>A model with fields of nullable types factory.</summary>
	public class ModelWithFieldsOfNullableTypesFactory
		: ModelFactoryBase<ModelWithFieldsOfNullableTypes>
	{
        /// <summary>The instance.</summary>
		public static ModelWithFieldsOfNullableTypesFactory Instance 
			= new ModelWithFieldsOfNullableTypesFactory();

        /// <summary>Assert is equal.</summary>
        ///
        /// <param name="actual">  The actual.</param>
        /// <param name="expected">The expected.</param>
		public override void AssertIsEqual(
			ModelWithFieldsOfNullableTypes actual, ModelWithFieldsOfNullableTypes expected)
		{
			ModelWithFieldsOfNullableTypes.AssertIsEqual(actual, expected);
		}

        /// <summary>Creates an instance.</summary>
        ///
        /// <param name="i">Zero-based index of the.</param>
        ///
        /// <returns>The new instance.</returns>
		public override ModelWithFieldsOfNullableTypes CreateInstance(int i)
		{
			return ModelWithFieldsOfNullableTypes.CreateConstant(i);
		}
	}
}