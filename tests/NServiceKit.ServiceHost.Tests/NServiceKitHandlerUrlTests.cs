using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using NUnit.Framework;
using NServiceKit.Text;

namespace NServiceKit.ServiceHost.Tests
{
    /// <summary>A service kit handler URL tests.</summary>
	[TestFixture]
	public class NServiceKitHandlerUrlTests
	{
        /// <summary>Resolve path.</summary>
        ///
        /// <param name="mode">The mode.</param>
        /// <param name="path">Full pathname of the file.</param>
        ///
        /// <returns>A string.</returns>
		public static string ResolvePath(string mode, string path)
		{
			return WebHost.Endpoints.Extensions.HttpRequestExtensions.
				GetPathInfo(path, mode, path.Split('/').First(x => x != ""));
		}

        /// <summary>A mock URL HTTP request.</summary>
		public class MockUrlHttpRequest : IHttpRequest
		{
            /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.Tests.NServiceKitHandlerUrlTests.MockUrlHttpRequest class.</summary>
			public MockUrlHttpRequest() { }

            /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.Tests.NServiceKitHandlerUrlTests.MockUrlHttpRequest class.</summary>
            ///
            /// <param name="mode">  The mode.</param>
            /// <param name="path">  Full pathname of the file.</param>
            /// <param name="rawUrl">The raw URL.</param>
			public MockUrlHttpRequest(string mode, string path, string rawUrl)
			{
				this.PathInfo = ResolvePath(mode, path);
				this.RawUrl = rawUrl;
				AbsoluteUri = "http://localhost" + rawUrl;
			}

            /// <summary>The underlying ASP.NET or HttpListener HttpRequest.</summary>
            ///
            /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
            ///
            /// <value>The original request.</value>
			public object OriginalRequest
			{
				get { throw new NotImplementedException(); }
			}

            /// <summary>Try resolve.</summary>
            ///
            /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
            ///
            /// <typeparam name="T">Generic type parameter.</typeparam>
            ///
            /// <returns>A T.</returns>
			public T TryResolve<T>()
			{
				throw new NotImplementedException();
			}

            /// <summary>The name of the service being called (e.g. Request DTO Name)</summary>
            ///
            /// <value>The name of the operation.</value>
			public string OperationName { get; set; }

            /// <summary>The request ContentType.</summary>
            ///
            /// <value>The type of the content.</value>
			public string ContentType { get; private set; }

            /// <summary>Gets the HTTP method.</summary>
            ///
            /// <value>The HTTP method.</value>
			public string HttpMethod { get; private set; }

            /// <summary>Gets or sets the user agent.</summary>
            ///
            /// <value>The user agent.</value>
			public string UserAgent { get; set; }

            /// <summary>Gets or sets a value indicating whether this object is local.</summary>
            ///
            /// <value>true if this object is local, false if not.</value>
            public bool IsLocal { get; set; }

            /// <summary>Gets the cookies.</summary>
            ///
            /// <value>The cookies.</value>
			public IDictionary<string, Cookie> Cookies { get; private set; }

            /// <summary>The expected Response ContentType for this request.</summary>
            ///
            /// <value>The type of the response content.</value>
			public string ResponseContentType { get; set; }

            /// <summary>Attach any data to this request that all filters and services can access.</summary>
            ///
            /// <value>The items.</value>
			public Dictionary<string, object> Items { get; private set; }

            /// <summary>Gets the headers.</summary>
            ///
            /// <value>The headers.</value>
			public NameValueCollection Headers { get; private set; }

            /// <summary>Gets the query string.</summary>
            ///
            /// <value>The query string.</value>
			public NameValueCollection QueryString { get; private set; }

            /// <summary>Gets information describing the form.</summary>
            ///
            /// <value>Information describing the form.</value>
			public NameValueCollection FormData { get; private set; }

            /// <summary>Buffer the Request InputStream so it can be re-read.</summary>
            ///
            /// <value>true if use buffered stream, false if not.</value>
		    public bool UseBufferedStream { get; set; }

            /// <summary>The entire string contents of Request.InputStream.</summary>
            ///
            /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
            ///
            /// <returns>The raw body.</returns>
		    public string GetRawBody()
			{
				throw new NotImplementedException();
			}

            /// <summary>Gets URL of the raw.</summary>
            ///
            /// <value>The raw URL.</value>
			public string RawUrl { get; private set; }

            /// <summary>Gets or sets URI of the absolute.</summary>
            ///
            /// <value>The absolute URI.</value>
			public string AbsoluteUri { get; set; }

            /// <summary>The Remote Ip as reported by Request.UserHostAddress.</summary>
            ///
            /// <value>The user host address.</value>
			public string UserHostAddress { get; private set; }

            /// <summary>The value of the Referrer, null if not available.</summary>
            ///
            /// <value>The URL referrer.</value>
            public Uri UrlReferrer { get; private set; }

            /// <summary>The Remote Ip as reported by X-Forwarded-For, X-Real-IP or Request.UserHostAddress.</summary>
            ///
            /// <value>The remote IP.</value>
            public string RemoteIp { get; set; }

            /// <summary>The value of the X-Forwarded-For header, null if null or empty.</summary>
            ///
            /// <value>The x coordinate forwarded for.</value>
            public string XForwardedFor { get; set; }

            /// <summary>The value of the X-Real-IP header, null if null or empty.</summary>
            ///
            /// <value>The x coordinate real IP.</value>
            public string XRealIp { get; set; }

            /// <summary>e.g. is https or not.</summary>
            ///
            /// <value>true if this object is secure connection, false if not.</value>
		    public bool IsSecureConnection { get; private set; }

