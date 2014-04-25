using System.Linq;
using Funq;
using NServiceKit.VirtualPath;
using NServiceKit.WebHost.Endpoints;
using NUnit.Framework;

namespace NServiceKit.Tests.VirtualPath
{
    [TestFixture]
    public class EmbeddedResourceVirtualPathProviderTests
    {
        private class AppHost : AppHostHttpListenerBase
        {
            public override void Configure(Container container)
            {
            }
        }

        [Test]
        public void TestEmbeddedAtRoot()
        {
            var p = new EmbeddedResourceVirtualPathProvider(new AppHost());
            p.IncludeAssemblies(typeof(EmbeddedResourceVirtualPathProviderTests).Assembly);
            p.PopulateFromEmbeddedResources();
            Assert.IsNotNull(p.GetFile("EmbedMe.cshtml"));
        }

        [Test]
        public void TestEmbeddedGlob()
        {
            var p = new EmbeddedResourceVirtualPathProvider(new AppHost());
            p.IncludeAssemblies(typeof(EmbeddedResourceVirtualPathProviderTests).Assembly);
            p.PopulateFromEmbeddedResources();
            Assert.AreEqual(1, p.GetAllMatchingFiles("*.resources").Count());
        }

        [Test]
        public void TestParseResourceWithNoDot()
        {
            string fileName;
            string[] directories;
            EmbeddedResourceVirtualPathProvider.InferFileNameAndDirectoryPath("Hello", out fileName, out directories);
            Assert.AreEqual("Hello", fileName);
            Assert.AreEqual(0, directories.Length);
        }

        [Test]
        public void TestParseResourceWithOneDot()
        {
            string fileName;
            string[] directories;
            EmbeddedResourceVirtualPathProvider.InferFileNameAndDirectoryPath("Hello.txt", out fileName, out directories);
            Assert.AreEqual("Hello.txt", fileName);
            Assert.AreEqual(0, directories.Length);
        }

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
