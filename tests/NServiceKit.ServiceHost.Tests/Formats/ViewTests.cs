using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NServiceKit.Common.Utils;
using NServiceKit.Common.Web;
using NServiceKit.Html;
using NServiceKit.IO;
using NServiceKit.ServiceHost.Tests.AppData;
using NServiceKit.ServiceInterface.Testing;
using NServiceKit.Text;
using NServiceKit.VirtualPath;
using NServiceKit.WebHost.Endpoints;
using NServiceKit.WebHost.Endpoints.Extensions;
using NServiceKit.WebHost.Endpoints.Formats;
using NServiceKit.WebHost.Endpoints.Support.Markdown;

namespace NServiceKit.ServiceHost.Tests.Formats
{
    /// <summary>A view tests.</summary>
	[TestFixture] 
	public class ViewTests
	{
		private CustomerDetailsResponse response;
		private MarkdownFormat markdownFormat;
		private AppHost appHost;

        /// <summary>Tests fixture set up.</summary>
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			var json = "~/AppData/ALFKI.json".MapProjectPath().ReadAllText();
			response = JsonSerializer.DeserializeFromString<CustomerDetailsResponse>(json);
		}

        /// <summary>Executes the before each test action.</summary>
		[SetUp]
		public void OnBeforeEachTest()
		{
			appHost = new AppHost();
			markdownFormat = new MarkdownFormat {
                VirtualPathProvider = appHost.VirtualPathProvider
            };
			markdownFormat.Register(appHost);
		}

        /// <summary>An application host.</summary>
		public class AppHost : IAppHost
		{
            /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.Tests.Formats.ViewTests.AppHost class.</summary>
			public AppHost()
			{
				this.Config = new EndpointHostConfig {
					HtmlReplaceTokens = new Dictionary<string, string>(),
					IgnoreFormatsInMetadata = new HashSet<string>(),
				};
				this.ContentTypeFilters = HttpResponseFilter.Instance;
				this.ResponseFilters = new List<Action<IHttpRequest, IHttpResponse, object>>();
				this.ViewEngines = new List<IViewEngine>();
				this.CatchAllHandlers = new List<HttpHandlerResolverDelegate>();
				this.VirtualPathProvider = new FileSystemVirtualPathProvider(this, "~".MapProjectPath());
			}

            /// <summary>Registers this object.</summary>
            ///
            /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
            ///
            /// <typeparam name="T">Generic type parameter.</typeparam>
            /// <param name="instance">.</param>
			public void Register<T>(T instance)
			{
				throw new NotImplementedException();
			}

            /// <summary>Registers as.</summary>
            ///
            /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
            ///
            /// <typeparam name="T">  Generic type parameter.</typeparam>
            /// <typeparam name="TAs">Type of as.</typeparam>
			public void RegisterAs<T, TAs>() where T : TAs
			{
				throw new NotImplementedException();
			}

            /// <summary>Allows the clean up for executed autowired services and filters. Calls directly after services and filters are executed.</summary>
            ///
            /// <param name="instance">.</param>
			public virtual void Release(object instance) { }
		    
            /// <summary>Called at the end of each request. Enables Request Scope.</summary>
            public void OnEndRequest() {}

            /// <summary>Register user-defined custom routes.</summary>
            ///
            /// <value>The routes.</value>
            public IServiceRoutes Routes { get; private set; }

            /// <summary>Try resolve.</summary>
            ///
            /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
            ///
            /// <typeparam name="T">Generic type parameter.</typeparam>
            ///
            /// <returns>A T.</returns>
		    public T TryResolve<T>()
			{
				throw new NotImplementedException();
			}

            /// <summary>Register custom ContentType serializers.</summary>
            ///
            /// <value>The content type filters.</value>
			public IContentTypeFilter ContentTypeFilters { get; set; }

