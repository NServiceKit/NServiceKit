using System.Runtime.Serialization;
using System.Web;
using NServiceKit.CacheAccess;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints.Extensions;

namespace NServiceKit.WebHost.IntegrationTests.Services
{
	public class CustomSession
	{
		public int Counter { get; set; }
	}

	[Route("/session")]
	public class Session
	{
		public string Value { get; set; }
	}

	public class SessionResponse
	{
		public CustomSession Typed { get; set; }
		public CustomSession UnTyped { get; set; }
	}

	public class SessionService : ServiceInterface.Service
	{
        public object Any(Session request)
		{
			var untyped = Session["untyped"] as CustomSession ?? new CustomSession();			
			var typed = Session.Get<CustomSession>("typed") ?? new CustomSession();

			untyped.Counter++;
			typed.Counter++;

			Session["untyped"] = untyped;
			Session.Set("typed", typed);

			var response = new SessionResponse {
				Typed = typed,
				UnTyped = untyped,
			};

			return response;
		}
	}
}