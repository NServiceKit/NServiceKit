using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NServiceKit.ServiceHost.Tests
{
    /// <summary>A request path.</summary>
    public class RequestPath
    {
        /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.Tests.RequestPath class.</summary>
        ///
        /// <param name="path">    The full pathname of the file.</param>
        /// <param name="host">    The host.</param>
        /// <param name="pathInfo">Information describing the path.</param>
        /// <param name="rawUrl">  The raw URL.</param>
        public RequestPath(string path, string host, string pathInfo, string rawUrl)
        {
            Path = path;
            Host = host;
            PathInfo = pathInfo;
            RawUrl = rawUrl;
            AbsoluteUri = "http://localhost" + rawUrl;
        }

        /// <summary>Gets or sets the full pathname of the file.</summary>
        ///
        /// <value>The full pathname of the file.</value>
        public string Path { get; set; }

        /// <summary>Gets or sets the host.</summary>
        ///
        /// <value>The host.</value>
        public string Host { get; set; }

        /// <summary>Gets or sets information describing the path.</summary>
        ///
        /// <value>Information describing the path.</value>
        public string PathInfo { get; set; }

        /// <summary>Gets or sets URL of the raw.</summary>
        ///
        /// <value>The raw URL.</value>
        public string RawUrl { get; set; }

        /// <summary>Gets or sets URI of the absolute.</summary>
        ///
        /// <value>The absolute URI.</value>
        public string AbsoluteUri { get; set; }
    }

    /// <summary>A service kit handler path tests.</summary>
    [TestFixture]
    public class NServiceKitHandlerPathTests
    {
        /// <summary>Resolve path.</summary>
        ///
        /// <param name="mode">The mode.</param>
        /// <param name="path">Full pathname of the file.</param>
        ///
        /// <returns>A string.</returns>
        public string ResolvePath(string mode, string path)
        {
            return WebHost.Endpoints.Extensions.HttpRequestExtensions.
                GetPathInfo(path, mode, path.Split('/').First(x => x != ""));
        }

        /// <summary>Can resolve root path.</summary>
        [Test]
        public void Can_resolve_root_path()
        {
            var results = new List<string> {
				ResolvePath(null, "/handler.all35"),
				ResolvePath(null, "/handler.all35/"),
				ResolvePath("api", "/location.api.wildcard35/api"),
				ResolvePath("api", "/location.api.wildcard35/api/"),
				ResolvePath("NServiceKit", "/location.NServiceKit.wildcard35/NServiceKit"),
				ResolvePath("NServiceKit", "/location.NServiceKit.wildcard35/NServiceKit/"),
			};

            Console.WriteLine(results.Dump());

            Assert.That(results.All(x => x == "/"));
        }

        /// <summary>Can resolve metadata paths.</summary>
        [Test]
        public void Can_resolve_metadata_paths()
        {
            var results = new List<string> {
				ResolvePath(null, "/handler.all35/metadata"),
				ResolvePath(null, "/handler.all35/metadata/"),
				ResolvePath("api", "/location.api.wildcard35/api/metadata"),
				ResolvePath("api", "/location.api.wildcard35/api/metadata/"),
				ResolvePath("NServiceKit", "/location.NServiceKit.wildcard35/NServiceKit/metadata"),
				ResolvePath("NServiceKit", "/location.NServiceKit.wildcard35/NServiceKit/metadata/"),
			};

            Console.WriteLine(results.Dump());

            Assert.That(results.All(x => x == "/metadata"));
        }

        /// <summary>Can resolve metadata JSON paths.</summary>
        [Test]
        public void Can_resolve_metadata_json_paths()
        {
            var results = new List<string> {
				ResolvePath(null, "/handler.all35/json/metadata"),
				ResolvePath(null, "/handler.all35/json/metadata/"),
				ResolvePath("api", "/location.api.wildcard35/api/json/metadata"),
				ResolvePath("api", "/location.api.wildcard35/api/json/metadata/"),
				ResolvePath("NServiceKit", "/location.api.wildcard35/NServiceKit/json/metadata"),
				ResolvePath("NServiceKit", "/location.api.wildcard35/NServiceKit/json/metadata/"),
			};

            Console.WriteLine(results.Dump());

            Assert.That(results.All(x => x == "/json/metadata"));
        }

        /// <summary>Can resolve paths with multipart root.</summary>
        [Test]
        public void Can_resolve_paths_with_multipart_root()
        {
            var results = new List<string> {
				WebHost.Endpoints.Extensions.HttpRequestExtensions.GetPathInfo("/api/foo/metadata", "api/foo", "api"),
				WebHost.Endpoints.Extensions.HttpRequestExtensions.GetPathInfo("/api/foo/1.0/wildcard/metadata", "api/foo/1.0/wildcard", "api"),
				WebHost.Endpoints.Extensions.HttpRequestExtensions.GetPathInfo("/location.api.wildcard35/api/foo/metadata", "api/foo", "api"),
				WebHost.Endpoints.Extensions.HttpRequestExtensions.GetPathInfo("/this/is/very/nested/metadata", "this/is/very/nested", "api"),
			};

            Console.WriteLine(results.Dump());

            Assert.That(results.All(x => x == "/metadata"));
        }

        /// <summary>Gets physical path honours web host physical path.</summary>
        [Test]
        [Ignore]
        public void GetPhysicalPath_Honours_WebHostPhysicalPath()
        {
            string root = "c:/MyWebRoot";
            HttpRequestMock mock = new HttpRequestMock();

            // Note: due to the static nature of EndpointHostConfig.Instance, running this
            // test twice withing NUnit fails the test. You'll need to reload betwen each
            // run.
            Assert.AreNotEqual(EndpointHostConfig.Instance.WebHostPhysicalPath, root);

            string originalPath = EndpointHostConfig.Instance.WebHostPhysicalPath;
            string path = mock.GetPhysicalPath();
            Assert.AreEqual(string.Format("{0}/{1}", originalPath, mock.PathInfo), path);

            EndpointHostConfig.Instance.WebHostPhysicalPath = root;
            path = mock.GetPhysicalPath();
            Assert.AreEqual(string.Format("{0}/{1}", root, mock.PathInfo), path);
        }
    }

}
