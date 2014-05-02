#if !SILVERLIGHT && !MONOTOUCH && !XBOX
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace NServiceKit.ServiceModel.Serialization
{
    /// <summary>A key value data contract deserializer.</summary>
    public class KeyValueDataContractDeserializer
    {
        /// <summary>The instance.</summary>
        public static KeyValueDataContractDeserializer Instance = new KeyValueDataContractDeserializer();

        /// <summary>Parses.</summary>
        ///
        /// <param name="nameValues">The name values.</param>
        /// <param name="returnType">Type of the return.</param>
        ///
        /// <returns>An object.</returns>
        public object Parse(NameValueCollection nameValues, Type returnType)
        {
            return Parse(nameValues.ToDictionary(), returnType);
        }

        readonly Dictionary<Type, StringMapTypeDeserializer> typeStringMapSerializerMap
            = new Dictionary<Type, StringMapTypeDeserializer>();

        /// <summary>Parses.</summary>
        ///
        /// <param name="keyValuePairs">The key value pairs.</param>
        /// <param name="returnType">   Type of the return.</param>
        ///
        /// <returns>An object.</returns>
        public object Parse(IDictionary<string, string> keyValuePairs, Type returnType)
        {
            StringMapTypeDeserializer stringMapTypeDeserializer;
            lock (typeStringMapSerializerMap)
            {
                if (!typeStringMapSerializerMap.TryGetValue(returnType, out stringMapTypeDeserializer))
                {
                    stringMapTypeDeserializer = new StringMapTypeDeserializer(returnType);
                    typeStringMapSerializerMap.Add(returnType, stringMapTypeDeserializer);
                }
            }

            return stringMapTypeDeserializer.CreateFromMap(keyValuePairs);
        }

        /// <summary>Parses the given key value pairs.</summary>
        ///
        /// <typeparam name="To">Type of to.</typeparam>
        /// <param name="keyValuePairs">The key value pairs.</param>
        ///
        /// <returns>To.</returns>
        public To Parse<To>(IDictionary<string, string> keyValuePairs)
        {
            return (To)Parse(keyValuePairs, typeof(To));
        }
    }
}
#endif