using System.Runtime.Serialization;

namespace NServiceKit.WebHost.Endpoints.Tests.Support.Types
{
    /// <summary>A customer.</summary>
	[DataContract]
	public class Customer
	{
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
		[DataMember]
		public long Id { get; set; }
	}
}