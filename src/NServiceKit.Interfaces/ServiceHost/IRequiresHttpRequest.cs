using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NServiceKit.ServiceHost
{
    /// <summary>Interface for requires HTTP request.</summary>
	public interface IRequiresHttpRequest
	{
        /// <summary>Gets or sets the HTTP request.</summary>
        ///
        /// <value>The HTTP request.</value>
		IHttpRequest HttpRequest { get; set; }
	}
}
