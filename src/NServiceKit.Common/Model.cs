using NServiceKit.Common.Utils;

namespace NServiceKit
{
    /// <summary>A model.</summary>
    public static class Model
    {
        /// <summary>A T extension method that converts an entity to an identifier.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="entity">The entity to act on.</param>
        ///
        /// <returns>entity as an object.</returns>
        public static object ToId<T>(this T entity)
        {
            return entity.GetId();
        }

        /// <summary>A T extension method that converts an entity to an URN.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="id">The identifier.</param>
        ///
        /// <returns>entity as a string.</returns>
        public static string ToUrn<T>(object id)
        {
            return IdUtils.CreateUrn<T>(id);
        }

        /// <summary>A T extension method that converts an entity to an URN.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="entity">The entity to act on.</param>
        ///
        /// <returns>entity as a string.</returns>
        public static string ToUrn<T>(this T entity)
        {
            return entity.CreateUrn();
        }

        /// <summary>Converts an idValue to a safe path cache key.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="idValue">The identifier value.</param>
        ///
        /// <returns>idValue as a string.</returns>
        public static string ToSafePathCacheKey<T>(string idValue)
        {
            return IdUtils.CreateCacheKeyPath<T>(idValue);
        }
    }
}