using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using NServiceKit.Common.Tests.ServiceClient.Web;
using NServiceKit.Common.Web;
using NServiceKit.ServiceHost;
using NUnit.Framework;
using NServiceKit.ServiceClient.Web;
using NServiceKit.Service;
using NServiceKit.ServiceInterface;
using NServiceKit.CacheAccess;
using NServiceKit.CacheAccess.Providers;
using NServiceKit.ServiceInterface.ServiceModel;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints.Utils;

namespace NServiceKit.WebHost.Endpoints.Tests
{

    //Always executed
    public class FilterTestAttribute : Attribute, IHasRequestFilter
    {
        private static ICacheClient previousCache;

        /// <summary>Gets or sets the cache.</summary>
        ///
        /// <value>The cache.</value>
        public ICacheClient Cache { get; set; }

        /// <summary>Order in which Request Filters are executed. &lt;0 Executed before global request filters &gt;0 Executed after global request filters.</summary>
        ///
        /// <value>The priority.</value>
        public int Priority { get; set; }

        /// <summary>The request filter is executed before the service.</summary>
        ///
        /// <param name="req">       The http request wrapper.</param>
        /// <param name="res">       The http response wrapper.</param>
        /// <param name="requestDto">The request DTO.</param>
        public void RequestFilter(IHttpRequest req, IHttpResponse res, object requestDto)
        {
            var dto = (AttributeFiltered)requestDto;
            dto.AttrsExecuted.Add(GetType().Name);
            dto.RequestFilterExecuted = true;

            //Check for equality to previous cache to ensure a filter attribute is no singleton
            dto.RequestFilterDependenyIsResolved = Cache != null && !Cache.Equals(previousCache);

            previousCache = Cache;
        }

        /// <summary>A new shallow copy of this filter is used on every request.</summary>
        ///
        /// <returns>An IHasRequestFilter.</returns>
        public IHasRequestFilter Copy()
        {
            return (IHasRequestFilter)this.MemberwiseClone();
        }
    }

    //Only executed for the provided HTTP methods (GET, POST) 
    public class ContextualFilterTestAttribute : RequestFilterAttribute
    {
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.ContextualFilterTestAttribute class.</summary>
        ///
        /// <param name="applyTo">The apply to.</param>
        public ContextualFilterTestAttribute(ApplyTo applyTo)
            : base(applyTo)
        {
        }

        /// <summary>This method is only executed if the HTTP method matches the <see cref="ApplyTo"/> property.</summary>
        ///
        /// <param name="req">       The http request wrapper.</param>
        /// <param name="res">       The http response wrapper.</param>
        /// <param name="requestDto">The request DTO.</param>
        public override void Execute(IHttpRequest req, IHttpResponse res, object requestDto)
        {
            var dto = (AttributeFiltered)requestDto;
            dto.AttrsExecuted.Add(GetType().Name);
            dto.ContextualRequestFilterExecuted = true;
        }
    }

    /// <summary>An attribute filtered.</summary>
    [Route("/attributefiltered")]
    [FilterTest]
    [ContextualFilterTest(ApplyTo.Delete | ApplyTo.Put)]
    public class AttributeFiltered
    {
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.AttributeFiltered class.</summary>
        public AttributeFiltered()
        {
            this.AttrsExecuted = new List<string>();
        }

        /// <summary>Gets or sets a value indicating whether the request filter executed.</summary>
        ///
        /// <value>true if request filter executed, false if not.</value>
        public bool RequestFilterExecuted { get; set; }

        /// <summary>Gets or sets a value indicating whether the contextual request filter executed.</summary>
        ///
        /// <value>true if contextual request filter executed, false if not.</value>
        public bool ContextualRequestFilterExecuted { get; set; }

        /// <summary>Gets or sets a value indicating whether the request filter dependeny is resolved.</summary>
        ///
        /// <value>true if request filter dependeny is resolved, false if not.</value>
        public bool RequestFilterDependenyIsResolved { get; set; }

        /// <summary>Gets or sets the attributes executed.</summary>
        ///
        /// <value>The attributes executed.</value>
        public List<string> AttrsExecuted { get; set; }
    }

    //Always executed
    public class ResponseFilterTestAttribute : Attribute, IHasResponseFilter
    {
        private static ICacheClient previousCache;

        /// <summary>Gets or sets the cache.</summary>
        ///
        /// <value>The cache.</value>
        public ICacheClient Cache { get; set; }

        /// <summary>Order in which Response Filters are executed. &lt;0 Executed before global response filters &gt;0 Executed after global response filters.</summary>
        ///
        /// <value>The priority.</value>
        public int Priority { get; set; }

