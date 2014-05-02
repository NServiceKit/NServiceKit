using System;
using NUnit.Framework;
using NServiceKit.WebHost.Endpoints.Tests.Support.Host;

namespace NServiceKit.WebHost.Endpoints.Tests
{
    /// <summary>A service host test base.</summary>
	public class ServiceHostTestBase
	{
        /// <summary>Creates application host.</summary>
        ///
        /// <returns>The new application host.</returns>
		public static TestAppHost CreateAppHost()
		{
			var appHost = new TestAppHost();
			appHost.Init();

			return appHost;
		}

        /// <summary>Should throw.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="action">The action.</param>
		public void ShouldThrow<T>(Action action)
			where T : Exception
		{
			ShouldThrow<T>(action, "Should Throw");
		}

        /// <summary>Should throw.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="action">                 The action.</param>
        /// <param name="errorMessageIfNotThrows">The error message if not throws.</param>
		public void ShouldThrow<T>(Action action, string errorMessageIfNotThrows)
			where T : Exception
		{
			try
			{
				action();
			}
			catch (T)
			{
				return;
			}
			Assert.Fail(errorMessageIfNotThrows);
		}

        /// <summary>Should not throw.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="action">The action.</param>
		public void ShouldNotThrow<T>(Action action)
			where T : Exception
		{
			ShouldNotThrow<T>(action, "Should not Throw");
		}

        /// <summary>Should not throw.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="action">              The action.</param>
        /// <param name="errorMessageIfThrows">The error message if throws.</param>
		public void ShouldNotThrow<T>(Action action, string errorMessageIfThrows)
			where T : Exception
		{
			try
			{
				action();
			}
			catch (T)
			{
				Assert.Fail(errorMessageIfThrows);
			}
		}

	}
}