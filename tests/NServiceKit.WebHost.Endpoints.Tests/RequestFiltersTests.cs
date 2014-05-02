using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using Funq;
using NUnit.Framework;
using NServiceKit.Common.Web;
using NServiceKit.Service;
using NServiceKit.ServiceClient.Web;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface.ServiceModel;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints.Support;
using NServiceKit.WebHost.Endpoints.Tests.Support;
using NServiceKit.WebHost.Endpoints.Tests.Support.Host;

namespace NServiceKit.WebHost.Endpoints.Tests
{
    /// <summary>A secure.</summary>
	[DataContract]
	[Route("/secure")]
	public class Secure
	{
        /// <summary>Gets or sets the name of the user.</summary>
        ///
        /// <value>The name of the user.</value>
		[DataMember]
		public string UserName { get; set; }
	}

    /// <summary>A secure response.</summary>
	[DataContract]
	public class SecureResponse : IHasResponseStatus
	{
        /// <summary>Gets or sets the result.</summary>
        ///
        /// <value>The result.</value>
		[DataMember]
		public string Result { get; set; }

        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
		[DataMember]
		public ResponseStatus ResponseStatus { get; set; }
	}

    /// <summary>A secure service.</summary>
	public class SecureService : ServiceInterface.Service
	{
        /// <summary>Executes the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>A SecureResponse.</returns>
        public SecureResponse Execute(Secure request)
		{
			return new SecureResponse { Result = "Confidential" };
		}
	}

    /// <summary>An insecure.</summary>
	[DataContract]
	[Route("/insecure")]
	public class Insecure
	{
        /// <summary>Gets or sets the name of the user.</summary>
        ///
        /// <value>The name of the user.</value>
		[DataMember]
		public string UserName { get; set; }
	}

    /// <summary>An insecure response.</summary>
	[DataContract]
	public class InsecureResponse : IHasResponseStatus
	{
        /// <summary>Gets or sets the result.</summary>
        ///
        /// <value>The result.</value>
		[DataMember]
		public string Result { get; set; }

        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
		[DataMember]
		public ResponseStatus ResponseStatus { get; set; }
	}

    /// <summary>An insecure service.</summary>
	public class InsecureService : ServiceInterface.Service
	{
        /// <summary>Executes the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An InsecureResponse.</returns>
        public InsecureResponse Execute(Insecure request)
		{
			return new InsecureResponse { Result = "Public" };
		}
	}

    /// <summary>A request filters tests.</summary>
	[TestFixture]
	public abstract class RequestFiltersTests
	{
		private const string ListeningOn = "http://localhost:82/";
		private const string ServiceClientBaseUri = "http://localhost:82/";

		private const string AllowedUser = "user";
		private const string AllowedPass = "p@55word";

        /// <summary>A request filters application host HTTP listener.</summary>
		public class RequestFiltersAppHostHttpListener
			: AppHostHttpListenerBase
		{
			private Guid currentSessionGuid;

            /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.RequestFiltersTests.RequestFiltersAppHostHttpListener class.</summary>
			public RequestFiltersAppHostHttpListener()
				: base("Request Filters Tests", typeof(GetFactorialService).Assembly) { }

            /// <summary>Configures the given container.</summary>
            ///
            /// <param name="container">The container.</param>
			public override void Configure(Container container)
			{
				this.RequestFilters.Add((req, res, dto) =>
				{
					var userPass = req.GetBasicAuthUserAndPassword();
					if (userPass == null)
					{
						return;
					}

					var userName = userPass.Value.Key;
					if (userName == AllowedUser && userPass.Value.Value == AllowedPass)
					{
						currentSessionGuid = Guid.NewGuid();
						var sessionKey = userName + "/" + currentSessionGuid.ToString("N");

						//set session for this request (as no cookies will be set on this request)
						req.Items["ss-session"] = sessionKey;
						res.SetPermanentCookie("ss-session", sessionKey);
					}
				});
				this.RequestFilters.Add((req, res, dto) =>
				{
					if (dto is Secure)
					{
						var sessionId = req.GetItemOrCookie("ss-session") ?? string.Empty;
						var sessionIdParts = sessionId.SplitOnFirst('/');
						if (sessionIdParts.Length < 2 || sessionIdParts[0] != AllowedUser || sessionIdParts[1] != currentSessionGuid.ToString("N"))
						{
							res.ReturnAuthRequired();
						}
					}
				});
			}
		}