            /// <summary>Add Request Filters, to be applied before the dto is deserialized.</summary>
            ///
            /// <value>The pre request filters.</value>
            public List<Action<IHttpRequest, IHttpResponse>> PreRequestFilters { get; set; }

            /// <summary>Add Request Filters.</summary>
            ///
            /// <value>The request filters.</value>
			public List<Action<IHttpRequest, IHttpResponse, object>> RequestFilters { get; set; }

            /// <summary>Add Response Filters.</summary>
            ///
            /// <value>The response filters.</value>
			public List<Action<IHttpRequest, IHttpResponse, object>> ResponseFilters { get; set; }

            /// <summary>Add alternative HTML View Engines.</summary>
            ///
            /// <value>The view engines.</value>
            public List<IViewEngine> ViewEngines { get; set; }

            /// <summary>Provide an exception handler for un-caught exceptions.</summary>
            ///
            /// <value>The exception handler.</value>
            public HandleUncaughtExceptionDelegate ExceptionHandler { get; set; }

            /// <summary>Provide an exception handler for unhandled exceptions.</summary>
            ///
            /// <value>The service exception handler.</value>
            public HandleServiceExceptionDelegate ServiceExceptionHandler { get; set; }

            /// <summary>Provide a catch-all handler that doesn't match any routes.</summary>
            ///
            /// <value>The catch all handlers.</value>
		    public List<HttpHandlerResolverDelegate> CatchAllHandlers { get; set; }

            /// <summary>Provide a custom model minder for a specific Request DTO.</summary>
            ///
            /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
            ///
            /// <value>The request binders.</value>
			public Dictionary<Type, Func<IHttpRequest, object>> RequestBinders
			{
				get { throw new NotImplementedException(); }
			}

            /// <summary>The AppHost config.</summary>
            ///
            /// <value>The configuration.</value>
			public EndpointHostConfig Config { get; set; }

            /// <summary>Register an Adhoc web service on Startup.</summary>
            ///
            /// <param name="serviceType">.</param>
            /// <param name="atRestPaths">.</param>
			public void RegisterService(Type serviceType, params string[] atRestPaths)
			{
				Config.ServiceManager.RegisterService(serviceType);
			}

            /// <summary>List of pre-registered and user-defined plugins to be enabled in this AppHost.</summary>
            ///
            /// <value>The plugins.</value>
		    public List<IPlugin> Plugins { get; private set; }

            /// <summary>Apply plugins to this AppHost.</summary>
            ///
            /// <param name="plugins">.</param>
		    public void LoadPlugin(params IPlugin[] plugins)
			{
				plugins.ToList().ForEach(x => x.Register(this));
			}

            /// <summary>Virtual access to file resources.</summary>
            ///
            /// <value>The virtual path provider.</value>
			public IVirtualPathProvider VirtualPathProvider { get; set; }

            /// <summary>Creates service runner.</summary>
            ///
            /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
            ///
            /// <typeparam name="TRequest">Type of the request.</typeparam>
            /// <param name="actionContext">Context for the action.</param>
            ///
            /// <returns>The new service runner.</returns>
            public IServiceRunner<TRequest> CreateServiceRunner<TRequest>(ActionContext actionContext)
		    {
		        throw new NotImplementedException();
		    }

            /// <summary>Resolve the absolute url for this request.</summary>
            ///
            /// <param name="virtualPath">Full pathname of the virtual file.</param>
            /// <param name="httpReq">    The HTTP request.</param>
            ///
            /// <returns>A string.</returns>
            public virtual string ResolveAbsoluteUrl(string virtualPath, IHttpRequest httpReq)
            {
                return httpReq.GetAbsoluteUrl(virtualPath);
            }
        }

