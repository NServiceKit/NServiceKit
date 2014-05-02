using System;
using System.Reflection;
using Funq;
using NUnit.Framework;
using NServiceKit.ServiceHost.Tests.Support;
using NServiceKit.ServiceInterface.Testing;
using NServiceKit.WebHost.Endpoints;

namespace NServiceKit.ServiceHost.Tests
{
    /// <summary>A service controller tests.</summary>
    [TestFixture]
    public class ServiceControllerTests
    {
        /// <summary>Tests fixture set up.</summary>
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            EndpointHostConfig.SkipRouteValidation = true;
        }

        /// <summary>Can register all services in an assembly.</summary>
        [Test]
        public void Can_register_all_services_in_an_assembly()
        {
            var serviceManager = new ServiceManager(typeof(BasicService).Assembly);
            serviceManager.Init();

            var container = serviceManager.Container;
            container.Register<IFoo>(c => new Foo());
            container.Register<IBar>(c => new Bar());

            var serviceController = serviceManager.ServiceController;

            var request = new AutoWire();

            var response = serviceController.Execute(request) as AutoWireResponse;

            Assert.That(response, Is.Not.Null);
        }

        /// <summary>Can override service creation with custom implementation.</summary>
        [Test]
        public void Can_override_service_creation_with_custom_implementation()
        {
            var serviceManager = new ServiceManager(typeof(BasicService).Assembly);
            serviceManager.Init();

            var container = serviceManager.Container;
            container.Register<IFoo>(c => new Foo());
            container.Register<IBar>(c => new Bar());

            var serviceController = serviceManager.ServiceController;

            var request = new AutoWire();

            var response = serviceController.Execute(request) as AutoWireResponse;

            Assert.That(response, Is.Not.Null);
            Assert.That(response.Foo as Foo, Is.Not.Null);
            Assert.That(response.Bar as Bar, Is.Not.Null);

            container.Register(c =>
                new AutoWireService(new Foo2())
                {
                    Bar = new Bar2()
                });

            response = serviceController.Execute(request) as AutoWireResponse;

            Assert.That(response, Is.Not.Null);
            Assert.That(response.Foo as Foo2, Is.Not.Null);
            Assert.That(response.Bar as Bar2, Is.Not.Null);
        }

        /// <summary>Can inject request context for i requires request context services.</summary>
        [Test]
        public void Can_inject_RequestContext_for_IRequiresRequestContext_services()
        {
            var serviceManager = new ServiceManager(typeof(RequiresContextService).Assembly);
            serviceManager.Init();

            var serviceController = serviceManager.ServiceController;

            var request = new RequiresContext();
            var response = serviceController.Execute(request, new HttpRequestContext(request))
                as RequiresContextResponse;

            Assert.That(response, Is.Not.Null);
        }

        /// <summary>Generic service should not get registered with generic parameter.</summary>
        [Test]
        public void Generic_Service_should_not_get_registered_with_generic_parameter()
        {
            var serviceManager = new ServiceManager(typeof(GenericService<>).Assembly);
            serviceManager.Init();

            // We should definately *not* be able to call the generic service with a "T" request object :)
            var serviceController = serviceManager.ServiceController;
            var requestType = typeof(GenericService<>).GetGenericArguments()[0];
            var exception = Assert.Throws<System.NotImplementedException>(() => serviceController.GetService(requestType));

            Assert.That(exception.Message, Is.StringContaining("Unable to resolve service"));
        }

        /// <summary>Generic service with recursive ceneric type should not get registered.</summary>
        [Test]
        public void Generic_service_with_recursive_ceneric_type_should_not_get_registered()
        {
            // Tell manager to register GenericService<Generic3<>>, which should not be possible since Generic3<> is an open type
            var serviceManager = new ServiceManager(null, new ServiceController(() => new[] { typeof(GenericService<>).MakeGenericType(new[] { typeof(Generic3<>) }) }));

            serviceManager.Init();

            var serviceController = serviceManager.ServiceController;
            var exception = Assert.Throws<System.NotImplementedException>(() => serviceController.GetService(typeof(Generic3<>)));

            Assert.That(exception.Message, Is.StringContaining("Unable to resolve service"));
        }

