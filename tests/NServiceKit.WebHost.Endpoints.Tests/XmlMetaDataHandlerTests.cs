using NUnit.Framework;
using NServiceKit.WebHost.Endpoints.Metadata;
using System.Runtime.Serialization;

namespace NServiceKit.WebHost.Endpoints.Tests
{
    [TestFixture]
    public class XmlMetaDataHandlerTests
    {
        [Test]
        public void when_creating_a_response_for_a_dto_with_no_default_constructor_the_response_is_not_empty()
        {
            var handler = new XmlMetadataHandler();
            var response = handler.CreateResponse(typeof(NoDefaultConstructor));
            Assert.That(response, Is.Not.Empty);
        }

    }

    [DataContract(Namespace = "http://schemas.NServiceKit.net/types")]
    public class NoDefaultConstructor
    {
        public NoDefaultConstructor(string test)
        { }

        [DataMember]
        public string Value { get; set; }
    }
}
