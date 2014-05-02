using System;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;

namespace NServiceKit.Auth.Tests
{
    /// <summary>A razor application host tests.</summary>
    [Explicit]
    [TestFixture]
    public class RazorAppHostTests
    {
        /// <summary>Executes for 10 mins operation.</summary>
        [Test]
        public void Run_for_10Mins()
        {
            using (var appHost = new AppHost())
            {
                appHost.Init();
                appHost.Start("http://localhost:11001/");

                Process.Start("http://localhost:11001/");

                Thread.Sleep(TimeSpan.FromMinutes(10));
            }
        }
    }
}