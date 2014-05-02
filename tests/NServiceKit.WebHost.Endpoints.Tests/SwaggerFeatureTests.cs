using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceClient.Web;
using NServiceKit.Service;
using NServiceKit.Api.Swagger;
using NServiceKit.ServiceInterface.Cors;
using NServiceKit.Text;

namespace NServiceKit.WebHost.Endpoints.Tests
{
    /// <summary>A swagger feature request.</summary>
    [ServiceHost.Api("Service Description")]
    [Route("/swagger/{Name}", "GET", Summary = @"GET Summary", Notes = "GET Notes")]
    [Route("/swagger/{Name}", "POST", Summary = @"POST Summary", Notes = "POST Notes")]
    public class SwaggerFeatureRequest
    {
        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
        [ApiMember(Name="Name", Description = "Name Description", 
            ParameterType = "path", DataType = SwaggerType.String, IsRequired = true)]
        public string Name { get; set; }
    }

    /// <summary>A swagger get list request.</summary>
    [ServiceHost.Api]
    [Route("/swaggerGetList/{Name}", "GET")]
    public class SwaggerGetListRequest : IReturn<List<SwaggerFeatureResponse>>
    {
        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
        public string Name { get; set; }
    }

    /// <summary>A swagger get array request.</summary>
    [ServiceHost.Api]
    [Route("/swaggerGetArray/{Name}", "GET")]
    public class SwaggerGetArrayRequest : IReturn<SwaggerFeatureResponse[]>
    {
        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
        public string Name { get; set; }
    }
    
    /// <summary>A swagger models request.</summary>
    [ServiceHost.Api]
    [Route("/swaggerModels/{UrlParam}", "POST")]
    public class SwaggerModelsRequest : IReturn<SwaggerFeatureResponse>
    {
        /// <summary>Gets or sets the URL parameter.</summary>
        ///
        /// <value>The URL parameter.</value>
        [ApiMember(Name = "UrlParam", Description = "URL parameter",
            ParameterType = "path", DataType = SwaggerType.String, IsRequired = true)]
        public string UrlParam { get; set; }

        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
        [ApiMember(Name = "RequestBody", Description = "The request body",
            ParameterType = "body", DataType = "SwaggerModelsRequest", IsRequired = true)]
        [System.ComponentModel.Description("Name description")]
        public string Name { get; set; }

        /// <summary>Gets or sets the nested model.</summary>
        ///
        /// <value>The nested model.</value>
        [System.ComponentModel.Description("NestedModel description")]
        public SwaggerNestedModel NestedModel { get; set;}

        /// <summary>Gets or sets the list property.</summary>
        ///
        /// <value>The list property.</value>
        public List<SwaggerNestedModel2> ListProperty { get; set; }

        /// <summary>Gets or sets the array property.</summary>
        ///
        /// <value>The array property.</value>
        public SwaggerNestedModel3[] ArrayProperty { get; set; }

        /// <summary>Gets or sets the byte property.</summary>
        ///
        /// <value>The byte property.</value>
        public byte ByteProperty { get; set; }

        /// <summary>Gets or sets the long property.</summary>
        ///
        /// <value>The long property.</value>
        public long LongProperty { get; set; }

        /// <summary>Gets or sets the float property.</summary>
        ///
        /// <value>The float property.</value>
        public float FloatProperty { get; set; }

        /// <summary>Gets or sets the double property.</summary>
        ///
        /// <value>The double property.</value>
        public double DoubleProperty { get; set; }

        /// <summary>Gets or sets the decimal property.</summary>
        ///
        /// <value>The decimal property.</value>
        public decimal DecimalProperty { get; set; }

        /// <summary>Gets or sets the Date/Time of the date property.</summary>
        ///
        /// <value>The date property.</value>
        public DateTime DateProperty { get; set; }
    }

    /// <summary>A data Model for the swagger nested.</summary>
    public class SwaggerNestedModel
    {
        /// <summary>Gets or sets a value indicating whether the nested property.</summary>
        ///
        /// <value>true if nested property, false if not.</value>
        [System.ComponentModel.Description("NestedProperty description")]
        public bool NestedProperty { get; set;}
    }

