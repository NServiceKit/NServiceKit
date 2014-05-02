using NServiceKit.Common.Utils;
using NServiceKit.Configuration;

namespace NServiceKit.WebHost.Endpoints.Tests.IntegrationTests
{
    /// <summary>An example configuration.</summary>
	public class ExampleConfig
	{
		/// <summary>
		/// Would've preferred to use [assembly: ContractNamespace] attribute but it is not supported in Mono
		/// </summary>
		public const string DefaultNamespace = "http://schemas.NServiceKit.net/types";

        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.IntegrationTests.ExampleConfig class.</summary>
		public ExampleConfig() { }

        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.IntegrationTests.ExampleConfig class.</summary>
        ///
        /// <param name="appConfig">The application configuration.</param>
		public ExampleConfig(IResourceManager appConfig)
		{
			ConnectionString = appConfig.GetString("ConnectionString");
			DefaultFibonacciLimit = appConfig.Get("DefaultFibonacciLimit", 10);
		}

        /// <summary>Gets or sets the connection string.</summary>
        ///
        /// <value>The connection string.</value>
		public string ConnectionString { get; set; }

        /// <summary>Gets or sets the default fibonacci limit.</summary>
        ///
        /// <value>The default fibonacci limit.</value>
		public int DefaultFibonacciLimit { get; set; }

	}
}