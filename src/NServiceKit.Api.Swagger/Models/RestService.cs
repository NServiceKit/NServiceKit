namespace NServiceKit.Api.Swagger.Models
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Models a swagger rest service endpoint.
    /// </summary>
    [DataContract]
    public class RestService
    {
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [DataMember(Name = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        [DataMember(Name = "path")]
        public string Path { get; set; }
    }
}
