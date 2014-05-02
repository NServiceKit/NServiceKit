#if !SILVERLIGHT && !MONOTOUCH && !XBOX && !ANDROIDINDIE
using System.ServiceModel;
using System.ServiceModel.Channels;
using NServiceKit.ServiceHost;

namespace NServiceKit.ServiceClient.Web
{
    /// <summary>Interface for one way.</summary>
    [ServiceContract(Namespace = "http://services.NServiceKit.net/")]
    public interface IOneWay
    {
        /// <summary>Sends an one way.</summary>
        ///
        /// <param name="requestMsg">Message describing the request.</param>
        [OperationContract(Action = "*", IsOneWay = true)]
        void SendOneWay(Message requestMsg);
    }
}
#endif