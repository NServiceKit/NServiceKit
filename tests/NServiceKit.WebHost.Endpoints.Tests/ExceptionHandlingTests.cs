using System;
using System.Net;
using NServiceKit.ServiceInterface.ServiceModel;
using NServiceKit.Common.Web;
using NServiceKit.ServiceHost;
using NUnit.Framework;
using Funq;
using NServiceKit.Service;
using NServiceKit.ServiceClient.Web;
using NServiceKit.Text;

namespace NServiceKit.WebHost.Endpoints.Tests
{
    /// <summary>An user.</summary>
    [Route("/users")]
    public class User { }
    /// <summary>A user response.</summary>
    public class UserResponse : IHasResponseStatus
    {
        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
        public ResponseStatus ResponseStatus { get; set; }
    }

    /// <summary>A user service.</summary>
    public class UserService : ServiceInterface.Service
    {
        /// <summary>Gets the given request.</summary>
        ///
        /// <param name="request">The request to put.</param>
        ///
        /// <returns>An object.</returns>
        public object Get(User request)
        {
            return new HttpError(HttpStatusCode.BadRequest, "CanNotExecute", "Failed to execute!");
        }

        /// <summary>Post this message.</summary>
        ///
        /// <exception cref="HttpError">Thrown when a HTTP error error condition occurs.</exception>
        ///
        /// <param name="request">The request to put.</param>
        ///
        /// <returns>An object.</returns>
        public object Post(User request)
        {
            throw new HttpError(HttpStatusCode.BadRequest, "CanNotExecute", "Failed to execute!");
        }

        /// <summary>Deletes the given request.</summary>
        ///
        /// <exception cref="HttpError">Thrown when a HTTP error error condition occurs.</exception>
        ///
        /// <param name="request">The request to put.</param>
        ///
        /// <returns>An object.</returns>
        public object Delete(User request)
        {
            throw new HttpError(HttpStatusCode.Forbidden, "CanNotExecute", "Failed to execute!");
        }

        /// <summary>Puts the given request.</summary>
        ///
        /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or illegal values.</exception>
        ///
        /// <param name="request">The request to put.</param>
        ///
        /// <returns>An object.</returns>
        public object Put(User request)
        {
            throw new ArgumentException();
        }
    }

    /// <summary>Exception for signalling custom errors.</summary>
    public class CustomException : ArgumentException
    {
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.CustomException class.</summary>
        public CustomException() : base("User Defined Error") { }
    }

    /// <summary>An exception with response status.</summary>
    public class ExceptionWithResponseStatus { }
    /// <summary>An exception with response status response.</summary>
    public class ExceptionWithResponseStatusResponse
    {
        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
        public ResponseStatus ResponseStatus { get; set; }
    }
    /// <summary>An exception with response status service.</summary>
    public class ExceptionWithResponseStatusService : ServiceInterface.Service
    {
        /// <summary>Anies the given request.</summary>
        ///
        /// <exception cref="CustomException">Thrown when a Custom error condition occurs.</exception>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Any(ExceptionWithResponseStatus request)
        {
            throw new CustomException();
        }
    }

    /// <summary>An exception no response status.</summary>
    public class ExceptionNoResponseStatus { }
    /// <summary>An exception no response status response.</summary>
    public class ExceptionNoResponseStatusResponse { }
    /// <summary>An exception no response status service.</summary>
    public class ExceptionNoResponseStatusService : ServiceInterface.Service
    {
        /// <summary>Anies the given request.</summary>
        ///
        /// <exception cref="CustomException">Thrown when a Custom error condition occurs.</exception>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Any(ExceptionNoResponseStatus request)
        {
            throw new CustomException();
        }
    }

    /// <summary>An exception no response dto.</summary>
    public class ExceptionNoResponseDto { }
    /// <summary>An exception no response dto service.</summary>
    public class ExceptionNoResponseDtoService : ServiceInterface.Service
    {
        /// <summary>Anies the given request.</summary>
        ///
        /// <exception cref="CustomException">Thrown when a Custom error condition occurs.</exception>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Any(ExceptionNoResponseDto request)
        {
            throw new CustomException();
        }
    }

