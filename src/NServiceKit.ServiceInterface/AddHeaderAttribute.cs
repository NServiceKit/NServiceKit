using System;
using System.Net;
using NServiceKit.Common.Web;
using NServiceKit.ServiceHost;

namespace NServiceKit.ServiceInterface
{
    /// <summary>Attribute for add header.</summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class AddHeaderAttribute : RequestFilterAttribute
    {
        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>Gets or sets the value.</summary>
        ///
        /// <value>The value.</value>
        public string Value { get; set; }

        /// <summary>Gets or sets the status.</summary>
        ///
        /// <value>The status.</value>
        public HttpStatusCode Status
        {
            get { return (HttpStatusCode) StatusCode.GetValueOrDefault(200); }
            set { StatusCode = (int) value; }
        }

        /// <summary>Gets or sets the status code.</summary>
        ///
        /// <value>The status code.</value>
        public int? StatusCode { get; set; }

        /// <summary>Gets or sets information describing the status.</summary>
        ///
        /// <value>Information describing the status.</value>
        public string StatusDescription { get; set; }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.AddHeaderAttribute class.</summary>
        public AddHeaderAttribute() { }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.AddHeaderAttribute class.</summary>
        ///
        /// <param name="name"> The name.</param>
        /// <param name="value">The value.</param>
        public AddHeaderAttribute(string name, string value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.AddHeaderAttribute class.</summary>
        ///
        /// <param name="status">           The status.</param>
        /// <param name="statusDescription">Information describing the status.</param>
        public AddHeaderAttribute(HttpStatusCode status, string statusDescription=null)
        {
            Status = status;
            StatusDescription = statusDescription;
        }

        /// <summary>This method is only executed if the HTTP method matches the <see cref="ApplyTo"/> property.</summary>
        ///
        /// <param name="req">       The http request wrapper.</param>
        /// <param name="res">       The http response wrapper.</param>
        /// <param name="requestDto">The request DTO.</param>
        public override void Execute(IHttpRequest req, IHttpResponse res, object requestDto)
        {
            if (StatusCode != null)
            {
                res.StatusCode = StatusCode.Value;
            }

            if (StatusDescription != null)
            {
                res.StatusDescription = StatusDescription;
            }

            if (!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Value))
            {
                if (Name.Equals(HttpHeaders.ContentType, StringComparison.InvariantCultureIgnoreCase))
                {
                    res.ContentType = Value;
                }
                else
                {
                    res.AddHeader(Name, Value);
                }
            }
        }

        /// <summary>Gets or sets the type of the content.</summary>
        ///
        /// <value>The type of the content.</value>
        public string ContentType
        {
            get { return Name == HttpHeaders.ContentType ? Value : null; }
            set
            {
                Name = HttpHeaders.ContentType;
                Value = value;
            }
        }

        /// <summary>Gets or sets the content encoding.</summary>
        ///
        /// <value>The content encoding.</value>
        public string ContentEncoding
        {
            get { return Name == HttpHeaders.ContentEncoding ? Value : null; }
            set
            {
                Name = HttpHeaders.ContentEncoding;
                Value = value;
            }
        }

        /// <summary>Gets or sets the length of the content.</summary>
        ///
        /// <value>The length of the content.</value>
        public string ContentLength
        {
            get { return Name == HttpHeaders.ContentLength ? Value : null; }
            set
            {
                Name = HttpHeaders.ContentLength;
                Value = value;
            }
        }

        /// <summary>Gets or sets the content disposition.</summary>
        ///
        /// <value>The content disposition.</value>
        public string ContentDisposition
        {
            get { return Name == HttpHeaders.ContentDisposition ? Value : null; }
            set
            {
                Name = HttpHeaders.ContentDisposition;
                Value = value;
            }
        }

        /// <summary>Gets or sets the location.</summary>
        ///
        /// <value>The location.</value>
        public string Location
        {
            get { return Name == HttpHeaders.Location ? Value : null; }
            set
            {
                Name = HttpHeaders.Location;
                Value = value;
            }
        }

        /// <summary>Gets or sets the set cookie.</summary>
        ///
        /// <value>The set cookie.</value>
        public string SetCookie
        {
            get { return Name == HttpHeaders.SetCookie ? Value : null; }
            set
            {
                Name = HttpHeaders.SetCookie;
                Value = value;
            }
        }

        /// <summary>Gets or sets the tag.</summary>
        ///
        /// <value>The e tag.</value>
        public string ETag
        {
            get { return Name == HttpHeaders.ETag ? Value : null; }
            set
            {
                Name = HttpHeaders.ETag;
                Value = value;
            }
        }

        /// <summary>Gets or sets the cache control.</summary>
        ///
        /// <value>The cache control.</value>
        public string CacheControl
        {
            get { return Name == HttpHeaders.CacheControl ? Value : null; }
            set
            {
                Name = HttpHeaders.CacheControl;
                Value = value;
            }
        }

        /// <summary>Gets or sets the last modified.</summary>
        ///
        /// <value>The last modified.</value>
        public string LastModified
        {
            get { return Name == HttpHeaders.LastModified ? Value : null; }
            set
            {
                Name = HttpHeaders.LastModified;
                Value = value;
            }
        }

    }
}