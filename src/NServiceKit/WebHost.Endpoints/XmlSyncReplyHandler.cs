using NServiceKit.Common.Web;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.Endpoints
{
    /// <summary>
    /// 
    /// </summary>
	public class XmlAsyncOneWayHandler : GenericHandler
	{
        /// <summary>
        /// 
        /// </summary>
		public XmlAsyncOneWayHandler()
			: base(ContentType.Xml, EndpointAttributes.OneWay | EndpointAttributes.Xml, Feature.Xml)
		{
		}
	}

    /// <summary>
    /// 
    /// </summary>
	public class XmlSyncReplyHandler : GenericHandler
	{
        /// <summary>
        /// 
        /// </summary>
		public XmlSyncReplyHandler()
			: base(ContentType.Xml, EndpointAttributes.Reply | EndpointAttributes.Xml, Feature.Xml)
		{
		}
	}
}