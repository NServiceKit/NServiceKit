using System;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Xml.Schema;
using NServiceKit.ServiceHost;
using NServiceKit.WebHost.Endpoints.Extensions;
using NServiceKit.WebHost.Endpoints.Support;
using NServiceKit.WebHost.Endpoints.Support.Metadata;
using NServiceKit.WebHost.Endpoints.Support.Metadata.Controls;
using NServiceKit.WebHost.Endpoints.Utils;

namespace NServiceKit.WebHost.Endpoints.Metadata
{
    /// <summary>A base SOAP metadata handler.</summary>
    public abstract class BaseSoapMetadataHandler : BaseMetadataHandler, INServiceKitHttpHandler
    {
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Metadata.BaseSoapMetadataHandler class.</summary>
		protected BaseSoapMetadataHandler()
		{
			OperationName = GetType().Name.Replace("Handler", "");
		}

        /// <summary>Gets or sets the name of the operation.</summary>
        ///
        /// <value>The name of the operation.</value>
		public string OperationName { get; set; }

        /// <summary>Executes the given context.</summary>
        ///
        /// <param name="context">The context.</param>
    	public override void Execute(System.Web.HttpContext context)
    	{
			ProcessRequest(
				new HttpRequestWrapper(OperationName, context.Request),
				new HttpResponseWrapper(context.Response), 
				OperationName);
    	}

        /// <summary>Process the request.</summary>
        ///
        /// <exception cref="ArgumentOutOfRangeException">Thrown when one or more arguments are outside the required range.</exception>
        ///
        /// <param name="httpReq">      The HTTP request.</param>
        /// <param name="httpRes">      The HTTP resource.</param>
        /// <param name="operationName">Name of the operation.</param>
		public new void ProcessRequest(IHttpRequest httpReq, IHttpResponse httpRes, string operationName)
    	{
            if (!AssertAccess(httpReq, httpRes, httpReq.QueryString["op"])) return;

			var operationTypes = EndpointHost.Metadata.GetAllTypes();

    		if (httpReq.QueryString["xsd"] != null)
    		{
				var xsdNo = Convert.ToInt32(httpReq.QueryString["xsd"]);
                var schemaSet = XsdUtils.GetXmlSchemaSet(operationTypes);
    			var schemas = schemaSet.Schemas();
    			var i = 0;
    			if (xsdNo >= schemas.Count)
    			{
    				throw new ArgumentOutOfRangeException("xsd");
    			}
    			httpRes.ContentType = "text/xml";
    			foreach (XmlSchema schema in schemaSet.Schemas())
    			{
    				if (xsdNo != i++) continue;
    				schema.Write(httpRes.OutputStream);
    				break;
    			}
    			return;
    		}

			using (var sw = new StreamWriter(httpRes.OutputStream))
			{
				var writer = new HtmlTextWriter(sw);
				httpRes.ContentType = "text/html";
				ProcessOperations(writer, httpReq, httpRes);
			}
    	}

        /// <summary>Renders the operations.</summary>
        ///
        /// <param name="writer">  The writer.</param>
        /// <param name="httpReq"> The HTTP request.</param>
        /// <param name="metadata">The metadata.</param>
    	protected override void RenderOperations(HtmlTextWriter writer, IHttpRequest httpReq, ServiceMetadata metadata)
    	{
			var defaultPage = new IndexOperationsControl {
				HttpRequest = httpReq,
                MetadataConfig = EndpointHost.Config.MetadataPagesConfig,                
				Title = EndpointHost.Config.ServiceName,
				Xsds = XsdTypes.Xsds,
				XsdServiceTypesIndex = 1,
                OperationNames = metadata.GetOperationNamesForMetadata(httpReq),
				MetadataPageBodyHtml = EndpointHost.Config.MetadataPageBodyHtml,
			};

			defaultPage.RenderControl(writer);
		}

    }
}