using System.Drawing;
using System.Net;
using System.Runtime.Serialization;
using NServiceKit.Api.Swagger;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.IntegrationTests.Services
{
    /// <summary>A swagger test.</summary>
    [Api("SwaggerTest Service Description")]
    [ApiResponse(HttpStatusCode.BadRequest, "Your request was not understood")]
    [ApiResponse(HttpStatusCode.InternalServerError, "Oops, something broke")]
    [Route("/swagger", "GET", Summary = @"GET / Summary", Notes = "GET / Notes")]
    [Route("/swagger/{Name}", "GET", Summary = @"GET Summary", Notes = "GET /Name Notes")]
    [Route("/swagger/{Name}", "POST", Summary = @"POST Summary", Notes = "POST /Name Notes")]
    [DataContract]
    public class SwaggerTest
    {
        /// <summary>Gets or sets the color.</summary>
        ///
        /// <value>The color.</value>
        [ApiMember(Description = "Color Description",
                   ParameterType = "path", DataType = "string", IsRequired = true)]
        [ApiAllowableValues("Color", typeof(Color))] //Enum
        public string Color { get; set; }

        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
        [ApiMember(Description = "Aliased Description",
                   ParameterType = "path", DataType = "string", IsRequired = true)]
        [DataMember(Name = "Aliased")]
        public string Name { get; set; }

        /// <summary>Gets or sets the not aliased.</summary>
        ///
        /// <value>The not aliased.</value>
        [ApiMember(Description = "Not Aliased Description",
                   ParameterType = "path", DataType = "string", IsRequired = true)]
        public string NotAliased { get; set; }
    }

    /// <summary>A swagger test service.</summary>
    public class SwaggerTestService : ServiceInterface.Service
    {
         /// <summary>Gets the given request.</summary>
         ///
         /// <param name="request">The request to get.</param>
         ///
         /// <returns>An object.</returns>
         public object Get(SwaggerTest request)
         {
             return request;
         }
    }
}