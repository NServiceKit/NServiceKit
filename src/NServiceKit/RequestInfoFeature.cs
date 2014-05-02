using System.Web;
using NServiceKit.WebHost.Endpoints;
using NServiceKit.WebHost.Endpoints.Support;

namespace NServiceKit
{
    /// <summary>A request information feature.</summary>
    public class RequestInfoFeature : IPlugin
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
            return pathParts.Length == 0 ? null : GetHandlerForPathParts(pathParts);
        }

        private static IHttpHandler GetHandlerForPathParts(string[] pathParts)
        {
            var pathController = string.Intern(pathParts[0].ToLower());
            return pathController == RequestInfoHandler.RestPath
                ? new RequestInfoHandler()
                : null;
        }
    }
}