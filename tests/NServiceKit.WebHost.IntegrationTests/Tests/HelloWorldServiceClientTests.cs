using System;
using System.Collections;
using System.Threading;
using NUnit.Framework;
using NServiceKit.Service;
using NServiceKit.ServiceClient.Web;
using NServiceKit.WebHost.IntegrationTests.Services;

namespace NServiceKit.WebHost.IntegrationTests.Tests
{
    [TestFixture]
    public class HelloWorldServiceClientTests
    {
        public static IEnumerable ServiceClients
        {
            get
            {
                return new IServiceClient[] {
					new JsonServiceClient(Config.NServiceKitBaseUri),
					new JsvServiceClient(Config.NServiceKitBaseUri),
					new XmlServiceClient(Config.NServiceKitBaseUri),
					new Soap11ServiceClient(Config.NServiceKitBaseUri),
					new Soap12ServiceClient(Config.NServiceKitBaseUri)
				};
            }
        }

        public static IEnumerable RestClients
        {
            get
            {
                return new IRestClient[] {
					new JsonServiceClient(Config.NServiceKitBaseUri),
					new JsvServiceClient(Config.NServiceKitBaseUri),
					new XmlServiceClient(Config.NServiceKitBaseUri),
				};
            }
        }

        [Test, TestCaseSource("ServiceClients")]
        public void Sync_Call_HelloWorld_with_Sync_ServiceClients_on_PreDefined_Routes(IServiceClient client)
        {
            var response = client.Send<HelloResponse>(new Hello { Name = "World!" });

            Assert.That(response.Result, Is.EqualTo("Hello, World!"));
        }

        [Test, TestCaseSource("RestClients")]
        public void Async_Call_HelloWorld_with_ServiceClients_on_PreDefined_Routes(IServiceClient client)
        {
            HelloResponse response = null;
            client.SendAsync<HelloResponse>(new Hello { Name = "World!" },
                r => response = r, (r, e) => Assert.Fail("NetworkError"));

            Thread.Sleep(TimeSpan.FromSeconds(1));

            Assert.That(response.Result, Is.EqualTo("Hello, World!"));
        }

        [Test, TestCaseSource("RestClients")]
        public void Sync_Call_HelloWorld_with_RestClients_on_UserDefined_Routes(IRestClient client)
        {
            var response = client.Get<HelloResponse>("/hello/World!");

            Assert.That(response.Result, Is.EqualTo("Hello, World!"));
        }

        [Test, TestCaseSource("RestClients")]
        public void Async_Call_HelloWorld_with_Async_ServiceClients_on_UserDefined_Routes(IServiceClient client)
        {
            HelloResponse response = null;
            client.GetAsync<HelloResponse>("/hello/World!",
                r => response = r, (r, e) => Assert.Fail("NetworkError"));

            var i = 0;
            while (response == null && i++ < 5)
                Thread.Sleep(TimeSpan.FromSeconds(1));

            Assert.That(response.Result, Is.EqualTo("Hello, World!"));
        }
    }
}