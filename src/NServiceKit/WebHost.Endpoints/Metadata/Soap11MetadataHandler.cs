using System;
using NServiceKit.Common.Utils;
using NServiceKit.Common.Extensions;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceModel.Serialization;
using NServiceKit.WebHost.Endpoints.Support.Metadata.Controls;

namespace NServiceKit.WebHost.Endpoints.Metadata
{
    /// <summary>A SOAP 11 metadata handler.</summary>
	public class Soap11MetadataHandler : BaseSoapMetadataHandler
	{
        /// <summary>Gets the format to use.</summary>
        ///
        /// <value>The format.</value>
        public override Format Format { get { return Format.Soap11; } }

        /// <summary>Creates a message.</summary>
        ///
        /// <param name="dtoType">Type of the dto.</param>
        ///
        /// <returns>The new message.</returns>
		protected override string CreateMessage(Type dtoType)
		{
			var requestObj = ReflectionUtils.PopulateObject(Activator.CreateInstance(dtoType));
			var xml = DataContractSerializer.Instance.Parse(requestObj, true);
			var soapEnvelope = string.Format(@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
    <soap:Body>

{0}

    </soap:Body>
</soap:Envelope>", xml);
			return soapEnvelope;
		}

        /// <summary>Renders the operation.</summary>
        ///
        /// <param name="writer">         The writer.</param>
        /// <param name="httpReq">        The HTTP request.</param>
        /// <param name="operationName">  Name of the operation.</param>
        /// <param name="requestMessage"> Message describing the request.</param>
        /// <param name="responseMessage">Message describing the response.</param>
        /// <param name="metadataHtml">   The metadata HTML.</param>
        protected override void RenderOperation(System.Web.UI.HtmlTextWriter writer, IHttpRequest httpReq, string operationName, string requestMessage, string responseMessage, string metadataHtml)
        {
            var operationControl = new Soap11OperationControl
            {
                HttpRequest = httpReq,
                MetadataConfig = EndpointHost.Config.ServiceEndpointsMetadataConfig,
                Title = EndpointHost.Config.ServiceName,
                Format = this.Format,
                OperationName = operationName,
                HostName = httpReq.GetUrlHostName(),
                RequestMessage = requestMessage,
                ResponseMessage = responseMessage,
                MetadataHtml = metadataHtml,
            };
            if (!this.ContentType.IsNullOrEmpty())
            {
                operationControl.ContentType = this.ContentType;
            }
            if (!this.ContentFormat.IsNullOrEmpty())
            {
                operationControl.ContentFormat = this.ContentFormat;
            }

            operationControl.Render(writer);
        }
	}
}