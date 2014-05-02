using System;
using NUnit.Framework;
using NServiceKit.ServiceClient.Web;
using NServiceKit.ServiceInterface.Auth;
using NServiceKit.Text;

namespace NServiceKit.Auth.Tests
{
    /// <summary>A test base.</summary>
    public class TestBase
    {
        /// <summary>Gets or sets the client.</summary>
        ///
        /// <value>The client.</value>
        protected JsonServiceClient Client { get; set; }
        /// <summary>URI of the base.</summary>
        protected static readonly string BaseUri = "http://localhost:8080/api";

        /// <summary>Executes the test fixture set up action.</summary>
        [TestFixtureSetUp]
        public void OnTestFixtureSetUp()
        {
            Client = new JsonServiceClient(BaseUri);
            var response= Client.Post<AuthResponse>("/auth",
                new ServiceInterface.Auth.Auth { UserName = "test1", Password = "test1" });

            Console.WriteLine(response.Dump());
        }

        /// <summary>Executes the test fixture tear down action.</summary>
        [TestFixtureTearDown]
        public void OnTestFixtureTearDown()
        {
            Client.Get<AuthResponse>("/auth/logout");
        }
    }

}
