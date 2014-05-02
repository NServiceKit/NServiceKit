using System;
using System.Collections.Generic;
using NUnit.Framework;
using NServiceKit.ServiceClient.Web;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface.ServiceModel;
using NServiceKit.Text;
using NServiceKit.WebHost.IntegrationTests.Tests;

namespace NServiceKit.WebHost.IntegrationTests.Services
{
    /// <summary>A funq request scope.</summary>
    public class FunqRequestScope
    {
        /// <summary>Number of.</summary>
        public static int Count = 0;
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.IntegrationTests.Services.FunqRequestScope class.</summary>
        public FunqRequestScope() { Count++; }
    }

    /// <summary>A funq singleton scope.</summary>
    public class FunqSingletonScope
    {
        /// <summary>Number of.</summary>
        public static int Count = 0;
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.IntegrationTests.Services.FunqSingletonScope class.</summary>
        public FunqSingletonScope() { Count++; }
    }

    /// <summary>A funq none scope.</summary>
    public class FunqNoneScope
    {
        /// <summary>Number of.</summary>
        public static int Count = 0;
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.IntegrationTests.Services.FunqNoneScope class.</summary>
        public FunqNoneScope() { Count++; }
    }
    
    /// <summary>An IOC scope.</summary>
    public class IocScope { }

    /// <summary>An IOC scope response.</summary>
    public class IocScopeResponse : IHasResponseStatus
    {
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.IntegrationTests.Services.IocScopeResponse class.</summary>
        public IocScopeResponse()
        {
            this.ResponseStatus = new ResponseStatus();
            this.Results = new Dictionary<string, int>();
        }

        /// <summary>Gets or sets the results.</summary>
        ///
        /// <value>The results.</value>
        public Dictionary<string, int> Results { get; set; }

        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
        public ResponseStatus ResponseStatus { get; set; }
    }

    /// <summary>An IOC scope service.</summary>
    [IocRequestFilter]
    public class IocScopeService : ServiceInterface.Service
    {
        /// <summary>Gets or sets the funq request scope.</summary>
        ///
        /// <value>The funq request scope.</value>
        public FunqRequestScope FunqRequestScope { get; set; }

        /// <summary>Gets or sets the funq singleton scope.</summary>
        ///
        /// <value>The funq singleton scope.</value>
        public FunqSingletonScope FunqSingletonScope { get; set; }

        /// <summary>Gets or sets the funq none scope.</summary>
        ///
        /// <value>The funq none scope.</value>
        public FunqNoneScope FunqNoneScope { get; set; }

        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An IocScopeResponse.</returns>
        public IocScopeResponse Any(IocScope request)
        {
            var response = new IocScopeResponse {
                Results = {
                    { typeof(FunqSingletonScope).Name, FunqSingletonScope.Count },
                    { typeof(FunqRequestScope).Name, FunqRequestScope.Count },
                    { typeof(FunqNoneScope).Name, FunqNoneScope.Count },
                },
            };

            return response;
        }

        /// <summary>Number of disposed.</summary>
        public static int DisposedCount = 0;
        /// <summary>The throw errors.</summary>
        public static bool ThrowErrors = false;

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public override void Dispose()
        {
            DisposedCount++;
        }
    }
    
    /// <summary>Attribute for IOC request filter.</summary>
    public class IocRequestFilterAttribute : Attribute, IHasRequestFilter
    {
        /// <summary>Gets or sets the funq singleton scope.</summary>
        ///
        /// <value>The funq singleton scope.</value>
        public FunqSingletonScope FunqSingletonScope { get; set; }

        /// <summary>Gets or sets the funq request scope.</summary>
        ///
        /// <value>The funq request scope.</value>
        public FunqRequestScope FunqRequestScope { get; set; }

        /// <summary>Gets or sets the funq none scope.</summary>
        ///
        /// <value>The funq none scope.</value>
        public FunqNoneScope FunqNoneScope { get; set; }

        /// <summary>Order in which Request Filters are executed. &lt;0 Executed before global request filters &gt;0 Executed after global request filters.</summary>
        ///
        /// <value>The priority.</value>
        public int Priority { get; set; }

        /// <summary>The request filter is executed before the service.</summary>
        ///
        /// <param name="req">       The http request wrapper.</param>
        /// <param name="res">       The http response wrapper.</param>
        /// <param name="requestDto">The request DTO.</param>
        public void RequestFilter(IHttpRequest req, IHttpResponse res, object requestDto)
        {
        }

        /// <summary>A new shallow copy of this filter is used on every request.</summary>
        ///
        /// <returns>An IHasRequestFilter.</returns>
        public IHasRequestFilter Copy()
        {
            return (IHasRequestFilter)this.MemberwiseClone();
        }
    }

    /// <summary>An IOC service tests.</summary>
    [TestFixture]
    public class IocServiceTests
    {
        /// <summary>Does create correct instances per scope.</summary>
        [Test]
        public void Does_create_correct_instances_per_scope()
        {

            var restClient = new JsonServiceClient(Config.NServiceKitBaseUri);
            var response1 = restClient.Get<IocScopeResponse>("iocscope");
            var response2 = restClient.Get<IocScopeResponse>("iocscope");

            Assert.That(response2.Results[typeof(FunqSingletonScope).Name], Is.EqualTo(1));

            var requestScopeCounter = response2.Results[typeof(FunqRequestScope).Name] - response1.Results[typeof(FunqRequestScope).Name];
            Assert.That(requestScopeCounter, Is.EqualTo(1));
            var noneScopeCounter = response2.Results[typeof(FunqNoneScope).Name] - response1.Results[typeof(FunqNoneScope).Name];
            Assert.That(noneScopeCounter, Is.EqualTo(2));
        }
    }
}