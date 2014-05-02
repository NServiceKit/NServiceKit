using System.ComponentModel;
using System.Web.Services;
using NServiceKit.WebHost.IntegrationTests.Services;

namespace NServiceKit.WebHost.IntegrationTests
{
	/// <summary>
	/// Summary description for Soap11WebService
	/// </summary>
	[WebService(Namespace = "http://schemas.NServiceKit.net/types")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[ToolboxItem(false)]
	// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
	// [System.Web.Script.Services.ScriptService]
	public class Soap11WebService : System.Web.Services.WebService
	{
        /// <summary>Hello world.</summary>
        ///
        /// <returns>A string.</returns>
		[WebMethod]
		public string HelloWorld()
		{
			return "Hello World";
		}

        /// <summary>Reverses the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>A ReverseResponse.</returns>
		[WebMethod]
		public ReverseResponse Reverse(Reverse request)
		{
			return new ReverseService().Any(request) as ReverseResponse;
		}
	}
}
