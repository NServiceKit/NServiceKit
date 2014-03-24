using System.IO;
using NServiceKit.Common.Web;
using NServiceKit.ServiceHost;
using NServiceKit.Text;

namespace NServiceKit.WebHost.Endpoints.Formats
{
	public class CsvFormat : IPlugin
	{
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

		public void SerializeToStream(IRequestContext requestContext, object request, Stream stream)
		{
			CsvSerializer.SerializeToStream(request, stream);
		}
	}
}