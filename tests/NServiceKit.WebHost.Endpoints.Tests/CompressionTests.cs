using System;
using System.Runtime.Serialization;
using Funq;
using Moq;
using NUnit.Framework;
using NServiceKit.Common;
using NServiceKit.Common.Extensions;
using NServiceKit.Common.Web;
using NServiceKit.Logging;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface.Testing;
using NServiceKit.ServiceModel.Serialization;
using NServiceKit.WebHost.Endpoints.Extensions;
using NServiceKit.WebHost.Endpoints.Tests.Mocks;
using DataContractSerializer = NServiceKit.ServiceModel.Serialization.DataContractSerializer;

namespace NServiceKit.WebHost.Endpoints.Tests
{
	[DataContract(Namespace = "http://schemas.ddnglobal.com/types/")]
	public class TestCompress
	{
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>

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

        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.TestCompress class.</summary>
		public TestCompress()
		{
		}

        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.TestCompress class.</summary>
        ///
        /// <param name="id">  The identifier.</param>
        /// <param name="name">The name.</param>
		public TestCompress(int id, string name)
		{
			Id = id;
			Name = name;
		}
	}


    /// <summary>A compression tests.</summary>
	[TestFixture]
	public class CompressionTests
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(CompressionTests));

        /// <summary>Can compress and decompress simple dto.</summary>
		[Test]
		public void Can_compress_and_decompress_SimpleDto()
		{
			var simpleDto = new TestCompress(1, "name");

			var simpleDtoXml = DataContractSerializer.Instance.Parse(simpleDto);

			var simpleDtoZip = simpleDtoXml.Deflate();

			Assert.That(simpleDtoZip.Length, Is.GreaterThan(0));

			var deserializedSimpleDtoXml = simpleDtoZip.Inflate();

			Assert.That(deserializedSimpleDtoXml, Is.Not.Empty);

			var deserializedSimpleDto = DataContractDeserializer.Instance.Parse<TestCompress>(
				deserializedSimpleDtoXml);

			Assert.That(deserializedSimpleDto, Is.Not.Null);

			Assert.That(deserializedSimpleDto.Id, Is.EqualTo(simpleDto.Id));
			Assert.That(deserializedSimpleDto.Name, Is.EqualTo(simpleDto.Name));
		}


        /// <summary>Tests response with compressed result.</summary>
		[Test]
		public void Test_response_with_CompressedResult()
		{
			EndpointHost.Config = new EndpointHostConfig(
				"ServiceName",
				new ServiceManager(GetType().Assembly));

			var assembly = typeof (CompressionTests).Assembly;
			EndpointHost.ConfigureHost(
				new TestAppHost(new Container(), assembly), "Name", new ServiceManager(assembly));

			var mockResponse = new HttpResponseMock();

			var simpleDto = new TestCompress(1, "name");

			var simpleDtoXml = DataContractSerializer.Instance.Parse(simpleDto);

			const string expectedXml = "<TestCompress xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"http://schemas.ddnglobal.com/types/\"><Id>1</Id><Name>name</Name></TestCompress>";

			Assert.That(simpleDtoXml, Is.EqualTo(expectedXml));

			var simpleDtoZip = simpleDtoXml.Deflate();

			Assert.That(simpleDtoZip.Length, Is.GreaterThan(0));

			var compressedResult = new CompressedResult(simpleDtoZip);

			var reponseWasAutoHandled = mockResponse.WriteToResponse(
				compressedResult, CompressionTypes.Deflate);

			Assert.That(reponseWasAutoHandled, Is.True);

			//var bytesToWriteToResponseStream = new byte[simpleDtoZip.Length - 4];
			//Array.Copy(simpleDtoZip, CompressedResult.Adler32ChecksumLength, bytesToWriteToResponseStream, 0, bytesToWriteToResponseStream.Length);

			var bytesToWriteToResponseStream = simpleDtoZip;

			var writtenBytes = mockResponse.GetOutputStreamAsBytes();
			Assert.That(writtenBytes, Is.EqualTo(bytesToWriteToResponseStream));
			Assert.That(mockResponse.ContentType, Is.EqualTo(MimeTypes.Xml));
			Assert.That(mockResponse.Headers[HttpHeaders.ContentEncoding], Is.EqualTo(CompressionTypes.Deflate));

			Log.Debug("Content-length: " + writtenBytes.Length);
			Log.Debug(BitConverter.ToString(writtenBytes));
		}

        /// <summary>Can gzip and gunzip simple dto.</summary>
		[Test]
		public void Can_gzip_and_gunzip_SimpleDto()
		{
			var simpleDto = new TestCompress(1, "name");

			var simpleDtoXml = DataContractSerializer.Instance.Parse(simpleDto);

			var simpleDtoZip = simpleDtoXml.GZip();

			Assert.That(simpleDtoZip.Length, Is.GreaterThan(0));

			var deserializedSimpleDtoXml = simpleDtoZip.GUnzip();

			Assert.That(deserializedSimpleDtoXml, Is.Not.Empty);

			var deserializedSimpleDto = DataContractDeserializer.Instance.Parse<TestCompress>(
				deserializedSimpleDtoXml);

			Assert.That(deserializedSimpleDto, Is.Not.Null);

			Assert.That(deserializedSimpleDto.Id, Is.EqualTo(simpleDto.Id));
			Assert.That(deserializedSimpleDto.Name, Is.EqualTo(simpleDto.Name));
		}

	}
}