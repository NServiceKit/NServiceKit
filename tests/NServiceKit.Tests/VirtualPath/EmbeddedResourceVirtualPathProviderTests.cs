using System.Linq;
using Funq;
using NServiceKit.VirtualPath;
using NServiceKit.WebHost.Endpoints;
using NUnit.Framework;

namespace NServiceKit.Tests.VirtualPath
{
    /// <summary>An embedded resource virtual path provider tests.</summary>
    [TestFixture]
    public class EmbeddedResourceVirtualPathProviderTests
    {
        private class AppHost : AppHostHttpListenerBase
        {
            /// <summary>Configures the given container.</summary>
            ///
            /// <param name="container">The container.</param>
            public override void Configure(Container container)
            {
            }
        }

        /// <summary>Tests embedded at root.</summary>
        [Test]
        public void TestEmbeddedAtRoot()
        {
            var p = new EmbeddedResourceVirtualPathProvider(new AppHost());
            p.IncludeAssemblies(typeof(EmbeddedResourceVirtualPathProviderTests).Assembly);
            p.PopulateFromEmbeddedResources();
            Assert.IsNotNull(p.GetFile("EmbedMe.cshtml"));
        }

        /// <summary>Tests embedded glob.</summary>
        [Test]
        public void TestEmbeddedGlob()
        {
            var p = new EmbeddedResourceVirtualPathProvider(new AppHost());
            p.IncludeAssemblies(typeof(EmbeddedResourceVirtualPathProviderTests).Assembly);
            p.PopulateFromEmbeddedResources();
            Assert.AreEqual(1, p.GetAllMatchingFiles("*.resources").Count());
        }

        /// <summary>Tests parse resource with no dot.</summary>
        [Test]
        public void TestParseResourceWithNoDot()
        {
            string fileName;
            string[] directories;
            EmbeddedResourceVirtualPathProvider.InferFileNameAndDirectoryPath("Hello", out fileName, out directories);
            Assert.AreEqual("Hello", fileName);
            Assert.AreEqual(0, directories.Length);
        }

        /// <summary>Tests parse resource with one dot.</summary>
        [Test]
        public void TestParseResourceWithOneDot()
        {
            string fileName;
            string[] directories;
            EmbeddedResourceVirtualPathProvider.InferFileNameAndDirectoryPath("Hello.txt", out fileName, out directories);
            Assert.AreEqual("Hello.txt", fileName);
            Assert.AreEqual(0, directories.Length);
        }

        /// <summary>Tests parse resource with many dots.</summary>
        [Test]
        public void TestParseResourceWithManyDots()
        {
            string fileName;
            string[] directories;
            EmbeddedResourceVirtualPathProvider.InferFileNameAndDirectoryPath("Hello.To.The.World.txt", out fileName, out directories);
            Assert.AreEqual("World.txt", fileName);
            Assert.AreEqual(3, directories.Length);
            Assert.AreEqual("Hello", directories[0]);
            Assert.AreEqual("To", directories[1]);
            Assert.AreEqual("The", directories[2]);
        }

        /// <summary>Tests exclude file.</summary>
        [Test]
        public void TestExcludeFile()
        {
            var p = new EmbeddedResourceVirtualPathProvider(new AppHost());
            p.IncludeAssemblies(typeof(EmbeddedResourceVirtualPathProviderTests).Assembly);
            p.FileExcluder = file => file.Name.Contains("EmbedMe");
            p.PopulateFromEmbeddedResources();
            Assert.IsNull(p.GetFile("EmbedMe.cshtml"));
        }
    }
}
