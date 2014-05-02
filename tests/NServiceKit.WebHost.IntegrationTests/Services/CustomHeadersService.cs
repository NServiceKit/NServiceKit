using System.Runtime.Serialization;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.IntegrationTests.Services
{
    /// <summary>A custom headers.</summary>
    [Route("/customHeaders")]
    [DataContract]
    public class CustomHeaders : IReturn<CustomHeadersResponse>
    { }

    /// <summary>A custom headers response.</summary>
    [DataContract]
    public class CustomHeadersResponse
    {
        /// <summary>Gets or sets the foo.</summary>
        ///
        /// <value>The foo.</value>
        [DataMember(Order = 1)]
        public string Foo { get; set; }

        /// <summary>Gets or sets the bar.</summary>
        ///
        /// <value>The bar.</value>
        [DataMember(Order = 2)]
        public string Bar { get; set; }
    }

    /// <summary>A custom headers service.</summary>
    public class CustomHeadersService : ServiceInterface.Service
    {
        /// <summary>Anies the given c.</summary>
        ///
        /// <param name="c">The CustomHeaders to process.</param>
        ///
        /// <returns>A CustomHeadersResponse.</returns>
        public CustomHeadersResponse Any(CustomHeaders c)
        {
            var response = new CustomHeadersResponse
                {
                    Foo = Request.Headers["Foo"], 
                    Bar = Request.Headers["Bar"]
                };
            return response;
        }

    }
}