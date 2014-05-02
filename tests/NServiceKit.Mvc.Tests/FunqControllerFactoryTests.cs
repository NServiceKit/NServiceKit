using NUnit.Framework;
using Funq;
using NServiceKit.Mvc.Stubs.Tests;
using NServiceKit.Mvc.Tests.Stubs;

namespace NServiceKit.Mvc.Tests
{
    /// <summary>A funq controller factory tests.</summary>
    [TestFixture]
    public class FunqControllerFactoryTests
    {
        /// <summary>Construct factory populates local controller by default.</summary>
        [Test]
        public void ConstructFactoryPopulatesLocalControllerByDefault()
        {
            var container = new Container();
            var factory = new FunqControllerFactory(container);
            var testController = container.Resolve<LocalController>();
            Assert.That(testController, Is.Not.Null);
        }

        /// <summary>Construct factory populates local controller and external controller by default.</summary>
        [Test]
        public void ConstructFactoryPopulatesLocalControllerAndExternalControllerByDefault()
        {
            var container = new Container();
            var factory = new FunqControllerFactory(container, typeof(ExternalController).Assembly);
            
            // test we can still resolve the local one (by default)
            var testController = container.Resolve<LocalController>();
            Assert.That(testController, Is.Not.Null);

            // test we can resolve the external controller (via params assembly)
            var externalController = container.Resolve<ExternalController>();
            Assert.That(externalController, Is.Not.Null);
        }
    }
}
