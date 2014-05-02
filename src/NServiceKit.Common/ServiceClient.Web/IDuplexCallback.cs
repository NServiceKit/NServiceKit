#if !SILVERLIGHT && !MONOTOUCH && !XBOX && !ANDROIDINDIE
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace NServiceKit.ServiceClient.Web
{
    /// <summary>Interface for duplex callback.</summary>
    [ServiceContract(Namespace = "http://services.NServiceKit.net/")]
    public interface IDuplexCallback
    {
        /// <summary>Executes the message received action.</summary>
        ///
        /// <param name="msg">The message.</param>
        [OperationContract(Action = "*", ReplyAction = "*")]
        void OnMessageReceived(Message msg);
    }
}
#endif