using System.Collections.Generic;
using NServiceKit.WebHost.Endpoints;

namespace NServiceKit.ServiceHost
{
    /// <summary>A metadata pages configuration.</summary>
    public class MetadataPagesConfig
    {
        private ServiceMetadata metadata;
        private HashSet<string> ignoredFormats;
        private readonly Dictionary<string, MetadataConfig> metadataConfigMap;

        /// <summary>Gets the available format configs.</summary>
        ///
        /// <value>The available format configs.</value>
        public List<MetadataConfig> AvailableFormatConfigs { get; private set; }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.MetadataPagesConfig class.</summary>
        ///
        /// <param name="metadata">          The metadata.</param>
        /// <param name="metadataConfig">    The metadata configuration.</param>
        /// <param name="ignoredFormats">    The ignored formats.</param>
        /// <param name="contentTypeFormats">The content type formats.</param>
        public MetadataPagesConfig(
            ServiceMetadata metadata,
            ServiceEndpointsMetadataConfig metadataConfig,
            HashSet<string> ignoredFormats,
            List<string> contentTypeFormats)
        {
            this.ignoredFormats = ignoredFormats;
            this.metadata = metadata;

            metadataConfigMap = new Dictionary<string, MetadataConfig> {
                {"xml", metadataConfig.Xml},
                {"json", metadataConfig.Json},
                {"jsv", metadataConfig.Jsv},
                {"soap11", metadataConfig.Soap11},
                {"soap12", metadataConfig.Soap12},
            };

            AvailableFormatConfigs = new List<MetadataConfig>();

            var config = GetMetadataConfig("xml");
            if (config != null) AvailableFormatConfigs.Add(config);
            config = GetMetadataConfig("json");
            if (config != null) AvailableFormatConfigs.Add(config);
            config = GetMetadataConfig("jsv");
            if (config != null) AvailableFormatConfigs.Add(config);

            foreach (var format in contentTypeFormats)
            {
                metadataConfigMap[format] = metadataConfig.Custom.Create(format);

                config = GetMetadataConfig(format);
                if (config != null) AvailableFormatConfigs.Add(config);
            }

            config = GetMetadataConfig("soap11");
            if (config != null) AvailableFormatConfigs.Add(config);
            config = GetMetadataConfig("soap12");
            if (config != null) AvailableFormatConfigs.Add(config);
        }

        /// <summary>Gets metadata configuration.</summary>
        ///
        /// <param name="format">Describes the format to use.</param>
        ///
        /// <returns>The metadata configuration.</returns>
        public MetadataConfig GetMetadataConfig(string format)
        {
            if (ignoredFormats.Contains(format)) return null;

            MetadataConfig conf;
            metadataConfigMap.TryGetValue(format, out conf);
            return conf;
        }

        /// <summary>Query if 'httpRequest' is visible.</summary>
        ///
        /// <param name="httpRequest">The HTTP request.</param>
        /// <param name="format">     Describes the format to use.</param>
        /// <param name="operation">  The operation.</param>
        ///
        /// <returns>true if visible, false if not.</returns>
        public bool IsVisible(IHttpRequest httpRequest, Format format, string operation)
        {
            if (ignoredFormats.Contains(format.FromFormat())) return false;
            return metadata.IsVisible(httpRequest, format, operation);
        }

        /// <summary>Determine if we can access.</summary>
        ///
        /// <param name="httpRequest">The HTTP request.</param>
        /// <param name="format">     Describes the format to use.</param>
        /// <param name="operation">  The operation.</param>
        ///
        /// <returns>true if we can access, false if not.</returns>
        public bool CanAccess(IHttpRequest httpRequest, Format format, string operation)
        {
            if (ignoredFormats.Contains(format.FromFormat())) return false;
            return metadata.CanAccess(httpRequest, format, operation);
        }

        /// <summary>Determine if we can access.</summary>
        ///
        /// <param name="format">   Describes the format to use.</param>
        /// <param name="operation">The operation.</param>
        ///
        /// <returns>true if we can access, false if not.</returns>
        public bool CanAccess(Format format, string operation)
        {
            if (ignoredFormats.Contains(format.FromFormat())) return false;
            return metadata.CanAccess(format, operation);
        }
    }
}