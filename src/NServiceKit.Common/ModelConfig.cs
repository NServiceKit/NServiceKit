using System;
using NServiceKit.Common.Utils;

namespace NServiceKit
{
    public class ModelConfig<T>
    {
        public static void Id(Func<T, object> getIdFn)
        {
            IdUtils<T>.CanGetId = getIdFn;
        }
    }
}