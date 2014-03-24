using NServiceKit.Redis.Pipeline;

namespace NServiceKit.Redis
{
    /// <summary>
    /// Base transaction interface, shared by typed and non-typed transactions
    /// </summary>
    public interface IRedisTransactionBase : IRedisPipelineShared
    {
    }
}
