using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Funq;
using NUnit.Framework;
using NServiceKit.Common.Extensions;
using NServiceKit.Common.Web;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints.Tests.Support.Services;

namespace NServiceKit.WebHost.Endpoints.Tests
{
    /// <summary>A request context tests.</summary>
	[TestFixture]
	public class RequestContextTests
	{
		private const string ListeningOn = "http://localhost:82/";

        /// <summary>The headers application host HTTP listener.</summary>
		public class HeadersAppHostHttpListener
			: AppHostHttpListenerBase
		{
            /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.RequestContextTests.HeadersAppHostHttpListener class.</summary>
			public HeadersAppHostHttpListener()
				: base("Request Filters Tests", typeof(HeadersService).Assembly) { }

            /// <summary>Configures the given container.</summary>
            ///
            /// <param name="container">The container.</param>
			public override void Configure(Container container)
			{
				EndpointHostConfig.Instance.GlobalResponseHeaders.Clear();

				//Signal advanced web browsers what HTTP Methods you accept
				base.SetConfig(new EndpointHostConfig
				{
					GlobalResponseHeaders =
					{
						{ "Access-Control-Allow-Origin", "*" },
						{ "Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS" },
					},
				});

				this.RequestFilters.Add((req, res, dto) =>
				{
					var requestFilter = dto as RequestFilter;
					if (requestFilter != null)
					{
						res.StatusCode = requestFilter.StatusCode;
						if (!requestFilter.HeaderName.IsNullOrEmpty())
						{
							res.AddHeader(requestFilter.HeaderName, requestFilter.HeaderValue);
						}
					}
				});
			}
		}

		HeadersAppHostHttpListener appHost;

        /// <summary>Executes the test fixture set up action.</summary>
		[TestFixtureSetUp]
		public void OnTestFixtureSetUp()
		{
			appHost = new HeadersAppHostHttpListener();
			appHost.Init();
			appHost.Start(ListeningOn);
		}

        /// <summary>Executes the test fixture tear down action.</summary>
		[TestFixtureTearDown]
		public void OnTestFixtureTearDown()
		{
			appHost.Dispose();
		}

		static string GetReceivedHeaderValue(string headerName)
		{
			var webRequest = (HttpWebRequest)WebRequest.Create(
				ListeningOn + "json/syncreply/Headers?Name=" + headerName.UrlEncode());

			var json = new StreamReader(webRequest.GetResponse().GetResponseStream()).ReadToEnd();
			Console.WriteLine(json);

			var response = JsonSerializer.DeserializeFromString<HeadersResponse>(json);

			return response.Value;
		}

        /// <summary>Gets response headers.</summary>
        ///
        /// <param name="url">URL of the document.</param>
        ///
        /// <returns>The response headers.</returns>
		public static Dictionary<string, string> GetResponseHeaders(String url)
		{
			var webRequest = (HttpWebRequest)WebRequest.Create(url);

			var webResponse = webRequest.GetResponse();

			var map = new Dictionary<string, string>();
			for (var i = 0; i < webResponse.Headers.Count; i++)
			{
				var header = webResponse.Headers.Keys[i];
				map[header] = webResponse.Headers[header];
			}

			return map;
		}

        /// <summary>Can resolve custom header.</summary>
		[Test]
		public void Can_resolve_CustomHeader()
		{
			var webRequest = (HttpWebRequest)WebRequest.Create(
				ListeningOn + "json/syncreply/Headers?Name=X-CustomHeader");
			webRequest.Headers["X-CustomHeader"] = "CustomValue";

			var response = JsonSerializer.DeserializeFromStream<HeadersResponse>(
				webRequest.GetResponse().GetResponseStream());

			Assert.That(response.Value, Is.EqualTo("CustomValue"));
		}

        /// <summary>Does send global headers.</summary>
		[Test]
		public void Does_Send_Global_Headers()
		{
            var headers = GetResponseHeaders(ListeningOn + "json/syncreply/Headers");
			Assert.That(headers["Access-Control-Allow-Origin"], Is.EqualTo("*"));
			Assert.That(headers["Access-Control-Allow-Methods"], Is.EqualTo("GET, POST, PUT, DELETE, OPTIONS"));
		}

        /// <summary>Does return bare 401 status code.</summary>
        ///
        /// <exception cref="401">Thrown when a 401 error condition occurs.</exception>
		[Test]
		public void Does_return_bare_401_StatusCode()
		{
			try
			{
				var webRequest = (HttpWebRequest)WebRequest.Create(
					ListeningOn + "json/syncreply/RequestFilter?StatusCode=401");

				webRequest.GetResponse();

				Assert.Fail("Should throw 401 WebException");
			}
			catch (WebException ex)
			{
				var httpResponse = (HttpWebResponse)ex.Response;
				Assert.That(httpResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
			}
		}

        /// <summary>Does return bare 401 with authentication required header.</summary>
        ///
        /// <exception cref="401">Thrown when a 401 error condition occurs.</exception>
		[Test]
		public void Does_return_bare_401_with_AuthRequired_header()
		{
			try
			{
				var webRequest = (HttpWebRequest)WebRequest.Create(ListeningOn 
					+ "json/syncreply/RequestFilter?StatusCode=401"
					+ "&HeaderName=" + HttpHeaders.WwwAuthenticate
					+ "&HeaderValue=" + "Basic realm=\"Auth Required\"".UrlEncode());

				webRequest.GetResponse();

				Assert.Fail("Should throw 401 WebException");
			}
			catch (WebException ex)
			{
				var httpResponse = (HttpWebResponse)ex.Response;
				Assert.That(httpResponse.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

				Assert.That(ex.Response.Headers[HttpHeaders.WwwAuthenticate],
					Is.EqualTo("Basic realm=\"Auth Required\""));
			}
		}


	}
}