    /// <summary>A swagger nested model 2.</summary>
    public class SwaggerNestedModel2
    {
        /// <summary>Gets or sets a value indicating whether the nested property 2.</summary>
        ///
        /// <value>true if nested property 2, false if not.</value>
        [System.ComponentModel.Description("NestedProperty2 description")]
        public bool NestedProperty2 { get; set;}
    }

    /// <summary>A swagger nested model 3.</summary>
    public class SwaggerNestedModel3
    {
        /// <summary>Gets or sets a value indicating whether the nested property 3.</summary>
        ///
        /// <value>true if nested property 3, false if not.</value>
        [System.ComponentModel.Description("NestedProperty3 description")]
        public bool NestedProperty3 { get; set;}
    }

    /// <summary>A name is not set request.</summary>
    [ServiceHost.Api]
    [Route("/swagger2/NameIsNotSetRequest", "GET")]
    public class NameIsNotSetRequest
    {
        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
        [ApiMember]
        public string Name { get; set; }
    }


    /// <summary>A multiple test request.</summary>
    [ServiceHost.Api("test")]
    [Route("/swg3/conference/count", "GET")]
    public class MultipleTestRequest : IReturn<int>
    {
        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
        [ApiMember]
        public string Name { get; set; }
    }

    /// <summary>A multiple test 2 request.</summary>
    [ServiceHost.Api]
    [Route("/swg3/conference/{Name}/conferences", "POST")]
    [Route("/swgb3/conference/{Name}/conferences", "POST")]
    public class MultipleTest2Request : IReturn<object>
    {
        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
        [ApiMember]
        public string Name { get; set; }
    }

    /// <summary>A multiple test 3 request.</summary>
    [ServiceHost.Api]
    [Route("/swg3/conference/{Name}/conferences", "DELETE")]
    public class MultipleTest3Request : IReturn<object>
    {
        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
        [ApiMember]
        public string Name { get; set; }
    }

    /// <summary>A multiple test 4 request.</summary>
    [ServiceHost.Api]
    [Route("/swg3/conference", "GET")]
    public class MultipleTest4Request : IReturn<object>
    {
        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
        [ApiMember]
        public string Name { get; set; }
    }

    /// <summary>A nullable response.</summary>
	public class NullableResponse
	{
        /// <summary>Gets or sets a value indicating whether the nested property 2.</summary>
        ///
        /// <value>true if nested property 2, false if not.</value>
		[System.ComponentModel.Description("NestedProperty2 description")]
		public bool NestedProperty2 { get; set; }

        /// <summary>Gets or sets the optional.</summary>
        ///
        /// <value>The optional.</value>
		public int? Optional { get; set; }
	}

    /// <summary>A nullable in request.</summary>
	[ServiceHost.Api]
	[Route("/swgnull/", "GET")]
	public class NullableInRequest : IReturn<NullableResponse>
	{
        /// <summary>Gets or sets the position.</summary>
        ///
        /// <value>The position.</value>
		[ApiMember]
		public int? Position { get; set; }
	}
	
    /// <summary>A nullable service.</summary>
	public class NullableService : ServiceInterface.Service
	{
        /// <summary>Gets the given request.</summary>
        ///
        /// <param name="request">The request to get.</param>
        ///
        /// <returns>An object.</returns>
		public object Get(NullableInRequest request)
		{
			return null;
		}
	}


    /// <summary>A swagger feature response.</summary>
    public class SwaggerFeatureResponse
    {
        /// <summary>Gets or sets a value indicating whether this object is success.</summary>
        ///
        /// <value>true if this object is success, false if not.</value>
        public bool IsSuccess { get; set; }
    }

    /// <summary>A multiple test request service.</summary>
    public class MultipleTestRequestService : ServiceInterface.Service
    {
        /// <summary>Gets the given request.</summary>
        ///
        /// <param name="request">The request to delete.</param>
        ///
        /// <returns>An object.</returns>
        public object Get(MultipleTestRequest request)
        {
            return null;
        }

        /// <summary>Post this message.</summary>
        ///
        /// <param name="request">The request to delete.</param>
        ///
        /// <returns>An object.</returns>
        public object Post(MultipleTest2Request request)
        {
            return null;
        }

