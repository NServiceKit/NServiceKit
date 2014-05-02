using System;
using NUnit.Framework;
using NServiceKit.Common;
using NServiceKit.Messaging;
using NServiceKit.ServiceInterface;

namespace NServiceKit.WebHost.IntegrationTests.Tests
{
    /// <summary>A session tests.</summary>
	[TestFixture]
	public class SessionTests
	{
        /// <summary>Adhocs this object.</summary>
        [Test]		 
        public void Adhoc()
        {
        	var appliesTo = ApplyTo.Post | ApplyTo.Put;
			Console.WriteLine(appliesTo.ToString());
			Console.WriteLine(appliesTo.ToDescription());
			Console.WriteLine(string.Join(", ", appliesTo.ToList().ToArray()));
        }
	}
}