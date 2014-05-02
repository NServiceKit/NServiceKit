using System.Linq;
using NUnit.Framework;
using NServiceKit.ServiceInterface;
using NServiceKit.ServiceInterface.Testing;
using NServiceKit.WebHost.Endpoints;

namespace NServiceKit.ServiceHost.Tests.Routes
{
    /// <summary>A service routes tests.</summary>
    [TestFixture]
    public class ServiceRoutesTests
    {
        BasicAppHost loadAppHost;

        /// <summary>Tests fixture set up.</summary>
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            loadAppHost = new BasicAppHost().Init();
        }

        /// <summary>Tests fixture tear down.</summary>
        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            loadAppHost.Dispose();
        }

        /// <summary>Can register new API routes from assembly.</summary>
        [Test]
        public void Can_Register_NewApi_Routes_From_Assembly()
        {
            var routes = new ServiceRoutes();
            routes.AddFromAssembly(typeof(NewApiRestServiceWithAllVerbsImplemented).Assembly);

            RestPath restWithAllMethodsRoute =
                (from r in routes.RestPaths
                 where r.Path == "/NewApiRequestDto"
                 select r).FirstOrDefault();

            Assert.That(restWithAllMethodsRoute, Is.Not.Null);

            Assert.That(restWithAllMethodsRoute.AllowedVerbs.Contains("GET"));
            Assert.That(restWithAllMethodsRoute.AllowedVerbs.Contains("POST"));
            Assert.That(restWithAllMethodsRoute.AllowedVerbs.Contains("PUT"));
            Assert.That(restWithAllMethodsRoute.AllowedVerbs.Contains("DELETE"));
            Assert.That(restWithAllMethodsRoute.AllowedVerbs.Contains("PATCH"));

            RestPath restWithAllMethodsRoute2 =
                (from r in routes.RestPaths
                 where r.Path == "/NewApiRequestDto2"
                 select r).FirstOrDefault();

            Assert.That(restWithAllMethodsRoute2, Is.Not.Null);

            Assert.That(restWithAllMethodsRoute2.AllowedVerbs.Contains("GET"));
            Assert.That(restWithAllMethodsRoute2.AllowedVerbs.Contains("POST"));
            Assert.That(restWithAllMethodsRoute2.AllowedVerbs.Contains("PUT"));
            Assert.That(restWithAllMethodsRoute2.AllowedVerbs.Contains("DELETE"));
            Assert.That(restWithAllMethodsRoute2.AllowedVerbs.Contains("PATCH"));
        }

        /// <summary>Can register new API routes with identifier and any fallback from assembly.</summary>
        [Test]
        public void Can_Register_NewApi_Routes_With_Id_and_Any_Fallback_From_Assembly()
        {
            var routes = new ServiceRoutes();
            routes.AddFromAssembly(typeof(NewApiRequestDtoWithIdService).Assembly);

            var route = (from r in routes.RestPaths
                         where r.Path == "/NewApiRequestDtoWithId"
                         select r).FirstOrDefault();

            Assert.That(route, Is.Not.Null);
            Assert.That(route.AllowedVerbs, Is.Null);

            route = (from r in routes.RestPaths
                     where r.Path == "/NewApiRequestDtoWithId/{Id}"
                     select r).FirstOrDefault();

            Assert.That(route, Is.Not.Null);
            Assert.That(route.AllowedVerbs, Is.Null);
        }

        /// <summary>Can register new API routes with field identifier and any fallback from assembly.</summary>
		[Test]
		public void Can_Register_NewApi_Routes_With_Field_Id_and_Any_Fallback_From_Assembly()
		{
			var routes = new ServiceRoutes();
			routes.AddFromAssembly(typeof(NewApiRequestDtoWithFieldIdService).Assembly);

			var route = (from r in routes.RestPaths
						 where r.Path == "/NewApiRequestDtoWithFieldId"
						 select r).FirstOrDefault();

			Assert.That(route, Is.Not.Null);
			Assert.That(route.AllowedVerbs, Is.Null);

			route = (from r in routes.RestPaths
					 where r.Path == "/NewApiRequestDtoWithFieldId/{Id}"
					 select r).FirstOrDefault();

			Assert.That(route, Is.Not.Null);
			Assert.That(route.AllowedVerbs, Is.Null);
		}

        /// <summary>Can register old API routes from assembly.</summary>
        [Test]
        public void Can_Register_OldApi_Routes_From_Assembly()
        {
            var routes = new ServiceRoutes();
            routes.AddFromAssembly(typeof(OldApiRestServiceWithAllVerbsImplemented).Assembly);

            RestPath restWithAllMethodsRoute =
                (from r in routes.RestPaths
                 where r.Path == "/OldApiRequestDto2"
                 select r).FirstOrDefault();

            Assert.That(restWithAllMethodsRoute, Is.Not.Null);

            Assert.That(restWithAllMethodsRoute.AllowedVerbs.Contains("GET"));
            Assert.That(restWithAllMethodsRoute.AllowedVerbs.Contains("POST"));
            Assert.That(restWithAllMethodsRoute.AllowedVerbs.Contains("PUT"));
            Assert.That(restWithAllMethodsRoute.AllowedVerbs.Contains("DELETE"));
            Assert.That(restWithAllMethodsRoute.AllowedVerbs.Contains("PATCH"));
        }

        /// <summary>Can register old API routes with partially implemented rest verbs.</summary>
        [Test]
        public void Can_Register_OldApi_Routes_With_Partially_Implemented_REST_Verbs()
        {
            var routes = new ServiceRoutes();
            routes.AddFromAssembly(typeof(OldApiRestServiceWithSomeVerbsImplemented).Assembly);

            RestPath restWithAFewMethodsRoute =
                (from r in routes.RestPaths
                 where r.Path == "/OldApiRequestDto"
                 select r).FirstOrDefault();

            Assert.That(restWithAFewMethodsRoute, Is.Not.Null);

            Assert.That(restWithAFewMethodsRoute.AllowedVerbs.Contains("GET"), Is.True);
            Assert.That(restWithAFewMethodsRoute.AllowedVerbs.Contains("POST"), Is.False);
            Assert.That(restWithAFewMethodsRoute.AllowedVerbs.Contains("PUT"), Is.True);
            Assert.That(restWithAFewMethodsRoute.AllowedVerbs.Contains("DELETE"), Is.False);
            Assert.That(restWithAFewMethodsRoute.AllowedVerbs.Contains("PATCH"), Is.False);
        }

        /// <summary>Can register routes using add extension.</summary>
        [Test]
        public void Can_Register_Routes_Using_Add_Extension()
        {
            var routes = new ServiceRoutes();
            routes.Add<Customer>("/Users/{0}/Orders/{1}", ApplyTo.Get, x => x.Name, x => x.OrderId);
            var route = routes.RestPaths[0];
            Assert.That(route.Path == "/Users/{Name}/Orders/{OrderId}");
        }
    }

    /// <summary>A customer.</summary>
    public class Customer
    {
        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>Gets or sets the identifier of the order.</summary>
        ///
        /// <value>The identifier of the order.</value>
        public int OrderId { get; set; }
    }
}