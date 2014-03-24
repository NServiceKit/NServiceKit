using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NServiceKit.ServiceHost
{
	public interface IRequiresHttpRequest
	{
		IHttpRequest HttpRequest { get; set; }
	}
}