        /// <summary>Deletes the given request.</summary>
        ///
        /// <param name="request">The request to delete.</param>
        ///
        /// <returns>An object.</returns>
        public object Delete(MultipleTest3Request request)
        {
            return null;
        }
    }
    /// <summary>A multiple test 2 request service.</summary>
    public class MultipleTest2RequestService : ServiceInterface.Service
    {
        /// <summary>Gets the given request.</summary>
        ///
        /// <param name="request">The request to get.</param>
        ///
        /// <returns>An object.</returns>
        public object Get(MultipleTest4Request request)
        {
            return null;
        }
    }


    /// <summary>A swagger feature service.</summary>
    public class SwaggerFeatureService : ServiceInterface.Service
    {
        /// <summary>Gets the given request.</summary>
        ///
        /// <param name="request">The request to get.</param>
        ///
        /// <returns>An object.</returns>
        public object Get(SwaggerFeatureRequest request)
        {
            return new SwaggerFeatureResponse { IsSuccess = true };
        }

        /// <summary>Post this message.</summary>
        ///
        /// <param name="request">The request to get.</param>
        ///
        /// <returns>An object.</returns>
        public object Post(SwaggerFeatureRequest request)
        {
            return new SwaggerFeatureResponse { IsSuccess = true };
        }

        /// <summary>Gets the given request.</summary>
        ///
        /// <param name="request">The request to get.</param>
        ///
        /// <returns>An object.</returns>
        public object Get(NameIsNotSetRequest request)
        {
            return 0;
        }

        /// <summary>Post this message.</summary>
        ///
        /// <param name="request">The request to get.</param>
        ///
        /// <returns>An object.</returns>
        public object Post(SwaggerModelsRequest request)
        {
            return new SwaggerFeatureResponse { IsSuccess = true };
        }

        /// <summary>Gets the given request.</summary>
        ///
        /// <param name="request">The request to get.</param>
        ///
        /// <returns>An object.</returns>
        public object Get(SwaggerGetListRequest request)
        {
            return new List<SwaggerFeatureResponse> { new SwaggerFeatureResponse { IsSuccess = true } };
        }

        /// <summary>Gets the given request.</summary>
        ///
        /// <param name="request">The request to get.</param>
        ///
        /// <returns>An object.</returns>
        public object Get(SwaggerGetArrayRequest request)
        {
            return new[] { new SwaggerFeatureResponse { IsSuccess = true } };
        }
    }


    
    /// <summary>A swagger feature service tests.</summary>
    [TestFixture]
    public class SwaggerFeatureServiceTests
    {
        private const string BaseUrl = "http://localhost:8024";
        private const string ListeningOn = BaseUrl + "/";

        /// <summary>A swagger feature application host HTTP listener.</summary>
        public class SwaggerFeatureAppHostHttpListener
            : AppHostHttpListenerBase
        {

            /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.SwaggerFeatureServiceTests.SwaggerFeatureAppHostHttpListener class.</summary>
            public SwaggerFeatureAppHostHttpListener()
                : base("Swagger Feature Tests", typeof(SwaggerFeatureServiceTests).Assembly) { }

            /// <summary>Configures the given container.</summary>
            ///
            /// <param name="container">The container.</param>
            public override void Configure(Funq.Container container)
            {
                Plugins.Add(new SwaggerFeature());

                SetConfig(new EndpointHostConfig
                {
                    DebugMode = true //Show StackTraces for easier debugging
                });
            }
        }

        SwaggerFeatureAppHostHttpListener appHost;

        /// <summary>Executes the test fixture set up action.</summary>
        [TestFixtureSetUp]
        public void OnTestFixtureSetUp()
        {
            appHost = new SwaggerFeatureAppHostHttpListener();
            appHost.Init();
            appHost.Start(ListeningOn);
        }

        /// <summary>Executes the test fixture tear down action.</summary>
        [TestFixtureTearDown]
        public void OnTestFixtureTearDown()
        {
            appHost.Dispose();
        }

        static IRestClient[] RestClients = 
        {
            new JsonServiceClient(ListeningOn)
            //new XmlServiceClient(ServiceClientBaseUri),
        };

        /// <summary>Executes for 5 mins operation.</summary>
        [Test, Explicit]
        public void RunFor5Mins()
        {
            appHost.LoadPlugin(new CorsFeature("http://localhost:50001"));

            Debug.WriteLine(ListeningOn + "resources");
            Thread.Sleep(TimeSpan.FromMinutes(5));
        }

