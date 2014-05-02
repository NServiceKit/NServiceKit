using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using NServiceKit.Common.Extensions;
using NServiceKit.Logging;
using NServiceKit.Logging.Support.Logging;
using NServiceKit.Service;
using NServiceKit.ServiceClient.Web;
using NServiceKit.WebHost.Endpoints.Tests.Support.Host;

namespace NServiceKit.WebHost.Endpoints.Tests
{
    /// <summary>The asynchronous rest client tests.</summary>
	public abstract class AsyncRestClientTests
	{
		private const string ListeningOn = "http://localhost:82/";

		ExampleAppHostHttpListener appHost;

        /// <summary>Executes the test fixture set up action.</summary>
		[TestFixtureSetUp]
		public void OnTestFixtureSetUp()
		{
			LogManager.LogFactory = new ConsoleLogFactory();

			appHost = new ExampleAppHostHttpListener();
			appHost.Init();
			appHost.Start(ListeningOn);
		}

        /// <summary>Executes the test fixture tear down action.</summary>
		[TestFixtureTearDown]
		public void OnTestFixtureTearDown()
		{
			appHost.Dispose();
		}

        /// <summary>Creates asynchronous rest client.</summary>
        ///
        /// <returns>The new asynchronous rest client.</returns>
		protected abstract IRestClientAsync CreateAsyncRestClient();

		private static void FailOnAsyncError<T>(T response, Exception ex)
		{
			Assert.Fail(ex.Message);
		}

        /// <summary>Can call get asynchronous on get factorial using rest client asynchronous.</summary>
		[Test]
		public void Can_call_GetAsync_on_GetFactorial_using_RestClientAsync()
		{
			var asyncClient = CreateAsyncRestClient();

			GetFactorialResponse response = null;
			asyncClient.GetAsync<GetFactorialResponse>("factorial/3", r => response = r, FailOnAsyncError);

			Thread.Sleep(1000);

			Assert.That(response, Is.Not.Null, "No response received");
			Assert.That(response.Result, Is.EqualTo(GetFactorialService.GetFactorial(3)));
		}

        /// <summary>Can call get asynchronous on movies using rest client asynchronous.</summary>
		[Test]
		public void Can_call_GetAsync_on_Movies_using_RestClientAsync()
		{
			var asyncClient = CreateAsyncRestClient();

			MoviesResponse response = null;
			asyncClient.GetAsync<MoviesResponse>("movies", r => response = r, FailOnAsyncError);

			Thread.Sleep(1000);

			Assert.That(response, Is.Not.Null, "No response received");
			Assert.That(response.Movies.EquivalentTo(ResetMoviesService.Top5Movies));
		}

        /// <summary>Can call get asynchronous on single movie using rest client asynchronous.</summary>
		[Test]
		public void Can_call_GetAsync_on_single_Movie_using_RestClientAsync()
		{
			var asyncClient = CreateAsyncRestClient();

			MovieResponse response = null;
			asyncClient.GetAsync<MovieResponse>("movies/1", r => response = r, FailOnAsyncError);

			Thread.Sleep(1000);

			Assert.That(response, Is.Not.Null, "No response received");
			Assert.That(response.Movie.Id, Is.EqualTo(1));
		}

        /// <summary>Can call post asynchronous to add new movie using rest client asynchronous.</summary>
		[Test]
		public void Can_call_PostAsync_to_add_new_Movie_using_RestClientAsync()
		{
			var asyncClient = CreateAsyncRestClient();

			var newMovie = new Movie
			{
				ImdbId = "tt0450259",
				Title = "Blood Diamond",
				Rating = 8.0m,
				Director = "Edward Zwick",
				ReleaseDate = new DateTime(2007, 1, 26),
				TagLine = "A fisherman, a smuggler, and a syndicate of businessmen match wits over the possession of a priceless diamond.",
				Genres = new List<string> { "Adventure", "Drama", "Thriller" },
			};

			MovieResponse response = null;
			asyncClient.PostAsync<MovieResponse>("movies", newMovie,
				r => response = r, FailOnAsyncError);

			Thread.Sleep(1000);

			Assert.That(response, Is.Not.Null, "No response received");

			var createdMovie = response.Movie;
			Assert.That(createdMovie.Id, Is.GreaterThan(0));
			Assert.That(createdMovie.ImdbId, Is.EqualTo(newMovie.ImdbId));
		}

        /// <summary>Can call delete asynchronous to delete movie using rest client asynchronous.</summary>
		[Test]
		public void Can_call_DeleteAsync_to_delete_Movie_using_RestClientAsync()
		{
			var asyncClient = CreateAsyncRestClient();

			var newMovie = new Movie
			{
				ImdbId = "tt0450259",
				Title = "Blood Diamond",
				Rating = 8.0m,
				Director = "Edward Zwick",
				ReleaseDate = new DateTime(2007, 1, 26),
				TagLine = "A fisherman, a smuggler, and a syndicate of businessmen match wits over the possession of a priceless diamond.",
				Genres = new List<string> { "Adventure", "Drama", "Thriller" },
			};

			MovieResponse response = null;
			Movie createdMovie = null;
			asyncClient.PostAsync<MovieResponse>("movies", newMovie,
				r =>
				{
					createdMovie = r.Movie;
					asyncClient.DeleteAsync<MovieResponse>("movies/" + createdMovie.Id, 
						dr => response = dr, FailOnAsyncError);
				}, FailOnAsyncError);

			Thread.Sleep(1000);

			Assert.That(response, Is.Not.Null, "No response received");
			Assert.That(createdMovie, Is.Not.Null);
			Assert.That(response.Movie, Is.Null);
		}

        /// <summary>A JSON asynchronous rest service client tests.</summary>
		[TestFixture]
		public class JsonAsyncRestServiceClientTests : AsyncRestClientTests
		{
            /// <summary>Creates asynchronous rest client.</summary>
            ///
            /// <returns>The new asynchronous rest client.</returns>
			protected override IRestClientAsync CreateAsyncRestClient()
			{
				return new JsonServiceClient(ListeningOn);
			}
		}

        /// <summary>A jsv asynchronous rest service client tests.</summary>
		[TestFixture]
		public class JsvAsyncRestServiceClientTests : AsyncRestClientTests
		{
            /// <summary>Creates asynchronous rest client.</summary>
            ///
            /// <returns>The new asynchronous rest client.</returns>
			protected override IRestClientAsync CreateAsyncRestClient()
			{
                return new JsonServiceClient(ListeningOn);
			}
		}

        /// <summary>An XML asynchronous rest service client tests.</summary>
		[TestFixture]
		public class XmlAsyncRestServiceClientTests : AsyncRestClientTests
		{
            /// <summary>Creates asynchronous rest client.</summary>
            ///
            /// <returns>The new asynchronous rest client.</returns>
			protected override IRestClientAsync CreateAsyncRestClient()
			{
                return new JsonServiceClient(ListeningOn);
			}
		}
	}
}