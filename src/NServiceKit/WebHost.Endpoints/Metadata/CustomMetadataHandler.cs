using System;
using System.IO;
using System.Text;
using System.Web.UI;
using NServiceKit.Common.Utils;
using NServiceKit.Common.Web;
using NServiceKit.Logging;
using NServiceKit.ServiceHost;
using NServiceKit.WebHost.Endpoints.Support.Metadata.Controls;

namespace NServiceKit.WebHost.Endpoints.Metadata
{
    /// <summary>A custom metadata handler.</summary>
	public class CustomMetadataHandler
		: BaseMetadataHandler
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(CustomMetadataHandler));

        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Metadata.CustomMetadataHandler class.</summary>
        ///
        /// <param name="contentType">Type of the content.</param>
        /// <param name="format">     The format.</param>
		public CustomMetadataHandler(string contentType, string format)
		{
			base.ContentType = contentType;
			base.ContentFormat = format;
		}

        /// <summary>Gets the format to use.</summary>
        ///
        /// <value>The format.</value>
        public override Format Format
		{
            get { return base.ContentFormat.ToFormat(); }
		}

        /// <summary>Creates a message.</summary>
        ///
        /// <param name="dtoType">Type of the dto.</param>
        ///
        /// <returns>The new message.</returns>
		protected override string CreateMessage(Type dtoType)
		{
			try
			{
				var requestObj = ReflectionUtils.PopulateObject(Activator.CreateInstance(dtoType));

				using (var ms = new MemoryStream())
				{
					EndpointHost.ContentTypeFilter.SerializeToStream(
						new SerializationContext(this.ContentType), requestObj, ms);

					return Encoding.UTF8.GetString(ms.ToArray());
				}
			}
			catch (Exception ex)
			{
				var error = string.Format("Error serializing type '{0}' with custom format '{1}'",
					dtoType.Name, this.ContentFormat);
				Log.Error(error, ex);

				return string.Format("{{Unable to show example output for type '{0}' using the custom '{1}' filter}}" + ex.Message,
					dtoType.Name, this.ContentFormat);
			}
		}

        /// <summary>Renders the operations.</summary>
        ///
        /// <param name="writer">  The writer.</param>
        /// <param name="httpReq"> The HTTP request.</param>
        /// <param name="metadata">The metadata.</param>
        protected override void RenderOperations(HtmlTextWriter writer, IHttpRequest httpReq, ServiceMetadata metadata)
		{
			var defaultPage = new OperationsControl
			{
				Title = EndpointHost.Config.ServiceName,
                OperationNames = metadata.GetOperationNamesForMetadata(httpReq, Format),
                MetadataOperationPageBodyHtml = EndpointHost.Config.MetadataOperationPageBodyHtml,
			};

			defaultPage.RenderControl(writer);
		}
	}
}