using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Web;
using System.Xml;
using NServiceKit.Common;
using NServiceKit.Common.Web;
using NServiceKit.ServiceClient.Web;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceModel.Serialization;
using NServiceKit.ServiceModel.Support;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints.Utils;
using HttpRequestWrapper = NServiceKit.WebHost.Endpoints.Extensions.HttpRequestWrapper;
using HttpResponseWrapper = NServiceKit.WebHost.Endpoints.Extensions.HttpResponseWrapper;

namespace NServiceKit.WebHost.Endpoints.Support
{
    /// <summary>A SOAP handler.</summary>
    public abstract class SoapHandler : EndpointHandlerBase, IOneWay, ISyncReply
    {
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Support.SoapHandler class.</summary>
        ///
        /// <param name="soapType">Type of the SOAP.</param>
        public SoapHandler(EndpointAttributes soapType)
        {
            this.HandlerAttributes = soapType;
        }

        /// <summary>Sends an one way.</summary>
        ///
        /// <param name="requestMsg">Message describing the request.</param>
        public void SendOneWay(Message requestMsg)
        {
            SendOneWay(requestMsg, null, null);
        }

        /// <summary>Sends an one way.</summary>
        ///
        /// <param name="requestMsg">  Message describing the request.</param>
        /// <param name="httpRequest"> The HTTP request.</param>
        /// <param name="httpResponse">The HTTP response.</param>
        protected void SendOneWay(Message requestMsg, IHttpRequest httpRequest, IHttpResponse httpResponse)
        {
            var endpointAttributes = EndpointAttributes.OneWay | this.HandlerAttributes;

            ExecuteMessage(requestMsg, endpointAttributes, httpRequest, httpResponse);
        }

        /// <summary>Gets request message from stream.</summary>
        ///
        /// <param name="requestStream">The request stream.</param>
        ///
        /// <returns>The request message from stream.</returns>
        protected abstract Message GetRequestMessageFromStream(Stream requestStream);

        /// <summary>Send this message.</summary>
        ///
        /// <param name="requestMsg">Message describing the request.</param>
        ///
        /// <returns>A Message.</returns>
        public Message Send(Message requestMsg)
        {
            var endpointAttributes = EndpointAttributes.Reply | this.HandlerAttributes;

            return ExecuteMessage(requestMsg, endpointAttributes, null, null);
        }

        /// <summary>Send this message.</summary>
        ///
        /// <param name="requestMsg">  Message describing the request.</param>
        /// <param name="httpRequest"> The HTTP request.</param>
        /// <param name="httpResponse">The HTTP response.</param>
        ///
        /// <returns>A Message.</returns>
        protected Message Send(Message requestMsg, IHttpRequest httpRequest, IHttpResponse httpResponse)
        {
            var endpointAttributes = EndpointAttributes.Reply | this.HandlerAttributes;

            return ExecuteMessage(requestMsg, endpointAttributes, httpRequest, httpResponse);
        }

        /// <summary>Empty response.</summary>
        ///
        /// <param name="requestMsg"> Message describing the request.</param>
        /// <param name="requestType">Type of the request.</param>
        ///
        /// <returns>A Message.</returns>
        public Message EmptyResponse(Message requestMsg, Type requestType)
        {
            var responseType = AssemblyUtils.FindType(requestType.FullName + "Response");
            var response = (responseType ?? typeof(object)).CreateInstance();

            return requestMsg.Headers.Action == null
                ? Message.CreateMessage(requestMsg.Version, null, response)
                : Message.CreateMessage(requestMsg.Version, requestType.Name + "Response", response);
        }

