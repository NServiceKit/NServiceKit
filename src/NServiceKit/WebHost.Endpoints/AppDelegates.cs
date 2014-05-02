using System;
using System.Web;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.Endpoints
{
    /// <summary>HTTP handler resolver delegate.</summary>
    ///
    /// <param name="httpMethod">The HTTP method.</param>
    /// <param name="pathInfo">  Information describing the path.</param>
    /// <param name="filePath">  Full pathname of the file.</param>
    ///
    /// <returns>An IHttpHandler.</returns>
	public delegate IHttpHandler HttpHandlerResolverDelegate(string httpMethod, string pathInfo, string filePath);

    /// <summary>Stream serializer resolver delegate.</summary>
    ///
    /// <param name="requestContext">Context for the request.</param>
    /// <param name="dto">           The dto.</param>
    /// <param name="httpRes">       The HTTP resource.</param>
    ///
    /// <returns>A bool.</returns>
	public delegate bool StreamSerializerResolverDelegate(IRequestContext requestContext, object dto, IHttpResponse httpRes);

    /// <summary>Handles the uncaught exception delegate.</summary>
    ///
    /// <param name="httpReq">      The HTTP request.</param>
    /// <param name="httpRes">      The HTTP resource.</param>
    /// <param name="operationName">Name of the operation.</param>
    /// <param name="ex">           The ex.</param>
    public delegate void HandleUncaughtExceptionDelegate(
        IHttpRequest httpReq, IHttpResponse httpRes, string operationName, Exception ex);

    /// <summary>Handles the service exception delegate.</summary>
    ///
    /// <param name="httpReq">The HTTP request.</param>
    /// <param name="request">The request.</param>
    /// <param name="ex">     The ex.</param>
    ///
    /// <returns>An object.</returns>
    public delegate object HandleServiceExceptionDelegate(IHttpRequest httpReq, object request, Exception ex);

    /// <summary>Fallback rest path delegate.</summary>
    ///
    /// <param name="httpMethod">The HTTP method.</param>
    /// <param name="pathInfo">  Information describing the path.</param>
    /// <param name="filePath">  Full pathname of the file.</param>
    ///
    /// <returns>A RestPath.</returns>
    public delegate RestPath FallbackRestPathDelegate(string httpMethod, string pathInfo, string filePath);
}