using System.ServiceModel.Channels;

namespace NServiceKit.ServiceHost
{
    public interface IRequiresSoapMessage
    {
        Message Message { get; set; }
    }
}