        /// <summary>The response filter is executed after the service.</summary>
        ///
        /// <param name="req">     The http request wrapper.</param>
        /// <param name="res">     The http response wrapper.</param>
        /// <param name="response">.</param>
        public void ResponseFilter(IHttpRequest req, IHttpResponse res, object response)
        {
            var dto = response.ToResponseDto() as AttributeFilteredResponse;
            dto.ResponseFilterExecuted = true;
            dto.ResponseFilterDependencyIsResolved = Cache != null && !Cache.Equals(previousCache);

            previousCache = Cache;
        }

        /// <summary>A new shallow copy of this filter is used on every request.</summary>
        ///
        /// <returns>An IHasResponseFilter.</returns>
        public IHasResponseFilter Copy()
        {
            return (IHasResponseFilter)this.MemberwiseClone();
        }
    }

    //Only executed for the provided HTTP methods (GET, POST) 
    public class ContextualResponseFilterTestAttribute : ResponseFilterAttribute
    {
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.ContextualResponseFilterTestAttribute class.</summary>
        ///
        /// <param name="applyTo">The apply to.</param>
        public ContextualResponseFilterTestAttribute(ApplyTo applyTo)
            : base(applyTo)
        {
        }

        /// <summary>This method is only executed if the HTTP method matches the <see cref="ApplyTo"/> property.</summary>
        ///
        /// <param name="req">        The http request wrapper.</param>
        /// <param name="res">        The http response wrapper.</param>
        /// <param name="responseDto">The response DTO.</param>
        public override void Execute(IHttpRequest req, IHttpResponse res, object responseDto)
        {
            var dto = responseDto as AttributeFilteredResponse;
            dto.ContextualResponseFilterExecuted = true;
        }
    }

    /// <summary>Attribute for throwing filter.</summary>
    public class ThrowingFilterAttribute : RequestFilterAttribute
    {
        /// <summary>This method is only executed if the HTTP method matches the <see cref="ApplyTo"/> property.</summary>
        ///
        /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or illegal values.</exception>
        ///
        /// <param name="req">       The http request wrapper.</param>
        /// <param name="res">       The http response wrapper.</param>
        /// <param name="requestDto">The request DTO.</param>
        public override void Execute(IHttpRequest req, IHttpResponse res, object requestDto)
        {
            throw new ArgumentException("exception message");
        }
    }

    /// <summary>A throwing attribute filtered.</summary>
    [Route("/throwingattributefiltered")]
    public class ThrowingAttributeFiltered : IReturn<string>
    {
    }

    /// <summary>A throwing attribute filtered service.</summary>
    [ThrowingFilter]
    public class ThrowingAttributeFilteredService : IService
    {
        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Any(ThrowingAttributeFiltered request)
        {
            return "OK";
        }
    }

    /// <summary>An attribute filtered response.</summary>
    [ResponseFilterTest]
    [ContextualResponseFilterTest(ApplyTo.Delete | ApplyTo.Put)]
    public class AttributeFilteredResponse
    {
        /// <summary>Gets or sets a value indicating whether the response filter executed.</summary>
        ///
        /// <value>true if response filter executed, false if not.</value>
        public bool ResponseFilterExecuted { get; set; }

        /// <summary>Gets or sets a value indicating whether the contextual response filter executed.</summary>
        ///
        /// <value>true if contextual response filter executed, false if not.</value>
        public bool ContextualResponseFilterExecuted { get; set; }

        /// <summary>Gets or sets a value indicating whether the request filter executed.</summary>
        ///
        /// <value>true if request filter executed, false if not.</value>
        public bool RequestFilterExecuted { get; set; }

        /// <summary>Gets or sets a value indicating whether the contextual request filter executed.</summary>
        ///
        /// <value>true if contextual request filter executed, false if not.</value>
        public bool ContextualRequestFilterExecuted { get; set; }

        /// <summary>Gets or sets a value indicating whether the request filter dependeny is resolved.</summary>
        ///
        /// <value>true if request filter dependeny is resolved, false if not.</value>
        public bool RequestFilterDependenyIsResolved { get; set; }

        /// <summary>Gets or sets a value indicating whether the response filter dependency is resolved.</summary>
        ///
        /// <value>true if response filter dependency is resolved, false if not.</value>
        public bool ResponseFilterDependencyIsResolved { get; set; }
    }

