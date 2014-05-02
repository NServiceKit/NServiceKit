using NUnit.Framework;
using NServiceKit.Plugins.ProtoBuf;
using NServiceKit.ServiceClient.Web;
using NServiceKit.WebHost.IntegrationTests.Services;

namespace NServiceKit.WebHost.IntegrationTests.Tests
{
    /// <summary>A custom headers tests.</summary>
    [TestFixture]
    public class CustomHeadersTests
    {
        /// <summary>Gets the request.</summary>
        [Test]
        public void GetRequest()
        {
            var client = new JsonServiceClient(Config.NServiceKitBaseUri);
            client.Headers.Add("Foo","abc123");
            var response = client.Get(new CustomHeaders());
            Assert.That(response.Foo, Is.EqualTo("abc123"));
            Assert.That(response.Bar, Is.Null);
        }

        /// <summary>Posts the request.</summary>
        [Test]
        public void PostRequest()
        {
            var client = new XmlServiceClient(Config.NServiceKitBaseUri);
            client.Headers.Add("Bar", "abc123");
            client.Headers.Add("Foo", "xyz");
            var response = client.Post(new CustomHeaders());
            Assert.That(response.Bar, Is.EqualTo("abc123"));
            Assert.That(response.Foo, Is.EqualTo("xyz"));
        }

        /// <summary>Deletes this object.</summary>
        [Test]
        public void Delete()
        {
            var client = new ProtoBufServiceClient(Config.NServiceKitBaseUri);
            client.Headers.Add("Bar", "abc123");
            client.Headers.Add("Foo", "xyz");
            var response = client.Delete(new CustomHeaders());
            Assert.That(response.Bar, Is.EqualTo("abc123"));
            Assert.That(response.Foo, Is.EqualTo("xyz"));
        }
    }
}