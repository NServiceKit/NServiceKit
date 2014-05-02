using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using NUnit.Framework;
using ProtoBuf;
using NServiceKit.Plugins.ProtoBuf;
using NServiceKit.Service;
using NServiceKit.ServiceClient.Web;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface.ServiceModel;

namespace NServiceKit.WebHost.Endpoints.Tests
{

    /// <summary>A reqstar.</summary>
    [Route("/reqstars")]
    [DataContract]
    public class Reqstar //: IReturn<List<Reqstar>>
    {
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
        [DataMember(Order = 1)]
        public int Id { get; set; }

        /// <summary>Gets or sets the person's first name.</summary>
        ///
        /// <value>The name of the first.</value>
        [DataMember(Order = 2)]
        public string FirstName { get; set; }

        /// <summary>Gets or sets the person's last name.</summary>
        ///
        /// <value>The name of the last.</value>
        [DataMember(Order = 3)]
        public string LastName { get; set; }

        /// <summary>Gets or sets the age.</summary>
        ///
        /// <value>The age.</value>
        [DataMember(Order = 4)]
        public int? Age { get; set; }
    }

    //New: No special naming convention

    [Route("/reqstars2/search")]
    [Route("/reqstars2/aged/{Age}")]
    [DataContract]
    public class SearchReqstars2 : IReturn<ReqstarsResponse>
    {
        /// <summary>Gets or sets the age.</summary>
        ///
        /// <value>The age.</value>
        [DataMember(Order = 1)]
        public int? Age { get; set; }
    }

    /// <summary>The reqstars response.</summary>
    [DataContract]
    public class ReqstarsResponse
    {
        /// <summary>Gets or sets the number of. </summary>
        ///
        /// <value>The total.</value>
        [DataMember(Order = 1)]
        public int Total { get; set; }

        /// <summary>Gets or sets the aged.</summary>
        ///
        /// <value>The aged.</value>
        [DataMember(Order = 2)]
        public int? Aged { get; set; }

