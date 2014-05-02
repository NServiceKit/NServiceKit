using System;
using System.Collections.Generic;
using System.Net;
using Funq;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface.Auth;
using NServiceKit.WebHost.Endpoints;

namespace NServiceKit.ServiceInterface.Testing
{
    /// <summary>A mock request context.</summary>
    public class MockRequestContext : IRequestContext
    {
        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.Testing.MockRequestContext class.</summary>
        public MockRequestContext()
        {
            this.Cookies = new Dictionary<string, Cookie>();
            this.Files = new IFile[0];
            this.Container = new Container();
            var httpReq = new MockHttpRequest { Container = this.Container };
            httpReq.AddSessionCookies();
            this.Container.Register<IHttpRequest>(httpReq);
            var httpRes = new MockHttpResponse();
            this.Container.Register<IHttpResponse>(httpRes);
            httpReq.Container = this.Container;
        }

        /// <summary>Gets the get.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        ///
        /// <returns>A T.</returns>
        public T Get<T>() where T : class
        {
            return Container.TryResolve<T>();
        }

        /// <summary>Gets the IP address.</summary>
        ///
        /// <value>The IP address.</value>
        public string IpAddress { get; private set; }

        /// <summary>Gets a header.</summary>
        ///
        /// <param name="headerName">Name of the header.</param>
        ///
        /// <returns>The header.</returns>
        public string GetHeader(string headerName)
        {
            return Get<IHttpRequest>().Headers[headerName];
        }

        /// <summary>Gets the container.</summary>
        ///
        /// <value>The container.</value>
        public Container Container { get; private set; }

        /// <summary>Gets the cookies.</summary>
        ///
        /// <value>The cookies.</value>
        public IDictionary<string, Cookie> Cookies { get; private set; }

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
        public string ContentType { get; private set; }

        /// <summary>Gets or sets the type of the response content.</summary>
        ///
        /// <value>The type of the response content.</value>
        public string ResponseContentType { get; set; }

        /// <summary>Gets the type of the compression.</summary>
        ///
        /// <value>The type of the compression.</value>
        public string CompressionType { get; private set; }

        /// <summary>Gets URI of the absolute.</summary>
        ///
        /// <value>The absolute URI.</value>
        public string AbsoluteUri { get; private set; }

        /// <summary>Gets information describing the path.</summary>
        ///
        /// <value>Information describing the path.</value>
        public string PathInfo { get; private set; }

        /// <summary>Gets the files.</summary>
        ///
        /// <value>The files.</value>
        public IFile[] Files { get; private set; }

        /// <summary>Removes the session.</summary>
        ///
        /// <returns>An AuthUserSession.</returns>
        public AuthUserSession RemoveSession()
        {
            var httpReq = this.Get<IHttpRequest>();
            httpReq.RemoveSession();
            return httpReq.GetSession() as AuthUserSession;
        }

        /// <summary>Reload session.</summary>
        ///
        /// <returns>An AuthUserSession.</returns>
        public AuthUserSession ReloadSession()
        {
            var httpReq = this.Get<IHttpRequest>();
            return httpReq.GetSession() as AuthUserSession;
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
        }

        /// <summary>Creates application host.</summary>
        ///
        /// <returns>The new application host.</returns>
        public IAppHost CreateAppHost()
        {
            return new TestAppHost(this.Container);
        }
    }
}