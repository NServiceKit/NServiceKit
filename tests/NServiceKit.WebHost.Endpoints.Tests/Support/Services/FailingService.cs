using System.Runtime.Serialization;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.Endpoints.Tests.Support.Services
{
    /// <summary>A failing request.</summary>
    [DataContract]
    public class FailingRequest { }

    /// <summary>A failing request response.</summary>
    [DataContract]
    public class FailingRequestResponse { }

    /// <summary>A failing service.</summary>
    public class FailingService : ServiceInterface.Service
    {
        private void ThisMethodAlwaysThrowsAnError(FailingRequest request)
        {
            throw new System.ArgumentException("Failure");
        }

        /// <summary>Executes the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>A FailingRequestResponse.</returns>
        public FailingRequestResponse Execute(FailingRequest request)
        {
            ThisMethodAlwaysThrowsAnError(request);
            return new FailingRequestResponse();
        }
    }
}