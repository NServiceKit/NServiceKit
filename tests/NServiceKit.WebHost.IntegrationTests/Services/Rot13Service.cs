using System.Runtime.Serialization;
using NServiceKit.ServiceInterface;
using NServiceKit.Text;

namespace NServiceKit.WebHost.IntegrationTests.Services
{
    /// <summary>A rot 13.</summary>
	[DataContract]
	public class Rot13
	{
        /// <summary>Gets or sets the value.</summary>
        ///
        /// <value>The value.</value>
		[DataMember]
		public string Value { get; set; }
	}

    /// <summary>A rot 13 response.</summary>
	[DataContract]
	public class Rot13Response
	{
        /// <summary>Gets or sets the result.</summary>
        ///
        /// <value>The result.</value>
		[DataMember]
		public string Result { get; set; }
	}

    /// <summary>A rot 13 service.</summary>
	public class Rot13Service 
		: ServiceInterface.Service
	{
        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public object Any(Rot13 request)
		{
			return new Rot13Response { Result = request.Value.ToRot13() };
		}
	}
}