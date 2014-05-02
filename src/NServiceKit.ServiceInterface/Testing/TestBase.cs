using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using NServiceKit.Common;
using NServiceKit.Common.Web;
using NServiceKit.Service;
using NServiceKit.ServiceClient.Web;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface.ServiceModel;
using NServiceKit.ServiceModel;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints;
using NServiceKit.WebHost.Endpoints.Support;

namespace NServiceKit.ServiceInterface.Testing
{
    /// <summary>A test base.</summary>
    public abstract class TestBase
    {
        /// <summary>Gets or sets the application host.</summary>
        ///
        /// <value>The application host.</value>
        protected IAppHost AppHost { get; set; }

        /// <summary>Gets or sets a value indicating whether this object has configured.</summary>
        ///
        /// <value>true if this object has configured, false if not.</value>
        protected bool HasConfigured { get; set; }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.Testing.TestBase class.</summary>
        ///
        /// <param name="serviceAssemblies">The service assemblies.</param>
        protected TestBase(params Assembly[] serviceAssemblies)
            : this(null, serviceAssemblies) { }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.Testing.TestBase class.</summary>
        ///
        /// <param name="serviceClientBaseUri">URI of the service client base.</param>
        /// <param name="serviceAssemblies">   The service assemblies.</param>
        protected TestBase(string serviceClientBaseUri, params Assembly[] serviceAssemblies)
        {
            if (serviceAssemblies.Length == 0)
                serviceAssemblies = new[] { GetType().Assembly };

            ServiceClientBaseUri = serviceClientBaseUri;
            ServiceAssemblies = serviceAssemblies;

            this.AppHost = new TestAppHost(null, serviceAssemblies);

            EndpointHost.ServiceManager = this.AppHost.Config.ServiceManager;

            EndpointHost.ConfigureHost(this.AppHost, "TestBase", EndpointHost.ServiceManager);
        }

        /// <summary>Configures the given container.</summary>
        ///
        /// <param name="container">The container.</param>
        protected abstract void Configure(Funq.Container container);

        /// <summary>Gets the container.</summary>
        ///
        /// <value>The container.</value>
        protected Funq.Container Container
        {
            get { return EndpointHost.ServiceManager.Container; }
        }

        /// <summary>Gets the routes.</summary>
        ///
        /// <value>The routes.</value>
        protected IServiceRoutes Routes
        {
            get { return EndpointHost.ServiceManager.ServiceController.Routes; }
        }

        //All integration tests call the Webservices hosted at the following location:
        protected string ServiceClientBaseUri { get; set; }

        /// <summary>Gets or sets the service assemblies.</summary>
        ///
        /// <value>The service assemblies.</value>
        protected Assembly[] ServiceAssemblies { get; set; }

        /// <summary>Executes the before test fixture action.</summary>
        public virtual void OnBeforeTestFixture()
        {
            OnConfigure();
        }

        /// <summary>Executes the configure action.</summary>
        protected virtual void OnConfigure()
        {
            if (HasConfigured) return;

            HasConfigured = true;
            Configure(Container);
            EndpointHost.AfterInit();
        }

        /// <summary>Executes the before each test action.</summary>
        public virtual void OnBeforeEachTest()
        {
            OnConfigure();
        }

        /// <summary>Creates new service client.</summary>
        ///
        /// <returns>The new new service client.</returns>
        protected virtual IServiceClient CreateNewServiceClient()
        {
            return new DirectServiceClient(this, EndpointHost.ServiceManager);
        }

        /// <summary>Creates new rest client.</summary>
        ///
        /// <returns>The new new rest client.</returns>
        protected virtual IRestClient CreateNewRestClient()
        {
            return new DirectServiceClient(this, EndpointHost.ServiceManager);
        }

        /// <summary>Creates new rest client asynchronous.</summary>
        ///
        /// <returns>The new new rest client asynchronous.</returns>
        protected virtual IRestClientAsync CreateNewRestClientAsync()
        {
            return new DirectServiceClient(this, EndpointHost.ServiceManager);
        }

        /// <summary>A direct service client.</summary>
        public class DirectServiceClient : IServiceClient, IRestClient
        {
            private readonly TestBase parent;
            ServiceManager ServiceManager { get; set; }