		RequestFiltersAppHostHttpListener appHost;

        /// <summary>Executes the test fixture set up action.</summary>
		[TestFixtureSetUp]
		public void OnTestFixtureSetUp()
		{
			appHost = new RequestFiltersAppHostHttpListener();
			appHost.Init();
			appHost.Start(ListeningOn);
		}

        /// <summary>Executes the test fixture tear down action.</summary>
		[TestFixtureTearDown]
		public void OnTestFixtureTearDown()
		{
			appHost.Dispose();
            EndpointHandlerBase.ServiceManager = null;
		}

        /// <summary>Creates new service client.</summary>
        ///
        /// <returns>The new new service client.</returns>
		protected abstract IServiceClient CreateNewServiceClient();

        /// <summary>Creates new rest client asynchronous.</summary>
        ///
        /// <returns>The new new rest client asynchronous.</returns>
		protected abstract IRestClientAsync CreateNewRestClientAsync();

        /// <summary>Gets the format.</summary>
        ///
        /// <returns>The format.</returns>
		protected virtual string GetFormat()
		{
			return null;
		}

		private static void Assert401(IServiceClient client, WebServiceException ex)
		{
			if (client is Soap11ServiceClient || client is Soap12ServiceClient)
			{
				if (ex.StatusCode != 401)
				{
					Console.WriteLine("WARNING: SOAP clients returning 500 instead of 401");
				}
				return;
			}

			Console.WriteLine(ex);
			Assert.That(ex.StatusCode, Is.EqualTo(401));
		}

		private static void FailOnAsyncError<T>(T response, Exception ex)
		{
			Assert.Fail(ex.Message);
		}

		private static bool Assert401(object response, Exception ex)
		{
			var webEx = (WebServiceException)ex;
			Assert.That(webEx.StatusCode, Is.EqualTo(401));
			return true;
		}

        /// <summary>Can login with basic authentication to access secure service.</summary>
		[Test]
		public void Can_login_with_Basic_auth_to_access_Secure_service()
		{
			var format = GetFormat();
			if (format == null) return;

			var req = (HttpWebRequest)WebRequest.Create(
				string.Format("http://localhost:82/{0}/syncreply/Secure", format));

			req.Headers[HttpHeaders.Authorization]
				= "basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(AllowedUser + ":" + AllowedPass));

			var dtoString = new StreamReader(req.GetResponse().GetResponseStream()).ReadToEnd();
			Assert.That(dtoString.Contains("Confidential"));
			Console.WriteLine(dtoString);
		}

        /// <summary>Can login with basic authentication to access secure service using service client.</summary>
		[Test]
		public void Can_login_with_Basic_auth_to_access_Secure_service_using_ServiceClient()
		{
			var format = GetFormat();
			if (format == null) return;

			var client = CreateNewServiceClient();
			client.SetCredentials(AllowedUser, AllowedPass);

			var response = client.Send<SecureResponse>(new Secure());

			Assert.That(response.Result, Is.EqualTo("Confidential"));
		}

        /// <summary>Can login with basic authentication to access secure service using rest client asynchronous.</summary>
		[Test]
		public void Can_login_with_Basic_auth_to_access_Secure_service_using_RestClientAsync()
		{
			var format = GetFormat();
			if (format == null) return;

			var client = CreateNewRestClientAsync();
			client.SetCredentials(AllowedUser, AllowedPass);

			SecureResponse response = null;
			client.GetAsync<SecureResponse>(ServiceClientBaseUri + "secure",
				r => response = r, FailOnAsyncError);

			Thread.Sleep(2000);
			Assert.That(response.Result, Is.EqualTo("Confidential"));
		}

