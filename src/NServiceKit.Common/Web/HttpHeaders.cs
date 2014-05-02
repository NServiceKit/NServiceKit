namespace NServiceKit.Common.Web
{
    /// <summary>A HTTP headers.</summary>
    public static class HttpHeaders
    {
        /// <summary>The parameter override prefix.</summary>
        public const string XParamOverridePrefix = "X-Param-Override-";

        /// <summary>The HTTP method override.</summary>
        public const string XHttpMethodOverride = "X-Http-Method-Override";

        /// <summary>Identifier for the user authentication.</summary>
        public const string XUserAuthId = "X-UAId";

        /// <summary>The forwarded for.</summary>
        public const string XForwardedFor = "X-Forwarded-For";

        /// <summary>The real IP.</summary>
        public const string XRealIp = "X-Real-IP";

        /// <summary>The referer.</summary>
        public const string Referer = "Referer";

        /// <summary>The cache control.</summary>
        public const string CacheControl = "Cache-Control";

        /// <summary>if modified since.</summary>
        public const string IfModifiedSince = "If-Modified-Since";

        /// <summary>A match specifying if none.</summary>
        public const string IfNoneMatch = "If-None-Match";

        /// <summary>The last modified.</summary>
        public const string LastModified = "Last-Modified";

        /// <summary>The accept.</summary>
        public const string Accept = "Accept";

        /// <summary>The accept encoding.</summary>
        public const string AcceptEncoding = "Accept-Encoding";

        /// <summary>Type of the content.</summary>
        public const string ContentType = "Content-Type";

        /// <summary>The content encoding.</summary>
        public const string ContentEncoding = "Content-Encoding";

        /// <summary>Length of the content.</summary>
        public const string ContentLength = "Content-Length";

        /// <summary>The content disposition.</summary>
        public const string ContentDisposition = "Content-Disposition";

        /// <summary>The location.</summary>
        public const string Location = "Location";

        /// <summary>The set cookie.</summary>
        public const string SetCookie = "Set-Cookie";

        /// <summary>The tag.</summary>
        public const string ETag = "ETag";

        /// <summary>The authorization.</summary>
        public const string Authorization = "Authorization";

        /// <summary>The WWW authenticate.</summary>
        public const string WwwAuthenticate = "WWW-Authenticate";

        /// <summary>The allow origin.</summary>
        public const string AllowOrigin = "Access-Control-Allow-Origin";

        /// <summary>The allow methods.</summary>
        public const string AllowMethods = "Access-Control-Allow-Methods";

        /// <summary>The allow headers.</summary>
        public const string AllowHeaders = "Access-Control-Allow-Headers";

        /// <summary>The allow credentials.</summary>
        public const string AllowCredentials = "Access-Control-Allow-Credentials";

        /// <summary>The accept ranges.</summary>
        public const string AcceptRanges = "Accept-Ranges";

        /// <summary>The content range.</summary>
        public const string ContentRange = "Content-Range";

        /// <summary>The range.</summary>
        public const string Range = "Range";
    }
}