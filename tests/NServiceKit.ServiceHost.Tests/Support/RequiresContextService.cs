using System;
using System.Runtime.Serialization;

namespace NServiceKit.ServiceHost.Tests.Support
{
    /// <summary>The requires context.</summary>
	[DataContract]
	public class RequiresContext { }

    /// <summary>The requires context response.</summary>
	[DataContract]
	public class RequiresContextResponse { }

    /// <summary>The requires context service.</summary>
	public class RequiresContextService : ServiceInterface.Service
	{
        /// <summary>Anies the given requires.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="requires">The requires.</param>
        ///
        /// <returns>A RequiresContextResponse.</returns>
        public RequiresContextResponse Any(RequiresContext requires)
		{
			if (RequestContext == null)
				throw new ArgumentNullException("RequestContext");
	
			return new RequiresContextResponse();
		}
	}
}