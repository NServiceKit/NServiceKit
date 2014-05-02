using System;
using NServiceKit.Common.Utils;

namespace NServiceKit
{
    /// <summary>A model configuration.</summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    public class ModelConfig<T>
    {
        /// <summary>Identifiers the given get identifier function.</summary>
        ///
        /// <param name="getIdFn">The get identifier function.</param>
        public static void Id(Func<T, object> getIdFn)
        {
            IdUtils<T>.CanGetId = getIdFn;
        }
    }
}