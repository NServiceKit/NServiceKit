using System.Collections.Generic;

namespace NServiceKit.WebHost.Endpoints.Support.Metadata
{
    internal static class XsdTypes
    {
        /// <summary>Gets the xsds.</summary>
        ///
        /// <value>The xsds.</value>
        public static IDictionary<int, string> Xsds { get; private set; }

        static XsdTypes()
        {
            Xsds = new Dictionary<int, string> 
            {
                {1, "Service Types"},
                {0, "Wcf Data Types"},
                {2, "Wcf Collection Types"},
            };
        }
    }
}