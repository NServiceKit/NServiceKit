using System.Runtime.Serialization;
using NServiceKit.Common;
using NServiceKit.ServiceHost;
using NServiceKit.WebHost.IntegrationTests.Tests;

namespace NServiceKit.WebHost.IntegrationTests.Services
{
    /// <summary>A cached prototype buffer email.</summary>
    [DataContract]
    [Route("/cached/protobuf")]
    [Route("/cached/protobuf/{FromAddress}")]
    public class CachedProtoBufEmail
    {
        /// <summary>Gets or sets from address.</summary>
        ///
        /// <value>from address.</value>
        [DataMember(Order = 1)]
        public string FromAddress { get; set; }
    }

    /// <summary>An uncached prototype buffer email.</summary>
    [DataContract]
    [Route("/uncached/protobuf")]
    [Route("/uncached/protobuf/{FromAddress}")]
    public class UncachedProtoBufEmail
    {
        /// <summary>Gets or sets from address.</summary>
        ///
        /// <value>from address.</value>
        [DataMember(Order = 1)]
        public string FromAddress { get; set; }
    }

    class UncachedProtoBufEmailService : ServiceInterface.Service
    {
        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Any(UncachedProtoBufEmail request)
        {
            return new ProtoBufEmail { FromAddress = request.FromAddress ?? "none" };
        }
    }

    class CachedProtoBufEmailService : ServiceInterface.Service
    {
        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Any(CachedProtoBufEmail request)
        {
            return base.RequestContext.ToOptimizedResultUsingCache(this.Cache,
                UrnId.Create<ProtoBufEmail>(request.FromAddress ?? "none"),
                () => new ProtoBufEmail { FromAddress = request.FromAddress ?? "none" });
        }
    }
}