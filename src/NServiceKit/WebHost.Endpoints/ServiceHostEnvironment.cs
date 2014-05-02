namespace NServiceKit.WebHost.Endpoints
{
    /// <summary>A service host environment.</summary>
	public class ServiceHostEnvironment
	{
        /// <summary>Gets or sets the web server.</summary>
        ///
        /// <value>The web server.</value>
		public WebServerType WebServer { get; set; }

        /// <summary>Gets or sets the name of the host.</summary>
        ///
        /// <value>The name of the host.</value>
		public string HostName { get; set; }

        /// <summary>Gets or sets the full pathname of the HTTP handler file.</summary>
        ///
        /// <value>The full pathname of the HTTP handler file.</value>
		public string HttpHandlerPath { get; set; }

        /// <summary>Gets or sets URL of the base.</summary>
        ///
        /// <value>The base URL.</value>
		public string BaseUrl { get; set; }

        /// <summary>Gets or sets the full pathname of the base file.</summary>
        ///
        /// <value>The full pathname of the base file.</value>
		public string BasePath { get; set; }
	}
}