        /// <summary>Gets a HTML.</summary>
        ///
        /// <param name="dto">   The dto.</param>
        /// <param name="format">Describes the format to use.</param>
        ///
        /// <returns>The HTML.</returns>
		public string GetHtml(object dto, string format)
		{
			var httpReq = new MockHttpRequest {
				Headers = new NameValueCollection(),
				OperationName = "OperationName",
				QueryString = new NameValueCollection(),
			};
			httpReq.QueryString.Add("format", format);
			using (var ms = new MemoryStream())
			{
				var httpRes = new HttpResponseStreamWrapper(ms);
                appHost.ViewEngines[0].ProcessRequest(httpReq, httpRes, dto);

				var utf8Bytes = ms.ToArray();
				var html = utf8Bytes.FromUtf8Bytes();
				return html;
			}
		}

        /// <summary>Gets a HTML.</summary>
        ///
        /// <param name="dto">The dto.</param>
        ///
        /// <returns>The HTML.</returns>
		public string GetHtml(object dto)
		{
			return GetHtml(dto, "html");
		}

        /// <summary>Does serve dynamic view HTML page with template.</summary>
		[Test]
		public void Does_serve_dynamic_view_HTML_page_with_template()
		{
			var html = GetHtml(response);

			Console.WriteLine(html);
			//File.WriteAllText("~/AppData/TestsResults/CustomerDetailsResponse.htm".MapAbsolutePath(), html);

			Assert.That(html.StartsWith("<!doctype html>"));
			Assert.That(html.Contains("Customer Orders Total:  $4,596.20"));
		}

        /// <summary>Does serve dynamic view HTML page without template.</summary>
		[Test]
		public void Does_serve_dynamic_view_HTML_page_without_template()
		{
			var html = GetHtml(response, "html.bare");

			Console.WriteLine(html);

			Assert.That(html.TrimStart().StartsWith("<h1>Maria Anders Customer Details (Berlin, Germany)</h1>"));
			Assert.That(html.Contains("Customer Orders Total:  $4,596.20"));
		}

        /// <summary>Does serve dynamic view markdown page with template.</summary>
		[Test]
		public void Does_serve_dynamic_view_Markdown_page_with_template()
		{
			var html = GetHtml(response, "markdown");

			Console.WriteLine(html);
			//File.WriteAllText("~/AppData/TestsResults/CustomerDetailsResponse.txt".MapAbsolutePath(), html);

			Assert.That(html.StartsWith("<!doctype html>"));
			Assert.That(html.Contains("# Maria Anders Customer Details (Berlin, Germany)"));
			Assert.That(html.Contains("Customer Orders Total:  $4,596.20"));
		}

        /// <summary>Does serve dynamic view markdown page without template.</summary>
		[Test]
		public void Does_serve_dynamic_view_Markdown_page_without_template()
		{
			var html = GetHtml(response, "markdown.bare");

			Console.WriteLine(html);

			Assert.That(html.TrimStart().StartsWith("# Maria Anders Customer Details (Berlin, Germany)"));
			Assert.That(html.Contains("Customer Orders Total:  $4,596.20"));
		}


        /// <summary>Does serve dynamic view HTML page with alternate template.</summary>
		[Test]
		public void Does_serve_dynamic_view_HTML_page_with_ALT_template()
		{
			var html = GetHtml(response.Customer);

            html.Print();
			//File.WriteAllText("~/AppData/TestsResults/Customer.htm".MapAbsolutePath(), html);

			Assert.That(html.StartsWith("<!doctype html>"));
			Assert.That(html.Contains("ALT Template"));
			Assert.That(html.Contains("<li><strong>Address:</strong> Obere Str. 57</li>"));
		}

        /// <summary>A mock HTTP response.</summary>
		public class MockHttpResponse : IHttpResponse
		{
            /// <summary>Gets or sets the memory stream.</summary>
            ///
            /// <value>The memory stream.</value>
			public MemoryStream MemoryStream { get; set; }

            /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.Tests.Formats.ViewTests.MockHttpResponse class.</summary>
			public MockHttpResponse()
			{
				this.Headers = new Dictionary<string, string>();
				this.MemoryStream = new MemoryStream();
				this.Cookies = new Cookies(this);
			}

