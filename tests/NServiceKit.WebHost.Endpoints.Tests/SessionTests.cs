using System;
using Funq;
using NUnit.Framework;
using NServiceKit.Common;
using NServiceKit.ServiceClient.Web;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface;
using NServiceKit.ServiceInterface.Auth;

namespace NServiceKit.WebHost.Endpoints.Tests
{
    /// <summary>A session increment.</summary>
    public class SessionIncr : IReturn<SessionResponse> { }

    /// <summary>A session response.</summary>
    public class SessionResponse
    {
        /// <summary>Gets or sets the counter.</summary>
        ///
        /// <value>The counter.</value>
        public int Counter { get; set; }
    }

    /// <summary>A session cartesian increment.</summary>
    public class SessionCartIncr : IReturn<Cart>
    {
        /// <summary>Gets or sets the identifier of the cartesian.</summary>
        ///
        /// <value>The identifier of the cartesian.</value>
        public Guid CartId { get; set; }
    }

    /// <summary>A cartesian.</summary>
    public class Cart
    {
        /// <summary>Gets or sets the qty.</summary>
        ///
        /// <value>The qty.</value>
        public int Qty { get; set; }
    }

    /// <summary>A session typed increment.</summary>
    public class SessionTypedIncr : IReturn<AuthUserSession> {}

    /// <summary>A session service.</summary>
    public class SessionService : ServiceInterface.Service
    {
        /// <summary>Gets the given request.</summary>
        ///
        /// <param name="request">The request to get.</param>
        ///
        /// <returns>An AuthUserSession.</returns>
        public SessionResponse Get(SessionIncr request)
        {
            var counter = base.Session.Get<int>("counter");

            base.Session["counter"] = ++counter;

            return new SessionResponse
            {
                Counter = counter
            };
        }

        /// <summary>Gets the given request.</summary>
        ///
        /// <param name="request">The request to get.</param>
        ///
        /// <returns>An AuthUserSession.</returns>
        public Cart Get(SessionCartIncr request)
        {
            var sessionKey = UrnId.Create<Cart>(request.CartId);
            var cart = base.Session.Get<Cart>(sessionKey) ?? new Cart();
            cart.Qty++;

            base.Session[sessionKey] = cart;

            return cart;
        }

        /// <summary>Gets the given request.</summary>
        ///
        /// <param name="request">The request to get.</param>
        ///
        /// <returns>An AuthUserSession.</returns>
        public AuthUserSession Get(SessionTypedIncr request)
        {
            var session = base.SessionAs<AuthUserSession>();
            session.Tag++;

            this.SaveSession(session);

            return session;
        }
    }

    /// <summary>A session tests.</summary>
    [TestFixture]
    public class SessionTests
    {
        private SessionAppHost appHost;

        /// <summary>Tests fixture set up.</summary>
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            appHost = new SessionAppHost();
            appHost.Init();
            appHost.Start(Config.AbsoluteBaseUri);
        }

        /// <summary>Tests fixture tear down.</summary>
        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            appHost.Dispose();
        }

        /// <summary>A session application host.</summary>
        public class SessionAppHost : AppHostHttpListenerBase
        {
            /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.SessionTests.SessionAppHost class.</summary>
            public SessionAppHost() : base(typeof(SessionTests).Name, typeof(SessionTests).Assembly) {}

            /// <summary>Configures the given container.</summary>
            ///
            /// <param name="container">The container.</param>
            public override void Configure(Container container)
            {
                Plugins.Add(new SessionFeature());
            }
        }

        /// <summary>Can increment session int counter.</summary>
        [Test]
        public void Can_increment_session_int_counter()
        {
            var client = new JsonServiceClient(Config.AbsoluteBaseUri);

            Assert.That(client.Get(new SessionIncr()).Counter, Is.EqualTo(1));
            Assert.That(client.Get(new SessionIncr()).Counter, Is.EqualTo(2));
        }

        /// <summary>Different clients have different session int counters.</summary>
        [Test]
        public void Different_clients_have_different_session_int_counters()
        {
            var client = new JsonServiceClient(Config.AbsoluteBaseUri);
            var altClient = new JsonServiceClient(Config.AbsoluteBaseUri);

            Assert.That(client.Get(new SessionIncr()).Counter, Is.EqualTo(1));
            Assert.That(client.Get(new SessionIncr()).Counter, Is.EqualTo(2));
            Assert.That(altClient.Get(new SessionIncr()).Counter, Is.EqualTo(1));
        }

        /// <summary>Can increment session cartesian qty.</summary>
        [Test]
        public void Can_increment_session_cart_qty()
        {
            var client = new JsonServiceClient(Config.AbsoluteBaseUri);
            var request = new SessionCartIncr { CartId = Guid.NewGuid() };

            Assert.That(client.Get(request).Qty, Is.EqualTo(1));
            Assert.That(client.Get(request).Qty, Is.EqualTo(2));
        }

        /// <summary>Different clients have different session cartesian qty.</summary>
        [Test]
        public void Different_clients_have_different_session_cart_qty()
        {
            var client = new JsonServiceClient(Config.AbsoluteBaseUri);
            var altClient = new JsonServiceClient(Config.AbsoluteBaseUri);
            var request = new SessionCartIncr { CartId = Guid.NewGuid() };

            Assert.That(client.Get(request).Qty, Is.EqualTo(1));
            Assert.That(client.Get(request).Qty, Is.EqualTo(2));
            Assert.That(altClient.Get(request).Qty, Is.EqualTo(1));
        }

        /// <summary>Can increment typed session tag.</summary>
        [Test]
        public void Can_increment_typed_session_tag()
        {
            var client = new JsonServiceClient(Config.AbsoluteBaseUri);

            Assert.That(client.Get(new SessionTypedIncr()).Tag, Is.EqualTo(1));
            Assert.That(client.Get(new SessionTypedIncr()).Tag, Is.EqualTo(2));
        }

        /// <summary>Different clients have different typed session tag.</summary>
        [Test]
        public void Different_clients_have_different_typed_session_tag()
        {
            var client = new JsonServiceClient(Config.AbsoluteBaseUri);
            var altClient = new JsonServiceClient(Config.AbsoluteBaseUri);

            Assert.That(client.Get(new SessionTypedIncr()).Tag, Is.EqualTo(1));
            Assert.That(client.Get(new SessionTypedIncr()).Tag, Is.EqualTo(2));
            Assert.That(altClient.Get(new SessionTypedIncr()).Tag, Is.EqualTo(1));
        }
    }
}