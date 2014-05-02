using System;
using NServiceKit.Common;
using NServiceKit.Common.Web;
using NServiceKit.Redis;
using NServiceKit.Text;

namespace NServiceKit.ServiceInterface.Validation
{
    /// <summary>An execute once only.</summary>
    public class ExecOnceOnly : IDisposable
    {
        private const string Flag = "Y";

        private readonly string hashKey;

        private readonly string correlationId;

        private readonly IRedisClient redis;

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.Validation.ExecOnceOnly class.</summary>
        ///
        /// <param name="redisManager"> Manager for redis.</param>
        /// <param name="forType">      Type of for.</param>
        /// <param name="correlationId">Identifier for the correlation.</param>
        public ExecOnceOnly(IRedisClientsManager redisManager, Type forType, string correlationId)
            : this(redisManager, "hash:nx:" + forType.Name, correlationId) { }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.Validation.ExecOnceOnly class.</summary>
        ///
        /// <param name="redisManager"> Manager for redis.</param>
        /// <param name="forType">      Type of for.</param>
        /// <param name="correlationId">Identifier for the correlation.</param>
        public ExecOnceOnly(IRedisClientsManager redisManager, Type forType, Guid? correlationId)
            : this(redisManager, "hash:nx:" + forType.Name, (correlationId.HasValue ? correlationId.Value.ToString("N") : null)) { }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.Validation.ExecOnceOnly class.</summary>
        ///
        /// <exception cref="Conflict">Thrown when a conflict error condition occurs.</exception>
        ///
        /// <param name="redisManager"> Manager for redis.</param>
        /// <param name="hashKey">      The hash key.</param>
        /// <param name="correlationId">Identifier for the correlation.</param>
        public ExecOnceOnly(IRedisClientsManager redisManager, string hashKey, string correlationId)
        {
            redisManager.ThrowIfNull("redisManager");
            hashKey.ThrowIfNull("hashKey");

            this.hashKey = hashKey;
            this.correlationId = correlationId;

            if (correlationId != null)
            {
                redis = redisManager.GetClient();
                var exists = !redis.SetEntryInHashIfNotExists(hashKey, correlationId, Flag);
                if (exists)
                    throw HttpError.Conflict("Request {0} has already been processed".Fmt(correlationId));
            }
        }

        /// <summary>Gets a value indicating whether the executed.</summary>
        ///
        /// <value>true if executed, false if not.</value>
        public bool Executed { get; private set; }

        /// <summary>Commits this object.</summary>
        public void Commit()
        {
            this.Executed = true;
        }

        /// <summary>Rollbacks this object.</summary>
        public void Rollback()
        {
            if (redis == null) return;

            redis.RemoveEntryFromHash(hashKey, correlationId);
            this.Executed = false;
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            if (correlationId != null && !Executed)
            {
                Rollback();
            }
            if (redis != null)
            {
                redis.Dispose(); //release back into the pool.
            }
        }
    }
}