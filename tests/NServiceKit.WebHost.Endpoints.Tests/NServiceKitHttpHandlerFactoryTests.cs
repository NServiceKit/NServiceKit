using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using NUnit.Framework;
using NServiceKit.ServiceHost;
using NServiceKit.WebHost.Endpoints.Metadata;
using NServiceKit.WebHost.Endpoints.Support;

namespace NServiceKit.WebHost.Endpoints.Tests
{
    /// <summary>A service kit HTTP handler factory tests.</summary>
    [TestFixture]
    public class NServiceKitHttpHandlerFactoryTests
    {
        readonly Dictionary<string, Type> pathInfoMap = new Dictionary<string, Type>
		{
			{"Metadata", typeof(IndexMetadataHandler)},
			{"Soap11", typeof(Soap11MessageSyncReplyHttpHandler)},
			{"Soap12", typeof(Soap12MessageSyncReplyHttpHandler)},

			{"Json/SyncReply", typeof(JsonSyncReplyHandler)},
			{"Json/AsyncOneWay", typeof(JsonAsyncOneWayHandler)},
			{"Json/Metadata", typeof(JsonMetadataHandler)},

			{"Xml/SyncReply", typeof(XmlSyncReplyHandler)},
			{"Xml/AsyncOneWay", typeof(XmlAsyncOneWayHandler)},
			{"Xml/Metadata", typeof(XmlMetadataHandler)},

			{"Jsv/SyncReply", typeof(JsvSyncReplyHandler)},
			{"Jsv/AsyncOneWay", typeof(JsvAsyncOneWayHandler)},
			{"Jsv/Metadata", typeof(JsvMetadataHandler)},

			{"Soap11/Wsdl", typeof(Soap11WsdlMetadataHandler)},
			{"Soap11/Metadata", typeof(Soap11MetadataHandler)},

			{"Soap12/Wsdl", typeof(Soap12WsdlMetadataHandler)},
			{"Soap12/Metadata", typeof(Soap12MetadataHandler)},
		};

        /// <summary>Setups this object.</summary>
        [TestFixtureSetUp]
        public void Setup()
        {
            RegisterConfig();
        }

        /// <summary>Tear down.</summary>
        [TestFixtureTearDown]
        public void TearDown()
        {
            EndpointHost.CatchAllHandlers.Clear();
        }

        /// <summary>Resolves the right handler for expexted paths.</summary>
        [Test]
        public void Resolves_the_right_handler_for_expexted_paths()
        {
            foreach (var item in pathInfoMap)
            {
                var expectedType = item.Value;
                var handler = NServiceKitHttpHandlerFactory.GetHandlerForPathInfo(null, item.Key, null, null);
                Assert.That(handler.GetType(), Is.EqualTo(expectedType));
            }
        }

        private void RegisterConfig()
        {
            EndpointHost.Config = new EndpointHostConfig("ServiceName", new ServiceManager(GetType().Assembly)) {
                RawHttpHandlers = new List<Func<IHttpRequest,IHttpHandler>>(),
				RouteNamingConventions = new List<RouteNamingConventionDelegate>(),
                CustomHttpHandlers = new Dictionary<HttpStatusCode, INServiceKitHttpHandler>(),
                WebHostPhysicalPath = "~/".MapServerPath()
            };
            EndpointHost.CatchAllHandlers.Add(new PredefinedRoutesFeature().ProcessRequest);
            EndpointHost.CatchAllHandlers.Add(new MetadataFeature().ProcessRequest);
        }

        /// <summary>Resolves the right handler for case insensitive expexted paths.</summary>
        [Test]
        public void Resolves_the_right_handler_for_case_insensitive_expexted_paths()
        {
            foreach (var item in pathInfoMap)
            {
                var expectedType = item.Value;
                var lowerPathInfo = item.Key.ToLower();
                var handler = NServiceKitHttpHandlerFactory.GetHandlerForPathInfo(null, lowerPathInfo, null, null);
                Assert.That(handler.GetType(), Is.EqualTo(expectedType));
            }
        }


    }
}