        /// <summary>Should get default name from property.</summary>
        ///
        /// <param name="client">The client.</param>
        [Test, TestCaseSource("RestClients")]
        public void Should_get_default_name_from_property(IRestClient client)
        {
            var resource = client.Get<ResourceResponse>("/resource/swagger2/NameIsNotSetRequest");

            var p = resource.Apis.SelectMany(t => t.Operations).SelectMany(t => t.Parameters);
            Assert.That(p.Count(), Is.EqualTo(1));
            Assert.That(p.FirstOrDefault(t=>t.Name == "Name"), Is.Not.Null);
        }

        /// <summary>Should group similar services.</summary>
        ///
        /// <param name="client">The client.</param>
        [Test, TestCaseSource("RestClients")]
        public void Should_group_similar_services(IRestClient client)
        {
            var resources = client.Get<ResourcesResponse>("/resources");
            resources.PrintDump();

            var swagger = resources.Apis.Where(t => t.Path.Contains("/resource/swg3"));
            Assert.That(swagger.Count(), Is.EqualTo(1));
        }

        /// <summary>Should distinct base path.</summary>
        ///
        /// <param name="client">The client.</param>
        [Test, TestCaseSource("RestClients")]
        public void Should_distinct_base_path(IRestClient client)
        {
            var resources = client.Get<ResourcesResponse>("/resources");
            resources.PrintDump();

            var swagger = resources.Apis.Where(t => t.Path.Contains("/resource/swgb3"));
            Assert.That(swagger.Count(), Is.EqualTo(1));
        }

        /// <summary>Should list services.</summary>
        ///
        /// <param name="client">The client.</param>
        [Test, TestCaseSource("RestClients")]
        public void Should_list_services(IRestClient client)
        {
            var resources = client.Get<ResourcesResponse>("/resources");
            Assert.That(resources.BasePath, Is.EqualTo(BaseUrl));
            Assert.That(resources.SwaggerVersion, Is.EqualTo("1.1"));
            Assert.That(resources.Apis, Is.Not.Null);

            var swagger = resources.Apis.FirstOrDefault(t => t.Path == "/resource/swagger");
            Assert.That(swagger, Is.Not.Null);
            Assert.That(swagger.Description, Is.EqualTo("Service Description"));
        }

        /// <summary>Should use webhosturl as resources base path when configured.</summary>
        ///
        /// <param name="client">The client.</param>
        [Test, TestCaseSource("RestClients")]
        public void Should_use_webhosturl_as_resources_base_path_when_configured(IRestClient client)
        {
            const string webHostUrl = "https://host.example.com/_api";
            try
            {
                appHost.Config.WebHostUrl = webHostUrl;

                var resources = client.Get<ResourcesResponse>("/resources");
                resources.PrintDump();

                Assert.That(resources.BasePath, Is.EqualTo(webHostUrl));
            }
            finally
            {
                appHost.Config.WebHostUrl = null;
            }
        }

        /// <summary>Should use webhosturl as resource base path when configured.</summary>
        ///
        /// <param name="client">The client.</param>
        [Test, TestCaseSource("RestClients")]
        public void Should_use_webhosturl_as_resource_base_path_when_configured(IRestClient client)
        {
            const string webHostUrl = "https://host.example.com/_api";
            try
            {
                appHost.Config.WebHostUrl = webHostUrl;

                var resource = client.Get<ResourceResponse>("/resource/swagger");
                resource.PrintDump();

                Assert.That(resource.BasePath, Is.EqualTo(webHostUrl));
            }
            finally
            {
                appHost.Config.WebHostUrl = null;
            }
        }

        /// <summary>Should use HTTPS for resources basepath when usehttpslinks configuration is true.</summary>
        ///
        /// <param name="client">The client.</param>
        [Test, TestCaseSource("RestClients")]
        public void Should_use_https_for_resources_basepath_when_usehttpslinks_config_is_true(IRestClient client)
        {
            try
            {
                appHost.Config.UseHttpsLinks = true;

                var resources = client.Get<ResourcesResponse>("/resources");
                resources.PrintDump();

                Assert.That(resources.BasePath.ToLowerInvariant(), Is.StringStarting("https"));
            }
            finally
            {
                appHost.Config.UseHttpsLinks = false;
            }
        }

