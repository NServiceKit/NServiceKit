namespace NServiceKit.WebHost.Endpoints
{
    /// <summary>A metadata configuration.</summary>
	public class MetadataConfig
	{
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.MetadataConfig class.</summary>
        ///
        /// <param name="format">            Describes the format to use.</param>
        /// <param name="name">              The name.</param>
        /// <param name="syncReplyUri">      The synchronise reply URI.</param>
        /// <param name="asyncOneWayUri">    The asynchronous one way URI.</param>
        /// <param name="defaultMetadataUri">The default metadata URI.</param>
        public MetadataConfig(string format, string name, string syncReplyUri, string asyncOneWayUri, string defaultMetadataUri)
        {
            Format = format;
		    Name = name;
			SyncReplyUri = syncReplyUri;
			AsyncOneWayUri = asyncOneWayUri;
			DefaultMetadataUri = defaultMetadataUri;
		}

        /// <summary>Gets or sets the format to use.</summary>
        ///
        /// <value>The format.</value>
        public string Format { get; set; }

        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>Gets or sets URI of the synchronise reply.</summary>
        ///
        /// <value>The synchronise reply URI.</value>
        public string SyncReplyUri { get; set; }

        /// <summary>Gets or sets URI of the asynchronous one way.</summary>
        ///
        /// <value>The asynchronous one way URI.</value>
		public string AsyncOneWayUri { get; set; }

        /// <summary>Gets or sets the default metadata URI.</summary>
        ///
        /// <value>The default metadata URI.</value>
		public string DefaultMetadataUri { get; set; }

        /// <summary>Creates a new MetadataConfig.</summary>
        ///
        /// <param name="format">Describes the format to use.</param>
        /// <param name="name">  The name.</param>
        ///
        /// <returns>A MetadataConfig.</returns>
        public MetadataConfig Create(string format, string name=null)
        {
            return new MetadataConfig
                (
                    format,
                    name ?? format.ToUpper(),
                    string.Format(SyncReplyUri, format),
                    string.Format(AsyncOneWayUri, format),
                    string.Format(DefaultMetadataUri, format)
                );            
        }
	}
}