            /// <summary>The underlying ASP.NET or HttpListener HttpResponse.</summary>
            ///
            /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
            ///
            /// <value>The original response.</value>
			public object OriginalResponse
			{
				get { throw new NotImplementedException(); }
			}

            /// <summary>Gets or sets the status code.</summary>
            ///
            /// <value>The status code.</value>
			public int StatusCode { set; get; }

            /// <summary>Gets or sets information describing the status.</summary>
            ///
            /// <value>Information describing the status.</value>
            public string StatusDescription { set; get; }

            /// <summary>Gets or sets the type of the content.</summary>
            ///
            /// <value>The type of the content.</value>
			public string ContentType { get; set; }

			private Dictionary<string, string> Headers { get; set; }

            /// <summary>Gets or sets the cookies.</summary>
            ///
            /// <value>The cookies.</value>
			public ICookies Cookies { get; set; }

            /// <summary>Adds a header to 'value'.</summary>
            ///
            /// <param name="name"> The name.</param>
            /// <param name="value">The value.</param>
			public void AddHeader(string name, string value)
			{
				this.Headers.Add(name, value);
			}

            /// <summary>Redirects the given document.</summary>
            ///
            /// <param name="url">URL of the document.</param>
			public void Redirect(string url)
			{
				this.Headers[HttpHeaders.Location] = url;
			}

            /// <summary>Gets the output stream.</summary>
            ///
            /// <value>The output stream.</value>
			public Stream OutputStream { get { return MemoryStream; } }

            /// <summary>Writes.</summary>
            ///
            /// <param name="text">The text to write.</param>
			public void Write(string text)
			{
				var bytes = Encoding.UTF8.GetBytes(text);
				MemoryStream.Write(bytes, 0, bytes.Length);
			}

            /// <summary>Gets or sets the contents.</summary>
            ///
            /// <value>The contents.</value>
			public string Contents { get; set; }

            /// <summary>
            /// Signal that this response has been handled and no more processing should be done. When used in a request or response filter, no more filters or processing is done on this request.
            /// </summary>
			public void Close()
			{
				this.Contents = Encoding.UTF8.GetString(MemoryStream.ToArray());
				MemoryStream.Close();
				this.IsClosed = true;
			}

            /// <summary>Calls Response.End() on ASP.NET HttpResponse otherwise is an alias for Close(). Useful when you want to prevent ASP.NET to provide it's own custom error page.</summary>
			public void End()
			{
				Close();
			}

            /// <summary>Response.Flush() and OutputStream.Flush() seem to have different behaviour in ASP.NET.</summary>
			public void Flush()
			{
				MemoryStream.Flush();
			}

            /// <summary>Gets a value indicating whether this instance is closed.</summary>
            ///
            /// <value>true if this object is closed, false if not.</value>
			public bool IsClosed { get; private set; }

            /// <summary>Sets content length.</summary>
            ///
            /// <param name="contentLength">Length of the content.</param>
		    public void SetContentLength(long contentLength)
		    {
		        Headers[HttpHeaders.ContentLength] = contentLength.ToString();
		    }
		}

        /// <summary>Does process markdown pages.</summary>
		[Test]
		public void Does_process_Markdown_pages()
		{
            var markdownHandler = new MarkdownHandler("/AppData/NoTemplate/Static") {
				MarkdownFormat = markdownFormat,
			};
			var httpReq = new MockHttpRequest { QueryString = new NameValueCollection() };
			var httpRes = new MockHttpResponse();
			markdownHandler.ProcessRequest(httpReq, httpRes, "Static");

			var expectedHtml = markdownFormat.Transform(
				File.ReadAllText("~/AppData/NoTemplate/Static.md".MapProjectPath()));

			httpRes.Close();
			Assert.That(httpRes.Contents, Is.EqualTo(expectedHtml));
		}

	}

}