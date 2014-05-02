using System;
using System.IO;
using System.Net;
using NServiceKit.Common.Web;
using NServiceKit.Service;
using NServiceKit.ServiceClient.Web;
using NServiceKit.ServiceHost;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints.Support.Mocks;
using NServiceKit.WebHost.Endpoints.Tests.Mocks;

namespace NServiceKit.WebHost.Endpoints.Tests.Support
{
    /// <summary>A direct service client.</summary>
	public class DirectServiceClient : IServiceClient, IRestClient
	{
		ServiceManager ServiceManager { get; set; }

		readonly HttpRequestMock httpReq = new HttpRequestMock();
		readonly HttpResponseMock httpRes = new HttpResponseMock();

        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.Support.DirectServiceClient class.</summary>
        ///
        /// <param name="serviceManager">Manager for service.</param>
		public DirectServiceClient(ServiceManager serviceManager)
		{
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

		private bool ApplyRequestFilters<TResponse>(object request)
		{
			if (EndpointHost.ApplyRequestFilters(httpReq, httpRes, request))
			{
				ThrowIfError<TResponse>(httpRes);
				return true;
			}
			return false;
		}

		private void ThrowIfError<TResponse>(HttpResponseMock httpRes)
		{
			if (httpRes.StatusCode >= 400)
			{
				var webEx = new WebServiceException("WebServiceException, StatusCode: " + httpRes.StatusCode) {
					StatusCode = httpRes.StatusCode,
					StatusDescription = httpRes.StatusDescription,
				};

				try
				{
					var deserializer = EndpointHost.AppHost.ContentTypeFilters.GetStreamDeserializer(httpReq.ResponseContentType);
					webEx.ResponseDto = deserializer(typeof(TResponse), httpRes.OutputStream);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
				}

				throw webEx;
			}
		}

		private bool ApplyResponseFilters<TResponse>(object response)
		{
			if (EndpointHost.ApplyResponseFilters(httpReq, httpRes, response))
			{
				ThrowIfError<TResponse>(httpRes);
				return true;
			}
			return false;
		}

        /// <summary>Send this message.</summary>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="request">The request.</param>
        ///
        /// <returns>A TResponse.</returns>
		public TResponse Send<TResponse>(object request)
		{
			httpReq.HttpMethod = HttpMethods.Post;			

			if (ApplyRequestFilters<TResponse>(request)) return default(TResponse);

			var response = ServiceManager.ServiceController.Execute(request,
				new HttpRequestContext(httpReq, httpRes, request, EndpointAttributes.HttpPost));

			if (ApplyResponseFilters<TResponse>(response)) return (TResponse)response;

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
        /// <param name="request">The request to get.</param>
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
        /// <param name="request">The request to get.</param>
	    public void Get(IReturnVoid request)
	    {
	        throw new NotImplementedException();
	    }

        /// <summary>Gets.</summary>
        ///
        /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or illegal values.</exception>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
        ///
        /// <returns>A TResponse.</returns>
	    public TResponse Get<TResponse>(string relativeOrAbsoluteUrl)
		{
			httpReq.HttpMethod = HttpMethods.Get;

			var requestTypeName = typeof(TResponse).Namespace + "." + relativeOrAbsoluteUrl;
			var requestType = typeof (TResponse).Assembly.GetType(requestTypeName);
			if (requestType == null)
				throw new ArgumentException("Type not found: " + requestTypeName);

			var request = requestType.CreateInstance();

			if (ApplyRequestFilters<TResponse>(request)) return default(TResponse);

			var response = ServiceManager.ServiceController.Execute(request,
				new HttpRequestContext(httpReq, httpRes, request, EndpointAttributes.HttpGet));

			if (ApplyResponseFilters<TResponse>(response)) return (TResponse)response;

			return (TResponse)response;
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
        /// <param name="request">The request to delete.</param>
	    public void Delete(IReturnVoid request)
	    {
	        throw new NotImplementedException();
	    }

        /// <summary>Deletes the given relativeOrAbsoluteUrl.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
        ///
        /// <returns>A TResponse.</returns>
	    public TResponse Delete<TResponse>(string relativeOrAbsoluteUrl)
		{
			throw new NotImplementedException();
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
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
        /// <param name="request">              The request.</param>
        ///
        /// <returns>A TResponse.</returns>
	    public TResponse Post<TResponse>(string relativeOrAbsoluteUrl, object request)
		{
			throw new NotImplementedException();
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
        /// <param name="request">The request to put.</param>
	    public void Put(IReturnVoid request)
	    {
	        throw new NotImplementedException();
	    }

        /// <summary>Puts.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
        /// <param name="request">              The request.</param>
        ///
        /// <returns>A TResponse.</returns>
	    public TResponse Put<TResponse>(string relativeOrAbsoluteUrl, object request)
		{
			throw new NotImplementedException();
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
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="relativeOrAbsoluteUrl">URL of the relative or absolute.</param>
        /// <param name="request">              The request.</param>
        ///
        /// <returns>A TResponse.</returns>
	    public TResponse Patch<TResponse>(string relativeOrAbsoluteUrl, object request)
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

        /// <summary>Heads the given request.</summary>
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
        /// <param name="fileInfo">             Information describing the file.</param>
        /// <param name="mimeType">             Type of the mime.</param>
        ///
        /// <returns>A TResponse.</returns>
	    public TResponse PostFile<TResponse>(string relativeOrAbsoluteUrl, Stream fileToUpload, string fileInfo, string mimeType)
        {
            throw new NotImplementedException();
        }

        /// <summary>Sends the asynchronous.</summary>
        ///
        /// <typeparam name="TResponse">Type of the response.</typeparam>
        /// <param name="request">  The request.</param>
        /// <param name="onSuccess">The on success.</param>
        /// <param name="onError">  The on error.</param>
		public void SendAsync<TResponse>(object request, Action<TResponse> onSuccess, Action<TResponse, Exception> onError)
		{
			var response = default(TResponse);
			try
			{
				try
				{
					if (ApplyRequestFilters<TResponse>(request))
					{
						onSuccess(default(TResponse));
						return;
					}
				}
				catch (Exception ex)
				{
					onError(default(TResponse), ex);
					return;
				}

				response = this.Send<TResponse>(request);

				try
				{
					if (ApplyResponseFilters<TResponse>(request))
					{
						onSuccess(response);
						return;
					}
				}
				catch (Exception ex)
				{
					onError(response, ex);
					return;
				}

				onSuccess(response);
			}
			catch (Exception ex)
			{
				if (onError != null)
				{
					onError(response, ex);
					return;
				}
				Console.WriteLine("Error: " + ex.Message);
			}
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
}