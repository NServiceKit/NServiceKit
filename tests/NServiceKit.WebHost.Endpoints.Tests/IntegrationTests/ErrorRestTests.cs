using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NUnit.Framework;
using NServiceKit.ServiceClient.Web;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface.ServiceModel;

namespace NServiceKit.WebHost.Endpoints.Tests.IntegrationTests
{
    /// <summary>An error rest tests.</summary>
	[TestFixture]
	public class ErrorRestTests : IntegrationTestBase
	{
        /// <summary>Tests reproduce error.</summary>
		[Test]
		public void ReproduceErrorTest()
		{
			var restClient = new JsonServiceClient(BaseUrl);

			var errorList = restClient.Get<ErrorCollectionResponse>("error");
			Assert.That(errorList.Result.Count, Is.EqualTo(1));

			var error = restClient.Post<ErrorResponse>("error", new Error { Id = "Test" });
			Assert.That(error, !Is.Null);
		}

        /// <summary>Use same rest client error.</summary>
		[Test]
		public void UseSameRestClientError()
		{
			var restClient = new JsonServiceClient(BaseUrl);
			var errorList = restClient.Get<ErrorCollectionResponse>("error");
			Assert.That(errorList.Result.Count, Is.EqualTo(1));

			var error = restClient.Get<ErrorResponse>("error/Test");
			Assert.That(error, !Is.Null);
		}
	}

    /// <summary>An error.</summary>
	[Route("/error")]
	[Route("/error/{Id}")]
	public class Error
	{
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.IntegrationTests.Error class.</summary>
		public Error()
		{
		}

        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
		public string Id { get; set; }

        /// <summary>Gets or sets the inner.</summary>
        ///
        /// <value>The inner.</value>
		public Error Inner { get; set; }
	}

    /// <summary>An error service.</summary>
	public class ErrorService : ServiceInterface.Service
	{
        /// <summary>Gets the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public object Get(Error request)
		{
			if (request != null && !String.IsNullOrEmpty(request.Id))
				return new ErrorResponse(new Error { Id = "Test" });

			return new ErrorCollectionResponse(new List<Error> { new Error { Id = "TestCollection" } });
		}

        /// <summary>Post this message.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public object Post(Error request)
		{
			return new ErrorResponse(request);
		}
	}

    /// <summary>An error response.</summary>
	public class ErrorResponse : IHasResponseStatus
	{
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.IntegrationTests.ErrorResponse class.</summary>
        ///
        /// <param name="result">The result.</param>
		public ErrorResponse(Error result)
		{
			Result = result;
			ResponseStatus = new ResponseStatus();
		}

        /// <summary>Gets or sets the result.</summary>
        ///
        /// <value>The result.</value>
		public Error Result { get; set; }

        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
		public ResponseStatus ResponseStatus { get; set; }
	}

    /// <summary>An error collection response.</summary>
	public class ErrorCollectionResponse : IHasResponseStatus
	{
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.IntegrationTests.ErrorCollectionResponse class.</summary>
        ///
        /// <param name="result">The result.</param>
		public ErrorCollectionResponse(IList<Error> result)
		{
			Result = new Collection<Error>(result);
			ResponseStatus = new ResponseStatus();
		}

        /// <summary>Gets or sets the result.</summary>
        ///
        /// <value>The result.</value>
		public Collection<Error> Result { get; set; }

        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
		public ResponseStatus ResponseStatus { get; set; }
	}

}