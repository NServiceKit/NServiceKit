using NUnit.Framework;
using NServiceKit.WebHost.Endpoints.Metadata;
using System.Runtime.Serialization;

namespace NServiceKit.WebHost.Endpoints.Tests
{
    /// <summary>An XML meta data handler tests.</summary>
    [TestFixture]
    public class XmlMetaDataHandlerTests
    {
        /// <summary>When creating a response for a dto with no default constructor the response is not empty.</summary>
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
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.NoDefaultConstructor class.</summary>
        ///
        /// <param name="test">The test.</param>
        public NoDefaultConstructor(string test)
        { }

        /// <summary>Gets or sets the value.</summary>
        ///
        /// <value>The value.</value>

        /// <summary>Gets or sets the value.</summary>
        ///
        /// <value>The value.</value>
        [DataMember]
        public string Value { get; set; }
    }
}