        /// <summary>Should use HTTPS for resource basepath when usehttpslinks configuration is true.</summary>
        ///
        /// <param name="client">The client.</param>
        [Test, TestCaseSource("RestClients")]
        public void Should_use_https_for_resource_basepath_when_usehttpslinks_config_is_true(IRestClient client)
        {
            try
            {
                appHost.Config.UseHttpsLinks = true;

                var resource = client.Get<ResourceResponse>("/resource/swagger");
                resource.PrintDump();

                Assert.That(resource.BasePath.ToLowerInvariant(), Is.StringStarting("https"));
            }
            finally
            {
                appHost.Config.UseHttpsLinks = false;
            }
        }

        /// <summary>Should retrieve service parameters.</summary>
        ///
        /// <param name="client">The client.</param>
        [Test, TestCaseSource("RestClients")]
        public void Should_retrieve_service_parameters(IRestClient client)
        {
            var resource = client.Get<ResourceResponse>("/resource/swagger");
            Assert.That(resource.BasePath, Is.EqualTo(BaseUrl));
            Assert.That(resource.ResourcePath, Is.EqualTo("/swagger"));
            Assert.That(resource.Apis, Is.Not.Empty);

            resource.Apis.PrintDump();

            var operations = new List<MethodOperation>();
            foreach(var api in resource.Apis) operations.AddRange(api.Operations);

            var getOperation = operations.Single(t => t.HttpMethod == "GET");
            Assert.That(getOperation.Summary, Is.EqualTo("GET Summary"));
            Assert.That(getOperation.Notes, Is.EqualTo("GET Notes"));
            Assert.That(getOperation.HttpMethod, Is.EqualTo("GET"));

            Assert.That(getOperation.Parameters, Is.Not.Empty);
            var p1 = getOperation.Parameters[0];
            Assert.That(p1.Name, Is.EqualTo("Name"));
            Assert.That(p1.Description, Is.EqualTo("Name Description"));
            Assert.That(p1.DataType, Is.EqualTo("string"));
            Assert.That(p1.ParamType, Is.EqualTo("path"));
            Assert.That(p1.Required, Is.EqualTo(true));


            var postOperation = operations.Single(t => t.HttpMethod == "POST");
            Assert.That(postOperation.Summary, Is.EqualTo("POST Summary"));
            Assert.That(postOperation.Notes, Is.EqualTo("POST Notes"));
            Assert.That(postOperation.HttpMethod, Is.EqualTo("POST"));
        }

        /// <summary>Should retrieve response class name.</summary>
        ///
        /// <param name="client">The client.</param>
        [Test, TestCaseSource("RestClients")]
        public void Should_retrieve_response_class_name(IRestClient client)
        {
            var resource = client.Get<ResourceResponse>("/resource/swaggerModels");
            Assert.That(resource.Apis, Is.Not.Empty);

            var postOperation = resource.Apis.SelectMany(api => api.Operations).Single(t => t.HttpMethod == "POST");
            postOperation.PrintDump();
            Assert.That(postOperation.ResponseClass, Is.EqualTo(typeof(SwaggerFeatureResponse).Name));
        }

        /// <summary>Should retrieve list response type information.</summary>
        ///
        /// <param name="client">The client.</param>
        [Test, TestCaseSource("RestClients")]
        public void Should_retrieve_list_response_type_info(IRestClient client)
        {
            var resource = client.Get<ResourceResponse>("/resource/swaggerGetList");
            Assert.That(resource.Apis, Is.Not.Empty);

            var operation = resource.Apis.SelectMany(api => api.Operations).Single(t => t.HttpMethod == "GET");
            operation.PrintDump();
            Assert.That(operation.ResponseClass, Is.EqualTo("List[SwaggerFeatureResponse]"));
            Assert.That(resource.Models.ContainsKey("SwaggerFeatureResponse"));
        }

        /// <summary>Should retrieve array response type information.</summary>
        ///
        /// <param name="client">The client.</param>
        [Test, TestCaseSource("RestClients")]
        public void Should_retrieve_array_response_type_info(IRestClient client)
        {
            var resource = client.Get<ResourceResponse>("/resource/swaggerGetArray");
            Assert.That(resource.Apis, Is.Not.Empty);

            var operation = resource.Apis.SelectMany(api => api.Operations).Single(t => t.HttpMethod == "GET");
            operation.PrintDump();
            Assert.That(operation.ResponseClass, Is.EqualTo("List[SwaggerFeatureResponse]"));
            Assert.That(resource.Models.ContainsKey("SwaggerFeatureResponse"));
        }

