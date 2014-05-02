using System.Collections.Generic;
using Northwind.Common.ComplexModel;
using NUnit.Framework;
using NServiceKit.Text;

namespace NServiceKit.Common.Tests.Perf
{
    /// <summary>A text serializer comparisons.</summary>
	[Ignore("Benchmarks on the war of the two text serializers")]
	[TestFixture]
	public class TextSerializerComparisons
		: PerfTestBase
	{
        /// <summary>Initializes a new instance of the NServiceKit.Common.Tests.Perf.TextSerializerComparisons class.</summary>
		public TextSerializerComparisons()
		{
			this.MultipleIterations = new List<int> { 10000 };
		}

		private void CompareSerializers<T>(T dto)
		{
			CompareMultipleRuns(
				"TypeSerializer", () => TypeSerializer.SerializeToString(dto),
				"TextSerializer", () => JsonSerializer.SerializeToString(dto)
				);

			var stringStr = TypeSerializer.SerializeToString(dto);
			var textStr = JsonSerializer.SerializeToString(dto);

			//return;

			CompareMultipleRuns(
				"TypeSerializer", () => TypeSerializer.DeserializeFromString<T>(stringStr),
				"JsonSerializer", () => JsonSerializer.DeserializeFromString<T>(textStr)
			);

			var seraializedStringDto = TypeSerializer.DeserializeFromString<T>(stringStr);
			Assert.That(seraializedStringDto.Equals(dto), Is.True);

			JsonSerializer.DeserializeFromString<T>(textStr);
			//Assert.That(seraializedTextDto.Equals(dto), Is.True);
		}

        /// <summary>Compare array dto with orders.</summary>
		[Test]
		public void Compare_ArrayDtoWithOrders()
		{
			CompareSerializers(DtoFactory.ArrayDtoWithOrders);
		}

        /// <summary>Compare customer dto.</summary>
		[Test]
		public void Compare_CustomerDto()
		{
			CompareSerializers(DtoFactory.CustomerDto);
		}

        /// <summary>Compare customer order array dto.</summary>
		[Test]
		public void Compare_CustomerOrderArrayDto()
		{
			CompareSerializers(DtoFactory.CustomerOrderArrayDto);
		}

        /// <summary>Compare customer order list dto.</summary>
		[Test]
		public void Compare_CustomerOrderListDto()
		{
			CompareSerializers(DtoFactory.CustomerOrderListDto);
		}

        /// <summary>Compare multi customer properties.</summary>
		[Test]
		public void Compare_MultiCustomerProperties()
		{
			CompareSerializers(DtoFactory.MultiCustomerProperties);
		}

        /// <summary>Compare multi dto with orders.</summary>
		[Test]
		public void Compare_MultiDtoWithOrders()
		{
			CompareSerializers(DtoFactory.MultiDtoWithOrders);
		}

        /// <summary>Compare multi order properties.</summary>
		[Test]
		public void Compare_MultiOrderProperties()
		{
			CompareSerializers(DtoFactory.MultiOrderProperties);
		}

        /// <summary>Compare order dto.</summary>
		[Test]
		public void Compare_OrderDto()
		{
			CompareSerializers(DtoFactory.OrderDto);
		}

        /// <summary>Compare supplier dto.</summary>
		[Test]
		public void Compare_SupplierDto()
		{
			CompareSerializers(DtoFactory.SupplierDto);
		}
	}

}