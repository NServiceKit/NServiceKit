using System;
using System.Runtime.Serialization;
using NServiceKit.ServiceInterface;

namespace NServiceKit.WebHost.IntegrationTests.Services
{
    /// <summary>A reverse.</summary>
	[DataContract]
	public class Reverse
	{
        /// <summary>Gets or sets the value.</summary>
        ///
        /// <value>The value.</value>
		[DataMember]
		public string Value { get; set; }
	}

    /// <summary>A reverse response.</summary>
	[DataContract]
	public class ReverseResponse
	{
        /// <summary>Gets or sets the result.</summary>
        ///
        /// <value>The result.</value>
		[DataMember]
		public string Result { get; set; }
	}

    /// <summary>A reverse service.</summary>
	public class ReverseService 
		: ServiceInterface.Service
	{
        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public object Any(Reverse request)
		{
			return new ReverseResponse { Result = Execute(request.Value) };
		}

        /// <summary>Executes.</summary>
        ///
        /// <param name="value">The value.</param>
        ///
        /// <returns>A string.</returns>
		public static string Execute(string value)
		{
			var valueBytes = value.ToCharArray();
			Array.Reverse(valueBytes);
			return new string(valueBytes);
		}
	}

}