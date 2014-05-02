using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Net;
using NServiceKit.Messaging;
using NServiceKit.Text;

namespace NServiceKit.ServiceHost
{
    /// <summary>A mq request context.</summary>
    public class MqRequestContext : IRequestContext
    {
        /// <summary>Gets or sets the resolver.</summary>
        ///
        /// <value>The resolver.</value>
        public IResolver Resolver { get; set; }

        /// <summary>Gets or sets the message.</summary>
        ///
        /// <value>The message.</value>
        public IMessage Message { get; set; }

        /// <summary>Gets or sets the request.</summary>
        ///
        /// <value>The request.</value>
        public MqRequest Request { get; set; }

        /// <summary>Gets or sets the response.</summary>
        ///
        /// <value>The response.</value>
        public MqResponse Response { get; set; }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.MqRequestContext class.</summary>
        public MqRequestContext()
            : this(null, new Message()) {}

        /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.MqRequestContext class.</summary>
        ///
        /// <param name="resolver">The resolver.</param>
        /// <param name="message"> The message.</param>
        public MqRequestContext(IResolver resolver, IMessage message)
        {
            this.Resolver = resolver;
            this.Message = message;
            this.ContentType = this.ResponseContentType = Common.Web.ContentType.Json;
            if (message.Body != null)
                this.PathInfo = "/json/oneway/" + OperationName;
            
            this.Request = new MqRequest(this);
            this.Response = new MqResponse(this);
        }

        private string operationName;

        /// <summary>Gets or sets the name of the operation.</summary>
        ///
        /// <value>The name of the operation.</value>
        public string OperationName
        {
            get { return operationName ?? (operationName = Message.Body != null ? Message.Body.GetType().Name : null); }
            set { operationName = value; }
        }

        /// <summary>Gets the get.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        ///
        /// <returns>A T.</returns>
        public T Get<T>() where T : class
        {
            if (typeof(T) == typeof(IHttpRequest))
                return (T)(object)Request;

            if (typeof(T) == typeof(IHttpResponse))
                return (T)(object)Response;

            return Resolver.TryResolve<T>();
        }

        /// <summary>Gets or sets the IP address.</summary>
        ///
        /// <value>The IP address.</value>
        public string IpAddress { get; set; }

        /// <summary>Gets a header.</summary>
        ///
        /// <param name="headerName">Name of the header.</param>
        ///
        /// <returns>The header.</returns>
        public string GetHeader(string headerName)
        {
            string headerValue;
            Headers.TryGetValue(headerName, out headerValue);
            return headerValue;
        }

        private Dictionary<string, string> headers;

        /// <summary>Gets the headers.</summary>
        ///
        /// <value>The headers.</value>
        public Dictionary<string, string> Headers
        {
            get
            {
                if (headers != null)
                {
                    headers = Message.ToHeaders();
                }
                return headers;
            }
        }

        /// <summary>Gets the cookies.</summary>
        ///
        /// <value>The cookies.</value>
        public IDictionary<string, Cookie> Cookies
        {
            get { return new Dictionary<string, Cookie>(); }
        }

        /// <summary>Gets the endpoint attributes.</summary>
        ///
        /// <value>The endpoint attributes.</value>
        public EndpointAttributes EndpointAttributes
        {
            get { return EndpointAttributes.LocalSubnet | EndpointAttributes.MessageQueue; }
        }

        /// <summary>Gets or sets the request attributes.</summary>
        ///
        /// <value>The request attributes.</value>
        public IRequestAttributes RequestAttributes { get; set; }

        /// <summary>Gets or sets the type of the content.</summary>
        ///
        /// <value>The type of the content.</value>
        public string ContentType { get; set; }

        /// <summary>Gets or sets the type of the response content.</summary>
        ///
        /// <value>The type of the response content.</value>
        public string ResponseContentType { get; set; }

        /// <summary>Gets or sets the type of the compression.</summary>
        ///
        /// <value>The type of the compression.</value>
        public string CompressionType { get; set; }

        /// <summary>Gets or sets URI of the absolute.</summary>
        ///
        /// <value>The absolute URI.</value>
        public string AbsoluteUri { get; set; }

        /// <summary>Gets or sets information describing the path.</summary>
        ///
        /// <value>Information describing the path.</value>
        public string PathInfo { get; set; }

        /// <summary>Gets or sets the files.</summary>
        ///
        /// <value>The files.</value>
        public IFile[] Files { get; set; }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
        }
    }


    /// <summary>A mq extensions.</summary>
    public static class MqExtensions
    {
        /// <summary>An IMessage extension method that converts a message to the headers.</summary>
        ///
        /// <param name="message">The message to act on.</param>
        ///
        /// <returns>message as a Dictionary&lt;string,string&gt;</returns>
        public static Dictionary<string,string> ToHeaders(this IMessage message)
        {
            var map = new Dictionary<string, string>
            {
                {"CreatedDate",message.CreatedDate.ToLongDateString()},
                {"Priority",message.Priority.ToString(CultureInfo.InvariantCulture)},
                {"RetryAttempts",message.RetryAttempts.ToString(CultureInfo.InvariantCulture)},
                {"ReplyId",message.ReplyId.HasValue ? message.ReplyId.Value.ToString() : null},
                {"ReplyTo",message.ReplyTo},
                {"Options",message.Options.ToString(CultureInfo.InvariantCulture)},
                {"Error",message.Error.Dump()},
            };
            return map;
        }
    }
}