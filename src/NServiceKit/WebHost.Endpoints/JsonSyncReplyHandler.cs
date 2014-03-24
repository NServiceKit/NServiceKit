using NServiceKit.Common.Web;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.Endpoints
{
    public class JsonAsyncOneWayHandler : GenericHandler
    {
        public JsonAsyncOneWayHandler()
            : base(ContentType.Json, EndpointAttributes.OneWay | EndpointAttributes.Json, Feature.Json)
        {
        }
    }

    public class JsonSyncReplyHandler : GenericHandler
    {
        public JsonSyncReplyHandler()
            : base(ContentType.Json, EndpointAttributes.Reply | EndpointAttributes.Json, Feature.Json)
        {
        }
    }
}