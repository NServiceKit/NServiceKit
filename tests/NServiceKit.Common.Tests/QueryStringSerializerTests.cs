using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;
using Funq;
using NUnit.Framework;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface.Testing;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints;
using NServiceKit.WebHost.Endpoints.Support.Mocks;

namespace NServiceKit.Common.Tests
{
    [TestFixture]
    public class QueryStringSerializerTests
    {
        [Test]
        public void Can_deserialize_TestRequest_QueryStringSerializer_output()
        {
            // Setup
            var testAppHost = new TestAppHost(new Container(), typeof(TestService).Assembly);
            var restPath = new RestPath(typeof(TestRequest), "/service", "GET");
            var restHandler = new RestHandler { RestPath = restPath };

            var requestString = "ListOfA={ListOfB:[{Property:prop1},{Property:prop2}]}";
            NameValueCollection queryString = HttpUtility.ParseQueryString(requestString);
            var httpReq = new HttpRequestMock("service", "GET", "application/json", "service", queryString, new MemoryStream(), new NameValueCollection());

            var request2 = (TestRequest)restHandler.CreateRequest(httpReq, "service");

            Assert.That(request2.ListOfA.Count, Is.EqualTo(1));
            Assert.That(request2.ListOfA.First().ListOfB.Count, Is.EqualTo(2));
        }

        [Test]
        public void QueryStringSerializer_TestRequest_output()
        {
            var testRequest = new TestRequest { ListOfA = new List<A> { new A { ListOfB = new List<B> { new B { Property = "prop1" }, new B { Property = "prop2" } } } } };
            var str = QueryStringSerializer.SerializeToString(testRequest);
            Assert.That(str, Is.EqualTo("ListOfA={ListOfB:[{Property:prop1},{Property:prop2}]}"));
        }

        public class TestService : ServiceInterface.Service
        {
            public object Get(TestRequest request)
            {
                return "OK";
            }
        }

        public class TestRequest
        {
            public List<A> ListOfA { get; set; }
        }

        public class A
        {
            public List<B> ListOfB { get; set; }
        }

        public class B
        {
            public string Property { get; set; }
        }
    }
}