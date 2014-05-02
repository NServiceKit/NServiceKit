using System.Collections.Generic;
using System.Linq;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.IntegrationTests.Services
{
    /// <summary>A cookies.</summary>
    [Route("/cookies")]
    public class Cookies : IReturn<CookiesResponse>
    { }

    /// <summary>The cookies response.</summary>
    public class CookiesResponse
    {
        /// <summary>Gets or sets a list of names of the request cookies.</summary>
        ///
        /// <value>A list of names of the request cookies.</value>
        public List<string> RequestCookieNames { get; set; }
    }

    /// <summary>The cookies service.</summary>
    public class CookiesService : ServiceInterface.Service
    {
        /// <summary>Anies the given c.</summary>
        ///
        /// <param name="c">The Cookies to process.</param>
        ///
        /// <returns>A CookiesResponse.</returns>
        public CookiesResponse Any(Cookies c)
        {
            var response = new CookiesResponse
            {
                RequestCookieNames = Request.Cookies.Keys.ToList(),
            };
            return response;
        }
    }
}