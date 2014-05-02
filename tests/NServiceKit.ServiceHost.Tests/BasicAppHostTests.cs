using System;
using System.Collections.Generic;
using System.Linq;
using NServiceKit.ServiceInterface.Testing;
using NUnit.Framework;

namespace NServiceKit.ServiceHost.Tests
{
    /// <summary>A basic application host tests.</summary>
    [TestFixture]
    public class BasicAppHostTests
    {
        /// <summary>Can dispose without initialise.</summary>
        [Test]
        public void Can_dispose_without_init()
        {
            BasicAppHost appHost = new BasicAppHost();
            appHost.Dispose();
        }
    }
}
