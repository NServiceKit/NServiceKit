using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using NServiceKit.Common.Web;
using NServiceKit.Configuration;

namespace NServiceKit.ServiceHost
{
    /// <summary>A HTTP request context.</summary>
	public class HttpRequestContext
		: IRequestContext
	{
		private readonly IHttpRequest httpReq;
		private readonly IHttpResponse httpRes;

        /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.HttpRequestContext class.</summary>
        ///
        /// <param name="dto">The dto.</param>
		public HttpRequestContext(object dto)
			: this(dto, null)
		{
		}

        /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.HttpRequestContext class.</summary>
        ///
        /// <param name="dto">               The dto.</param>
        /// <param name="endpointAttributes">The endpoint attributes.</param>
		public HttpRequestContext(object dto, EndpointAttributes endpointAttributes)
			: this(dto, endpointAttributes, null)
		{
		}

        /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.HttpRequestContext class.</summary>
        ///
        /// <param name="httpReq">The HTTP request.</param>
        /// <param name="httpRes">The HTTP resource.</param>
        /// <param name="dto">    The dto.</param>
		public HttpRequestContext(IHttpRequest httpReq, IHttpResponse httpRes, object dto)
			: this(httpReq, httpRes, dto, EndpointAttributes.None)
		{
		}

        /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.HttpRequestContext class.</summary>
        ///
        /// <param name="httpReq">           The HTTP request.</param>
        /// <param name="httpRes">           The HTTP resource.</param>
        /// <param name="dto">               The dto.</param>
        /// <param name="endpointAttributes">The endpoint attributes.</param>
		public HttpRequestContext(IHttpRequest httpReq, IHttpResponse httpRes, object dto, EndpointAttributes endpointAttributes)
			: this(dto, endpointAttributes, null)
		{
			this.httpReq = httpReq;
			this.httpRes = httpRes;
			if (this.httpReq != null)
			{
				this.Files = httpReq.Files;
			}
			if (HttpContext.Current == null && httpReq != null)
			{
				this.RequestAttributes = new RequestAttributes(httpReq);
			}
		}

        /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.HttpRequestContext class.</summary>
        ///
        /// <param name="requestDto">The request dto.</param>
        /// <param name="factory">   The factory.</param>
		public HttpRequestContext(object requestDto, IFactoryProvider factory)
			: this(requestDto, EndpointAttributes.None, factory)
		{
		}

        /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.HttpRequestContext class.</summary>
        ///
        /// <param name="dto">               The dto.</param>
        /// <param name="endpointAttributes">The endpoint attributes.</param>
        /// <param name="factory">           The factory.</param>
		public HttpRequestContext(object dto, EndpointAttributes endpointAttributes, IFactoryProvider factory)
		{
			this.Dto = dto;
			this.EndpointAttributes = endpointAttributes;
			this.Factory = factory;
			this.RequestAttributes = new RequestAttributes(HttpContext.Current);
			this.Files = new IFile[0];
		}

        /// <summary>Gets or sets a value indicating whether the automatic dispose.</summary>
        ///
        /// <value>true if automatic dispose, false if not.</value>
		public bool AutoDispose { get; set; }

        /// <summary>Gets or sets the dto.</summary>
        ///
        /// <value>The dto.</value>
		public object Dto { get; set; }

        /// <summary>Gets the cookies.</summary>
        ///
        /// <value>The cookies.</value>
		public IDictionary<string, System.Net.Cookie> Cookies
		{
			get { return this.httpReq.Cookies; }
		}

        /// <summary>Gets the endpoint attributes.</summary>
        ///
        /// <value>The endpoint attributes.</value>
		public EndpointAttributes EndpointAttributes { get; private set; }

        /// <summary>Gets the request attributes.</summary>
        ///
        /// <value>The request attributes.</value>
		public IRequestAttributes RequestAttributes { get; private set; }

        /// <summary>Gets the type of the content.</summary>
        ///
        /// <value>The type of the content.</value>
		public string ContentType
		{
			get { return this.httpReq.ContentType; }
		}

	    private string responseContentType;

        /// <summary>Gets or sets the type of the response content.</summary>
        ///
        /// <value>The type of the response content.</value>
		public string ResponseContentType
		{
            get { return responseContentType ?? this.httpReq.ResponseContentType; }
            set { responseContentType = value; }
		}

        /// <summary>Gets the get.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        ///
        /// <returns>A T.</returns>
		public T Get<T>() where T : class
		{
			if (typeof(T) == typeof(IHttpRequest))
				return (T)this.httpReq;
			if (typeof(T) == typeof(IHttpResponse))
				return (T)this.httpRes;

			var isDto = this.Dto as T;
			return isDto ?? (this.Factory != null ? this.Factory.Resolve<T>() : null);
		}

        /// <summary>Gets a header.</summary>
        ///
        /// <param name="headerName">Name of the header.</param>
        ///
        /// <returns>The header.</returns>
		public string GetHeader(string headerName)
		{
			return this.httpReq.Headers.Get(headerName);
		}

        /// <summary>Gets or sets the factory.</summary>
        ///
        /// <value>The factory.</value>
		public IFactoryProvider Factory { get; set; }

        /// <summary>Gets the type of the mime.</summary>
        ///
        /// <value>The type of the mime.</value>
		public string MimeType
		{
			get
			{
				if ((this.EndpointAttributes & EndpointAttributes.Json) == EndpointAttributes.Json)
					return MimeTypes.Json;

				if ((this.EndpointAttributes & EndpointAttributes.Xml) == EndpointAttributes.Xml)
					return MimeTypes.Xml;

				if ((this.EndpointAttributes & EndpointAttributes.Jsv) == EndpointAttributes.Jsv)
					return MimeTypes.Jsv;

				if ((this.EndpointAttributes & EndpointAttributes.Csv) == EndpointAttributes.Csv)
					return MimeTypes.Csv;

				if ((this.EndpointAttributes & EndpointAttributes.ProtoBuf) == EndpointAttributes.ProtoBuf)
					return MimeTypes.ProtoBuf;

				return null;
			}
		}

        /// <summary>Gets the type of the compression.</summary>
        ///
        /// <value>The type of the compression.</value>
		public string CompressionType
		{
			get
			{
				if (this.RequestAttributes.AcceptsDeflate)
					return CompressionTypes.Deflate;

				if (this.RequestAttributes.AcceptsGzip)
					return CompressionTypes.GZip;

				return null;
			}
		}

        /// <summary>Gets URI of the absolute.</summary>
        ///
        /// <value>The absolute URI.</value>
		public string AbsoluteUri
		{
			get
			{
				return this.httpReq != null ? this.httpReq.AbsoluteUri : null;
			}
		}

        /// <summary>Gets information describing the path.</summary>
        ///
        /// <value>Information describing the path.</value>
		public string PathInfo
		{
			get { return this.httpReq != null ? this.httpReq.PathInfo : null; }
		}

        /// <summary>Gets or sets the files.</summary>
        ///
        /// <value>The files.</value>
		public IFile[] Files { get; set; }

		private string ipAddress;

        /// <summary>Gets the IP address.</summary>
        ///
        /// <value>The IP address.</value>
		public string IpAddress
		{
			get
			{
				if (ipAddress == null)
				{
					ipAddress = GetIpAddress();
				}
				return ipAddress;
			}
		}

        /// <summary>Gets IP address.</summary>
        ///
        /// <returns>The IP address.</returns>
		public static string GetIpAddress()
		{
			return HttpContext.Current != null
				? HttpContext.Current.Request.UserHostAddress
				: null;
		}

        /// <summary>Releases the unmanaged resources used by the NServiceKit.ServiceHost.HttpRequestContext and optionally releases the managed resources.</summary>
		public void Dispose()
		{
			Dispose(true);
		}

        /// <summary>Releases the unmanaged resources used by the NServiceKit.ServiceHost.HttpRequestContext and optionally releases the managed resources.</summary>
        ///
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
		public virtual void Dispose(bool disposing)
		{
			if (disposing)
				GC.SuppressFinalize(this);

			if (this.Factory != null)
			{
				this.Factory.Dispose();
			}
		}
	}
}