    /// <summary>An attribute filtered service.</summary>
    public class AttributeFilteredService : ServiceInterface.Service
    {
        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Any(AttributeFiltered request)
        {
            return new AttributeFilteredResponse {
                ResponseFilterExecuted = false,
                ContextualResponseFilterExecuted = false,
                RequestFilterExecuted = request.RequestFilterExecuted,
                ContextualRequestFilterExecuted = request.ContextualRequestFilterExecuted,
                RequestFilterDependenyIsResolved = request.RequestFilterDependenyIsResolved,
                ResponseFilterDependencyIsResolved = false
            };
        }
    }

    /// <summary>An attribute filters test.</summary>
    [TestFixture]
    public class AttributeFiltersTest
    {
        private const string ListeningOn = "http://localhost:82/";
        private const string ServiceClientBaseUri = "http://localhost:82/";

        /// <summary>An attribute filters application host HTTP listener.</summary>
        public class AttributeFiltersAppHostHttpListener
            : AppHostHttpListenerBase
        {

            /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.AttributeFiltersTest.AttributeFiltersAppHostHttpListener class.</summary>
            public AttributeFiltersAppHostHttpListener()
                : base("Attribute Filters Tests", typeof(AttributeFilteredService).Assembly) { }

            /// <summary>Configures the given container.</summary>
            ///
            /// <param name="container">The container.</param>
            public override void Configure(Funq.Container container)
            {
                container.Register<ICacheClient>(c => new MemoryCacheClient()).ReusedWithin(Funq.ReuseScope.None);
                SetConfig(new EndpointHostConfig { DebugMode = true }); //show stacktraces
            }
        }

        AttributeFiltersAppHostHttpListener appHost;

        /// <summary>Executes the test fixture set up action.</summary>
        [TestFixtureSetUp]
        public void OnTestFixtureSetUp()
        {
            appHost = new AttributeFiltersAppHostHttpListener();
            appHost.Init();
            appHost.Start(ListeningOn);
        }

        /// <summary>Executes the test fixture tear down action.</summary>
        [TestFixtureTearDown]
        public void OnTestFixtureTearDown()
        {
            appHost.Dispose();
        }

        static IServiceClient[] ServiceClients = 
        {
            new JsonServiceClient(ServiceClientBaseUri),
            new XmlServiceClient(ServiceClientBaseUri),
            new JsvServiceClient(ServiceClientBaseUri)
			//SOAP not supported in HttpListener
			//new Soap11ServiceClient(ServiceClientBaseUri),
			//new Soap12ServiceClient(ServiceClientBaseUri)
        };

        /// <summary>Request and response filters are executed using service client.</summary>
        ///
        /// <param name="client">The client.</param>
        [Test, TestCaseSource("ServiceClients")]
        public void Request_and_Response_Filters_are_executed_using_ServiceClient(IServiceClient client)
        {
            var response = client.Send<AttributeFilteredResponse>(
                new AttributeFiltered { RequestFilterExecuted = false });
            Assert.IsTrue(response.RequestFilterExecuted);
            Assert.IsTrue(response.ResponseFilterExecuted);
            Assert.IsFalse(response.ContextualRequestFilterExecuted);
            Assert.IsFalse(response.ContextualResponseFilterExecuted);
            Assert.IsTrue(response.RequestFilterDependenyIsResolved);
            Assert.IsTrue(response.ResponseFilterDependencyIsResolved);
        }

        static IRestClient[] RestClients = 
        {
            new JsonServiceClient(ServiceClientBaseUri),
            new XmlServiceClient(ServiceClientBaseUri),
            new JsvServiceClient(ServiceClientBaseUri)
        };

        /// <summary>Proper exception is serialized to client.</summary>
        [Test]
        public void Proper_exception_is_serialized_to_client()
        {
            var client = new HtmlServiceClient(ServiceClientBaseUri);
            client.SetBaseUri(ServiceClientBaseUri);

            try
            {
                client.Get(new ThrowingAttributeFiltered());
            }
            catch (WebServiceException e)
            {
                //Ensure we have stack trace present
                Assert.IsTrue(e.ResponseBody.Contains("ThrowingFilterAttribute"), "No stack trace in the response (it's probably empty)");
            }
        }

