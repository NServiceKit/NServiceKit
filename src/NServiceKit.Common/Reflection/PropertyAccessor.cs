using System;
using System.Reflection;
using NServiceKit.Text;

namespace NServiceKit.Common.Reflection
{

    /// <summary>A property accessor.</summary>
    public static class PropertyAccessor
    {
        /// <summary>Gets property function.</summary>
        ///
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
        /// <param name="propertyName">Name of the property.</param>
        ///
        /// <returns>The property function.</returns>
        public static Func<TEntity, object> GetPropertyFn<TEntity>(string propertyName)
        {
            return new PropertyAccessor<TEntity>(propertyName).GetPropertyFn();
        }

        //public static Func<object, object> GetPropertyFnByType(Type type, string propertyName)
        //{
        //    var mi = typeof(PropertyAccessor).GetMethod("GetPropertyFn");
        //    var genericMi = mi.MakeGenericMethod(type);
        //    var getPropertyFn = genericMi.Invoke(null, new object[] { propertyName });

        //    return (Func<object, object>)getPropertyFn;
        //}

        /// <summary>Sets property function.</summary>
        ///
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
        /// <param name="propertyName">Name of the property.</param>
        ///
        /// <returns>An Action&lt;TEntity,object&gt;</returns>
        public static Action<TEntity, object> SetPropertyFn<TEntity>(string propertyName)
        {
            return new PropertyAccessor<TEntity>(propertyName).SetPropertyFn();
        }

        //public static Action<object, object> SetPropertyFnByType(Type type, string propertyName)
        //{
        //    var mi = typeof(PropertyAccessor).GetMethod("SetPropertyFn");
        //    var genericMi = mi.MakeGenericMethod(type);
        //    var setPropertyFn = genericMi.Invoke(null, new object[] { propertyName });

        //    return (Action<object, object>)setPropertyFn;
        //}
    }

    /// <summary>A property accessor.</summary>
    /// <typeparam name="TEntity">Type of the entity.</typeparam>
    public class PropertyAccessor<TEntity>
    {
        readonly PropertyInfo pi;

        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>Gets or sets the type of the property.</summary>
        ///
        /// <value>The type of the property.</value>
        public Type PropertyType { get; set; }

        private readonly Func<TEntity, object> getPropertyFn;
        private readonly Action<TEntity, object> setPropertyFn;

        /// <summary>Initializes a new instance of the NServiceKit.Common.Reflection.PropertyAccessor&lt;TEntity&gt; class.</summary>
        ///
        /// <param name="propertyName">Name of the property.</param>
        public PropertyAccessor(string propertyName)
        {
            this.pi = typeof(TEntity).GetPropertyInfo(propertyName);
            this.Name = propertyName;
            this.PropertyType = pi.PropertyType;

            getPropertyFn = StaticAccessors<TEntity>.ValueUnTypedGetPropertyTypeFn(pi);
            setPropertyFn = StaticAccessors<TEntity>.ValueUnTypedSetPropertyTypeFn(pi);
        }

        /// <summary>Gets property function.</summary>
        ///
        /// <returns>The property function.</returns>
        public Func<TEntity, object> GetPropertyFn()
        {
            return getPropertyFn;
        }

        /// <summary>Sets property function.</summary>
        ///
        /// <returns>An Action&lt;TEntity,object&gt;</returns>
        public Action<TEntity, object> SetPropertyFn()
        {
            return setPropertyFn;
        }

        /// <summary>
        /// Func to get the Strongly-typed field
        /// </summary>
        public Func<TEntity, TId> TypedGetPropertyFn<TId>()
        {
            return StaticAccessors<TEntity>.TypedGetPropertyFn<TId>(pi);
        }

        /// <summary>
        /// Required to cast the return ValueType to an object for caching
        /// </summary>
        public Func<TEntity, object> ValueTypedGetPropertyFn<TId>()
        {
            return StaticAccessors<TEntity>.ValueUnTypedGetPropertyFn<TId>(pi);
        }

        /// <summary>Un typed get property function.</summary>
        ///
        /// <typeparam name="TId">Type of the identifier.</typeparam>
        ///
        /// <returns>A Func&lt;object,object&gt;</returns>
        public Func<object, object> UnTypedGetPropertyFn<TId>()
        {
            return StaticAccessors<TEntity>.UnTypedGetPropertyFn<TId>(pi);
        }

        /// <summary>
        /// Func to set the Strongly-typed field
        /// </summary>
        public Action<TEntity, TId> TypedSetPropertyFn<TId>()
        {
            return StaticAccessors<TEntity>.TypedSetPropertyFn<TId>(pi);
        }

        /// <summary>
        /// Required to cast the ValueType to an object for caching
        /// </summary>
        public Action<TEntity, object> ValueTypesSetPropertyFn<TId>()
        {
            return StaticAccessors<TEntity>.ValueUnTypedSetPropertyFn<TId>(pi);
        }

        /// <summary>
        /// Required to cast the ValueType to an object for caching
        /// </summary>
        public Action<object, object> UnTypedSetPropertyFn<TId>()
        {
            return StaticAccessors<TEntity>.UnTypedSetPropertyFn<TId>(pi);
        }
    }
}