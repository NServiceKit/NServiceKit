using NServiceKit.Common.Web;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.Endpoints
{
    /// <summary>A JSON asynchronous one way handler.</summary>
    public class JsonAsyncOneWayHandler : GenericHandler
    {
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.JsonAsyncOneWayHandler class.</summary>
        public JsonAsyncOneWayHandler()
            : base(ContentType.Json, EndpointAttributes.OneWay | EndpointAttributes.Json, Feature.Json)
        {
        }
    }

    /// <summary>A JSON synchronise reply handler.</summary>
    public class JsonSyncReplyHandler : GenericHandler
    {
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.JsonSyncReplyHandler class.</summary>
        public JsonSyncReplyHandler()
            : base(ContentType.Json, EndpointAttributes.Reply | EndpointAttributes.Json, Feature.Json)
        {
        }
    }
}