            /// <summary>Gets a list of types of the accepts.</summary>
            ///
            /// <value>A list of types of the accepts.</value>
			public string[] AcceptTypes { get; private set; }

            /// <summary>Gets information describing the path.</summary>
            ///
            /// <value>Information describing the path.</value>
			public string PathInfo { get; private set; }

            /// <summary>Gets the input stream.</summary>
            ///
            /// <value>The input stream.</value>
			public Stream InputStream { get; private set; }

            /// <summary>Gets the length of the content.</summary>
            ///
            /// <value>The length of the content.</value>
			public long ContentLength { get; private set; }

            /// <summary>Access to the multi-part/formdata files posted on this request.</summary>
            ///
            /// <value>The files.</value>
			public IFile[] Files { get; private set; }

            /// <summary>Gets the full pathname of the application file.</summary>
            ///
            /// <value>The full pathname of the application file.</value>
			public string ApplicationFilePath { get; private set; }
		}

		readonly List<MockUrlHttpRequest> allResults = new List<MockUrlHttpRequest> {
			new MockUrlHttpRequest(null, "/handler.all35/json/metadata", "/handler.all35/json/metadata?op=Hello"),
			new MockUrlHttpRequest(null, "/handler.all35/json/metadata/", "/handler.all35/json/metadata/?op=Hello"),
		};

		readonly List<MockUrlHttpRequest> apiResults = new List<MockUrlHttpRequest> {
			new MockUrlHttpRequest(null, "/location.api.wildcard35/api/json/metadata", "/location.api.wildcard35/api/json/metadata?op=Hello"),
			new MockUrlHttpRequest(null, "/location.api.wildcard35/api/json/metadata/", "/location.api.wildcard35/api/json/metadata/?op=Hello"),
		};

		readonly List<MockUrlHttpRequest> NServiceKitsResults = new List<MockUrlHttpRequest> {
			new MockUrlHttpRequest(null, "/location.NServiceKit.wildcard35/NServiceKit/json/metadata", "/location.NServiceKit.wildcard35/NServiceKit/json/metadata?op=Hello"),
			new MockUrlHttpRequest(null, "/location.NServiceKit.wildcard35/NServiceKit/json/metadata/", "/location.NServiceKit.wildcard35/NServiceKit/json/metadata/?op=Hello"),
		};

        /// <summary>Does return expected absolute and path urls.</summary>
		[Test]
		public void Does_return_expected_absolute_and_path_urls()
		{
			var absolutePaths = allResults.ConvertAll(x => x.GetAbsolutePath());
			var pathUrls = allResults.ConvertAll(x => x.GetPathUrl());
			Assert.That(absolutePaths.All(x => x == "/handler.all35/json/metadata"));
			Assert.That(pathUrls.All(x => x == "http://localhost/handler.all35/json/metadata"));

			absolutePaths = apiResults.ConvertAll(x => x.GetAbsolutePath());
			pathUrls = apiResults.ConvertAll(x => x.GetPathUrl());
			Assert.That(absolutePaths.All(x => x == "/location.api.wildcard35/api/json/metadata"));
			Assert.That(pathUrls.All(x => x == "http://localhost/location.api.wildcard35/api/json/metadata"));

			absolutePaths = NServiceKitsResults.ConvertAll(x => x.GetAbsolutePath());
			pathUrls = NServiceKitsResults.ConvertAll(x => x.GetPathUrl());
			Assert.That(absolutePaths.All(x => x == "/location.NServiceKit.wildcard35/NServiceKit/json/metadata"));
			Assert.That(pathUrls.All(x => x == "http://localhost/location.NServiceKit.wildcard35/NServiceKit/json/metadata"));
		}

        /// <summary>Does return expected parent absolute and path urls.</summary>
		[Test]
		public void Does_return_expected_parent_absolute_and_path_urls()
		{
			var absolutePaths = allResults.ConvertAll(x => x.GetParentAbsolutePath());
			var pathUrls = allResults.ConvertAll(x => x.GetParentPathUrl());

			Assert.That(absolutePaths.All(x => x == "/handler.all35/json"));
			Assert.That(pathUrls.All(x => x == "http://localhost/handler.all35/json"));

			absolutePaths = apiResults.ConvertAll(x => x.GetParentAbsolutePath());
			pathUrls = apiResults.ConvertAll(x => x.GetParentPathUrl());
			Assert.That(absolutePaths.All(x => x == "/location.api.wildcard35/api/json"));
			Assert.That(pathUrls.All(x => x == "http://localhost/location.api.wildcard35/api/json"));

			absolutePaths = NServiceKitsResults.ConvertAll(x => x.GetParentAbsolutePath());
			pathUrls = NServiceKitsResults.ConvertAll(x => x.GetParentPathUrl());
			Assert.That(absolutePaths.All(x => x == "/location.NServiceKit.wildcard35/NServiceKit/json"));
			Assert.That(pathUrls.All(x => x == "http://localhost/location.NServiceKit.wildcard35/NServiceKit/json"));
		}

        /// <summary>Can get URL host name.</summary>
		[Test]
		public void Can_Get_UrlHostName()
		{
			var urls = new List<string> { "http://localhost/a", "https://localhost/a", 
				"http://localhost:81", "http://localhost:81/", "http://localhost" };

			var httpReqs = urls.ConvertAll(x => new MockUrlHttpRequest { AbsoluteUri = x });
			var hostNames = httpReqs.ConvertAll(x => x.GetUrlHostName());

			Console.WriteLine(hostNames.Dump());

			Assert.That(hostNames.All(x => x == "localhost"));
		}

	}
}