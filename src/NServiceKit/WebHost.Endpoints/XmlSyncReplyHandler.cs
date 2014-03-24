using NServiceKit.Common.Web;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.Endpoints
{
	public class XmlAsyncOneWayHandler : GenericHandler
	{
		public XmlAsyncOneWayHandler()
			: base(ContentType.Xml, EndpointAttributes.OneWay | EndpointAttributes.Xml, Feature.Xml)
		{
		}
	}

	public class XmlSyncReplyHandler : GenericHandler
	{
		public XmlSyncReplyHandler()
			: base(ContentType.Xml, EndpointAttributes.Reply | EndpointAttributes.Xml, Feature.Xml)
		{
		}
	}
}