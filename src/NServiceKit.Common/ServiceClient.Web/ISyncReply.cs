#if !SILVERLIGHT && !MONOTOUCH && !XBOX && !ANDROIDINDIE
using System.ServiceModel;
using System.ServiceModel.Channels;
using NServiceKit.ServiceHost;

namespace NServiceKit.ServiceClient.Web
{
    [ServiceContract(Namespace = "http://services.NServiceKit.net/")]
    public interface ISyncReply
    {
        [OperationContract(Action = "*", ReplyAction = "*")]
        Message Send(Message requestMsg);
    }
}
#endif