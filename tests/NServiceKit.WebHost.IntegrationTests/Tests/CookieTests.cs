using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NUnit.Framework;
using NServiceKit.ServiceClient.Web;
using NServiceKit.WebHost.IntegrationTests.Services;

namespace NServiceKit.WebHost.IntegrationTests.Tests
{
    [TestFixture]
    public class CookieTests
    {
        [Test]
        public void Handles_malicious_php_cookies()
        {
            var client = new JsonServiceClient(Config.NServiceKitBaseUri);
            client.StoreCookies = false;
            client.LocalHttpWebRequestFilter = r => r.Headers["Cookie"] = "$Version=1; $Path=/; $Path=/; RealCookie=choc-chip";
            //client.Headers.Add("Cookie", "$Version=1; $Path=/; $Path=/");

            var response = client.Get(new Cookies());
            Assert.That(response.RequestCookieNames, Contains.Item("RealCookie"));
        }
    }
}