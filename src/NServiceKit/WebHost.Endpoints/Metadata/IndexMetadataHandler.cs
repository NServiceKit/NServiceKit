using System;
using NServiceKit.Common.Web;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.Endpoints.Metadata
{
    /// <summary>An index metadata handler.</summary>
	public class IndexMetadataHandler : BaseSoapMetadataHandler
	{
        /// <summary>Gets the format to use.</summary>
        ///
        /// <value>The format.</value>
        public override Format Format { get { return Format.Soap12; } }

        /// <summary>Creates a message.</summary>
        ///
        /// <param name="dtoType">Type of the dto.</param>
        ///
        /// <returns>The new message.</returns>
		protected override string CreateMessage(Type dtoType)
		{
			return null;
			//throw new System.NotImplementedException();
		}
	}
}