    /// <summary>Exception for signalling uncatched errors.</summary>
    public class UncatchedException { }
    /// <summary>An uncatched exception response.</summary>
    public class UncatchedExceptionResponse { }
    /// <summary>An uncatched exception service.</summary>
    public class UncatchedExceptionService : ServiceInterface.Service
    {
        /// <summary>Anies.</summary>
        ///
        /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or illegal values.</exception>
        ///
        /// <param name="request">Details of the exception.</param>
        ///
        /// <returns>An object.</returns>
        public object Any(UncatchedException request)
        {
            //We don't wrap a try..catch block around the service (which happens with ServiceBase<> automatically)
            //so the global exception handling strategy is invoked
            throw new ArgumentException();
        }
    }


    /// <summary>An exception handling tests.</summary>
    [TestFixture]
    public class ExceptionHandlingTests
    {
        private const string ListeningOn = "http://localhost:82/";

        /// <summary>An exception handling application host HTTP listener.</summary>
        public class ExceptionHandlingAppHostHttpListener
            : AppHostHttpListenerBase
        {

            /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.ExceptionHandlingTests.ExceptionHandlingAppHostHttpListener class.</summary>
            public ExceptionHandlingAppHostHttpListener()
                : base("Exception handling tests", typeof(UserService).Assembly) { }

            /// <summary>Configures the given container.</summary>
            ///
            /// <exception cref="exception">Thrown when an exception error condition occurs.</exception>
            ///
            /// <param name="container">The container.</param>
            public override void Configure(Container container)
            {
                JsConfig.EmitCamelCaseNames = true;

                SetConfig(new EndpointHostConfig { DebugMode = false });

                //Custom global uncaught exception handling strategy
                this.ExceptionHandler = (req, res, operationName, ex) =>
                {
                    res.Write(string.Format("Exception {0}", ex.GetType().Name));
                    res.EndRequest(skipHeaders: true);
                };

                this.ServiceExceptionHandler = (httpReq, request, exception) =>
                {
                    if (request is UncatchedException)
                        throw exception;

                    return null;
                };
            }
        }

        ExceptionHandlingAppHostHttpListener appHost;

        /// <summary>Executes the test fixture set up action.</summary>
        [TestFixtureSetUp]
        public void OnTestFixtureSetUp()
        {
            appHost = new ExceptionHandlingAppHostHttpListener();
            appHost.Init();
            appHost.Start(ListeningOn);
        }

        /// <summary>Executes the test fixture tear down action.</summary>
        [TestFixtureTearDown]
        public void OnTestFixtureTearDown()
        {
            appHost.Dispose();
            EndpointHost.ExceptionHandler = null;
        }

        static IRestClient[] ServiceClients = 
		{
			new JsonServiceClient(ListeningOn),
			new XmlServiceClient(ListeningOn),
			new JsvServiceClient(ListeningOn)
			//SOAP not supported in HttpListener
			//new Soap11ServiceClient(ServiceClientBaseUri),
			//new Soap12ServiceClient(ServiceClientBaseUri)
		};

        /// <summary>Handles returned HTTP error.</summary>
        ///
        /// <param name="client">The client.</param>
        [Test, TestCaseSource("ServiceClients")]
        public void Handles_Returned_Http_Error(IRestClient client)
        {
            try
            {
                client.Get<UserResponse>("/users");
                Assert.Fail();
            }
            catch (WebServiceException ex)
            {
                Assert.That(ex.ErrorCode, Is.EqualTo("CanNotExecute"));
                Assert.That(ex.StatusCode, Is.EqualTo((int)System.Net.HttpStatusCode.BadRequest));
                Assert.That(ex.Message, Is.EqualTo("CanNotExecute"));
            }
        }

        /// <summary>Handles thrown HTTP error.</summary>
        ///
        /// <param name="client">The client.</param>
        [Test, TestCaseSource("ServiceClients")]
        public void Handles_Thrown_Http_Error(IRestClient client)
        {
            try
            {
                client.Post<UserResponse>("/users", new User());
                Assert.Fail();
            }
            catch (WebServiceException ex)
            {
                Assert.That(ex.ErrorCode, Is.EqualTo("CanNotExecute"));
                Assert.That(ex.StatusCode, Is.EqualTo((int)System.Net.HttpStatusCode.BadRequest));
                Assert.That(ex.Message, Is.EqualTo("CanNotExecute"));
            }
        }

