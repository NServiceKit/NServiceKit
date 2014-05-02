using System.Collections.Generic;
using System.Runtime.Serialization;
using NUnit.Framework;
using NServiceKit.ServiceClient.Web;
using NServiceKit.ServiceInterface.ServiceModel;

namespace NServiceKit.WebHost.Endpoints.Tests
{
    /// <summary>A with status response.</summary>
	[DataContract]
	public class WithStatusResponse
	{
        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
		[DataMember]
		public ResponseStatus ResponseStatus { get; set; }
	}

    /// <summary>A no status response.</summary>
	[DataContract]
	public class NoStatusResponse
	{
	}

    /// <summary>A web service exception tests.</summary>
	[TestFixture]
	public class WebServiceExceptionTests
	{
        /// <summary>Can retrieve errors from dto with status response.</summary>
		[Test]
		public void Can_retrieve_Errors_from_Dto_WithStatusResponse()
		{
			var webEx = new WebServiceException
			{
				ResponseDto = new WithStatusResponse
				{
					ResponseStatus = new ResponseStatus
					{
						ErrorCode = "errorCode",
						Message = "errorMessage",
						StackTrace = "stackTrace"
					}
				}
			};

			Assert.That(webEx.ErrorCode, Is.EqualTo("errorCode"));
			Assert.That(webEx.ErrorMessage, Is.EqualTo("errorMessage"));
			Assert.That(webEx.ServerStackTrace, Is.EqualTo("stackTrace"));
		}

        /// <summary>Can retrieve empty errors from dto no status response.</summary>
		[Test]
		public void Can_retrieve_empty_Errors_from_Dto_NoStatusResponse()
		{
			var webEx = new WebServiceException
			{
				ResponseDto = new NoStatusResponse()
			};

			Assert.That(webEx.ErrorCode, Is.Null);
			Assert.That(webEx.ErrorMessage, Is.Null);
			Assert.That(webEx.ServerStackTrace, Is.Null);
		}

        /// <summary>Can retrieve errors from response body if response dto does not contain response status.</summary>
	    [Test]
	    public void Can_Retrieve_Errors_From_ResponseBody_If_ResponseDto_Does_Not_Contain_ResponseStatus()
	    {
	        var webEx = new WebServiceException
	            {
	                ResponseDto = new List<string> {"123"},
	                ResponseBody = "{\"ResponseStatus\":" +
	                               "{\"ErrorCode\":\"UnauthorizedAccessException\"," +
	                               "\"Message\":\"Error Message\"," +
	                               "\"StackTrace\":\"Some Stack Trace\",\"Errors\":[]}}"
	            };
	        Assert.That(webEx.ErrorCode, Is.EqualTo("UnauthorizedAccessException"));
            Assert.That(webEx.ErrorMessage, Is.EqualTo("Error Message"));
            Assert.That(webEx.ServerStackTrace, Is.EqualTo("Some Stack Trace"));
	    }
	}

}