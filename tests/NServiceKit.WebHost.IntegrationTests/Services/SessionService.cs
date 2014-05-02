using System.Runtime.Serialization;
using System.Web;
using NServiceKit.CacheAccess;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints.Extensions;

namespace NServiceKit.WebHost.IntegrationTests.Services
{
    /// <summary>A custom session.</summary>
	public class CustomSession
	{
        /// <summary>Gets or sets the counter.</summary>
        ///
        /// <value>The counter.</value>
		public int Counter { get; set; }
	}

    /// <summary>A session.</summary>
	[Route("/session")]
	public class Session
	{
        /// <summary>Gets or sets the value.</summary>
        ///
        /// <value>The value.</value>
		public string Value { get; set; }
	}

    /// <summary>A session response.</summary>
	public class SessionResponse
	{
        /// <summary>Gets or sets the typed.</summary>
        ///
        /// <value>The typed.</value>
		public CustomSession Typed { get; set; }

        /// <summary>Gets or sets the un typed.</summary>
        ///
        /// <value>The un typed.</value>
		public CustomSession UnTyped { get; set; }
	}

    /// <summary>A session service.</summary>
	public class SessionService : ServiceInterface.Service
	{
        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
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