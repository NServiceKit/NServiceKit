using System;
using System.Text;
using NUnit.Framework;
using NServiceKit.ServiceClient.Web;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface;

namespace NServiceKit.WebHost.Endpoints.Tests
{
    //[Route("/HelloWorld/Greeting/{FirstName}/{LastName}", "GET")]
    /// <summary>A hello world name.</summary>
    [Route("/HelloWorld/Greeting/{FirstName}", "GET")]
    [Restrict(EndpointAttributes.InternalNetworkAccess)]
    public class HelloWorldName : IReturn<HelloWorldGreeting>
    {
        /// <summary>Gets or sets the person's first name.</summary>
        ///
        /// <value>The name of the first.</value>
        public string FirstName { get; set; }

        /// <summary>Gets or sets the person's last name.</summary>
        ///
        /// <value>The name of the last.</value>
        public string LastName { get; set; }
    }

    /// <summary>A hello world greeting.</summary>
    public class HelloWorldGreeting
    {
        /// <summary>Gets or sets the greeting.</summary>
        ///
        /// <value>The greeting.</value>
        public string Greeting { get; set; }
    }

    /// <summary>A hello world service.</summary>
    public class HelloWorldService : ServiceInterface.Service
    {
        /// <summary>Gets the given request.</summary>
        ///
        /// <param name="request">The request to get.</param>
        ///
        /// <returns>A HelloWorldGreeting.</returns>
        public HelloWorldGreeting Get(HelloWorldName request)
        {
            var answer = new HelloWorldGreeting
            {
                Greeting = "Hello " + request.FirstName + " " + request.LastName
            };
            return answer;
        }
    }

    /// <summary>An encoding tests application host.</summary>
    public class EncodingTestsAppHost : AppHostHttpListenerBase
    {
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.EncodingTestsAppHost class.</summary>
        public EncodingTestsAppHost() : base("EncodingTests", typeof(HelloWorldService).Assembly) { }

        /// <summary>Configures the given container.</summary>
        ///
        /// <param name="container">The container.</param>
        public override void Configure(Funq.Container container) {}
    }

    /// <summary>An encoding tests.</summary>
    [TestFixture]
    public class EncodingTests
    {
        private EncodingTestsAppHost appHost;

        /// <summary>Tests fixture set up.</summary>
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            appHost = new EncodingTestsAppHost();
            appHost.Init();
            appHost.Start(Config.AbsoluteBaseUri);
        }

        /// <summary>Tests fixture tear down.</summary>
        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            appHost.Dispose();
            appHost = null;
        }

        private HelloWorldGreeting PerformRequest(string firstName, string lastName)
        {
            var client = new JsonServiceClient(Config.NServiceKitBaseUri);
            var query = string.Format("/HelloWorld/Greeting/{0}?lastname={1}", firstName, lastName);
            return client.Get<HelloWorldGreeting>(query);
        }

        /// <summary>Can get greeting when querystring contains non ASCII characters.</summary>
        [Test]
        public void Can_Get_Greeting_When_Querystring_Contains_Non_ASCII_Chars()
        {
            var firstName = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes("Pål"));
            var lastName = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes("Smådalø"));
            Assert.That(PerformRequest(firstName, lastName).Greeting, Is.EqualTo(string.Format("Hello {0} {1}", firstName, lastName)));
        }

        /// <summary>Can get greeting when only URL contains non ASCII characters.</summary>
        [Test]
        public void Can_Get_Greeting_When_Only_Url_Contains_Non_ASCII_Chars()
        {
            var firstName = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes("Pål"));
            var lastName = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes("Smith"));
            Assert.That(PerformRequest(firstName, lastName).Greeting, Is.EqualTo(string.Format("Hello {0} {1}", firstName, lastName)));
        }

        /// <summary>Can get greeting when querystring contains only ASCII characters.</summary>
        [Test]
        public void Can_Get_Greeting_When_Querystring_Contains_Only_ASCII_Chars()
        {
            var firstName = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes("John"));
            var lastName = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes("Smith"));
            Assert.That(PerformRequest(firstName, lastName).Greeting, Is.EqualTo(string.Format("Hello {0} {1}", firstName, lastName)));
        }
    }

}