        /// <summary>Handles thrown HTTP error with forbidden status code.</summary>
        ///
        /// <param name="client">The client.</param>
        [Test, TestCaseSource("ServiceClients")]
        public void Handles_Thrown_Http_Error_With_Forbidden_status_code(IRestClient client)
        {
            try
            {
                client.Delete<UserResponse>("/users");
                Assert.Fail();
            }
            catch (WebServiceException ex)
            {
                Assert.That(ex.ErrorCode, Is.EqualTo("CanNotExecute"));
                Assert.That(ex.StatusCode, Is.EqualTo((int)System.Net.HttpStatusCode.Forbidden));
                Assert.That(ex.Message, Is.EqualTo("CanNotExecute"));
            }
        }

        /// <summary>Handles normal exception.</summary>
        ///
        /// <param name="client">The client.</param>
        [Test, TestCaseSource("ServiceClients")]
        public void Handles_Normal_Exception(IRestClient client)
        {
            try
            {
                client.Put<UserResponse>("/users", new User());
                Assert.Fail();
            }
            catch (WebServiceException ex)
            {
                Assert.That(ex.ErrorCode, Is.EqualTo("ArgumentException"));
                Assert.That(ex.StatusCode, Is.EqualTo((int)System.Net.HttpStatusCode.BadRequest));
            }
        }

        /// <summary>Predefined JSON URL.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        ///
        /// <returns>A string.</returns>
        public string PredefinedJsonUrl<T>()
        {
            return ListeningOn + "json/reply/" + typeof(T).Name;
        }

        /// <summary>Returns populated dto when has response status.</summary>
        [Test]
        public void Returns_populated_dto_when_has_ResponseStatus()
        {
            try
            {
                var json = PredefinedJsonUrl<ExceptionWithResponseStatus>().GetJsonFromUrl();
                Assert.Fail("Should throw");
            }
            catch (WebException webEx)
            {
                var errorResponse = ((HttpWebResponse)webEx.Response);
                var body = errorResponse.GetResponseStream().ReadFully().FromUtf8Bytes();
                Assert.That(body, Is.EqualTo(
                    "{\"responseStatus\":{\"errorCode\":\"CustomException\",\"message\":\"User Defined Error\",\"errors\":[]}}"));
            }
        }

        /// <summary>Returns empty dto when no response status.</summary>
        [Test]
        public void Returns_empty_dto_when_NoResponseStatus()
        {
            try
            {
                var json = PredefinedJsonUrl<ExceptionNoResponseStatus>().GetJsonFromUrl();
                Assert.Fail("Should throw");
            }
            catch (WebException webEx)
            {
                var errorResponse = ((HttpWebResponse)webEx.Response);
                var body = errorResponse.GetResponseStream().ReadFully().FromUtf8Bytes();
                Assert.That(body, Is.EqualTo("{}"));
            }
        }

        /// <summary>Returns no body when no response dto.</summary>
        [Test]
        public void Returns_no_body_when_NoResponseDto()
        {
            try
            {
                var json = PredefinedJsonUrl<ExceptionNoResponseDto>().GetJsonFromUrl();
                Assert.Fail("Should throw");
            }
            catch (WebException webEx)
            {
                var errorResponse = ((HttpWebResponse)webEx.Response);
                var body = errorResponse.GetResponseStream().ReadFully().FromUtf8Bytes();
                Assert.That(body, Is.StringStarting("{\"responseStatus\":{\"errorCode\":\"CustomException\",\"message\":\"User Defined Error\""));
            }
        }

        /// <summary>Can override global exception handling.</summary>
        [Test]
        public void Can_override_global_exception_handling()
        {
            var req = (HttpWebRequest)WebRequest.Create(PredefinedJsonUrl<UncatchedException>());
            var res = req.GetResponse().DownloadText();
            Assert.AreEqual("Exception ArgumentException", res);
        }
    }
}