        /// <summary>Should retrieve response model.</summary>
        ///
        /// <param name="client">The client.</param>
        [Test, TestCaseSource("RestClients")]
        public void Should_retrieve_response_model(IRestClient client)
        {
            var resource = client.Get<ResourceResponse>("/resource/swaggerModels");
            Assert.That(resource.Models, Is.Not.Empty);

            Assert.That(resource.Models.ContainsKey(typeof(SwaggerFeatureResponse).Name), Is.True);
            var responseClassModel = resource.Models[typeof(SwaggerFeatureResponse).Name];
            responseClassModel.PrintDump();

            Assert.That(responseClassModel.Id, Is.EqualTo(typeof(SwaggerFeatureResponse).Name));
            Assert.That(responseClassModel.Properties, Is.Not.Empty);
            Assert.That(responseClassModel.Properties.ContainsKey("IsSuccess"), Is.True);
            Assert.That(responseClassModel.Properties["IsSuccess"].Type, Is.EqualTo(SwaggerType.Boolean));
        }

        /// <summary>Should retrieve request body model.</summary>
        ///
        /// <param name="client">The client.</param>
        [Test, TestCaseSource("RestClients")]
        public void Should_retrieve_request_body_model(IRestClient client)
        {
            var resource = client.Get<ResourceResponse>("/resource/swaggerModels");
            Assert.That(resource.Models, Is.Not.Empty);
            resource.Models.PrintDump();

            Assert.That(resource.Models.ContainsKey(typeof(SwaggerModelsRequest).Name), Is.True);
            var requestClassModel = resource.Models[typeof(SwaggerModelsRequest).Name];

            Assert.That(requestClassModel.Id, Is.EqualTo(typeof(SwaggerModelsRequest).Name));
            Assert.That(requestClassModel.Properties, Is.Not.Empty);

            Assert.That(requestClassModel.Properties.ContainsKey("UrlParam"), Is.False);

            Assert.That(requestClassModel.Properties.ContainsKey("Name"), Is.True);
            Assert.That(requestClassModel.Properties["Name"].Type, Is.EqualTo(SwaggerType.String));
            Assert.That(requestClassModel.Properties["Name"].Description, Is.EqualTo("Name description"));

            Assert.That(requestClassModel.Properties.ContainsKey("ByteProperty"));
            Assert.That(requestClassModel.Properties["ByteProperty"].Type, Is.EqualTo(SwaggerType.Byte));
            Assert.That(resource.Models.ContainsKey(typeof(byte).Name), Is.False);

            Assert.That(requestClassModel.Properties.ContainsKey("LongProperty"));
            Assert.That(requestClassModel.Properties["LongProperty"].Type, Is.EqualTo(SwaggerType.Long));
            Assert.That(resource.Models.ContainsKey(typeof(long).Name), Is.False);

            Assert.That(requestClassModel.Properties.ContainsKey("FloatProperty"));
            Assert.That(requestClassModel.Properties["FloatProperty"].Type, Is.EqualTo(SwaggerType.Float));
            Assert.That(resource.Models.ContainsKey(typeof(float).Name), Is.False);

            Assert.That(requestClassModel.Properties.ContainsKey("DoubleProperty"));
            Assert.That(requestClassModel.Properties["DoubleProperty"].Type, Is.EqualTo(SwaggerType.Double));
            Assert.That(resource.Models.ContainsKey(typeof(double).Name), Is.False);

            // Swagger has no concept of a "decimal" type
            Assert.That(requestClassModel.Properties.ContainsKey("DecimalProperty"));
            Assert.That(requestClassModel.Properties["DecimalProperty"].Type, Is.EqualTo(SwaggerType.Double));
            Assert.That(resource.Models.ContainsKey(typeof(decimal).Name), Is.False);

            Assert.That(requestClassModel.Properties.ContainsKey("DateProperty"));
            Assert.That(requestClassModel.Properties["DateProperty"].Type, Is.EqualTo(SwaggerType.Date));
            Assert.That(resource.Models.ContainsKey(typeof(DateTime).Name), Is.False);

            Assert.That(requestClassModel.Properties.ContainsKey("NestedModel"), Is.True);
            Assert.That(requestClassModel.Properties["NestedModel"].Type, Is.EqualTo("SwaggerNestedModel"));
            Assert.That(requestClassModel.Properties["NestedModel"].Description, Is.EqualTo("NestedModel description"));

            Assert.That(resource.Models.ContainsKey(typeof(SwaggerNestedModel).Name), Is.True);
            var nestedClassModel = resource.Models[typeof(SwaggerNestedModel).Name];

            Assert.That(nestedClassModel.Properties.ContainsKey("NestedProperty"), Is.True);
            Assert.That(nestedClassModel.Properties["NestedProperty"].Type, Is.EqualTo(SwaggerType.Boolean));
            Assert.That(nestedClassModel.Properties["NestedProperty"].Description, Is.EqualTo("NestedProperty description"));
        }

