using System;

namespace NServiceKit.ServiceClient.Web
{
    /// <summary>A HTTP method.</summary>
    [Obsolete("Moved to NServiceKit.Common.Web.HttpMethods")]
    public static class HttpMethod
    {
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
    }
}