namespace NServiceKit.Common.Tests.Models
{
    /// <summary>A model with complex types factory.</summary>
	public class ModelWithComplexTypesFactory
		: ModelFactoryBase<ModelWithComplexTypes>
	{
        /// <summary>The instance.</summary>
		public static ModelWithComplexTypesFactory Instance 
			= new ModelWithComplexTypesFactory();

        /// <summary>Assert is equal.</summary>
        ///
        /// <param name="actual">  The actual.</param>
        /// <param name="expected">The expected.</param>
		public override void AssertIsEqual(
			ModelWithComplexTypes actual, ModelWithComplexTypes expected)
		{
			ModelWithComplexTypes.AssertIsEqual(actual, expected);
		}

        /// <summary>Creates an instance.</summary>
        ///
        /// <param name="i">Zero-based index of the.</param>
        ///
        /// <returns>The new instance.</returns>
		public override ModelWithComplexTypes CreateInstance(int i)
		{
			return ModelWithComplexTypes.CreateConstant(i);
		}
	}
}