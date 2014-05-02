using System.Runtime.Serialization;

namespace NServiceKit.ServiceInterface.ServiceModel
{
    /// <summary>
    /// Generic ResponseStatus for when Response Type can't be inferred.
    /// In schemaless formats like JSON, JSV it has the same shape as a typed Response DTO
    /// </summary>

    [DataContract]
    public class ErrorResponse : IHasResponseStatus
    {
        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
        [DataMember(Order = 1)]
        public ResponseStatus ResponseStatus { get; set; }
    }
}