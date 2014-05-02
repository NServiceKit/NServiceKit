using System;
using NUnit.Framework;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints.Tests;

namespace NServiceKit.Auth.Tests
{
    /// <summary>A secure tests.</summary>
	[TestFixture, Explicit]
	public class SecureTests:TestBase
	{
        /// <summary>Can execute secure service.</summary>
		[Test]
		public void can_execute_secure_service ()
		{
			
			var secure= Client.Post<SecureResponse>("/secure",
			new Secure()
			{
				UserName="angel"
			});
			//fails against webhost-xsp2!,   runs fine against console host 
            //DB: Could be a result of certain webhosts hijacking particular HTTP Exceptions
			
			Console.WriteLine(secure.Dump());
						
		}
	}
}

// run NServiceKit.WebHostApp first with xsp2 
// http://127.0.0.1:8080/api/auth?UserName=test1&Password=test1&format=json  OK
// http://127.0.0.1:8080/api/secure?UserName=test1&format=json OK
// http://127.0.0.1:8080/api/auth/logout?format=json
