using NUnit.Framework;
using NServiceKit.Razor;
using NServiceKit.ServiceInterface.Testing;
using NServiceKit.Text;
using NServiceKit.VirtualPath;

namespace NServiceKit.ServiceHost.Tests.Formats_Razor
{
    /// <summary>A stand alone example tests.</summary>
    [TestFixture]
    public class StandAloneExampleTests
    {
        /// <summary>Simple static example.</summary>
        [Test]
        public void Simple_static_example()
        {
            RazorFormat.Instance = null;
            var razor = new RazorFormat {
                VirtualPathProvider = new InMemoryVirtualPathProvider(new BasicAppHost()),
                EnableLiveReload = false,
            }.Init();

            var page = razor.CreatePage("Hello @Model.Name! Welcome to Razor!");
            var html = razor.RenderToHtml(page, new { Name = "World" });
            html.Print();

            Assert.That(html, Is.EqualTo("Hello World! Welcome to Razor!"));
        }
    }
}