        /// <summary>Can login without authorization to access insecure service.</summary>
		[Test]
		public void Can_login_without_authorization_to_access_Insecure_service()
		{
			var format = GetFormat();
			if (format == null) return;

			var req = (HttpWebRequest)WebRequest.Create(
				string.Format("{0}{1}/syncreply/Insecure", ServiceClientBaseUri, format));

			req.Headers[HttpHeaders.Authorization]
				= "basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(AllowedUser + ":" + AllowedPass));

			var dtoString = new StreamReader(req.GetResponse().GetResponseStream()).ReadToEnd();
			Assert.That(dtoString.Contains("Public"));
			Console.WriteLine(dtoString);
		}

        /// <summary>Can login without authorization to access insecure service using service client.</summary>
		[Test]
		public void Can_login_without_authorization_to_access_Insecure_service_using_ServiceClient()
		{
			var format = GetFormat();
			if (format == null) return;

			var client = CreateNewServiceClient();

			var response = client.Send<InsecureResponse>(new Insecure());

			Assert.That(response.Result, Is.EqualTo("Public"));
		}

        /// <summary>Can login without authorization to access insecure service using rest client asynchronous.</summary>
		[Test]
		public void Can_login_without_authorization_to_access_Insecure_service_using_RestClientAsync()
		{
			var format = GetFormat();
			if (format == null) return;

			var client = CreateNewRestClientAsync();

			InsecureResponse response = null;
			client.GetAsync<InsecureResponse>(ServiceClientBaseUri + "insecure",
				r => response = r, FailOnAsyncError);

			Thread.Sleep(2000);
			Assert.That(response.Result, Is.EqualTo("Public"));
		}

        /// <summary>Can login with session cookie to access secure service.</summary>
		[Test]
		public void Can_login_with_session_cookie_to_access_Secure_service()
		{
			var format = GetFormat();
			if (format == null) return;

			var req = (HttpWebRequest)WebRequest.Create(
				string.Format("http://localhost:82/{0}/syncreply/Secure", format));

			req.Headers[HttpHeaders.Authorization]
				= "basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(AllowedUser + ":" + AllowedPass));

			var res = (HttpWebResponse)req.GetResponse();
			var cookie = res.Cookies["ss-session"];
			if (cookie != null)
			{
				req = (HttpWebRequest)WebRequest.Create(
					string.Format("http://localhost:82/{0}/syncreply/Secure", format));
				req.CookieContainer.Add(new Cookie("ss-session", cookie.Value));

				var dtoString = new StreamReader(req.GetResponse().GetResponseStream()).ReadToEnd();
				Assert.That(dtoString.Contains("Confidential"));
				Console.WriteLine(dtoString);
			}
		}

