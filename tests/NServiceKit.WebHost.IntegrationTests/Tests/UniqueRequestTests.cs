using Funq;
using NUnit.Framework;
using NServiceKit.Common;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface;
using NServiceKit.Text;

namespace NServiceKit.WebHost.IntegrationTests.Tests
{
    /// <summary>A unique request.</summary>
    [Route("/request/{Id}")]
    public class UniqueRequest
    {
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
        public string Id { get; set; }
    }

    /// <summary>A unique request service.</summary>
    public class UniqueRequestService : IService
    {
        /// <summary>Gets the given unique request.</summary>
        ///
        /// <param name="uniqueRequest">The unique request to get.</param>
        ///
        /// <returns>A string.</returns>
        public string Get(UniqueRequest uniqueRequest)
        {
            return uniqueRequest.Id;
        }
    }

    /// <summary>A unique request tests.</summary>
    [TestFixture]
    public class UniqueRequestTests
    {
        private const string BaseUri = Config.AbsoluteBaseUri;

        [Test]
        [Explicit("ASP.NET does not allow invalid chars see http://stackoverflow.com/questions/13691829/path-parameters-w-url-unfriendly-characters")]
        public void Can_handle_encoded_chars()
        {
            var response = BaseUri.CombineWith("request/123%20456").GetStringFromUrl();
            Assert.That(response, Is.EqualTo("123%20456"));
            response = BaseUri.CombineWith("request/123%7C456").GetStringFromUrl();
            Assert.That(response, Is.EqualTo("123%7C456"));
        }

    }
}
