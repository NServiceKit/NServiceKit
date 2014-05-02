using System.Web;
using NServiceKit.ServiceHost;
using NServiceKit.WebHost.Endpoints;

namespace NServiceKit
{
    /// <summary>A predefined routes feature.</summary>
    public class PredefinedRoutesFeature : IPlugin
    {
        /// <summary>Registers this object.</summary>
        ///
        /// <param name="appHost">The application host.</param>
        public void Register(IAppHost appHost)
        {
            appHost.CatchAllHandlers.Add(ProcessRequest);
        }

        /// <summary>Process the request.</summary>
        ///
        /// <param name="httpMethod">The HTTP method.</param>
        /// <param name="pathInfo">  Information describing the path.</param>
        /// <param name="filePath">  Full pathname of the file.</param>
        ///
        /// <returns>An IHttpHandler.</returns>
        public IHttpHandler ProcessRequest(string httpMethod, string pathInfo, string filePath)
        {
            var pathParts = pathInfo.TrimStart('/').Split('/');
            if (pathParts.Length == 0) return null;
            return GetHandlerForPathParts(pathParts);
        }

        private static IHttpHandler GetHandlerForPathParts(string[] pathParts)
        {
            var pathController = string.Intern(pathParts[0].ToLower());
            if (pathParts.Length == 1)
            {
                if (pathController == "soap11")
                    return new Soap11MessageSyncReplyHttpHandler();
                if (pathController == "soap12")
                    return new Soap12MessageSyncReplyHttpHandler();

                return null;
            }

            var pathAction = string.Intern(pathParts[1].ToLower());
            var requestName = pathParts.Length > 2 ? pathParts[2] : null;
            var isReply = pathAction == "syncreply" || pathAction == "reply";
            var isOneWay = pathAction == "asynconeway" || pathAction == "oneway";
            switch (pathController)
            {
                case "json":
                    if (isReply)
                        return new JsonSyncReplyHandler { RequestName = requestName };
                    if (isOneWay)
                        return new JsonAsyncOneWayHandler { RequestName = requestName };
                    break;

                case "xml":
                    if (isReply)
                        return new XmlSyncReplyHandler { RequestName = requestName };
                    if (isOneWay)
                        return new XmlAsyncOneWayHandler { RequestName = requestName };
                    break;

                case "jsv":
                    if (isReply)
                        return new JsvSyncReplyHandler { RequestName = requestName };
                    if (isOneWay)
                        return new JsvAsyncOneWayHandler { RequestName = requestName };
                    break;

                default:
                    string contentType;
                    if (EndpointHost.ContentTypeFilter.ContentTypeFormats.TryGetValue(pathController, out contentType))
                    {
                        var feature = Common.Web.ContentType.ToFeature(contentType);
                        if (feature == Feature.None) feature = Feature.CustomFormat;

                        if (isReply)
                            return new GenericHandler(contentType, EndpointAttributes.Reply, feature) {
                                RequestName = requestName
                            };
                        if (isOneWay)
                            return new GenericHandler(contentType, EndpointAttributes.OneWay, feature) {
                                RequestName = requestName
                            };
                    }
                    break;
            }

            return null;
        }
    }
}