            /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.Testing.TestBase.DirectServiceClient class.</summary>
            ///
            /// <param name="parent">        The parent.</param>
            /// <param name="serviceManager">Manager for service.</param>
            public DirectServiceClient(TestBase parent, ServiceManager serviceManager)
            {
                this.parent = parent;
                this.ServiceManager = serviceManager;
            }

            /// <summary>Sends an one way.</summary>
            ///
            /// <param name="request">The request.</param>
            public void SendOneWay(object request)
            {
                ServiceManager.Execute(request);
            }

            /// <summary>Sends an one way.</summary>
            ///
            /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
            /// <param name="request">              The request.</param>
            public void SendOneWay(string relativeOrAbsoluteUrl, object request)
            {
                ServiceManager.Execute(request);
            }

            /// <summary>Send this message.</summary>
            ///
            /// <exception cref="WebServiceException">Thrown when a Web Service error condition occurs.</exception>
            ///
            /// <typeparam name="TResponse">Type of the response.</typeparam>
            /// <param name="request">The request.</param>
            ///
            /// <returns>A TResponse.</returns>
            public TResponse Send<TResponse>(object request)
            {
                var response = ServiceManager.Execute(request);
                var httpResult = response as IHttpResult;
                if (httpResult != null)
                {
                    if (httpResult.StatusCode >= HttpStatusCode.BadRequest)
                    {
                        var webEx = new WebServiceException(httpResult.StatusDescription) {
                            ResponseDto = httpResult.Response,
                            StatusCode = httpResult.Status,
                        };
                        throw webEx;
                    }
                    return (TResponse) httpResult.Response;
                }

                return (TResponse)response;
            }