        /// <summary>Gets or sets the results.</summary>
        ///
        /// <value>The results.</value>
        [DataMember(Order = 3)]
        public List<Reqstar> Results { get; set; }

        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
        [DataMember(Order = 4)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    //Naming convention:{Request DTO}Response

    [Route("/reqstars/search")]
    [Route("/reqstars/aged/{Age}")]
    [DataContract]
    public class SearchReqstars : IReturn<SearchReqstarsResponse>
    {
        /// <summary>Gets or sets the age.</summary>
        ///
        /// <value>The age.</value>
        [DataMember(Order = 1)]
        public int? Age { get; set; }
    }


    /// <summary>A search reqstars response.</summary>
    [DataContract]
    public class SearchReqstarsResponse
    {
        /// <summary>Gets or sets the number of. </summary>
        ///
        /// <value>The total.</value>
        [DataMember(Order = 1)]
        public int Total { get; set; }

        /// <summary>Gets or sets the aged.</summary>
        ///
        /// <value>The aged.</value>
        [DataMember(Order = 2)]
        public int? Aged { get; set; }

        /// <summary>Gets or sets the results.</summary>
        ///
        /// <value>The results.</value>
        [DataMember(Order = 3)]
        public List<Reqstar> Results { get; set; }

        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
        [DataMember(Order = 4)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    /// <summary>The reqstars service.</summary>
    public class ReqstarsService : IService
    {
        /// <summary>
        /// Testmethod following the 'old' naming convention (DTO/DTOResponse)
        /// </summary>
        public object Any(SearchReqstars request)
        {
            if (request.Age.HasValue && request.Age <= 0)
                throw new ArgumentException("Invalid Age");

            var response = new SearchReqstarsResponse {
                Total = 2,
                Aged = 10,
                Results = new List<Reqstar> {
                    new Reqstar { Id = 1, FirstName = "Max", LastName = "Meier", Age = 10 },
                    new Reqstar { Id = 2, FirstName = "Susan", LastName = "Stark", Age = 10 }
                }
            };

            return response;
        }

        /// <summary>
        /// Testmethod following no special naming convention (the new behavior)
        /// </summary>
        public object Any(SearchReqstars2 request)
        {
            if (request.Age.HasValue && request.Age <= 0)
                throw new ArgumentException("Invalid Age");

            var response = new ReqstarsResponse() {
                Total = 2,
                Aged = 10,
                Results = new List<Reqstar> {
                    new Reqstar { Id = 1, FirstName = "Max", LastName = "Meier", Age = 10 },
                    new Reqstar { Id = 2, FirstName = "Susan", LastName = "Stark", Age = 10 }
                }
            };

            return response;
        }
    }
    
    /// <summary>An application host.</summary>
    public class AppHost : AppHostHttpListenerBase
    {
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.AppHost class.</summary>
        public AppHost()
            : base("Test ErrorHandling", typeof(ReqstarsService).Assembly)
        {
        }

        /// <summary>Configures the given container.</summary>
        ///
        /// <param name="container">The container.</param>
        public override void Configure(Funq.Container container)
        {
            Plugins.Add(new ProtoBufFormat());
        }
    }

    /// <summary>An exception handling 2 tests.</summary>
    [TestFixture]
    public class ExceptionHandling2Tests
    {
        private static string testUri = "http://localhost:1337/";

        AppHost appHost;

        /// <summary>Initialises this object.</summary>
        [TestFixtureSetUp]
        public void Init()
        {
            try
            {
                appHost = new AppHost();
                appHost.Init();
                EndpointHost.Config.DebugMode = true;
                appHost.Start("http://*:1337/");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>Tear down.</summary>
        [TestFixtureTearDown]
        public void TearDown()
        {
            appHost.Dispose();
            appHost = null;
        }

        static IRestClient[] ServiceClients = 
		{
			new JsonServiceClient(testUri),
			new XmlServiceClient(testUri),
			new JsvServiceClient(testUri),
			new ProtoBufServiceClient(testUri)
		};


        /// <summary>
        ///A test for a good response
        ///</summary>
        [Test, TestCaseSource("ServiceClients")]
        [Category("OldNamingConvention")]
        public void OldNamingConv_Get_ExpectingResults(IRestClient client)
        {
            var response = client.Get(new SearchReqstars { Age = 10 });

            Assert.AreEqual(2, response.Total);
        }

        /// <summary>
        ///A GET test for receiving a WebServiceException with status ArgumentException and message "Invalid Age"
        ///</summary>
        [Test, TestCaseSource("ServiceClients")]
        [Category("OldNamingConvention")]
        public void OldNamingConv_Get_ArgumentException_InvalidAge(IRestClient client)
        {
            try
            {
                client.Get(new SearchReqstars { Age = -1 });
            }
            catch (WebServiceException ex)
            {
                Assert.AreEqual("ArgumentException", ex.StatusDescription, "Wrong ExceptionType");
                Assert.AreEqual("Invalid Age", ex.ErrorMessage, "Wrong message");
            }
        }

        /// <summary>
        ///A test for a good response with POST request
        ///</summary>
        [Test, TestCaseSource("ServiceClients")]
        [Category("OldNamingConvention")]
        public void OldNamingConv_Post_ExpectingResults(IRestClient client)
        {
            var response = client.Post(new SearchReqstars { Age = 10 });

            Assert.AreEqual(2, response.Total);
        }

        /// <summary>
        ///A POST test for receiving a WebServiceException with status ArgumentException and message "Invalid Age"
        ///</summary>
        [Test, TestCaseSource("ServiceClients")]
        [Category("OldNamingConvention")]
        public void OldNamingConv_Post_ArgumentException_InvalidAge(IRestClient client)
        {
            try
            {
                client.Post(new SearchReqstars { Age = -1 });
            }
            catch (WebServiceException ex)
            {
                Assert.AreEqual("ArgumentException", ex.StatusDescription, "Wrong ExceptionType");
                Assert.AreEqual("Invalid Age", ex.ErrorMessage, "Wrong message");
            }
        }


        /// <summary>
        ///A test for a good response
        ///</summary>
        [Test, TestCaseSource("ServiceClients")]
        [Category("NoNamingConvention")]
        public void NoNamingConv_Get_ExpectingResults(IRestClient client)
        {
            var response = client.Get(new SearchReqstars2 { Age = 10 });

            Assert.AreEqual(2, response.Total);
        }

        /// <summary>
        ///A GET test for receiving a WebServiceException with status ArgumentException and message "Invalid Age"
        ///</summary>
        [Test, TestCaseSource("ServiceClients")]
        [Category("NoNamingConvention")]
        public void NoNamingConv_Get_ArgumentException_InvalidAge(IRestClient client)
        {
            try
            {
                client.Get(new SearchReqstars2 { Age = -1 });
            }
            catch (WebServiceException ex)
            {
                Assert.AreEqual("ArgumentException", ex.StatusDescription, "Wrong ExceptionType");
                Assert.AreEqual("Invalid Age", ex.ErrorMessage, "Wrong message");
            }
        }

        /// <summary>
        ///A test for a good response with POST request
        ///</summary>
        [Test, TestCaseSource("ServiceClients")]
        [Category("NoNamingConvention")]
        public void NoNamingConv_Post_ExpectingResults(IRestClient client)
        {
            var response = client.Post(new SearchReqstars2 { Age = 10 });

            Assert.AreEqual(2, response.Total);
        }

        /// <summary>
        ///A POST test for receiving a WebServiceException with status ArgumentException and message "Invalid Age"
        ///</summary>
        [Test, TestCaseSource("ServiceClients")]
        [Category("NoNamingConvention")]
        public void NoNamingConv_Post_ArgumentException_InvalidAge(IRestClient client)
        {
            try
            {
                client.Post(new SearchReqstars2 { Age = -1 });
            }
            catch (WebServiceException ex)
            {
                Assert.AreEqual("ArgumentException", ex.StatusDescription, "Wrong ExceptionType");
                Assert.AreEqual("Invalid Age", ex.ErrorMessage, "Wrong message");
            }
        }


    }

}
