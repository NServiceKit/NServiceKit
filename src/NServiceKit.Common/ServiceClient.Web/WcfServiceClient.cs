#if !SILVERLIGHT && !MONOTOUCH && !XBOX && !ANDROIDINDIE
using System;
using System.IO;
using System.Net;
using System.Xml;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using NServiceKit.Common.Utils;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface.ServiceModel;


namespace NServiceKit.ServiceClient.Web
{
    /// <summary>
    /// Adds the singleton instance of <see cref="CookieManagerMessageInspector"/> to an endpoint on the client.
    /// </summary>
    /// <remarks>
    /// Based on http://megakemp.wordpress.com/2009/02/06/managing-shared-cookies-in-wcf/
    /// </remarks>
    public class CookieManagerEndpointBehavior : IEndpointBehavior
    {
        /// <summary>Implement to pass data at runtime to bindings to support custom behavior.</summary>
        ///
        /// <param name="endpoint">         The endpoint to modify.</param>
        /// <param name="bindingParameters">The objects that binding elements require to support the behavior.</param>
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
            return;
        }

        /// <summary>
        /// Adds the singleton of the <see cref="CookieManagerMessageInspector"/> class to the client endpoint's message inspectors.
        /// </summary>
        /// <param name="endpoint">The endpoint that is to be customized.</param>
        /// <param name="clientRuntime">The client runtime to be customized.</param>
        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            var cm = CookieManagerMessageInspector.Instance;
            cm.Uri = endpoint.ListenUri.AbsoluteUri;
            clientRuntime.MessageInspectors.Add(cm);
        }

        /// <summary>Implements a modification or extension of the service across an endpoint.</summary>
        ///
        /// <param name="endpoint">          The endpoint that exposes the contract.</param>
        /// <param name="endpointDispatcher">The endpoint dispatcher to be modified or extended.</param>
        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            return;
        }

        /// <summary>Implement to confirm that the endpoint meets some intended criteria.</summary>
        ///
        /// <param name="endpoint">The endpoint to validate.</param>
        public void Validate(ServiceEndpoint endpoint)
        {
            return;
        }
    }

    /// <summary>
    /// Maintains a copy of the cookies contained in the incoming HTTP response received from any service
    /// and appends it to all outgoing HTTP requests.
    /// </summary>
    /// <remarks>
    /// This class effectively allows to send any received HTTP cookies to different services,
    /// reproducing the same functionality available in ASMX Web Services proxies with the <see cref="System.Net.CookieContainer"/> class.
    /// Based on http://megakemp.wordpress.com/2009/02/06/managing-shared-cookies-in-wcf/
    /// </remarks>
    public class CookieManagerMessageInspector : IClientMessageInspector
    {
        private static CookieManagerMessageInspector instance;
        private CookieContainer cookieContainer;

        /// <summary>Gets or sets URI of the document.</summary>
        ///
        /// <value>The URI.</value>
        public string Uri { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CookieContainer"/> class.
        /// </summary>
        public CookieManagerMessageInspector()
        {
            cookieContainer = new CookieContainer();
            Uri = "http://tempuri.org";
        }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceClient.Web.CookieManagerMessageInspector class.</summary>
        ///
        /// <param name="uri">URI of the document.</param>
        public CookieManagerMessageInspector(string uri)
        {
            cookieContainer = new CookieContainer();
            Uri = uri;
        }

        /// <summary>
        /// Gets the singleton <see cref="CookieManagerMessageInspector" /> instance.
        /// </summary>
        public static CookieManagerMessageInspector Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CookieManagerMessageInspector();
                }

                return instance;
            }
        }

        /// <summary>
        /// Inspects a message after a reply message is received but prior to passing it back to the client application.
        /// </summary>
        /// <param name="reply">The message to be transformed into types and handed back to the client application.</param>
        /// <param name="correlationState">Correlation state data.</param>
        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            var httpResponse = reply.Properties[HttpResponseMessageProperty.Name] as HttpResponseMessageProperty;

            if (httpResponse != null)
            {
                string cookie = httpResponse.Headers[HttpResponseHeader.SetCookie];

                if (!string.IsNullOrEmpty(cookie))
                {
                    cookieContainer.SetCookies(new System.Uri(Uri), cookie);
                }
            }
        }

        /// <summary>
        /// Inspects a message before a request message is sent to a service.
        /// </summary>
        /// <param name="request">The message to be sent to the service.</param>
        /// <param name="channel">The client object channel.</param>
        /// <returns>
        /// <strong>Null</strong> since no message correlation is used.
        /// </returns>
        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            HttpRequestMessageProperty httpRequest;

            // The HTTP request object is made available in the outgoing message only when
            // the Visual Studio Debugger is attacched to the running process
            if (!request.Properties.ContainsKey(HttpRequestMessageProperty.Name))
            {
                request.Properties.Add(HttpRequestMessageProperty.Name, new HttpRequestMessageProperty());
            }

            httpRequest = (HttpRequestMessageProperty)request.Properties[HttpRequestMessageProperty.Name];
            httpRequest.Headers.Add(HttpRequestHeader.Cookie, cookieContainer.GetCookieHeader(new System.Uri(Uri)));

            return null;
        }
    }

    /// <summary>A WCF service client.</summary>
    public abstract class WcfServiceClient : IWcfServiceClient
    {
        const string XPATH_SOAP_FAULT = "/s:Fault";
        const string XPATH_SOAP_FAULT_REASON = "/s:Fault/s:Reason";
        const string NAMESPACE_SOAP = "http://www.w3.org/2003/05/soap-envelope";
        const string NAMESPACE_SOAP_ALIAS = "s";

        /// <summary>Gets or sets URI of the document.</summary>
        ///
        /// <value>The URI.</value>
        public string Uri { get; set; }

        /// <summary>Sets a proxy.</summary>
        ///
        /// <param name="proxyAddress">The proxy address.</param>
        public abstract void SetProxy(Uri proxyAddress);

        /// <summary>Gets the message version.</summary>
        ///
        /// <value>The message version.</value>
        protected abstract MessageVersion MessageVersion { get; }

        /// <summary>Gets the binding.</summary>
        ///
        /// <value>The binding.</value>
        protected abstract Binding Binding { get; }

        /// <summary>
        /// Specifies if cookies should be stored
        /// </summary>
        // CCB Custom
        public bool StoreCookies { get; set; }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceClient.Web.WcfServiceClient class.</summary>
        public WcfServiceClient()
        {
            // CCB Custom
            this.StoreCookies = true;
        }

        private static XmlNamespaceManager GetNamespaceManager(XmlDocument doc)
        {
            var nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace(NAMESPACE_SOAP_ALIAS, NAMESPACE_SOAP);
            return nsmgr;
        }

        private static Exception CreateException(Exception e, XmlReader reader)
        {
            var doc = new XmlDocument();
            doc.Load(reader);
            var node = doc.SelectSingleNode(XPATH_SOAP_FAULT, GetNamespaceManager(doc));
            if (node != null)
            {
                string errMsg = null;
                var nodeReason = doc.SelectSingleNode(XPATH_SOAP_FAULT_REASON, GetNamespaceManager(doc));
                if (nodeReason != null)
                {
                    errMsg = nodeReason.FirstChild.InnerXml;
                }
                return new Exception(string.Format("SOAP FAULT '{0}': {1}", errMsg, node.InnerXml), e);
            }
            return e;
        }

        private ServiceEndpoint SyncReply
        {
            get
            {
                var contract = new ContractDescription("NServiceKit.ServiceClient.Web.ISyncReply", "http://services.NServiceKit.net/");
                var addr = new EndpointAddress(Uri);
                var endpoint = new ServiceEndpoint(contract, Binding, addr);
                return endpoint;
            }
        }

        /// <summary>Send this message.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>A Message.</returns>
        public Message Send(object request)
        {
            return Send(request, request.GetType().Name);
        }

        /// <summary>Send this message.</summary>
        ///
        /// <param name="request">The request.</param>
        /// <param name="action"> The action.</param>
        ///
        /// <returns>A Message.</returns>
        public Message Send(object request, string action)
        {
            return Send(Message.CreateMessage(MessageVersion, action, request));
        }

        /// <summary>Send this message.</summary>
        ///
        /// <param name="reader">The reader.</param>
        /// <param name="action">The action.</param>
        ///
        /// <returns>A Message.</returns>
        public Message Send(XmlReader reader, string action)
        {
            return Send(Message.CreateMessage(MessageVersion, action, reader));
        }

        /// <summary>Send this message.</summary>
        ///
        /// <param name="message">The message.</param>
        ///
        /// <returns>A Message.</returns>
        public Message Send(Message message)
        {
            using (var client = new GenericProxy<ISyncReply>(SyncReply))
            {
                // CCB Custom...add behavior to propagate cookies across SOAP method calls
                if (StoreCookies)
                    client.ChannelFactory.Endpoint.Behaviors.Add(new CookieManagerEndpointBehavior());
                var response = client.Proxy.Send(message);
                return response;
            }
        }

        /// <summary>Gets a body.</summary>
        ///
        /// <exception cref="CreateException">Thrown when a Create error condition occurs.</exception>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="message">The message.</param>
        ///
        /// <returns>The body.</returns>
        public static T GetBody<T>(Message message)
        {
            var buffer = message.CreateBufferedCopy(int.MaxValue);
            try
            {
                return buffer.CreateMessage().GetBody<T>();
            }
            catch (Exception ex)
            {
                throw CreateException(ex, buffer.CreateMessage().GetReaderAtBodyContents());
            }
        }

        /// <summary>Send this message.</summary>
        ///
        /// <exception cref="WebServiceException">Thrown when a Web Service error condition occurs.</exception>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="request">The request.</param>
        ///
        /// <returns>A TResponse.</returns>
        public T Send<T>(object request)
        {
            try
            {
                var responseMsg = Send(request);
                var response = responseMsg.GetBody<T>();
                var responseStatus = GetResponseStatus(response);
                if (responseStatus != null && !string.IsNullOrEmpty(responseStatus.ErrorCode))
                {
                    throw new WebServiceException(responseStatus.Message, null) {
                        StatusCode = 500,
                        ResponseDto = response,
                        StatusDescription = responseStatus.Message,
                    };
                }
                return response;
            }
            catch (WebServiceException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var webEx = ex as WebException ?? ex.InnerException as WebException;
                if (webEx == null)
                {
                    throw new WebServiceException(ex.Message, ex) {
                        StatusCode = 500,
                    };
                }

                var httpEx = webEx.Response as HttpWebResponse;
                throw new WebServiceException(webEx.Message, webEx) {
                    StatusCode = httpEx != null ? (int)httpEx.StatusCode : 500
                };
            }
        }

        /// <summary>Send this message.</summary>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="request">The request.</param>
        ///
        /// <returns>A TResponse.</returns>
        public TResponse Send<TResponse>(IReturn<TResponse> request)
        {
            return Send<TResponse>((object)request);
        }

        /// <summary>Send this message.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <param name="request">The request.</param>
        public void Send(IReturnVoid request)
        {
            throw new NotImplementedException();
        }

        /// <summary>Gets response status.</summary>
        ///
        /// <param name="response">The response.</param>
        ///
        /// <returns>The response status.</returns>
        public ResponseStatus GetResponseStatus(object response)
        {
            if (response == null)
                return null;

            var hasResponseStatus = response as IHasResponseStatus;
            if (hasResponseStatus != null)
                return hasResponseStatus.ResponseStatus;

            var propertyInfo = response.GetType().GetProperty("ResponseStatus");
            if (propertyInfo == null)
                return null;

            return ReflectionUtils.GetProperty(response, propertyInfo) as ResponseStatus;
        }

        /// <summary>Posts a file.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
        /// <param name="fileToUpload">         The file to upload.</param>
        /// <param name="mimeType">             Type of the mime.</param>
        ///
        /// <returns>A TResponse.</returns>
        public TResponse PostFile<TResponse>(string relativeOrAbsoluteUrl, FileInfo fileToUpload, string mimeType)
        {
            throw new NotImplementedException();
        }

        /// <summary>Posts a file.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
        /// <param name="fileToUpload">         The file to upload.</param>
        /// <param name="fileName">             Filename of the file.</param>
        /// <param name="mimeType">             Type of the mime.</param>
        ///
        /// <returns>A TResponse.</returns>
        public TResponse PostFile<TResponse>(string relativeOrAbsoluteUrl, Stream fileToUpload, string fileName, string mimeType)
        {
            throw new NotImplementedException();
        }

        /// <summary>Sends an one way.</summary>
        ///
        /// <param name="request">The request.</param>
        public void SendOneWay(object request)
        {
            SendOneWay(request, request.GetType().Name);
        }

        /// <summary>Sends an one way.</summary>
        ///
        /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
        /// <param name="request">              The request.</param>
        public void SendOneWay(string relativeOrAbsoluteUrl, object request)
        {
            SendOneWay(Message.CreateMessage(MessageVersion, relativeOrAbsoluteUrl, request));
        }

        /// <summary>Sends an one way.</summary>
        ///
        /// <param name="request">The request.</param>
        /// <param name="action"> The action.</param>
        public void SendOneWay(object request, string action)
        {
            SendOneWay(Message.CreateMessage(MessageVersion, action, request));
        }

        /// <summary>Sends an one way.</summary>
        ///
        /// <param name="reader">The reader.</param>
        /// <param name="action">The action.</param>
        public void SendOneWay(XmlReader reader, string action)
        {
            SendOneWay(Message.CreateMessage(MessageVersion, action, reader));
        }

        /// <summary>Sends an one way.</summary>
        ///
        /// <param name="message">The message.</param>
        public void SendOneWay(Message message)
        {
            using (var client = new GenericProxy<IOneWay>(SyncReply))
            {
                client.Proxy.SendOneWay(message);
            }
        }

        /// <summary>Sends the asynchronous.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="request">  The request.</param>
        /// <param name="onSuccess">The on success.</param>
        /// <param name="onError">  The on error.</param>
        public void SendAsync<TResponse>(object request, Action<TResponse> onSuccess, Action<TResponse, Exception> onError)
        {
            throw new NotImplementedException();
        }

        /// <summary>Sets the credentials.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        public void SetCredentials(string userName, string password)
        {
            throw new NotImplementedException();
        }

        /// <summary>Gets the asynchronous.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="request">  The request.</param>
        /// <param name="onSuccess">The on success.</param>
        /// <param name="onError">  The on error.</param>
        public void GetAsync<TResponse>(IReturn<TResponse> request, Action<TResponse> onSuccess, Action<TResponse, Exception> onError)
        {
            throw new NotImplementedException();
        }

        /// <summary>Gets the asynchronous.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
        /// <param name="onSuccess">            The on success.</param>
        /// <param name="onError">              The on error.</param>
        public void GetAsync<TResponse>(string relativeOrAbsoluteUrl, Action<TResponse> onSuccess, Action<TResponse, Exception> onError)
        {
            throw new NotImplementedException();
        }

        /// <summary>Deletes the asynchronous.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
        /// <param name="onSuccess">            The on success.</param>
        /// <param name="onError">              The on error.</param>
        public void DeleteAsync<TResponse>(string relativeOrAbsoluteUrl, Action<TResponse> onSuccess, Action<TResponse, Exception> onError)
        {
            throw new NotImplementedException();
        }

        /// <summary>Deletes the asynchronous.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="request">  The request.</param>
        /// <param name="onSuccess">The on success.</param>
        /// <param name="onError">  The on error.</param>
        public void DeleteAsync<TResponse>(IReturn<TResponse> request, Action<TResponse> onSuccess, Action<TResponse, Exception> onError)
        {
            throw new NotImplementedException();
        }

        /// <summary>Posts the asynchronous.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="request">  The request.</param>
        /// <param name="onSuccess">The on success.</param>
        /// <param name="onError">  The on error.</param>
        public void PostAsync<TResponse>(IReturn<TResponse> request, Action<TResponse> onSuccess, Action<TResponse, Exception> onError)
        {
            throw new NotImplementedException();
        }

        /// <summary>Posts the asynchronous.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
        /// <param name="request">              The request.</param>
        /// <param name="onSuccess">            The on success.</param>
        /// <param name="onError">              The on error.</param>
        public void PostAsync<TResponse>(string relativeOrAbsoluteUrl, object request, Action<TResponse> onSuccess, Action<TResponse, Exception> onError)
        {
            throw new NotImplementedException();
        }

        /// <summary>Puts the asynchronous.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="request">  The request.</param>
        /// <param name="onSuccess">The on success.</param>
        /// <param name="onError">  The on error.</param>
        public void PutAsync<TResponse>(IReturn<TResponse> request, Action<TResponse> onSuccess, Action<TResponse, Exception> onError)
        {
            throw new NotImplementedException();
        }

        /// <summary>Puts the asynchronous.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
        /// <param name="request">              The request.</param>
        /// <param name="onSuccess">            The on success.</param>
        /// <param name="onError">              The on error.</param>
        public void PutAsync<TResponse>(string relativeOrAbsoluteUrl, object request, Action<TResponse> onSuccess, Action<TResponse, Exception> onError)
        {
            throw new NotImplementedException();
        }

        /// <summary>Custom method asynchronous.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="httpVerb"> The HTTP verb.</param>
        /// <param name="request">  The request.</param>
        /// <param name="onSuccess">The on success.</param>
        /// <param name="onError">  The on error.</param>
        public void CustomMethodAsync<TResponse>(string httpVerb, IReturn<TResponse> request, Action<TResponse> onSuccess, Action<TResponse, Exception> onError)
        {
            throw new NotImplementedException();
        }

        /// <summary>Cancel asynchronous.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        public void CancelAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
        }

        /// <summary>Posts a file with request.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
        /// <param name="fileToUpload">         The file to upload.</param>
        /// <param name="request">              The request.</param>
        ///
        /// <returns>A TResponse.</returns>
        public TResponse PostFileWithRequest<TResponse>(string relativeOrAbsoluteUrl, FileInfo fileToUpload, object request)
        {
            throw new NotImplementedException();
        }

        /// <summary>Posts a file with request.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
        /// <param name="fileToUpload">         The file to upload.</param>
        /// <param name="fileName">             Filename of the file.</param>
        /// <param name="request">              The request.</param>
        ///
        /// <returns>A TResponse.</returns>
        public TResponse PostFileWithRequest<TResponse>(string relativeOrAbsoluteUrl, Stream fileToUpload, string fileName, object request)
        {
            throw new NotImplementedException();
        }
    }
}
#endif
