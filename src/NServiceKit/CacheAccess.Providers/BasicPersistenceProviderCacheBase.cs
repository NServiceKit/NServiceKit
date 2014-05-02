using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NServiceKit.Common;
using NServiceKit.Common.Extensions;
using NServiceKit.Common.Utils;
using NServiceKit.DataAccess;
using NServiceKit.Text;

namespace NServiceKit.CacheAccess.Providers
{
    /// <summary>A basic persistence provider cache base.</summary>
	public abstract class BasicPersistenceProviderCacheBase : IPersistenceProviderCache
	{
        /// <summary>Gets or sets the cache client.</summary>
        ///
        /// <value>The cache client.</value>
		public ICacheClient CacheClient { get; set; }

        /// <summary>Initializes a new instance of the NServiceKit.CacheAccess.Providers.BasicPersistenceProviderCacheBase class.</summary>
        ///
        /// <param name="cacheClient">The cache client.</param>
		protected BasicPersistenceProviderCacheBase(ICacheClient cacheClient)
		{
			CacheClient = cacheClient;
		}

        /// <summary>Gets basic persistence provider.</summary>
        ///
        /// <returns>The basic persistence provider.</returns>
		public abstract IBasicPersistenceProvider GetBasicPersistenceProvider();

        /// <summary>Gets by identifier.</summary>
        ///
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
        /// <param name="entityId">Identifier for the entity.</param>
        ///
        /// <returns>The by identifier.</returns>
		public TEntity GetById<TEntity>(object entityId)
			where TEntity : class, new()
		{
			var cacheKey = IdUtils.CreateUrn<TEntity>(entityId);
			var cacheEntity = this.CacheClient.Get<TEntity>(cacheKey);
			if (Equals(cacheEntity, default(TEntity)))
			{
				using (var db = GetBasicPersistenceProvider())
				{
					cacheEntity = db.GetById<TEntity>(entityId);
					this.SetCache(cacheKey, cacheEntity);
				}
			}
			return cacheEntity;
		}

        /// <summary>Sets a cache.</summary>
        ///
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
		public void SetCache<TEntity>(TEntity entity)
			where TEntity : class, new()
		{
			var cacheKey = entity.CreateUrn();
			this.SetCache(cacheKey, entity);
		}

		private void SetCache<TEntity>(string cacheKey, TEntity entity)
		{
			this.CacheClient.Set(cacheKey, entity);
		}

        /// <summary>Stores the given entity.</summary>
        ///
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
		public void Store<TEntity>(TEntity entity)
			where TEntity : class, new()
		{
			var cacheKey = entity.CreateUrn();
			using (var db = GetBasicPersistenceProvider())
			{
				db.Store(entity);
				this.SetCache(cacheKey, entity);
			}
		}

        /// <summary>Stores all.</summary>
        ///
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
        /// <param name="entities">A variable-length parameters list containing entities.</param>
		public void StoreAll<TEntity>(params TEntity[] entities) 
			where TEntity : class, new()
		{
			using (var db = GetBasicPersistenceProvider())
			{
				db.StoreAll(entities);
			}

			foreach (var entity in entities)
			{
				this.SetCache(entity);
			}
		}

        /// <summary>Gets by identifiers.</summary>
        ///
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
        /// <param name="entityIds">List of identifiers for the entities.</param>
        ///
        /// <returns>The by identifiers.</returns>
		public List<TEntity> GetByIds<TEntity>(ICollection entityIds)
			where TEntity : class, new()
		{
			if (entityIds.Count == 0) return new List<TEntity>();

			var cacheKeys = entityIds.ConvertAll(x => x.CreateUrn());
			var cacheEntitiesMap = this.CacheClient.GetAll<TEntity>(cacheKeys);

			if (cacheEntitiesMap.Count < entityIds.Count)
			{
				var entityIdType = entityIds.First().GetType();

				var entityIdsNotInCache = cacheKeys
					.Where(x => !cacheEntitiesMap.ContainsKey(x))
					.ConvertAll(x =>
						TypeSerializer.DeserializeFromString(UrnId.GetStringId(x), entityIdType));

				using (var db = GetBasicPersistenceProvider())
				{
					var cacheEntities = db.GetByIds<TEntity>(entityIdsNotInCache);

					foreach (var cacheEntity in cacheEntities)
					{
						var cacheKey = cacheEntity.CreateUrn();
						this.CacheClient.Set(cacheKey, cacheEntity);
						cacheEntitiesMap[cacheKey] = cacheEntity;
					}
				}
			}

			return cacheEntitiesMap.Values.ToList();
		}

        /// <summary>Clears all described by entityIds.</summary>
        ///
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
        /// <param name="entityIds">List of identifiers for the entities.</param>
		public void ClearAll<TEntity>(ICollection entityIds)
			where TEntity : class, new()
		{
			var cacheKeys = entityIds.ConvertAll(x => IdUtils.CreateUrn<TEntity>(x));
			this.CacheClient.RemoveAll(cacheKeys);
		}

        /// <summary>Clears this object to its blank/initial state.</summary>
        ///
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
        /// <param name="entityIds">List of identifiers for the entities.</param>
		public void Clear<TEntity>(params object[] entityIds)
			where TEntity : class, new()
		{
			this.ClearAll<TEntity>(entityIds.ToList());
		}
	}
}