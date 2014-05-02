using System.ServiceModel.Channels;

namespace NServiceKit.ServiceHost
{
    /// <summary>Interface for requires SOAP message.</summary>
    public interface IRequiresSoapMessage
    {
        /// <summary>Gets or sets the message.</summary>
        ///
        /// <value>The message.</value>
        Message Message { get; set; }
    }
}