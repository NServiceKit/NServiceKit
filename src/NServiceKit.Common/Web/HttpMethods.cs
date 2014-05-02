using System.Collections.Generic;
using System.Linq;
using NServiceKit.ServiceHost;
#if WINDOWS_PHONE && !WP
using NServiceKit.Text.WP;
#endif

namespace NServiceKit.Common.Web
{
    /// <summary>A HTTP methods.</summary>
    public static class HttpMethods
    {
        static readonly string[] allVerbs = new[] {
            "OPTIONS", "GET", "HEAD", "POST", "PUT", "DELETE", "TRACE", "CONNECT", // RFC 2616
            "PROPFIND", "PROPPATCH", "MKCOL", "COPY", "MOVE", "LOCK", "UNLOCK",    // RFC 2518
            "VERSION-CONTROL", "REPORT", "CHECKOUT", "CHECKIN", "UNCHECKOUT",
            "MKWORKSPACE", "UPDATE", "LABEL", "MERGE", "BASELINE-CONTROL", "MKACTIVITY",  // RFC 3253
            "ORDERPATCH", // RFC 3648
            "ACL",        // RFC 3744
            "PATCH",      // https://datatracker.ietf.org/doc/draft-dusseault-http-patch/
            "SEARCH",     // https://datatracker.ietf.org/doc/draft-reschke-webdav-search/
            "BCOPY", "BDELETE", "BMOVE", "BPROPFIND", "BPROPPATCH", "NOTIFY",  
            "POLL",  "SUBSCRIBE", "UNSUBSCRIBE" //MS Exchange WebDav: http://msdn.microsoft.com/en-us/library/aa142917.aspx
        };

        /// <summary>all verbs.</summary>
        public static HashSet<string> AllVerbs = new HashSet<string>(allVerbs);

        /// <summary>Query if 'httpVerb' has verb.</summary>
        ///
        /// <param name="httpVerb">The HTTP verb.</param>
        ///
        /// <returns>true if verb, false if not.</returns>
        public static bool HasVerb(string httpVerb)
        {
#if NETFX_CORE
            return allVerbs.Any(p => p.Equals(httpVerb.ToUpper()));
#else
            return AllVerbs.Contains(httpVerb.ToUpper());
#endif
        }

        /// <summary>The get.</summary>
        public const string Get = "GET";
        /// <summary>The put.</summary>
        public const string Put = "PUT";
        /// <summary>The post.</summary>
        public const string Post = "POST";
        /// <summary>The delete.</summary>
        public const string Delete = "DELETE";
        /// <summary>Options for controlling the operation.</summary>
        public const string Options = "OPTIONS";
        /// <summary>The head.</summary>
        public const string Head = "HEAD";
        /// <summary>The patch.</summary>
        public const string Patch = "PATCH";

        /// <summary>Gets endpoint attribute.</summary>
        ///
        /// <param name="httpMethod">The HTTP method.</param>
        ///
        /// <returns>The endpoint attribute.</returns>
        public static EndpointAttributes GetEndpointAttribute(string httpMethod)
        {
            switch (httpMethod.ToUpper())
            {
                case Get:
                    return EndpointAttributes.HttpGet;
                case Put:
                    return EndpointAttributes.HttpPut;
                case Post:
                    return EndpointAttributes.HttpPost;
                case Delete:
                    return EndpointAttributes.HttpDelete;
                case Patch:
                    return EndpointAttributes.HttpPatch;
                case Head:
                    return EndpointAttributes.HttpHead;
                case Options:
                    return EndpointAttributes.HttpOptions;
            }

            return EndpointAttributes.HttpOther;
        }
    }
}
