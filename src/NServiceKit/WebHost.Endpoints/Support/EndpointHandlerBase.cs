using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using System.Web;
using NServiceKit.Common.Extensions;
using NServiceKit.Common.Web;
using NServiceKit.Logging;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceModel.Serialization;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints.Extensions;
using HttpRequestExtensions = NServiceKit.WebHost.Endpoints.Extensions.HttpRequestExtensions;
using HttpRequestWrapper = NServiceKit.WebHost.Endpoints.Extensions.HttpRequestWrapper;
using HttpResponseWrapper = NServiceKit.WebHost.Endpoints.Extensions.HttpResponseWrapper;

namespace NServiceKit.WebHost.Endpoints.Support
{
    /// <summary>An endpoint handler base.</summary>
    public abstract class EndpointHandlerBase
        : INServiceKitHttpHandler, IHttpHandler
    {
        internal static readonly ILog Log = LogManager.GetLogger(typeof(EndpointHandlerBase));
        internal static readonly Dictionary<byte[], byte[]> NetworkInterfaceIpv4Addresses = new Dictionary<byte[], byte[]>();
        internal static readonly byte[][] NetworkInterfaceIpv6Addresses = new byte[0][];

        /// <summary>Gets or sets the name of the request.</summary>
        ///
        /// <value>The name of the request.</value>
        public string RequestName { get; set; }

        static EndpointHandlerBase()
        {
            try
            {
                IPAddressExtensions.GetAllNetworkInterfaceIpv4Addresses().ForEach((x, y) => NetworkInterfaceIpv4Addresses[x.GetAddressBytes()] = y.GetAddressBytes());

                NetworkInterfaceIpv6Addresses = IPAddressExtensions.GetAllNetworkInterfaceIpv6Addresses().ConvertAll(x => x.GetAddressBytes()).ToArray();
            }
            catch (Exception ex)
            {
                Log.Warn("Failed to retrieve IP Addresses, some security restriction features may not work: " + ex.Message, ex);
            }
        }

        /// <summary>Gets or sets the handler attributes.</summary>
        ///
        /// <value>The handler attributes.</value>
        public EndpointAttributes HandlerAttributes { get; set; }

        /// <summary>Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler" /> instance.</summary>
        ///
        /// <value>true if the <see cref="T:System.Web.IHttpHandler" /> instance is reusable; otherwise, false.</value>
        public bool IsReusable
        {
            get { return false; }
        }

        /// <summary>Creates a request.</summary>
        ///
        /// <param name="request">      The request.</param>
        /// <param name="operationName">Name of the operation.</param>
        ///
        /// <returns>The new request.</returns>
        public abstract object CreateRequest(IHttpRequest request, string operationName);

        /// <summary>Gets a response.</summary>
        ///
        /// <param name="httpReq">The HTTP request.</param>
        /// <param name="httpRes">The HTTP resource.</param>
        /// <param name="request">The request.</param>
        ///
        /// <returns>The response.</returns>
        public abstract object GetResponse(IHttpRequest httpReq, IHttpResponse httpRes, object request);

        /// <summary>Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler" /> interface.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <param name="httpReq">      The HTTP request.</param>
        /// <param name="httpRes">      The HTTP resource.</param>
        /// <param name="operationName">Name of the operation.</param>
        public virtual void ProcessRequest(IHttpRequest httpReq, IHttpResponse httpRes, string operationName)
        {
            throw new NotImplementedException();
        }

        /// <summary>Deserialize HTTP request.</summary>
        ///
        /// <exception cref="SerializationException">Thrown when a Serialization error condition occurs.</exception>
        ///
        /// <param name="operationType">Type of the operation.</param>
        /// <param name="httpReq">      The HTTP request.</param>
        /// <param name="contentType">  Type of the content.</param>
        ///
        /// <returns>An object.</returns>
        public static object DeserializeHttpRequest(Type operationType, IHttpRequest httpReq, string contentType)
        {
            var httpMethod = httpReq.HttpMethod;
            var queryString = httpReq.QueryString;

            if (httpMethod == HttpMethods.Get || httpMethod == HttpMethods.Delete || httpMethod == HttpMethods.Options)
            {
                try
                {
                    return KeyValueDataContractDeserializer.Instance.Parse(queryString, operationType);
                }
                catch (Exception ex)
                {
                    var msg = "Could not deserialize '{0}' request using KeyValueDataContractDeserializer: '{1}'.\nError: '{2}'"
                        .Fmt(operationType, queryString, ex);
                    throw new SerializationException(msg);
                }
            }

            var isFormData = httpReq.HasAnyOfContentTypes(ContentType.FormUrlEncoded, ContentType.MultiPartFormData);
            if (isFormData)
            {
                try
                {
                    return KeyValueDataContractDeserializer.Instance.Parse(httpReq.FormData, operationType);
                }
                catch (Exception ex)
                {
                    throw new SerializationException("Error deserializing FormData: " + httpReq.FormData, ex);
                }
            }

            var request = CreateContentTypeRequest(httpReq, operationType, contentType);
            return request;
        }

        /// <summary>Creates content type request.</summary>
        ///
        /// <exception cref="SerializationException">Thrown when a Serialization error condition occurs.</exception>
        ///
        /// <param name="httpReq">    The HTTP request.</param>
        /// <param name="requestType">Type of the request.</param>
        /// <param name="contentType">Type of the content.</param>
        ///
        /// <returns>The new content type request.</returns>
        protected static object CreateContentTypeRequest(IHttpRequest httpReq, Type requestType, string contentType)
        {
            try
            {
                if (!string.IsNullOrEmpty(contentType) && httpReq.ContentLength > 0)
                {
                    var deserializer = EndpointHost.AppHost.ContentTypeFilters.GetStreamDeserializer(contentType);
                    if (deserializer != null)
                    {
                        return deserializer(requestType, httpReq.InputStream);
                    }
                }
            }
            catch (Exception ex)
            {
                var msg = "Could not deserialize '{0}' request using {1}'\nError: {2}"
                    .Fmt(contentType, requestType, ex);
                throw new SerializationException(msg);
            }
            return requestType.CreateInstance(); //Return an empty DTO, even for empty request bodies
        }

        /// <summary>Gets custom request from binder.</summary>
        ///
        /// <param name="httpReq">    The HTTP request.</param>
        /// <param name="requestType">Type of the request.</param>
        ///
        /// <returns>The custom request from binder.</returns>
        protected static object GetCustomRequestFromBinder(IHttpRequest httpReq, Type requestType)
        {
            Func<IHttpRequest, object> requestFactoryFn;
            (ServiceManager ?? EndpointHost.ServiceManager).ServiceController.RequestTypeFactoryMap.TryGetValue(
                requestType, out requestFactoryFn);

            return requestFactoryFn != null ? requestFactoryFn(httpReq) : null;
        }

        /// <summary>Default handled request.</summary>
        ///
        /// <param name="context">An <see cref="T:System.Web.HttpContext" /> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to
        /// service HTTP requests.
        /// </param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        protected static bool DefaultHandledRequest(HttpListenerContext context)
        {
            return false;
        }

        /// <summary>Default handled request.</summary>
        ///
        /// <param name="context">An <see cref="T:System.Web.HttpContext" /> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to
        /// service HTTP requests.
        /// </param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        protected static bool DefaultHandledRequest(HttpContext context)
        {
            return false;
        }

        /// <summary>Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler" /> interface.</summary>
        ///
        /// <param name="context">An <see cref="T:System.Web.HttpContext" /> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to
        /// service HTTP requests.
        /// </param>
        public virtual void ProcessRequest(HttpContext context)
        {
            var operationName = this.RequestName ?? context.Request.GetOperationName();

            if (string.IsNullOrEmpty(operationName)) return;

            if (DefaultHandledRequest(context)) return;

            ProcessRequest(
                new HttpRequestWrapper(operationName, context.Request),
                new HttpResponseWrapper(context.Response),
                operationName);
        }

        /// <summary>Process the request described by context.</summary>
        ///
        /// <param name="context">The context.</param>
        public virtual void ProcessRequest(HttpListenerContext context)
        {
            var operationName = this.RequestName ?? context.Request.GetOperationName();

            if (string.IsNullOrEmpty(operationName)) return;

            if (DefaultHandledRequest(context)) return;

            ProcessRequest(
                new HttpListenerRequestWrapper(operationName, context.Request),
                new HttpListenerResponseWrapper(context.Response),
                operationName);
        }

        /// <summary>Gets or sets the manager for service.</summary>
        ///
        /// <value>The service manager.</value>
        public static ServiceManager ServiceManager { get; set; }

        /// <summary>Gets operation type.</summary>
        ///
        /// <param name="operationName">Name of the operation.</param>
        ///
        /// <returns>The operation type.</returns>
        public static Type GetOperationType(string operationName)
        {
            return ServiceManager != null
                ? ServiceManager.Metadata.GetOperationType(operationName)
                : EndpointHost.Metadata.GetOperationType(operationName);
        }

        /// <summary>Executes the service operation.</summary>
        ///
        /// <param name="request">           The request.</param>
        /// <param name="endpointAttributes">The endpoint attributes.</param>
        /// <param name="httpReq">           The HTTP request.</param>
        /// <param name="httpRes">           The HTTP resource.</param>
        ///
        /// <returns>An object.</returns>
        protected static object ExecuteService(object request, EndpointAttributes endpointAttributes,
            IHttpRequest httpReq, IHttpResponse httpRes)
        {
            return EndpointHost.ExecuteService(request, endpointAttributes, httpReq, httpRes);
        }

        /// <summary>Gets endpoint attributes.</summary>
        ///
        /// <param name="operationContext">Context for the operation.</param>
        ///
        /// <returns>The endpoint attributes.</returns>
        public EndpointAttributes GetEndpointAttributes(System.ServiceModel.OperationContext operationContext)
        {
            if (!EndpointHost.Config.EnableAccessRestrictions) return default(EndpointAttributes);

            var portRestrictions = default(EndpointAttributes);
            var ipAddress = GetIpAddress(operationContext);

            portRestrictions |= HttpRequestExtensions.GetAttributes(ipAddress);

            //TODO: work out if the request was over a secure channel			
            //portRestrictions |= request.IsSecureConnection ? PortRestriction.Secure : PortRestriction.InSecure;

            return portRestrictions;
        }

        /// <summary>Gets IP address.</summary>
        ///
        /// <param name="context">The context.</param>
        ///
        /// <returns>The IP address.</returns>
        public static IPAddress GetIpAddress(System.ServiceModel.OperationContext context)
        {
#if !MONO
            var prop = context.IncomingMessageProperties;
            if (context.IncomingMessageProperties.ContainsKey(System.ServiceModel.Channels.RemoteEndpointMessageProperty.Name))
            {
                var endpoint = prop[System.ServiceModel.Channels.RemoteEndpointMessageProperty.Name]
                    as System.ServiceModel.Channels.RemoteEndpointMessageProperty;
                if (endpoint != null)
                {
                    return IPAddress.Parse(endpoint.Address);
                }
            }
#endif
            return null;
        }

        /// <summary>Queries if a given assert operation exists.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <param name="operationName">Name of the operation.</param>
        /// <param name="type">         The type.</param>
        protected static void AssertOperationExists(string operationName, Type type)
        {
            if (type == null)
            {
                throw new NotImplementedException(
                    string.Format("The operation '{0}' does not exist for this service", operationName));
            }
        }

        /// <summary>Handles the exception.</summary>
        ///
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        ///
        /// <param name="httpReq">      The HTTP request.</param>
        /// <param name="httpRes">      The HTTP resource.</param>
        /// <param name="operationName">Name of the operation.</param>
        /// <param name="ex">           The ex.</param>
        protected void HandleException(IHttpRequest httpReq, IHttpResponse httpRes, string operationName, Exception ex)
        {
            var errorMessage = string.Format("Error occured while Processing Request: {0}", ex.Message);
            Log.Error(errorMessage, ex);

            try
            {
                EndpointHost.ExceptionHandler(httpReq, httpRes, operationName, ex);
            }
            catch (Exception writeErrorEx)
            {
                //Exception in writing to response should not hide the original exception
                Log.Info("Failed to write error to response: {0}", writeErrorEx);
                //rethrow the original exception
                throw ex;
            }
            finally
            {
                httpRes.EndRequest(skipHeaders: true);
            }
        }

        /// <summary>Assert access.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="httpReq">      The HTTP request.</param>
        /// <param name="httpRes">      The HTTP resource.</param>
        /// <param name="feature">      The feature.</param>
        /// <param name="operationName">Name of the operation.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        protected bool AssertAccess(IHttpRequest httpReq, IHttpResponse httpRes, Feature feature, string operationName)
        {
            if (operationName == null)
                throw new ArgumentNullException("operationName");

            if (EndpointHost.Config.EnableFeatures != Feature.All)
            {
                if (!EndpointHost.Config.HasFeature(feature))
                {
                    EndpointHost.Config.HandleErrorResponse(httpReq, httpRes, HttpStatusCode.Forbidden, "Feature Not Available");
                    return false;
                }
            }

            var format = feature.ToFormat();
            if (!EndpointHost.Metadata.CanAccess(httpReq, format, operationName))
            {
                EndpointHost.Config.HandleErrorResponse(httpReq, httpRes, HttpStatusCode.Forbidden, "Service Not Available");
                return false;
            }
            return true;
        }

    }
}