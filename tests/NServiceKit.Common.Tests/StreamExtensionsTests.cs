using System.Runtime.Serialization;
using NUnit.Framework;
using NServiceKit.Common.Extensions;
using NServiceKit.ServiceModel.Serialization;
using DataContractSerializer=NServiceKit.ServiceModel.Serialization.DataContractSerializer;

namespace NServiceKit.Common.Tests
{
    /// <summary>A stream extensions tests.</summary>
	[TestFixture]
	public class StreamExtensionsTests
	{
        /// <summary>A simple dto.</summary>
		[DataContract]
		public class SimpleDto
		{
            /// <summary>Gets or sets the identifier.</summary>
            ///
            /// <value>The identifier.</value>
			[DataMember]
			public int Id { get; set; }

            /// <summary>Gets or sets the name.</summary>
            ///
            /// <value>The name.</value>
			[DataMember]
			public string Name { get; set; }

            /// <summary>Initializes a new instance of the NServiceKit.Common.Tests.StreamExtensionsTests.SimpleDto class.</summary>
            ///
            /// <param name="id">  The identifier.</param>
            /// <param name="name">The name.</param>
			public SimpleDto(int id, string name)
			{
				Id = id;
				Name = name;
			}
		}

        /// <summary>Can compress and decompress simple dto.</summary>
		[Test]
		public void Can_compress_and_decompress_SimpleDto()
		{
			var simpleDto = new SimpleDto(1, "name");

			var simpleDtoXml = DataContractSerializer.Instance.Parse(simpleDto);

			var simpleDtoZip = simpleDtoXml.Deflate();

			Assert.That(simpleDtoZip.Length, Is.GreaterThan(0));

			var deserializedSimpleDtoXml = simpleDtoZip.Inflate();

			Assert.That(deserializedSimpleDtoXml, Is.Not.Empty);

			var deserializedSimpleDto = DataContractDeserializer.Instance.Parse<SimpleDto>(
				deserializedSimpleDtoXml);

			Assert.That(deserializedSimpleDto, Is.Not.Null);

			Assert.That(deserializedSimpleDto.Id, Is.EqualTo(simpleDto.Id));
			Assert.That(deserializedSimpleDto.Name, Is.EqualTo(simpleDto.Name));
		}

        /// <summary>Can compress and decompress simple dto with gzip.</summary>
		[Test]
		public void Can_compress_and_decompress_SimpleDto_with_Gzip()
		{
			var simpleDto = new SimpleDto(1, "name");

			var simpleDtoXml = DataContractSerializer.Instance.Parse(simpleDto);

			var simpleDtoZip = simpleDtoXml.GZip();

			Assert.That(simpleDtoZip.Length, Is.GreaterThan(0));

			var deserializedSimpleDtoXml = simpleDtoZip.GUnzip();

			Assert.That(deserializedSimpleDtoXml, Is.Not.Empty);

			var deserializedSimpleDto = DataContractDeserializer.Instance.Parse<SimpleDto>(
				deserializedSimpleDtoXml);

			Assert.That(deserializedSimpleDto, Is.Not.Null);

			Assert.That(deserializedSimpleDto.Id, Is.EqualTo(simpleDto.Id));
			Assert.That(deserializedSimpleDto.Name, Is.EqualTo(simpleDto.Name));
		}
	}
}