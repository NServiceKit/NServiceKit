using Funq;
using NUnit.Framework;
using NServiceKit.Common.Web;
using NServiceKit.ServiceInterface.Testing;
using NServiceKit.WebHost.IntegrationTests.Services;

namespace NServiceKit.WebHost.IntegrationTests.Tests
{
    /// <summary>A rest path resolution unit tests.</summary>
	[TestFixture]
	public class RestPathResolutionUnitTests
		: TestBase
	{
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.IntegrationTests.Tests.RestPathResolutionUnitTests class.</summary>
		public RestPathResolutionUnitTests()
			: base(Config.NServiceKitBaseUri, typeof(ReverseService).Assembly)
		{
		}

        /// <summary>Configures the given container.</summary>
        ///
        /// <param name="container">The container.</param>
		protected override void Configure(Funq.Container container)
		{
			
		}

        /// <summary>Executes the before test action.</summary>
		[SetUp]
		public void OnBeforeTest()
		{
			base.OnBeforeEachTest();
		}

        /// <summary>Can execute echo request rest path.</summary>
		[Test]
		public void Can_execute_EchoRequest_rest_path()
		{
			var request = (EchoRequest)GetRequest("/echo/1/One");
			Assert.That(request, Is.Not.Null);
			Assert.That(request.Id, Is.EqualTo(1));
			Assert.That(request.String, Is.EqualTo("One"));
		}

        /// <summary>Can call echo request with query string.</summary>
		[Test]
		public void Can_call_EchoRequest_with_QueryString()
		{
			var request = (EchoRequest)GetRequest("/echo/1/One?Long=2&Bool=True");

			Assert.That(request.Id, Is.EqualTo(1));
			Assert.That(request.String, Is.EqualTo("One"));
			Assert.That(request.Long, Is.EqualTo(2));
			Assert.That(request.Bool, Is.EqualTo(true));
		}

        /// <summary>Can get empty basic wildcard.</summary>
        [Test]
        public void Can_get_empty_BasicWildcard()
        {
            var request = GetRequest<BasicWildcard>("/path");
            Assert.That(request.Tail, Is.Null);
            request = GetRequest<BasicWildcard>("/path/");
            Assert.That(request.Tail, Is.Null);
            request = GetRequest<BasicWildcard>("/path/a");
            Assert.That(request.Tail, Is.EqualTo("a"));
            request = GetRequest<BasicWildcard>("/path/a/b/c");
            Assert.That(request.Tail, Is.EqualTo("a/b/c"));
        }

        /// <summary>Can call wild card request with alternate matching wild card defined.</summary>
        [Test]
        public void Can_call_WildCardRequest_with_alternate_matching_WildCard_defined()
        {
            var request = (WildCardRequest)GetRequest("/wildcard/1/aPath/edit");
            Assert.That(request.Id, Is.EqualTo(1));
            Assert.That(request.Path, Is.EqualTo("aPath"));
            Assert.That(request.Action, Is.EqualTo("edit"));
            Assert.That(request.RemainingPath, Is.Null);
        }

        /// <summary>Can call wild card request wild card mapping.</summary>
		[Test]
		public void Can_call_WildCardRequest_WildCard_mapping()
		{
			var request = (WildCardRequest)GetRequest("/wildcard/1/remaining/path/to/here");
			Assert.That(request.Id, Is.EqualTo(1));
			Assert.That(request.Path, Is.Null);
			Assert.That(request.Action, Is.Null);
			Assert.That(request.RemainingPath, Is.EqualTo("remaining/path/to/here"));
		}

        /// <summary>Can call wild card request wild card mapping with query string.</summary>
		[Test]
		public void Can_call_WildCardRequest_WildCard_mapping_with_QueryString()
		{
			var request = (WildCardRequest)GetRequest("/wildcard/1/remaining/path/to/here?Action=edit");
			Assert.That(request.Id, Is.EqualTo(1));
			Assert.That(request.Path, Is.Null);
			Assert.That(request.Action, Is.EqualTo("edit"));
			Assert.That(request.RemainingPath, Is.EqualTo("remaining/path/to/here"));
		}

        /// <summary>Can call get on verb match services.</summary>
		[Test]
		public void Can_call_GET_on_VerbMatch_Services()
		{
			var request = (VerbMatch1)GetRequest(HttpMethods.Get, "/verbmatch");
			Assert.That(request.Name, Is.Null);

			var request2 = (VerbMatch1)GetRequest(HttpMethods.Get, "/verbmatch/arg");
			Assert.That(request2.Name, Is.EqualTo("arg"));
		}

        /// <summary>Can call post on verb match services.</summary>
		[Test]
		public void Can_call_POST_on_VerbMatch_Services()
		{
			var request = (VerbMatch2)GetRequest(HttpMethods.Post, "/verbmatch");
			Assert.That(request.Name, Is.Null);

			var request2 = (VerbMatch2)GetRequest(HttpMethods.Post, "/verbmatch/arg");
			Assert.That(request2.Name, Is.EqualTo("arg"));
		}

        /// <summary>Can call delete on verb match services.</summary>
		[Test]
		public void Can_call_DELETE_on_VerbMatch_Services()
		{
			var request = (VerbMatch1)GetRequest(HttpMethods.Delete, "/verbmatch");
			Assert.That(request.Name, Is.Null);

			var request2 = (VerbMatch1)GetRequest(HttpMethods.Delete, "/verbmatch/arg");
			Assert.That(request2.Name, Is.EqualTo("arg"));
		}

        /// <summary>Can call put on verb match services.</summary>
		[Test]
		public void Can_call_PUT_on_VerbMatch_Services()
		{
			var request = (VerbMatch2)GetRequest(HttpMethods.Put, "/verbmatch");
			Assert.That(request.Name, Is.Null);

			var request2 = (VerbMatch2)GetRequest(HttpMethods.Put, "/verbmatch/arg");
			Assert.That(request2.Name, Is.EqualTo("arg"));
		}

        /// <summary>Can call patch on verb match services.</summary>
		[Test]
		public void Can_call_PATCH_on_VerbMatch_Services()
		{
			var request = (VerbMatch2)GetRequest(HttpMethods.Patch, "/verbmatch");
			Assert.That(request.Name, Is.Null);

			var request2 = (VerbMatch2)GetRequest(HttpMethods.Patch, "/verbmatch/arg");
			Assert.That(request2.Name, Is.EqualTo("arg"));
		}

	}
}