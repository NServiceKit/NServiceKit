#if !SILVERLIGHT && !MONOTOUCH && !XBOX && !ANDROIDINDIE
using System.ServiceModel;
using System.ServiceModel.Channels;
using NServiceKit.ServiceHost;

namespace NServiceKit.ServiceClient.Web
{
    [ServiceContract(Namespace = "http://services.NServiceKit.net/")]
    public interface IOneWay
    {
        [OperationContract(Action = "*", IsOneWay = true)]
        void SendOneWay(Message requestMsg);
    }
}
#endif