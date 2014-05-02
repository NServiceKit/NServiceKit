using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using NServiceKit.Plugins.ProtoBuf;
using NServiceKit.ServiceClient.Web;
using NServiceKit.Text;
using NServiceKit.WebHost.IntegrationTests.Services;

namespace NServiceKit.WebHost.IntegrationTests.Tests
{
    /// <summary>A binary serialized tests.</summary>
    [TestFixture]
    public class BinarySerializedTests
    {
        private string RandomString(int Length)
        {
            var rnd = new Random();
            var tmp = new StringBuilder();
            for (Int64 i = 0; i < Length; i++)
            {
                tmp.Append(Convert.ToChar(((byte)rnd.Next(254))).ToString());
            }
            return Convert.ToBase64String(tmp.ToString().ToUtf8Bytes());
        }

        /// <summary>Can serialize random string.</summary>
        [Test]
        public void Can_serialize_RandomString()
        {
            var rand = RandomString(32);
            using (var ms = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(ms, rand);
                ms.Position = 0;
                var fromBytes = ProtoBuf.Serializer.Deserialize<string>(ms);

                Assert.That(rand, Is.EqualTo(fromBytes));
            }
        }

        /// <summary>Can call cached web service with protobuf.</summary>
        [Test]
        public void Can_call_cached_WebService_with_Protobuf()
        {
            var client = new ProtoBufServiceClient(Config.NServiceKitBaseUri);

            try
            {
                var fromEmail = RandomString(32);
                var response = client.Post<ProtoBufEmail>(
                    "/cached/protobuf", 
                    new CachedProtoBufEmail {
                        FromAddress = fromEmail
                    });

                response.PrintDump();

                Assert.That(response.FromAddress, Is.EqualTo(fromEmail));
            }
            catch (WebServiceException webEx)
            {
                webEx.ResponseDto.PrintDump();
                Assert.Fail(webEx.Message);
            }
        }

        /// <summary>Can call web service with protobuf.</summary>
        [Test]
        public void Can_call_WebService_with_Protobuf()
        {
            //new ProtoBufServiceTests().Can_Send_ProtoBuf_request();

            var client = new ProtoBufServiceClient(Config.NServiceKitBaseUri);

            try
            {
                var fromEmail = RandomString(32);
                var response = client.Post<ProtoBufEmail>(
                    "/cached/protobuf",
                    new UncachedProtoBufEmail {
                        FromAddress = fromEmail
                    });

                response.PrintDump();

                Assert.That(response.FromAddress, Is.EqualTo(fromEmail));
            }
            catch (WebServiceException webEx)
            {
                webEx.ResponseDto.PrintDump();
                Assert.Fail(webEx.Message);
            }
        }

    }
}