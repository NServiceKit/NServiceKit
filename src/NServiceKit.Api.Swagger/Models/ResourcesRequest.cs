namespace NServiceKit.Api.Swagger.Models
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Models a request for retrieveing swagger resources.
    /// </summary>
    [DataContract]
    public class ResourcesRequest
    {
        /// <summary>
        /// Gets or sets the api key.
        /// </summary>
        [DataMember(Name = "apiKey")]
        public string ApiKey { get; set; }
    }
}