        /// <summary>Request and response filters are executed using rest client.</summary>
        ///
        /// <param name="client">The client.</param>
        [Test, TestCaseSource("RestClients")]
        public void Request_and_Response_Filters_are_executed_using_RestClient(IRestClient client)
        {
            var response = client.Post<AttributeFilteredResponse>("attributefiltered", new AttributeFiltered() { RequestFilterExecuted = false });
            Assert.IsTrue(response.RequestFilterExecuted);
            Assert.IsTrue(response.ResponseFilterExecuted);
            Assert.IsFalse(response.ContextualRequestFilterExecuted);
            Assert.IsFalse(response.ContextualResponseFilterExecuted);
            Assert.IsTrue(response.RequestFilterDependenyIsResolved);
            Assert.IsTrue(response.ResponseFilterDependencyIsResolved);
        }

        /// <summary>Contextual request and response filters are executed using rest client.</summary>
        ///
        /// <param name="client">The client.</param>
        [Test, TestCaseSource("RestClients")]
        public void Contextual_Request_and_Response_Filters_are_executed_using_RestClient(IRestClient client)
        {
            var response = client.Delete<AttributeFilteredResponse>("attributefiltered");
            Assert.IsTrue(response.RequestFilterExecuted);
            Assert.IsTrue(response.ResponseFilterExecuted);
            Assert.IsTrue(response.ContextualRequestFilterExecuted);
            Assert.IsTrue(response.ContextualResponseFilterExecuted);
            Assert.IsTrue(response.RequestFilterDependenyIsResolved);
            Assert.IsTrue(response.ResponseFilterDependencyIsResolved);
        }

        /// <summary>Multi contextual request and response filters are executed using rest client.</summary>
        ///
        /// <param name="client">The client.</param>
        [Test, TestCaseSource("RestClients")]
        public void Multi_Contextual_Request_and_Response_Filters_are_executed_using_RestClient(IRestClient client)
        {
            var response = client.Put<AttributeFilteredResponse>("attributefiltered", new AttributeFiltered() { RequestFilterExecuted = false });
            Assert.IsTrue(response.RequestFilterExecuted);
            Assert.IsTrue(response.ResponseFilterExecuted);
            Assert.IsTrue(response.ContextualRequestFilterExecuted);
            Assert.IsTrue(response.ContextualResponseFilterExecuted);
            Assert.IsTrue(response.RequestFilterDependenyIsResolved);
            Assert.IsTrue(response.ResponseFilterDependencyIsResolved);
        }

        /// <summary>Attribute for executed first.</summary>
        public class ExecutedFirstAttribute : RequestFilterAttribute
        {
            /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.AttributeFiltersTest.ExecutedFirstAttribute class.</summary>
            public ExecutedFirstAttribute()
            {
                Priority = int.MinValue;
            }

            /// <summary>This method is only executed if the HTTP method matches the <see cref="ApplyTo"/> property.</summary>
            ///
            /// <param name="req">       The http request wrapper.</param>
            /// <param name="res">       The http response wrapper.</param>
            /// <param name="requestDto">The request DTO.</param>
            public override void Execute(IHttpRequest req, IHttpResponse res, object requestDto)
            {
                var dto = (AttributeFiltered)requestDto;
                dto.AttrsExecuted.Add(GetType().Name);
            }
        }

        /// <summary>A dummy holder.</summary>
        [ExecutedFirst]
        [FilterTest]
        [RequiredRole("test")]
        [Authenticate]
        [RequiredPermission("test")]
        public class DummyHolder { }

        /// <summary>Request filters are prioritized.</summary>
        [Test]
        public void RequestFilters_are_prioritized()
        {
            EndpointHost.ServiceManager = new ServiceManager(typeof(DummyHolder).Assembly);

            EndpointHost.ServiceManager.Metadata.Add(typeof(AttributeFilteredService), typeof(DummyHolder), null);

            var attributes = FilterAttributeCache.GetRequestFilterAttributes(typeof(DummyHolder));
            var attrPriorities = attributes.ToList().ConvertAll(x => x.Priority);
            Assert.That(attrPriorities, Is.EquivalentTo(new[] { int.MinValue, -100, -90, -80, 0 }));

            var execOrder = new IHasRequestFilter[attributes.Length];
            var i = 0;
            for (; i < attributes.Length && attributes[i].Priority < 0; i++)
            {
                execOrder[i] = attributes[i];
                Console.WriteLine(attributes[i].Priority);
            }

            Console.WriteLine("break;");

            for (; i < attributes.Length; i++)
            {
                execOrder[i] = attributes[i];
                Console.WriteLine(attributes[i].Priority);
            }

            var execOrderPriorities = execOrder.ToList().ConvertAll(x => x.Priority);
            Console.WriteLine(execOrderPriorities.Dump());
            Assert.That(execOrderPriorities, Is.EquivalentTo(new[] { int.MinValue, -100, -90, -80, 0 }));
        }
    }
}
