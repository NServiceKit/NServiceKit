using System;
using NServiceKit.Common.Web;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.Endpoints.Metadata
{
	public class IndexMetadataHandler : BaseSoapMetadataHandler
	{
        public override Format Format { get { return Format.Soap12; } }

		protected override string CreateMessage(Type dtoType)
		{
			return null;
			//throw new System.NotImplementedException();
		}
	}
}