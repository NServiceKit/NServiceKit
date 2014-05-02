using System;
using NUnit.Framework;
using NServiceKit.Common.Web;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface.Testing;
using NServiceKit.WebHost.Endpoints.Extensions;

namespace NServiceKit.Common.Tests
{
    /// <summary>An endpoint handler base tests.</summary>
    [TestFixture]
    public class EndpointHandlerBaseTests
    {
        /// <summary>Creates a request.</summary>
        ///
        /// <param name="userHostAddress">The user host address.</param>
        ///
        /// <returns>The new request.</returns>
        public IHttpRequest CreateRequest(string userHostAddress)
        {
            var httpReq = new MockHttpRequest("test", HttpMethods.Get, ContentType.Json, "/", null, null, null) {
                UserHostAddress = userHostAddress
            };
            return httpReq;
        }

        /// <summary>Can parse ips.</summary>
        [Test]
        public void Can_parse_Ips()
        {
            var result = CreateRequest("204.2.145.235").GetAttributes();

            Assert.That(result.Has(EndpointAttributes.External));
            Assert.That(result.Has(EndpointAttributes.HttpGet));
            Assert.That(result.Has(EndpointAttributes.InSecure));
        }

        [Flags]
        enum A : int { B = 0, C = 2, D = 4 }

        /// <summary>Can parse int enums.</summary>
        [Test]
        public void Can_parse_int_enums()
        {
            var result = A.B | A.C;
            Assert.That(result.Has(A.C));
            Assert.That(!result.Has(A.D));
        }
    }
}