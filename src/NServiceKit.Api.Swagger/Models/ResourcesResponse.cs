namespace NServiceKit.Api.Swagger.Models
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Models the response from the swagger resource service.
    /// </summary>
    [DataContract]
    public class ResourcesResponse
    {
        /// <summary>
        /// Gets or sets the api version.
        /// </summary>
        [DataMember(Name = "apiVersion")]
        public string ApiVersion { get; set; }

        /// <summary>
        /// Gets or sets the apis.
        /// </summary>
        [DataMember(Name = "apis")]
        public List<RestService> Apis { get; set; }

        /// <summary>
        /// Gets or sets the base path.
        /// </summary>
        [DataMember(Name = "basePath")]
        public string BasePath { get; set; }

        /// <summary>
        /// Gets or sets the swagger version.
        /// </summary>
        [DataMember(Name = "swaggerVersion")]
        public string SwaggerVersion { get; set; }
    }
}
