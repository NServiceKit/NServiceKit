#if !SILVERLIGHT && !MONOTOUCH && !XBOX && !ANDROIDINDIE
using System.ServiceModel;
using System.ServiceModel.Channels;
using NServiceKit.ServiceHost;

namespace NServiceKit.ServiceClient.Web
{
    /// <summary>Interface for synchronise reply.</summary>
    [ServiceContract(Namespace = "http://services.NServiceKit.net/")]
    public interface ISyncReply
    {
        /// <summary>Send this message.</summary>
        ///
        /// <param name="requestMsg">Message describing the request.</param>
        ///
        /// <returns>A Message.</returns>
        [OperationContract(Action = "*", ReplyAction = "*")]
        Message Send(Message requestMsg);
    }
}
#endif