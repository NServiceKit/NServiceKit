using System;
using System.IO;
using System.Net;
using NUnit.Framework;
using NServiceKit.Common.Web;
using NServiceKit.Logging;
using NServiceKit.ServiceInterface.ServiceModel;
using NServiceKit.ServiceInterface.Testing;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints;
using NServiceKit.WebHost.IntegrationTests.Services;

namespace NServiceKit.WebHost.IntegrationTests.Tests
{
    /// <summary>The rests test base.</summary>
	public class RestsTestBase
		: TestBase
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof (RestsTestBase));

		readonly EndpointHostConfig defaultConfig = new EndpointHostConfig();

        /// <summary>Initializes a new instance of the NServiceKit.WebHost.IntegrationTests.Tests.RestsTestBase class.</summary>
		public RestsTestBase()
			: base(Config.NServiceKitBaseUri, typeof(HelloService).Assembly)
			//: base("http://localhost:4000", typeof(HelloService).Assembly) //Uncomment to test on dev web server
		{
		}

        /// <summary>Configures the given container.</summary>
        ///
        /// <param name="container">The container.</param>
		protected override void Configure(Funq.Container container) { }

        /// <summary>Gets web response.</summary>
        ///
        /// <param name="uri">               URI of the document.</param>
        /// <param name="acceptContentTypes">List of types of the accept contents.</param>
        ///
        /// <returns>The web response.</returns>
		public HttpWebResponse GetWebResponse(string uri, string acceptContentTypes)
		{
			var webRequest = (HttpWebRequest)WebRequest.Create(uri);
			webRequest.Accept = acceptContentTypes;
			return (HttpWebResponse)webRequest.GetResponse();
		}

        /// <summary>Gets web response.</summary>
        ///
        /// <param name="httpMethod">   The HTTP method.</param>
        /// <param name="uri">          URI of the document.</param>
        /// <param name="contentType">  Type of the content.</param>
        /// <param name="contentLength">Length of the content.</param>
        ///
        /// <returns>The web response.</returns>
		public static HttpWebResponse GetWebResponse(string httpMethod, string uri, string contentType, int contentLength)
		{
			var webRequest = (HttpWebRequest)WebRequest.Create(uri);
			webRequest.Accept = contentType;
			webRequest.ContentType = contentType;
			webRequest.Method = HttpMethods.Post;
			webRequest.ContentLength = contentLength;
			return (HttpWebResponse)webRequest.GetResponse();
		}

        /// <summary>Gets the contents.</summary>
        ///
        /// <param name="webResponse">The web response.</param>
        ///
        /// <returns>The contents.</returns>
		public static string GetContents(WebResponse webResponse)
		{
			using (var stream = webResponse.GetResponseStream())
			{
				var contents = new StreamReader(stream).ReadToEnd();
				return contents;
			}
		}

        /// <summary>Deserialize contents.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="webResponse">The web response.</param>
        ///
        /// <returns>A T.</returns>
		public T DeserializeContents<T>(WebResponse webResponse)
		{
			var contentType = webResponse.ContentType ?? defaultConfig.DefaultContentType;
			return DeserializeContents<T>(webResponse, contentType);
		}

		private static T DeserializeContents<T>(WebResponse webResponse, string contentType)
		{
			try
			{
				var contents = GetContents(webResponse);
				var result = DeserializeResult<T>(webResponse, contents, contentType);
				return result;
			}
			catch (WebException webEx)
			{
				if (webEx.Status == WebExceptionStatus.ProtocolError)
				{
					var errorResponse = ((HttpWebResponse)webEx.Response);
					Log.Error(webEx);
					Log.DebugFormat("Status Code : {0}", errorResponse.StatusCode);
					Log.DebugFormat("Status Description : {0}", errorResponse.StatusDescription);

					try
					{
						using (var stream = errorResponse.GetResponseStream())
						{
							var response = HttpResponseFilter.Instance.DeserializeFromStream(contentType, typeof(T), stream);
							return (T)response;
						}
					}
					catch (WebException)
					{
						// Oh, well, we tried
						throw;
					}
				}

				throw;
			}
		}

        /// <summary>Assert response.</summary>
        ///
        /// <param name="response">   The response.</param>
        /// <param name="contentType">Type of the content.</param>
		public void AssertResponse(HttpWebResponse response, string contentType)
		{
			var statusCode = (int)response.StatusCode;
			Assert.That(statusCode, Is.LessThan(400));
			Assert.That(response.ContentType.StartsWith(contentType));
		}

        /// <summary>Assert error response.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="webResponse">     The web response.</param>
        /// <param name="statusCode">      The status code.</param>
        /// <param name="responseStatusFn">The response status function.</param>
		public void AssertErrorResponse<T>(HttpWebResponse webResponse, HttpStatusCode statusCode, Func<T, ResponseStatus> responseStatusFn)
		{
			Assert.That(webResponse.StatusCode, Is.EqualTo(statusCode));
			var response = DeserializeContents<T>(webResponse);
			Assert.That(responseStatusFn(response).ErrorCode, Is.Not.Null);
		}

        /// <summary>Assert error response.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="webResponse">     The web response.</param>
        /// <param name="statusCode">      The status code.</param>
        /// <param name="responseStatusFn">The response status function.</param>
        /// <param name="errorCode">       The error code.</param>
		public void AssertErrorResponse<T>(HttpWebResponse webResponse, HttpStatusCode statusCode, Func<T, ResponseStatus> responseStatusFn, string errorCode)
		{
			Assert.That(webResponse.StatusCode, Is.EqualTo(statusCode));
			var response = DeserializeContents<T>(webResponse);
			Assert.That(responseStatusFn(response).ErrorCode, Is.EqualTo(errorCode));
		}

        /// <summary>Assert error response.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="webResponse">The web response.</param>
        /// <param name="statusCode"> The status code.</param>
		public void AssertErrorResponse<T>(HttpWebResponse webResponse, HttpStatusCode statusCode)
			where T : IHasResponseStatus
		{
			Assert.That(webResponse.StatusCode, Is.EqualTo(statusCode));
			var response = DeserializeContents<T>(webResponse);
			Assert.That(response.ResponseStatus.ErrorCode, Is.Not.Null);
		}

        /// <summary>Assert error response.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="webResponse">The web response.</param>
        /// <param name="statusCode"> The status code.</param>
        /// <param name="errorCode">  The error code.</param>
		public void AssertErrorResponse<T>(HttpWebResponse webResponse, HttpStatusCode statusCode, string errorCode)
			where T : IHasResponseStatus
		{
			Assert.That(webResponse.StatusCode, Is.EqualTo(statusCode));
			var response = DeserializeContents<T>(webResponse);
			Assert.That(response.ResponseStatus.ErrorCode, Is.EqualTo(errorCode));
		}

        /// <summary>Assert response.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="response">    The response.</param>
        /// <param name="customAssert">The custom assert.</param>
		public void AssertResponse<T>(HttpWebResponse response, Action<T> customAssert)
		{
			var contentType = response.ContentType ?? defaultConfig.DefaultContentType;

			AssertResponse(response, contentType);

			var result = DeserializeContents<T>(response, contentType);

			customAssert(result);
		}

        /// <summary>Assert response.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="response">    The response.</param>
        /// <param name="contentType"> Type of the content.</param>
        /// <param name="customAssert">The custom assert.</param>
		public void AssertResponse<T>(HttpWebResponse response, string contentType, Action<T> customAssert)
		{
			contentType = contentType ?? defaultConfig.DefaultContentType;

			AssertResponse(response, contentType);

			var result = DeserializeContents<T>(response, contentType);

			customAssert(result);
		}

		private static T DeserializeResult<T>(WebResponse response, string contents, string contentType)
		{
			T result;
			switch (contentType)
			{
				case ContentType.Xml:
					result = XmlSerializer.DeserializeFromString<T>(contents);
					break;

				case ContentType.Json:
				case ContentType.Json + ContentType.Utf8Suffix:
					result = JsonSerializer.DeserializeFromString<T>(contents);
					break;

				case ContentType.Jsv:
					result = TypeSerializer.DeserializeFromString<T>(contents);
					break;

				default:
					throw new NotSupportedException(response.ContentType);
			}
			return result;
		}

	}
}