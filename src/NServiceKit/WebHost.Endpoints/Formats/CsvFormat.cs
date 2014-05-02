using System.IO;
using NServiceKit.Common.Web;
using NServiceKit.ServiceHost;
using NServiceKit.Text;

namespace NServiceKit.WebHost.Endpoints.Formats
{
    /// <summary>A CSV format.</summary>
	public class CsvFormat : IPlugin
	{
        /// <summary>Registers this object.</summary>
        ///
        /// <param name="appHost">The application host.</param>
		public void Register(IAppHost appHost)
		{
			//Register the 'text/csv' content-type and serializers (format is inferred from the last part of the content-type)
			appHost.ContentTypeFilters.Register(ContentType.Csv,
				SerializeToStream, CsvSerializer.DeserializeFromStream);

			//Add a response filter to add a 'Content-Disposition' header so browsers treat it natively as a .csv file
			appHost.ResponseFilters.Add((req, res, dto) =>
			{
				if (req.ResponseContentType == ContentType.Csv)
				{
					res.AddHeader(HttpHeaders.ContentDisposition,
						string.Format("attachment;filename={0}.csv", req.OperationName));
				}
			});
		}

        /// <summary>Serialize to stream.</summary>
        ///
        /// <param name="requestContext">Context for the request.</param>
        /// <param name="request">       The request.</param>
        /// <param name="stream">        The stream.</param>
		public void SerializeToStream(IRequestContext requestContext, object request, Stream stream)
		{
			CsvSerializer.SerializeToStream(request, stream);
		}
	}
}