        /// <summary>Gets 401 when accessing secure using fake sessionid cookie.</summary>
		[Test]
		public void Get_401_When_accessing_Secure_using_fake_sessionid_cookie()
		{
			var format = GetFormat();
			if (format == null) return;

			var req = (HttpWebRequest)WebRequest.Create(
				string.Format("http://localhost:82/{0}/syncreply/Secure", format));

			req.CookieContainer = new CookieContainer();
			req.CookieContainer.Add(new Cookie("ss-session", AllowedUser + "/" + Guid.NewGuid().ToString("N"), "/", "localhost"));

			try
			{
				req.GetResponse();
			}
			catch (WebException x)
			{
				Assert.That(((HttpWebResponse)x.Response).StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
			}
		}

        /// <summary>Gets 401 when accessing secure using service client without authorization.</summary>
        ///
        /// <exception cref="StatusCode">Thrown when the status code error condition occurs.</exception>
		[Test]
		public void Get_401_When_accessing_Secure_using_ServiceClient_without_Authorization()
		{
			var client = CreateNewServiceClient();

			try
			{
				var response = client.Send<SecureResponse>(new Secure());
				Console.WriteLine(response.Dump());
			}
			catch (WebServiceException ex)
			{
				Assert401(client, ex);
				return;
			}
			Assert.Fail("Should throw WebServiceException.StatusCode == 401");
		}

        /// <summary>Gets 401 when accessing secure using rest client get without authorization.</summary>
        ///
        /// <exception cref="StatusCode">Thrown when the status code error condition occurs.</exception>
		[Test]
		public void Get_401_When_accessing_Secure_using_RestClient_GET_without_Authorization()
		{
			var client = CreateNewRestClientAsync();
			if (client == null) return;

			SecureResponse response = null;
			var wasError = false;
			client.GetAsync<SecureResponse>(ServiceClientBaseUri + "secure",
				r => response = r, (r, ex) => wasError = Assert401(r, ex));

			Thread.Sleep(1000);
			Assert.That(wasError, Is.True,
				"Should throw WebServiceException.StatusCode == 401");
			Assert.IsNull(response);
		}

        /// <summary>Gets 401 when accessing secure using rest client delete without authorization.</summary>
        ///
        /// <exception cref="StatusCode">Thrown when the status code error condition occurs.</exception>
		[Test]
		public void Get_401_When_accessing_Secure_using_RestClient_DELETE_without_Authorization()
		{
			var client = CreateNewRestClientAsync();
			if (client == null) return;

			SecureResponse response = null;
			var wasError = false;
			client.DeleteAsync<SecureResponse>(ServiceClientBaseUri + "secure",
				r => response = r, (r, ex) => wasError = Assert401(r, ex));

			Thread.Sleep(1000);
			Assert.That(wasError, Is.True,
				"Should throw WebServiceException.StatusCode == 401");
			Assert.IsNull(response);
		}

        /// <summary>Gets 401 when accessing secure using rest client post without authorization.</summary>
        ///
        /// <exception cref="StatusCode">Thrown when the status code error condition occurs.</exception>
		[Test]
		public void Get_401_When_accessing_Secure_using_RestClient_POST_without_Authorization()
		{
			var client = CreateNewRestClientAsync();
			if (client == null) return;

			SecureResponse response = null;
			var wasError = false;
			client.PostAsync<SecureResponse>(ServiceClientBaseUri + "secure", new Secure(),
				r => response = r, (r, ex) => wasError = Assert401(r, ex));

			Thread.Sleep(1000);
			Assert.That(wasError, Is.True,
				"Should throw WebServiceException.StatusCode == 401");
			Assert.IsNull(response);
		}

        /// <summary>Gets 401 when accessing secure using rest client put without authorization.</summary>
        ///
        /// <exception cref="StatusCode">Thrown when the status code error condition occurs.</exception>
		[Test]
		public void Get_401_When_accessing_Secure_using_RestClient_PUT_without_Authorization()
		{
			var client = CreateNewRestClientAsync();
			if (client == null) return;

			SecureResponse response = null;
			var wasError = false;
			client.PutAsync<SecureResponse>(ServiceClientBaseUri + "secure", new Secure(),
				r => response = r, (r, ex) => wasError = Assert401(r, ex));

			Thread.Sleep(1000);
			Assert.That(wasError, Is.True,
						"Should throw WebServiceException.StatusCode == 401");
			Assert.IsNull(response);
		}


        /// <summary>A unit tests.</summary>
		public class UnitTests : RequestFiltersTests
		{
            /// <summary>Creates new service client.</summary>
            ///
            /// <returns>The new new service client.</returns>
			protected override IServiceClient CreateNewServiceClient()
			{
                EndpointHandlerBase.ServiceManager = new ServiceManager(typeof(SecureService).Assembly).Init();
				return new DirectServiceClient(EndpointHandlerBase.ServiceManager);
			}

            /// <summary>Creates new rest client asynchronous.</summary>
            ///
            /// <returns>The new new rest client asynchronous.</returns>
			protected override IRestClientAsync CreateNewRestClientAsync()
			{
				return null; //TODO implement REST calls with DirectServiceClient (i.e. Unit Tests)
				//EndpointHandlerBase.ServiceManager = new ServiceManager(true, typeof(SecureService).Assembly);
				//return new DirectServiceClient(EndpointHandlerBase.ServiceManager);
			}
		}

        /// <summary>An XML integration tests.</summary>
		public class XmlIntegrationTests : RequestFiltersTests
		{
            /// <summary>Gets the format.</summary>
            ///
            /// <returns>The format.</returns>
			protected override string GetFormat()
			{
				return "xml";
			}

            /// <summary>Creates new service client.</summary>
            ///
            /// <returns>The new new service client.</returns>
			protected override IServiceClient CreateNewServiceClient()
			{
				return new XmlServiceClient(ServiceClientBaseUri);
			}

            /// <summary>Creates new rest client asynchronous.</summary>
            ///
            /// <returns>The new new rest client asynchronous.</returns>
			protected override IRestClientAsync CreateNewRestClientAsync()
			{
				return new XmlServiceClient(ServiceClientBaseUri);
			}
		}

        /// <summary>A JSON integration tests.</summary>
		[TestFixture]
		public class JsonIntegrationTests : RequestFiltersTests
		{
            /// <summary>Gets the format.</summary>
            ///
            /// <returns>The format.</returns>
			protected override string GetFormat()
			{
				return "json";
			}

            /// <summary>Creates new service client.</summary>
            ///
            /// <returns>The new new service client.</returns>
			protected override IServiceClient CreateNewServiceClient()
			{
				return new JsonServiceClient(ServiceClientBaseUri);
			}

            /// <summary>Creates new rest client asynchronous.</summary>
            ///
            /// <returns>The new new rest client asynchronous.</returns>
			protected override IRestClientAsync CreateNewRestClientAsync()
			{
                return new JsonServiceClient(ServiceClientBaseUri);
			}
		}

        /// <summary>A jsv integration tests.</summary>
		[TestFixture]
		public class JsvIntegrationTests : RequestFiltersTests
		{
            /// <summary>Gets the format.</summary>
            ///
            /// <returns>The format.</returns>
			protected override string GetFormat()
			{
				return "jsv";
			}

            /// <summary>Creates new service client.</summary>
            ///
            /// <returns>The new new service client.</returns>
			protected override IServiceClient CreateNewServiceClient()
			{
				return new JsvServiceClient(ServiceClientBaseUri);
			}

            /// <summary>Creates new rest client asynchronous.</summary>
            ///
            /// <returns>The new new rest client asynchronous.</returns>
			protected override IRestClientAsync CreateNewRestClientAsync()
			{
                return new JsonServiceClient(ServiceClientBaseUri);
			}
		}

#if !MONOTOUCH

        /// <summary>A SOAP 11 integration tests.</summary>
		[TestFixture]
		public class Soap11IntegrationTests : RequestFiltersTests
		{
            /// <summary>Creates new service client.</summary>
            ///
            /// <returns>The new new service client.</returns>
			protected override IServiceClient CreateNewServiceClient()
			{
				return new Soap11ServiceClient(ServiceClientBaseUri);
			}

            /// <summary>Creates new rest client asynchronous.</summary>
            ///
            /// <returns>The new new rest client asynchronous.</returns>
			protected override IRestClientAsync CreateNewRestClientAsync()
			{
				return null;
			}
		}

        /// <summary>A SOAP 12 integration tests.</summary>
		[TestFixture]
		public class Soap12IntegrationTests : RequestFiltersTests
		{
            /// <summary>Creates new service client.</summary>
            ///
            /// <returns>The new new service client.</returns>
			protected override IServiceClient CreateNewServiceClient()
			{
				return new Soap12ServiceClient(ServiceClientBaseUri);
			}

            /// <summary>Creates new rest client asynchronous.</summary>
            ///
            /// <returns>The new new rest client asynchronous.</returns>
			protected override IRestClientAsync CreateNewRestClientAsync()
			{
				return null;
			}
		}

#endif

	}
}