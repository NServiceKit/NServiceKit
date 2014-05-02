#if !SILVERLIGHT && !MONOTOUCH && !XBOX && !ANDROIDINDIE
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace NServiceKit.ServiceClient.Web
{
    /// <summary>Interface for duplex.</summary>
    [ServiceContract(Namespace = "http://services.NServiceKit.net/", CallbackContract = typeof(IDuplexCallback))]
    public interface IDuplex
    {
        /// <summary>Begins a send.</summary>
        ///
        /// <param name="msg">The message.</param>
        [OperationContract(Action = "*", ReplyAction = "*")]
        void BeginSend(Message msg);
    }
}
#endif