        /// <summary>Executes the message operation.</summary>
        ///
        /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null.</exception>
        /// <exception>    Thrown when an unauthorized access error condition occurs.</exception>
        /// <exception cref="SerializationException">Thrown when a Serialization error condition occurs.</exception>
        ///
        /// <param name="message">           The message.</param>
        /// <param name="endpointAttributes">The endpoint attributes.</param>
        /// <param name="httpRequest">       The HTTP request.</param>
        /// <param name="httpResponse">      The HTTP response.</param>
        ///
        /// <returns>A Message.</returns>
        protected Message ExecuteMessage(Message message, EndpointAttributes endpointAttributes, IHttpRequest httpRequest, IHttpResponse httpResponse)
        {
            var soapFeature = endpointAttributes.ToSoapFeature();
            EndpointHost.Config.AssertFeatures(soapFeature);

            var httpReq = HttpContext.Current != null && httpRequest == null
                    ? new HttpRequestWrapper(HttpContext.Current.Request)
                    : httpRequest;
            var httpRes = HttpContext.Current != null && httpResponse == null
                ? new HttpResponseWrapper(HttpContext.Current.Response)
                : httpResponse;

            if (httpReq == null)
                throw new ArgumentNullException("httpRequest");

            if (httpRes == null)
                throw new ArgumentNullException("httpResponse");

            if (EndpointHost.ApplyPreRequestFilters(httpReq, httpRes))
                return PrepareEmptyResponse(message, httpReq);

            var requestMsg = message ?? GetRequestMessageFromStream(httpReq.InputStream);
            string requestXml = GetRequestXml(requestMsg);
            var requestType = GetRequestType(requestMsg, requestXml);
            if (!EndpointHost.Metadata.CanAccess(endpointAttributes, soapFeature.ToFormat(), requestType.Name))
                throw EndpointHost.Config.UnauthorizedAccess(endpointAttributes);

            try
            {
                var useXmlSerializerRequest = requestType.HasAttribute<XmlSerializerFormatAttribute>();

                var request = useXmlSerializerRequest
                                  ? XmlSerializableDeserializer.Instance.Parse(requestXml, requestType)
                                  : DataContractDeserializer.Instance.Parse(requestXml, requestType);
                
                var requiresSoapMessage = request as IRequiresSoapMessage;
                if (requiresSoapMessage != null)
                {
                    requiresSoapMessage.Message = requestMsg;
                }

                httpReq.OperationName = requestType.Name;
                httpReq.SetItem("SoapMessage", requestMsg);

                var hasRequestFilters = EndpointHost.RequestFilters.Count > 0
                    || FilterAttributeCache.GetRequestFilterAttributes(request.GetType()).Any();

                if (hasRequestFilters && EndpointHost.ApplyRequestFilters(httpReq, httpRes, request))
                    return EmptyResponse(requestMsg, requestType);

                var response = ExecuteService(request, endpointAttributes, httpReq, httpRes);

                var hasResponseFilters = EndpointHost.ResponseFilters.Count > 0
                   || FilterAttributeCache.GetResponseFilterAttributes(response.GetType()).Any();

                if (hasResponseFilters && EndpointHost.ApplyResponseFilters(httpReq, httpRes, response))
                    return EmptyResponse(requestMsg, requestType);

                var httpResult = response as IHttpResult;
                if (httpResult != null)
                    response = httpResult.Response;

                var useXmlSerializerResponse = response.GetType().HasAttribute<XmlSerializerFormatAttribute>();
                
                if (useXmlSerializerResponse)
                    return requestMsg.Headers.Action == null
                        ? Message.CreateMessage(requestMsg.Version, null, response, new XmlSerializerWrapper(response.GetType()))
                        : Message.CreateMessage(requestMsg.Version, requestType.Name + "Response", response, new XmlSerializerWrapper(response.GetType()));
                
                return requestMsg.Headers.Action == null
                    ? Message.CreateMessage(requestMsg.Version, null, response)
                    : Message.CreateMessage(requestMsg.Version, requestType.Name + "Response", response);
            }
            catch (Exception ex)
            {
                throw new SerializationException("3) Error trying to deserialize requestType: "
                    + requestType
                    + ", xml body: " + requestXml, ex);
            }
        }

        private Message PrepareEmptyResponse(Message message, IHttpRequest httpRequest)
        {
            var requestMessage = message ?? GetRequestMessageFromStream(httpRequest.InputStream);
            string requestXml = GetRequestXml(requestMessage);
            var requestType = GetRequestType(requestMessage, requestXml);
            return EmptyResponse(requestMessage, requestType);
        }

        private static string GetRequestXml(Message requestMsg)
        {
            string requestXml;
            using (var reader = requestMsg.GetReaderAtBodyContents())
            {
                requestXml = reader.ReadOuterXml();
            }
            return requestXml;
        }

        /// <summary>Gets SOAP 12 request message.</summary>
        ///
        /// <param name="inputStream">Stream to read data from.</param>
        ///
        /// <returns>The SOAP 12 request message.</returns>
        protected static Message GetSoap12RequestMessage(Stream inputStream)
        {
            return GetRequestMessage(inputStream, MessageVersion.Soap12WSAddressingAugust2004);
        }

