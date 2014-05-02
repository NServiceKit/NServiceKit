using System;
using System.Collections;
using System.Linq;
using System.Net.NetworkInformation;
using Moq;
using NUnit.Framework;
using NServiceKit.ServiceHost;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints.Extensions;

namespace NServiceKit.WebHost.Endpoints.Support.Tests
{
    /// <summary>An endpoint handler base tests.</summary>
    [TestFixture]
    public class EndpointHandlerBaseTests
    {
        class TestHandler : EndpointHandlerBase
        {
            /// <summary>Creates a request.</summary>
            ///
            /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
            ///
            /// <param name="request">      The request.</param>
            /// <param name="operationName">Name of the operation.</param>
            ///
            /// <returns>The new request.</returns>
            public override object CreateRequest(ServiceHost.IHttpRequest request, string operationName)
            {
                throw new NotImplementedException();
            }

            /// <summary>Gets a response.</summary>
            ///
            /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
            ///
            /// <param name="httpReq">The HTTP request.</param>
            /// <param name="httpRes">The HTTP resource.</param>
            /// <param name="request">The request.</param>
            ///
            /// <returns>The response.</returns>
            public override object GetResponse(IHttpRequest httpReq, IHttpResponse httpRes, object request)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>Gets endpoint attributes accepts user host address formats.</summary>
        ///
        /// <param name="format">  Describes the format to use.</param>
        /// <param name="expected">The expected.</param>
        [Test, TestCaseSource(typeof(EndpointHandlerBaseTests), "EndpointExpectations")]
        public void GetEndpointAttributes_AcceptsUserHostAddressFormats(string format, EndpointAttributes expected)
        {
            var handler = new TestHandler();
            var request = new Mock<IHttpRequest>();
            request.Expect(req => req.UserHostAddress).Returns(format);
            request.Expect(req => req.IsSecureConnection).Returns(false);
            request.Expect(req => req.HttpMethod).Returns("GET");

            Assert.AreEqual(expected | EndpointAttributes.HttpGet | EndpointAttributes.InSecure, request.Object.GetAttributes());
        }

        /// <summary>Gets the endpoint expectations.</summary>
        ///
        /// <value>The endpoint expectations.</value>
        public static IEnumerable EndpointExpectations
        {
            get
            {
                var ipv6Addresses = NetworkInterface.GetAllNetworkInterfaces()
                    .Where(nic => nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                    .SelectMany(nic => nic.GetIPProperties()
                        .UnicastAddresses.Select(unicast => unicast.Address))
                        .Where(address => address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6).ToList();

                //this covers all the different flavors of ipv6 address -- scoped, link local, etc
                foreach (var address in ipv6Addresses)
                {
                    yield return new TestCaseData(address.ToString(), EndpointAttributes.LocalSubnet);
                    yield return new TestCaseData("[" + address + "]:57", EndpointAttributes.LocalSubnet);
                    // HttpListener Format w/Port
                    yield return new TestCaseData("[{0}]:8080".Fmt(address), EndpointAttributes.LocalSubnet);
                }

                yield return new TestCaseData("fe80::100:7f:fffe%10", EndpointAttributes.LocalSubnet);
                yield return new TestCaseData("[fe80::100:7f:fffe%10]:57", EndpointAttributes.LocalSubnet);
                yield return new TestCaseData("[fe80::100:7f:fffe%10]:8080", EndpointAttributes.LocalSubnet);

                //ipv6 loopback
                yield return new TestCaseData("::1", EndpointAttributes.Localhost);
                yield return new TestCaseData("[::1]:83", EndpointAttributes.Localhost);

                //ipv4
                yield return new TestCaseData("192.168.100.2", EndpointAttributes.External);
                yield return new TestCaseData("192.168.100.2:47", EndpointAttributes.External);

                //ipv4 loopback
                yield return new TestCaseData("127.0.0.1", EndpointAttributes.Localhost);
                yield return new TestCaseData("127.0.0.1:20", EndpointAttributes.Localhost);

                //ipv4 in X-FORWARDED-FOR HTTP Header format
                yield return new TestCaseData("192.168.100.2, 192.168.0.1", EndpointAttributes.External);
                yield return new TestCaseData("192.168.100.2, 192.168.0.1, 10.1.1.1", EndpointAttributes.External);
            }
        }
    }
}