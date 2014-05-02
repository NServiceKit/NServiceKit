using System;
using System.IO;
using System.Threading;
using NServiceKit.Common.Web;
using NServiceKit.Service;
using NServiceKit.ServiceHost;
using NServiceKit.Text;

namespace NServiceKit.ServiceClient.Web
{
    /// <summary>A JSON rest client asynchronous.</summary>
    [Obsolete("Use JsonServiceClient")]
    public class JsonRestClientAsync 
        : IRestClientAsync
    {
        /// <summary>Type of the content.</summary>
        public const string ContentType = "application/json";

        /// <summary>Initializes a new instance of the NServiceKit.ServiceClient.Web.JsonRestClientAsync class.</summary>
        ///
        /// <param name="baseUri">The base URI.</param>
        public JsonRestClientAsync(string baseUri)
            : this()
        {
            this.BaseUri = baseUri.WithTrailingSlash();
        }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceClient.Web.JsonRestClientAsync class.</summary>
        public JsonRestClientAsync()
        {
            this.client = new AsyncServiceClient {
                ContentType = ContentType,
                StreamSerializer = SerializeToStream,
                StreamDeserializer = JsonSerializer.DeserializeFromStream
            };
        }

        /// <summary>Gets or sets the timeout.</summary>
        ///
        /// <value>The timeout.</value>
        public TimeSpan? Timeout
        {
            get { return this.client.Timeout; }
            set { this.client.Timeout = value; }
        }

        private static void SerializeToStream(IRequestContext requestContext, object dto, Stream stream)
        {
            JsonSerializer.SerializeToStream(dto, stream);
        }

        private readonly AsyncServiceClient client;

        /// <summary>Gets or sets URI of the base.</summary>
        ///
        /// <value>The base URI.</value>
        public string BaseUri { get; set; }

        /// <summary>Set the credentials to be used when making requests.</summary>
        ///
        /// <param name="userName">.</param>
        /// <param name="password">.</param>
        public void SetCredentials(string userName, string password)
        {
            this.client.SetCredentials(userName, password);
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

        private string GetUrl(string relativeOrAbsoluteUrl)
        {
            return relativeOrAbsoluteUrl.StartsWith("http:")
                || relativeOrAbsoluteUrl.StartsWith("https:")
                     ? relativeOrAbsoluteUrl
                     : this.BaseUri + relativeOrAbsoluteUrl;
        }

        /// <summary>Gets the asynchronous.</summary>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
        /// <param name="onSuccess">            The on success.</param>
        /// <param name="onError">              The on error.</param>
        public void GetAsync<TResponse>(string relativeOrAbsoluteUrl, Action<TResponse> onSuccess, Action<TResponse, Exception> onError)
        {
            this.client.SendAsync(HttpMethods.Get, GetUrl(relativeOrAbsoluteUrl), null, onSuccess, onError);
        }

        /// <summary>Deletes the asynchronous.</summary>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
        /// <param name="onSuccess">            The on success.</param>
        /// <param name="onError">              The on error.</param>
        public void DeleteAsync<TResponse>(string relativeOrAbsoluteUrl, Action<TResponse> onSuccess, Action<TResponse, Exception> onError)
        {
            this.client.SendAsync(HttpMethods.Delete, GetUrl(relativeOrAbsoluteUrl), null, onSuccess, onError);
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
            this.client.SendAsync(HttpMethods.Post, GetUrl(relativeOrAbsoluteUrl), request, onSuccess, onError);
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
            this.client.SendAsync(HttpMethods.Put, GetUrl(relativeOrAbsoluteUrl), request, onSuccess, onError);
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

        /// <summary>Cancels pending async operations.</summary>
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
    }
}