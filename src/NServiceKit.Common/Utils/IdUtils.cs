using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using NServiceKit.Common.Reflection;
using NServiceKit.DesignPatterns.Model;
using NServiceKit.Text;

namespace NServiceKit.Common.Utils
{
    /// <summary>An identifier utilities.</summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    public static class IdUtils<T>
    {
        internal static Func<T, object> CanGetId;

        static IdUtils()
        {

#if !SILVERLIGHT && !MONOTOUCH && !XBOX
            var hasIdInterfaces = typeof(T).FindInterfaces(
                (t, critera) => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IHasId<>), null);

            if (hasIdInterfaces.Length > 0)
            {
                CanGetId = HasId<T>.GetId;
                return;
            }
#endif

            if (typeof(T).IsClass())
            {
                if (typeof(T).GetPropertyInfo(IdUtils.IdField) != null
                    && typeof(T).GetPropertyInfo(IdUtils.IdField).GetMethodInfo() != null)
                {
                    CanGetId = HasPropertyId<T>.GetId;
                    return;
                }

                foreach (var pi in typeof(T).GetPublicProperties()
                    .Where(pi => pi.CustomAttributes()
                             .Cast<Attribute>()
                             .Any(attr => attr.GetType().Name == "PrimaryKeyAttribute")))
                {
                    CanGetId = StaticAccessors<T>.ValueUnTypedGetPropertyTypeFn(pi);
                    return;
                }
            }

            CanGetId = x => x.GetHashCode();
        }

        /// <summary>Gets an identifier.</summary>
        ///
        /// <param name="entity">The entity.</param>
        ///
        /// <returns>The identifier.</returns>
        public static object GetId(T entity)
        {
            return CanGetId(entity);
        }
    }

    internal static class HasPropertyId<TEntity>
    {
        private static readonly Func<TEntity, object> GetIdFn;

        static HasPropertyId()
        {
            var pi = typeof(TEntity).GetPropertyInfo(IdUtils.IdField);
            GetIdFn = StaticAccessors<TEntity>.ValueUnTypedGetPropertyTypeFn(pi);
        }

        /// <summary>Gets an identifier.</summary>
        ///
        /// <param name="entity">The entity.</param>
        ///
        /// <returns>The identifier.</returns>
        public static object GetId(TEntity entity)
        {
            return GetIdFn(entity);
        }
    }

    internal static class HasId<TEntity>
    {
        private static readonly Func<TEntity, object> GetIdFn;

        static HasId()
        {

#if MONOTOUCH || SILVERLIGHT
            GetIdFn = HasPropertyId<TEntity>.GetId;
#else
            var hasIdInterfaces = typeof(TEntity).FindInterfaces(
                (t, critera) => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IHasId<>), null);

            var genericArg = hasIdInterfaces[0].GetGenericArguments()[0];
            var genericType = typeof(HasIdGetter<,>).MakeGenericType(typeof(TEntity), genericArg);

            var oInstanceParam = System.Linq.Expressions.Expression.Parameter(typeof(TEntity), "oInstanceParam");
            var exprCallStaticMethod = System.Linq.Expressions.Expression.Call
                (
                    genericType,
                    "GetId",
                    new Type[0],
                    oInstanceParam
                );
            GetIdFn = System.Linq.Expressions.Expression.Lambda<Func<TEntity, object>>
                (
                    exprCallStaticMethod,
                    oInstanceParam
                ).Compile();
#endif
        }

        /// <summary>Gets an identifier.</summary>
        ///
        /// <param name="entity">The entity.</param>
        ///
        /// <returns>The identifier.</returns>
        public static object GetId(TEntity entity)
        {
            return GetIdFn(entity);
        }
    }

    internal class HasIdGetter<TEntity, TId>
        where TEntity : IHasId<TId>
    {
        /// <summary>Gets an identifier.</summary>
        ///
        /// <param name="entity">The entity.</param>
        ///
        /// <returns>The identifier.</returns>
        public static object GetId(TEntity entity)
        {
            return entity.Id;
        }
    }

    /// <summary>An identifier utilities.</summary>
    public static class IdUtils
    {
        /// <summary>The identifier field.</summary>
        public const string IdField = "Id";

        /// <summary>An object extension method that gets object identifier.</summary>
        ///
        /// <param name="entity">The entity to act on.</param>
        ///
        /// <returns>The object identifier.</returns>
        public static object GetObjectId(this object entity)
        {
            return entity.GetType().GetPropertyInfo(IdField).GetMethodInfo().Invoke(entity, new object[0]);
        }

        /// <summary>A T extension method that gets an identifier.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="entity">The entity to act on.</param>
        ///
        /// <returns>The identifier.</returns>
        public static object GetId<T>(this T entity)
        {
            return IdUtils<T>.GetId(entity);
        }

        /// <summary>A T extension method that creates an URN.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="id">The identifier.</param>
        ///
        /// <returns>The new URN.</returns>
        public static string CreateUrn<T>(object id)
        {
            return string.Format("urn:{0}:{1}", typeof(T).Name.ToLowerInvariant(), id);
        }

        /// <summary>Creates an URN.</summary>
        ///
        /// <param name="type">The type.</param>
        /// <param name="id">  The identifier.</param>
        ///
        /// <returns>The new URN.</returns>
        public static string CreateUrn(Type type, object id)
        {
            return string.Format("urn:{0}:{1}", type.Name.ToLowerInvariant(), id);
        }

        /// <summary>A T extension method that creates an URN.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="entity">The entity to act on.</param>
        ///
        /// <returns>The new URN.</returns>
        public static string CreateUrn<T>(this T entity)
        {
            var id = GetId(entity);
            return string.Format("urn:{0}:{1}", typeof(T).Name.ToLowerInvariant(), id);
        }

        /// <summary>Creates cache key path.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="idValue">The identifier value.</param>
        ///
        /// <returns>The new cache key path.</returns>
        public static string CreateCacheKeyPath<T>(string idValue)
        {
            if (idValue.Length < 4)
            {
                idValue = idValue.PadLeft(4, '0');
            }
            idValue = idValue.Replace(" ", "-");

            var rootDir = typeof(T).Name;
            var dir1 = idValue.Substring(0, 2);
            var dir2 = idValue.Substring(2, 2);

            var path = string.Format("{1}{0}{2}{0}{3}{0}{4}", Text.StringExtensions.DirSeparatorChar,
                rootDir, dir1, dir2, idValue);

            return path;
        }

    }

}