        /// <summary>Should retrieve list property model.</summary>
        ///
        /// <param name="client">The client.</param>
        [Test, TestCaseSource("RestClients")]
        public void Should_retrieve_list_property_model(IRestClient client)
        {
            var resource = client.Get<ResourceResponse>("/resource/swaggerModels");
            Assert.That(resource.Models.ContainsKey(typeof(SwaggerModelsRequest).Name), Is.True);
            var requestClassModel = resource.Models[typeof(SwaggerModelsRequest).Name];

            Assert.That(requestClassModel.Properties.ContainsKey("ListProperty"), Is.True);
            Assert.That(requestClassModel.Properties["ListProperty"].Type, Is.EqualTo(SwaggerType.Array));
            Assert.That(requestClassModel.Properties["ListProperty"].Items["$ref"], Is.EqualTo(typeof(SwaggerNestedModel2).Name));
            Assert.That(resource.Models.ContainsKey(typeof(SwaggerNestedModel2).Name), Is.True);
        }

        /// <summary>Should retrieve array property model.</summary>
        ///
        /// <param name="client">The client.</param>
        [Test, TestCaseSource("RestClients")]
        public void Should_retrieve_array_property_model(IRestClient client)
        {
            var resource = client.Get<ResourceResponse>("/resource/swaggerModels");
            Assert.That(resource.Models.ContainsKey(typeof(SwaggerModelsRequest).Name), Is.True);
            var requestClassModel = resource.Models[typeof(SwaggerModelsRequest).Name];

            Assert.That(requestClassModel.Properties.ContainsKey("ArrayProperty"), Is.True);
            Assert.That(requestClassModel.Properties["ArrayProperty"].Type, Is.EqualTo(SwaggerType.Array));
            Assert.That(requestClassModel.Properties["ArrayProperty"].Items["$ref"], Is.EqualTo(typeof(SwaggerNestedModel3).Name));
            Assert.That(resource.Models.ContainsKey(typeof(SwaggerNestedModel3).Name), Is.True);
        }

        /// <summary>Should retrieve valid nullable fields.</summary>
        ///
        /// <param name="client">The client.</param>
		[Test, TestCaseSource("RestClients")]
		public void Should_retrieve_valid_nullable_fields(IRestClient client)
		{
			var resource = client.Get<ResourceResponse>("/resource/swgnull");
			Assert.That(resource.Models.ContainsKey(typeof(NullableInRequest).Name), Is.True);
			var requestClassModel = resource.Models[typeof(NullableInRequest).Name];

			Assert.That(requestClassModel.Properties.ContainsKey("Position"), Is.True);
			Assert.That(requestClassModel.Properties["Position"].Type, Is.EqualTo(SwaggerType.Int));
			Assert.That(resource.Models.ContainsKey(typeof(NullableResponse).Name), Is.True);

			var responseModel = resource.Models[typeof (NullableResponse).Name];
			Assert.That(responseModel.Properties.ContainsKey("Optional"), Is.True);
			Assert.That(responseModel.Properties["Optional"].Required, Is.False);
			Assert.That(responseModel.Properties["Optional"].Type, Is.EqualTo(SwaggerType.Int));
			Assert.That(responseModel.Properties["NestedProperty2"].Required, Is.True);
		}
    }
}