        /// <summary>Generic service can be registered with closed types.</summary>
        [Test]
        public void Generic_service_can_be_registered_with_closed_types()
        {
            var serviceManager = new ServiceManager(null, new ServiceController(() => new[]
            {
                typeof(GenericService<Generic1>),
                typeof(GenericService<>).MakeGenericType(new[] { typeof (Generic2) }), // GenericService<Generic2> created through reflection
                typeof(GenericService<Generic3<string>>),
                typeof(GenericService<Generic3<int>>),
                typeof(GenericService<>).MakeGenericType(new[] { typeof (Generic3<>).MakeGenericType(new[] { typeof(double) }) }), // GenericService<Generic3<double>> created through reflection
            }));

            serviceManager.Init();
            var serviceController = serviceManager.ServiceController;

            Assert.AreEqual(typeof(Generic1).FullName, ((Generic1Response)serviceController.Execute(new Generic1())).Data);
            Assert.AreEqual(typeof(Generic2).FullName, ((Generic1Response)serviceController.Execute(new Generic2())).Data);
            Assert.AreEqual(typeof(Generic3<string>).FullName, ((Generic1Response)serviceController.Execute(new Generic3<string>())).Data);
            Assert.AreEqual(typeof(Generic3<int>).FullName, ((Generic1Response)serviceController.Execute(new Generic3<int>())).Data);
            Assert.AreEqual(typeof(Generic3<double>).FullName, ((Generic1Response)serviceController.Execute(new Generic3<double>())).Data);
        }


        /// <summary>A no slash prefix.</summary>
        [Route("route/{Id}")]
        public class NoSlashPrefix : IReturn
        {
            /// <summary>Gets or sets the identifier.</summary>
            ///
            /// <value>The identifier.</value>
            public long Id { get; set; }
        }

        /// <summary>The uses query string.</summary>
        [Route("/route?id={Id}")]
        public class UsesQueryString : IReturn
        {
            /// <summary>Gets or sets the identifier.</summary>
            ///
            /// <value>The identifier.</value>
            public long Id { get; set; }
        }

        /// <summary>my service.</summary>
        public class MyService : IService
        {
            /// <summary>Anies the given request.</summary>
            ///
            /// <param name="request">The request.</param>
            ///
            /// <returns>An object.</returns>
            public object Any(NoSlashPrefix request)
            {
                return null;
            }

            /// <summary>Anies the given request.</summary>
            ///
            /// <param name="request">The request.</param>
            ///
            /// <returns>An object.</returns>
            public object Any(UsesQueryString request)
            {
                return null;
            }
        }

        /// <summary>Does throw on invalid route definitions.</summary>
        [Test]
        public void Does_throw_on_invalid_Route_Definitions()
        {
            EndpointHostConfig.SkipRouteValidation = false;

            var controller = new ServiceController(() => new[] { typeof(MyService) });

            Assert.Throws<ArgumentException>(
                () => controller.RegisterRestPaths(typeof(NoSlashPrefix)));

            Assert.Throws<ArgumentException>(
                () => controller.RegisterRestPaths(typeof(UsesQueryString)));

            EndpointHostConfig.SkipRouteValidation = true;
        }

        /// <summary>Service with generic i get marker interface can be registered without default request attribute.</summary>
        [Test]
        public void Service_with_generic_IGet_marker_interface_can_be_registered_without_DefaultRequestAttribute()
        {
            var appHost = new AppHost();

            var routes = (ServiceRoutes) appHost.Routes;
            Assert.That(routes.RestPaths.Count, Is.EqualTo(0));

            appHost.RegisterService<GetMarkerService>("/route");

            Assert.That(routes.RestPaths.Count, Is.EqualTo(1));
        }
    }

    /// <summary>A get request.</summary>
    public class GetRequest {}

    /// <summary>A get request response.</summary>
    public class GetRequestResponse {}

    /// <summary>A get marker service.</summary>
    [DefaultRequest(typeof(GetRequest))]
    public class GetMarkerService : ServiceInterface.Service
    {
        /// <summary>Gets the given request.</summary>
        ///
        /// <param name="request">The request to get.</param>
        ///
        /// <returns>An object.</returns>
        public object Get(GetRequest request)
        {
            return new GetRequestResponse();
        }
    }

    /// <summary>An application host.</summary>
    public class AppHost : AppHostHttpListenerBase
    {
        /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.Tests.AppHost class.</summary>
        public AppHost() : base("Test", typeof(AppHost).Assembly) {}

        /// <summary>Configures the given container.</summary>
        ///
        /// <param name="container">The container.</param>
        public override void Configure(Container container)
        {
        }
    }
}
