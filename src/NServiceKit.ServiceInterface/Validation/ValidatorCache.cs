using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using NServiceKit.FluentValidation;
using NServiceKit.ServiceHost;

namespace NServiceKit.ServiceInterface.Validation
{
    /// <summary>A validator cache.</summary>
    public static class ValidatorCache
    {
        private static Dictionary<Type, ResolveValidatorDelegate> delegateCache 
        = new Dictionary<Type, ResolveValidatorDelegate>();
        
        private delegate IValidator ResolveValidatorDelegate(IHttpRequest httpReq);

        /// <summary>Gets a validator.</summary>
        ///
        /// <param name="httpReq">The HTTP request.</param>
        /// <param name="type">   The type.</param>
        ///
        /// <returns>The validator.</returns>
        public static IValidator GetValidator(IHttpRequest httpReq, Type type)
        {
            ResolveValidatorDelegate parseFn;
            if (delegateCache.TryGetValue(type, out parseFn)) return parseFn.Invoke(httpReq);			

            var genericType = typeof(ValidatorCache<>).MakeGenericType(type);
            var mi = genericType.GetMethod("GetValidator", BindingFlags.Public | BindingFlags.Static);
            parseFn = (ResolveValidatorDelegate)Delegate.CreateDelegate(typeof(ResolveValidatorDelegate), mi);

            Dictionary<Type, ResolveValidatorDelegate> snapshot, newCache;
            do
            {
                snapshot = delegateCache;
                newCache = new Dictionary<Type, ResolveValidatorDelegate>(delegateCache);
                newCache[type] = parseFn;

            } while (!ReferenceEquals(
            Interlocked.CompareExchange(ref delegateCache, newCache, snapshot), snapshot));

            return parseFn.Invoke(httpReq);
        }		
    }

    /// <summary>A validator cache.</summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    public class ValidatorCache<T>
    {
        /// <summary>Gets a validator.</summary>
        ///
        /// <param name="httpReq">The HTTP request.</param>
        ///
        /// <returns>The validator.</returns>
        public static IValidator GetValidator(IHttpRequest httpReq)
        {
            return httpReq.TryResolve<IValidator<T>>();
        }
    }

}