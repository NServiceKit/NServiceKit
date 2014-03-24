using System;
using NUnit.Framework;
using NServiceKit.ServiceClient.Web;
using NServiceKit.ServiceInterface.Auth;
using NServiceKit.Text;

namespace NServiceKit.Auth.Tests
{
    public class TestBase
    {
        protected JsonServiceClient Client { get; set; }
        protected static readonly string BaseUri = "http://localhost:8080/api";

        [TestFixtureSetUp]
        public void OnTestFixtureSetUp()
        {
            Client = new JsonServiceClient(BaseUri);
            var response= Client.Post<AuthResponse>("/auth",
                new ServiceInterface.Auth.Auth { UserName = "test1", Password = "test1" });

            Console.WriteLine(response.Dump());
        }

        [TestFixtureTearDown]
        public void OnTestFixtureTearDown()
        {
            Client.Get<AuthResponse>("/auth/logout");
        }
    }

}
