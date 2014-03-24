using NServiceKit.Redis.Pipeline;

namespace NServiceKit.Redis.Generic
{
    /// <summary>
    /// Interface to redis typed pipeline
    /// </summary>
    public interface IRedisTypedPipeline<T> : IRedisPipelineShared, IRedisTypedQueueableOperation<T>
    {
    }
}
