using System.Net;
using System.Web;
using System.Web.Security;
using NServiceKit.ServiceClient.Web;

namespace NServiceKit.WebHost.Endpoints
{
    /// <summary>A HTTP web request configuration.</summary>
	public static class HttpWebRequestConfig
	{
        /// <summary>Configures this object.</summary>
		public static void Configure()
		{
			ServiceClientBase.HttpWebRequestFilter = TransferAuthenticationTokens;
		}

        /// <summary>Transfer authentication tokens.</summary>
        ///
        /// <param name="httpWebRequest">The HTTP web request.</param>
		public static void TransferAuthenticationTokens(HttpWebRequest httpWebRequest)
		{
			var cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
			if (cookie == null) return;
			
			var authenticationCookie = new Cookie(
				FormsAuthentication.FormsCookieName,
				cookie.Value,
				cookie.Path,
				HttpContext.Current.Request.Url.Authority);

			httpWebRequest.CookieContainer = new CookieContainer();
			httpWebRequest.CookieContainer.Add(authenticationCookie);
		}
	}
}