        /// <summary>Gets SOAP 11 request message.</summary>
        ///
        /// <param name="inputStream">Stream to read data from.</param>
        ///
        /// <returns>The SOAP 11 request message.</returns>
        protected static Message GetSoap11RequestMessage(Stream inputStream)
        {
            return GetRequestMessage(inputStream, MessageVersion.Soap11WSAddressingAugust2004);
        }

        /// <summary>Gets request message.</summary>
        ///
        /// <param name="inputStream">Stream to read data from.</param>
        /// <param name="msgVersion"> The message version.</param>
        ///
        /// <returns>The request message.</returns>
        protected static Message GetRequestMessage(Stream inputStream, MessageVersion msgVersion)
        {
            using (var sr = new StreamReader(inputStream))
            {
                var requestXml = sr.ReadToEnd();

                var doc = new XmlDocument();
                doc.LoadXml(requestXml);

                var msg = Message.CreateMessage(new XmlNodeReader(doc), int.MaxValue,
                    msgVersion);

                return msg;
            }
        }

        /// <summary>Gets request type.</summary>
        ///
        /// <param name="requestMsg">Message describing the request.</param>
        /// <param name="xml">       The XML.</param>
        ///
        /// <returns>The request type.</returns>
        protected Type GetRequestType(Message requestMsg, string xml)
        {
            var action = GetAction(requestMsg, xml);

            var operationType = EndpointHost.Metadata.GetOperationType(action);
            AssertOperationExists(action, operationType);

            return operationType;
        }

        /// <summary>Gets an action.</summary>
        ///
        /// <param name="requestMsg">Message describing the request.</param>
        /// <param name="xml">       The XML.</param>
        ///
        /// <returns>The action.</returns>
        protected string GetAction(Message requestMsg, string xml)
        {
            var action = GetActionFromHttpContext();
            if (action != null) return action;

            if (requestMsg.Headers.Action != null)
            {
                return requestMsg.Headers.Action;
            }

            return xml.StartsWith("<")
                ? xml.Substring(1, xml.IndexOf(" ") - 1).SplitOnFirst(':').Last()
                : null;
        }

        /// <summary>Gets action from HTTP context.</summary>
        ///
        /// <returns>The action from HTTP context.</returns>
        protected static string GetActionFromHttpContext()
        {
            var context = HttpContext.Current;
            return context == null ? null : GetAction(context.Request.ContentType);
        }

        private static string GetAction(string contentType)
        {
            if (contentType != null)
            {
                return GetOperationName(contentType);
            }

            return null;
        }

        private static string GetOperationName(string contentType)
        {
            var urlActionPos = contentType.IndexOf("action=\"");
            if (urlActionPos != -1)
            {
                var startIndex = urlActionPos + "action=\"".Length;
                var urlAction = contentType.Substring(
                    startIndex,
                    contentType.IndexOf('"', startIndex) - startIndex);

                var parts = urlAction.Split('/');
                var operationName = parts.Last();
                return operationName;
            }

            return null;
        }

        /// <summary>Gets SOAP content type.</summary>
        ///
        /// <param name="contentType">Type of the content.</param>
        ///
        /// <returns>The SOAP content type.</returns>
        public string GetSoapContentType(string contentType)
        {
            var requestOperationName = GetAction(contentType);
            return requestOperationName != null
                    ? contentType.Replace(requestOperationName, requestOperationName + "Response")
                    : (this.HandlerAttributes == EndpointAttributes.Soap11 ? ContentType.Soap11 : ContentType.Soap12);
        }

        /// <summary>Creates a request.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <param name="request">      The request.</param>
        /// <param name="operationName">Name of the operation.</param>
        ///
        /// <returns>The new request.</returns>
        public override object CreateRequest(IHttpRequest request, string operationName)
        {
            throw new NotImplementedException();
        }

        /// <summary>Gets a response.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <param name="httpReq">The HTTP request.</param>
        /// <param name="httpRes">The HTTP resource.</param>
        /// <param name="request">The request.</param>
        ///
        /// <returns>The response.</returns>
        public override object GetResponse(IHttpRequest httpReq, IHttpResponse httpRes, object request)
        {
            throw new NotImplementedException();
        }
    }
}
