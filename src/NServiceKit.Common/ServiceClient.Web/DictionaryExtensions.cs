#if !SILVERLIGHT && !MONOTOUCH && !XBOX

using System.Collections.Generic;
using System.Collections.Specialized;

namespace NServiceKit.ServiceModel.Extensions
{
    /// <summary>A dictionary extensions.</summary>
    public static class DictionaryExtensions
    {
        /// <summary>A NameValueCollection extension method that converts the nameValues to a dictionary.</summary>
        ///
        /// <param name="nameValues">The nameValues to act on.</param>
        ///
        /// <returns>nameValues as a Dictionary&lt;string,string&gt;</returns>
        public static Dictionary<string, string> ToDictionary(this NameValueCollection nameValues)
        {
            var map = new Dictionary<string, string>();
            foreach (var key in nameValues.AllKeys)
            {
                if (key == null)
                {
                    //occurs when no value is specified, e.g. 'path/to/page?debug'
                    //throw new ArgumentNullException("key", "nameValues: " + nameValues);
                    continue;
                }

                var values = nameValues.GetValues(key);
                if (values != null && values.Length > 0)
                {
                    map[key] = values[0];
                }
            }
            return map;
        }		
    }

}
#endif