            /// <summary>Send this message.</summary>
            ///
            /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
            ///
            /// <typeparam name="TResponse">Type of the response.</typeparam>
            /// <param name="request">The request.</param>
            ///
            /// <returns>A TResponse.</returns>
            public TResponse Send<TResponse>(IReturn<TResponse> request)
            {
                throw new NotImplementedException();
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

            /// <summary>Gets.</summary>
            ///
            /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
            ///
            /// <typeparam name="TResponse">Type of the response.</typeparam>
            /// <param name="request">The request.</param>
            ///
            /// <returns>A TResponse.</returns>
            public TResponse Get<TResponse>(IReturn<TResponse> request)
            {
                throw new NotImplementedException();
            }

            /// <summary>Gets the given request.</summary>
            ///
            /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
            ///
            /// <param name="request">The request.</param>
            public void Get(IReturnVoid request)
            {
                throw new NotImplementedException();
            }

            /// <summary>Gets.</summary>
            ///
            /// <typeparam name="TResponse">Type of the response.</typeparam>
            /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
            ///
            /// <returns>A TResponse.</returns>
            public TResponse Get<TResponse>(string relativeOrAbsoluteUrl)
            {
                return parent.ExecutePath<TResponse>(HttpMethods.Get, new UrlParts(relativeOrAbsoluteUrl), null);
            }

            /// <summary>Deletes the given relativeOrAbsoluteUrl.</summary>
            ///
            /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
            ///
            /// <typeparam name="TResponse">Type of the response.</typeparam>
            /// <param name="request">The request.</param>
            ///
            /// <returns>A TResponse.</returns>
            public TResponse Delete<TResponse>(IReturn<TResponse> request)
            {
                throw new NotImplementedException();
            }

            /// <summary>Deletes the given request.</summary>
            ///
            /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
            ///
            /// <param name="request">The request.</param>
            public void Delete(IReturnVoid request)
            {
                throw new NotImplementedException();
            }

            /// <summary>Deletes the given relativeOrAbsoluteUrl.</summary>
            ///
            /// <typeparam name="TResponse">Type of the response.</typeparam>
            /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
            ///
            /// <returns>A TResponse.</returns>
            public TResponse Delete<TResponse>(string relativeOrAbsoluteUrl)
            {
                return parent.ExecutePath<TResponse>(HttpMethods.Delete, new UrlParts(relativeOrAbsoluteUrl), null);
            }

            /// <summary>Post this message.</summary>
            ///
            /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
            ///
            /// <typeparam name="TResponse">Type of the response.</typeparam>
            /// <param name="request">The request.</param>
            ///
            /// <returns>A TResponse.</returns>
            public TResponse Post<TResponse>(IReturn<TResponse> request)
            {
                throw new NotImplementedException();
            }

            /// <summary>Post this message.</summary>
            ///
            /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
            ///
            /// <param name="request">The request.</param>
            public void Post(IReturnVoid request)
            {
                throw new NotImplementedException();
            }

            /// <summary>Post this message.</summary>
            ///
            /// <typeparam name="TResponse">Type of the response.</typeparam>
            /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
            /// <param name="request">              The request.</param>
            ///
            /// <returns>A TResponse.</returns>
            public TResponse Post<TResponse>(string relativeOrAbsoluteUrl, object request)
            {
                return parent.ExecutePath<TResponse>(HttpMethods.Post, new UrlParts(relativeOrAbsoluteUrl), request);
            }

            /// <summary>Puts.</summary>
            ///
            /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
            ///
            /// <typeparam name="TResponse">Type of the response.</typeparam>
            /// <param name="request">The request.</param>
            ///
            /// <returns>A TResponse.</returns>
            public TResponse Put<TResponse>(IReturn<TResponse> request)
            {
                throw new NotImplementedException();
            }

            /// <summary>Puts the given request.</summary>
            ///
            /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
            ///
            /// <param name="request">The request.</param>
            public void Put(IReturnVoid request)
            {
                throw new NotImplementedException();
            }

            /// <summary>Puts.</summary>
            ///
            /// <typeparam name="TResponse">Type of the response.</typeparam>
            /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
            /// <param name="request">              The request.</param>
            ///
            /// <returns>A TResponse.</returns>
            public TResponse Put<TResponse>(string relativeOrAbsoluteUrl, object request)
            {
                return parent.ExecutePath<TResponse>(HttpMethods.Put, new UrlParts(relativeOrAbsoluteUrl), request);
            }

            /// <summary>Patches.</summary>
            ///
            /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
            ///
            /// <typeparam name="TResponse">Type of the response.</typeparam>
            /// <param name="request">The request.</param>
            ///
            /// <returns>A TResponse.</returns>
            public TResponse Patch<TResponse>(IReturn<TResponse> request)
            {
                throw new NotImplementedException();
            }

            /// <summary>Patches the given request.</summary>
            ///
            /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
            ///
            /// <param name="request">The request.</param>
            public void Patch(IReturnVoid request)
            {
                throw new NotImplementedException();
            }

            /// <summary>Patches.</summary>
            ///
            /// <typeparam name="TResponse">Type of the response.</typeparam>
            /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
            /// <param name="request">              The request.</param>
            ///
            /// <returns>A TResponse.</returns>
            public TResponse Patch<TResponse>(string relativeOrAbsoluteUrl, object request)
            {
                return parent.ExecutePath<TResponse>(HttpMethods.Patch, new UrlParts(relativeOrAbsoluteUrl), request);
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

            /// <summary>Custom method.</summary>
            ///
            /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
            ///
            /// <param name="httpVerb">The HTTP verb.</param>
            /// <param name="request"> The request.</param>
            public void CustomMethod(string httpVerb, IReturnVoid request)
            {
                throw new NotImplementedException();
            }

            /// <summary>Custom method.</summary>
            ///
            /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
            ///
            /// <typeparam name="TResponse">Type of the response.</typeparam>
            /// <param name="httpVerb">The HTTP verb.</param>
            /// <param name="request"> The request.</param>
            ///
            /// <returns>A TResponse.</returns>
            public TResponse CustomMethod<TResponse>(string httpVerb, IReturn<TResponse> request)
            {
                throw new NotImplementedException();
            }

            /// <summary>Heads.</summary>
            ///
            /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
            ///
            /// <param name="request">The request.</param>
            ///
            /// <returns>A HttpWebResponse.</returns>
            public HttpWebResponse Head(IReturn request)
            {
                throw new NotImplementedException();
            }

            /// <summary>Heads.</summary>
            ///
            /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
            ///
            /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
            ///
            /// <returns>A HttpWebResponse.</returns>
            public HttpWebResponse Head(string relativeOrAbsoluteUrl)
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

            /// <summary>Sends the asynchronous.</summary>
            ///
            /// <typeparam name="TResponse">Type of the response.</typeparam>
            /// <param name="request">  The request.</param>
            /// <param name="onSuccess">The on success.</param>
            /// <param name="onError">  The on error.</param>
            public void SendAsync<TResponse>(object request,
                Action<TResponse> onSuccess, Action<TResponse, Exception> onError)
            {
                try
                {
                    var response = (TResponse)ServiceManager.Execute(request);
                    onSuccess(response);
                }
                catch (Exception ex)
                {
                    HandleException(ex, onError);
                }
            }

            private static void HandleException<TResponse>(Exception exception, Action<TResponse, Exception> onError)
            {
                var response = (TResponse)typeof(TResponse).CreateInstance();
                var hasResponseStatus = response as IHasResponseStatus;
                if (hasResponseStatus != null)
                {
                    hasResponseStatus.ResponseStatus = new ResponseStatus {
                        ErrorCode = exception.GetType().Name,
                        Message = exception.Message,
                        StackTrace = exception.StackTrace,
                    };
                }
                var webServiceEx = new WebServiceException(exception.Message, exception);
                if (onError != null) onError(response, webServiceEx);
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
            /// <typeparam name="TResponse">Type of the response.</typeparam>
            /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
            /// <param name="onSuccess">            The on success.</param>
            /// <param name="onError">              The on error.</param>
            public void GetAsync<TResponse>(string relativeOrAbsoluteUrl, Action<TResponse> onSuccess, Action<TResponse, Exception> onError)
            {
                try
                {
                    var response = parent.ExecutePath<TResponse>(HttpMethods.Get, new UrlParts(relativeOrAbsoluteUrl), default(TResponse));
                    onSuccess(response);
                }
                catch (Exception ex)
                {
                    HandleException(ex, onError);
                }
            }

            /// <summary>Deletes the asynchronous.</summary>
            ///
            /// <typeparam name="TResponse">Type of the response.</typeparam>
            /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
            /// <param name="onSuccess">            The on success.</param>
            /// <param name="onError">              The on error.</param>
            public void DeleteAsync<TResponse>(string relativeOrAbsoluteUrl, Action<TResponse> onSuccess, Action<TResponse, Exception> onError)
            {
                try
                {
                    var response = parent.ExecutePath<TResponse>(HttpMethods.Delete, new UrlParts(relativeOrAbsoluteUrl), default(TResponse));
                    onSuccess(response);
                }
                catch (Exception ex)
                {
                    HandleException(ex, onError);
                }
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
            /// <typeparam name="TResponse">Type of the response.</typeparam>
            /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
            /// <param name="request">              The request.</param>
            /// <param name="onSuccess">            The on success.</param>
            /// <param name="onError">              The on error.</param>
            public void PostAsync<TResponse>(string relativeOrAbsoluteUrl, object request, Action<TResponse> onSuccess, Action<TResponse, Exception> onError)
            {
                try
                {
                    var response = parent.ExecutePath<TResponse>(HttpMethods.Post, new UrlParts(relativeOrAbsoluteUrl), request);
                    onSuccess(response);
                }
                catch (Exception ex)
                {
                    HandleException(ex, onError);
                }
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
            /// <typeparam name="TResponse">Type of the response.</typeparam>
            /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
            /// <param name="request">              The request.</param>
            /// <param name="onSuccess">            The on success.</param>
            /// <param name="onError">              The on error.</param>
            public void PutAsync<TResponse>(string relativeOrAbsoluteUrl, object request, Action<TResponse> onSuccess, Action<TResponse, Exception> onError)
            {
                try
                {
                    var response = parent.ExecutePath<TResponse>(HttpMethods.Put, new UrlParts(relativeOrAbsoluteUrl), request);
                    onSuccess(response);
                }
                catch (Exception ex)
                {
                    HandleException(ex, onError);
                }
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
            public void Dispose() { }

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

        /// <summary>Executes the path operation.</summary>
        ///
        /// <param name="pathInfo">Information describing the path.</param>
        ///
        /// <returns>An object.</returns>
        public object ExecutePath(string pathInfo)
        {
            return ExecutePath(HttpMethods.Get, pathInfo);
        }

        private class UrlParts
        {
            /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.Testing.TestBase.UrlParts class.</summary>
            ///
            /// <param name="pathInfo">Information describing the path.</param>
            public UrlParts(string pathInfo)
            {
                this.PathInfo = pathInfo.UrlDecode();
                var qsIndex = pathInfo.IndexOf("?");
                if (qsIndex != -1)
                {
                    var qs = pathInfo.Substring(qsIndex + 1);
                    this.PathInfo = pathInfo.Substring(0, qsIndex);
                    var kvps = qs.Split('&');

                    this.QueryString = new Dictionary<string, string>();
                    foreach (var kvp in kvps)
                    {
                        var parts = kvp.Split('=');
                        this.QueryString[parts[0]] = parts.Length > 1 ? parts[1] : null;
                    }
                }
            }

            /// <summary>Gets information describing the path.</summary>
            ///
            /// <value>Information describing the path.</value>
            public string PathInfo { get; private set; }

            /// <summary>Gets the query string.</summary>
            ///
            /// <value>The query string.</value>
            public Dictionary<string, string> QueryString { get; private set; }
        }

        /// <summary>Executes the path operation.</summary>
        ///
        /// <param name="httpMethod">The HTTP method.</param>
        /// <param name="pathInfo">  Information describing the path.</param>
        ///
        /// <returns>An object.</returns>
        public object ExecutePath(string httpMethod, string pathInfo)
        {
            var urlParts = new UrlParts(pathInfo);
            return ExecutePath(httpMethod, urlParts.PathInfo, urlParts.QueryString, null, null);
        }

        private TResponse ExecutePath<TResponse>(string httpMethod, UrlParts urlParts, object requestDto)
        {
            return (TResponse)ExecutePath(httpMethod, urlParts.PathInfo, urlParts.QueryString, null, requestDto);
        }

        /// <summary>Executes the path operation.</summary>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <param name="pathInfo">  Information describing the path.</param>
        /// <param name="requestDto">The request dto.</param>
        ///
        /// <returns>An object.</returns>
        public TResponse ExecutePath<TResponse>(string httpMethod, string pathInfo, object requestDto)
        {
            var urlParts = new UrlParts(pathInfo);
            return (TResponse)ExecutePath(httpMethod, urlParts.PathInfo, urlParts.QueryString, null, requestDto);
        }

        /// <summary>Executes the path operation.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="httpMethod"> The HTTP method.</param>
        /// <param name="pathInfo">   Information describing the path.</param>
        /// <param name="queryString">The query string.</param>
        /// <param name="formData">   Information describing the form.</param>
        /// <param name="requestBody">The request body.</param>
        ///
        /// <returns>An object.</returns>
        public object ExecutePath<T>(
            string httpMethod,
            string pathInfo,
            Dictionary<string, string> queryString,
            Dictionary<string, string> formData,
            T requestBody)
        {
            var isDefault = Equals(requestBody, default(T));
            var json = !isDefault ? JsonSerializer.SerializeToString(requestBody) : null;
            return ExecutePath(httpMethod, pathInfo, queryString, formData, json);
        }

        /// <summary>Executes the path operation.</summary>
        ///
        /// <exception cref="WebServiceException">Thrown when a Web Service error condition occurs.</exception>
        ///
        /// <param name="httpMethod"> The HTTP method.</param>
        /// <param name="pathInfo">   Information describing the path.</param>
        /// <param name="queryString">The query string.</param>
        /// <param name="formData">   Information describing the form.</param>
        /// <param name="requestBody">The request body.</param>
        ///
        /// <returns>An object.</returns>
        public object ExecutePath(
            string httpMethod,
            string pathInfo,
            Dictionary<string, string> queryString,
            Dictionary<string, string> formData,
            string requestBody)
        {
            var httpHandler = GetHandler(httpMethod, pathInfo);

            var contentType = (formData != null && formData.Count > 0)
                ? ContentType.FormUrlEncoded
                : requestBody != null ? ContentType.Json : null;

            var httpReq = new MockHttpRequest(
                    httpHandler.RequestName, httpMethod, contentType,
                    pathInfo,
                    queryString.ToNameValueCollection(),
                    requestBody == null ? null : new MemoryStream(Encoding.UTF8.GetBytes(requestBody)),
                    formData.ToNameValueCollection()
                );

            var request = httpHandler.CreateRequest(httpReq, httpHandler.RequestName);
            object response;
            try
            {
                response = httpHandler.GetResponse(httpReq, null, request);
            }
            catch (Exception ex)
            {
                response = DtoUtils.HandleException(AppHost, request, ex);
            }

            var httpRes = response as IHttpResult;
            if (httpRes != null)
            {
                var httpError = httpRes as IHttpError;
                if (httpError != null)
                {
                    throw new WebServiceException(httpError.Message) {
                        StatusCode = httpError.Status,
                        ResponseDto = httpError.Response
                    };
                }
                var hasResponseStatus = httpRes.Response as IHasResponseStatus;
                if (hasResponseStatus != null)
                {
                    var status = hasResponseStatus.ResponseStatus;
                    if (status != null && !status.ErrorCode.IsNullOrEmpty())
                    {
                        throw new WebServiceException(status.Message) {
                            StatusCode = (int)HttpStatusCode.InternalServerError,
                            ResponseDto = httpRes.Response,
                        };
                    }
                }

                return httpRes.Response;
            }

            return response;
        }

        /// <summary>Gets a request.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="pathInfo">Information describing the path.</param>
        ///
        /// <returns>The request.</returns>
        public T GetRequest<T>(string pathInfo)
        {
            return (T) GetRequest(pathInfo);
        }

        /// <summary>Gets a request.</summary>
        ///
        /// <param name="pathInfo">Information describing the path.</param>
        ///
        /// <returns>The request.</returns>
        public object GetRequest(string pathInfo)
        {
            var urlParts = new UrlParts(pathInfo);
            return GetRequest(HttpMethods.Get, urlParts.PathInfo, urlParts.QueryString, null, null);
        }

        /// <summary>Gets a request.</summary>
        ///
        /// <param name="httpMethod">The HTTP method.</param>
        /// <param name="pathInfo">  Information describing the path.</param>
        ///
        /// <returns>The request.</returns>
        public object GetRequest(string httpMethod, string pathInfo)
        {
            var urlParts = new UrlParts(pathInfo);
            return GetRequest(httpMethod, urlParts.PathInfo, urlParts.QueryString, null, null);
        }

        /// <summary>Gets a request.</summary>
        ///
        /// <param name="httpMethod"> The HTTP method.</param>
        /// <param name="pathInfo">   Information describing the path.</param>
        /// <param name="queryString">The query string.</param>
        /// <param name="formData">   Information describing the form.</param>
        /// <param name="requestBody">The request body.</param>
        ///
        /// <returns>The request.</returns>
        public object GetRequest(
                string httpMethod,
                string pathInfo,
                Dictionary<string, string> queryString,
                Dictionary<string, string> formData,
                string requestBody)
        {
            var httpHandler = GetHandler(httpMethod, pathInfo);

            var contentType = (formData != null && formData.Count > 0)
                ? ContentType.FormUrlEncoded
                : requestBody != null ? ContentType.Json : null;

            var httpReq = new MockHttpRequest(
                    httpHandler.RequestName, httpMethod, contentType,
                    pathInfo,
                    queryString.ToNameValueCollection(),
                    requestBody == null ? null : new MemoryStream(Encoding.UTF8.GetBytes(requestBody)),
                    formData.ToNameValueCollection()
                );

            var request = httpHandler.CreateRequest(httpReq, httpHandler.RequestName);
            return request;
        }

        private static EndpointHandlerBase GetHandler(string httpMethod, string pathInfo)
        {
            var httpHandler = NServiceKitHttpHandlerFactory.GetHandlerForPathInfo(httpMethod, pathInfo, pathInfo, null) as EndpointHandlerBase;
            if (httpHandler == null)
                throw new NotSupportedException(pathInfo);
            return httpHandler;
        }
    }

}