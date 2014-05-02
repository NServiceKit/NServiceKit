using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using NUnit.Framework;
using NServiceKit.Common.Extensions;
using NServiceKit.Common.Web;
using NServiceKit.ServiceClient.Web;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints.Tests.Support.Host;

namespace NServiceKit.WebHost.Endpoints.Tests
{
    /// <summary>A CSV content type filter tests.</summary>
	[TestFixture]
	public class CsvContentTypeFilterTests
	{
		const int HeaderRowCount = 1;
		private const string ListeningOn = "http://localhost:1182/";

		ExampleAppHostHttpListener appHost;

        /// <summary>Executes the test fixture set up action.</summary>
		[TestFixtureSetUp]
		public void OnTestFixtureSetUp()
		{
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

        /// <summary>Executes for 30secs operation.</summary>
		[Test]
		[Explicit("Helps debugging when you need to find out WTF is going on")]
		public void Run_for_30secs()
		{
			Thread.Sleep(30000);
		}

		private static void FailOnAsyncError<T>(T response, Exception ex)
		{
			Assert.Fail(ex.Message);
		}

        /// <summary>Can serialize movies dto.</summary>
		[Test]
		public void Can_Serialize_Movies_Dto()
		{
			var csv = CsvSerializer.SerializeToString(ResetMoviesService.Top5Movies);
			var csvRows = csv.Split('\n').Where(x => !x.IsNullOrEmpty()).ToArray();
			Assert.That(csvRows.Length, Is.EqualTo(HeaderRowCount + ResetMoviesService.Top5Movies.Count));
		}

        /// <summary>Can serialize movie response dto.</summary>
		[Test]
		public void Can_Serialize_MovieResponse_Dto()
		{
			var request = new MovieResponse { Movie = ResetMoviesService.Top5Movies[0] };
			var csv = CsvSerializer.SerializeToString(request);
			var csvRows = csv.Split('\n').Where(x => !x.IsNullOrEmpty()).ToArray();
			Assert.That(csvRows.Length, Is.EqualTo(HeaderRowCount + 1));
		}

        /// <summary>Can serialize movies response dto.</summary>
		[Test]
		public void Can_Serialize_MoviesResponse_Dto()
		{
			var request = new MoviesResponse { Movies = ResetMoviesService.Top5Movies };
			var csv = CsvSerializer.SerializeToString(request);
			var csvRows = csv.Split('\n').Where(x => !x.IsNullOrEmpty()).ToArray();
			Assert.That(csvRows.Length, Is.EqualTo(HeaderRowCount + ResetMoviesService.Top5Movies.Count));
		}

        /// <summary>Can download movies in CSV.</summary>
		[Test][Ignore("Fails because CSV Deserializer is not implemented")]
		public void Can_download_movies_in_Csv()
		{
			var asyncClient = new AsyncServiceClient
			{
				ContentType = ContentType.Csv,
				StreamSerializer = (r,o,s) => CsvSerializer.SerializeToStream(o,s),
				StreamDeserializer = CsvSerializer.DeserializeFromStream,
			};

			MoviesResponse response = null;
			asyncClient.SendAsync<MoviesResponse>(HttpMethods.Get, ListeningOn + "movies", null,
				r => response = r, FailOnAsyncError);

			Thread.Sleep(1000);

			Assert.That(response, Is.Not.Null, "No response received");
		}

        /// <summary>Can download CSV movies using CSV syncreply endpoint.</summary>
		[Test]
		public void Can_download_CSV_movies_using_csv_syncreply_endpoint()
		{
			var req = (HttpWebRequest)WebRequest.Create(ListeningOn + "csv/syncreply/Movies");
			
			var res = req.GetResponse();
			Assert.That(res.ContentType, Is.EqualTo(ContentType.Csv));
			Console.WriteLine(res.Headers);
			Assert.That(res.Headers[HttpHeaders.ContentDisposition], Is.EqualTo("attachment;filename=Movies.csv"));
			
			var csvRows = new StreamReader(res.GetResponseStream()).ReadLines().ToList();

			const int headerRowCount = 1;
			Assert.That(csvRows, Has.Count.EqualTo(headerRowCount + ResetMoviesService.Top5Movies.Count));
			//Console.WriteLine(csvRows.Join("\n"));
		}

        /// <summary>Can download CSV movies using CSV accept and rest path.</summary>
		[Test]
		public void Can_download_CSV_movies_using_csv_Accept_and_RestPath()
		{
			var req = (HttpWebRequest)WebRequest.Create(ListeningOn + "movies");
			req.Accept = ContentType.Csv;

			var res = req.GetResponse();
			Assert.That(res.ContentType, Is.EqualTo(ContentType.Csv));
			Assert.That(res.Headers[HttpHeaders.ContentDisposition], Is.EqualTo("attachment;filename=Movies.csv"));

			var csvRows = new StreamReader(res.GetResponseStream()).ReadLines().ToList();

			Assert.That(csvRows, Has.Count.EqualTo(HeaderRowCount + ResetMoviesService.Top5Movies.Count));
			//Console.WriteLine(csvRows.Join("\n"));
		}

        /// <summary>Can download CSV hello using CSV syncreply endpoint.</summary>
		[Test]
		public void Can_download_CSV_Hello_using_csv_syncreply_endpoint()
		{
			var req = (HttpWebRequest)WebRequest.Create(ListeningOn + "csv/syncreply/Hello?Name=World!");

			var res = req.GetResponse();
			Assert.That(res.ContentType, Is.EqualTo(ContentType.Csv));
			Assert.That(res.Headers[HttpHeaders.ContentDisposition], Is.EqualTo("attachment;filename=Hello.csv"));

			var csv = new StreamReader(res.GetResponseStream()).ReadToEnd();
			Assert.That(csv, Is.EqualTo("Result\r\n\"Hello, World!\"\r\n"));

			Console.WriteLine(csv);
		}

        /// <summary>Can download CSV hello using CSV accept and rest path.</summary>
		[Test]
		public void Can_download_CSV_Hello_using_csv_Accept_and_RestPath()
		{
			var req = (HttpWebRequest)WebRequest.Create(ListeningOn + "hello/World!");
			req.Accept = ContentType.Csv;

			var res = req.GetResponse();
			Assert.That(res.ContentType, Is.EqualTo(ContentType.Csv));
			Assert.That(res.Headers[HttpHeaders.ContentDisposition], Is.EqualTo("attachment;filename=Hello.csv"));

			var csv = new StreamReader(res.GetResponseStream()).ReadToEnd();
			Assert.That(csv, Is.EqualTo("Result\r\n\"Hello, World!\"\r\n"));

			Console.WriteLine(csv);
		}

        /// <summary>Can download CSV movies using CSV synchronise reply path.</summary>
		[Test]
		public void Can_download_CSV_movies_using_csv_SyncReply_Path()
		{
			var req = (HttpWebRequest)WebRequest.Create(ListeningOn + "csv/syncreply/Movies");
			req.Accept = "application/xml";

			var res = req.GetResponse();
			Assert.That(res.ContentType, Is.EqualTo(ContentType.Csv));
			Assert.That(res.Headers[HttpHeaders.ContentDisposition], Is.EqualTo("attachment;filename=Movies.csv"));

			var csvRows = new StreamReader(res.GetResponseStream()).ReadLines().ToList();

			Assert.That(csvRows, Has.Count.EqualTo(HeaderRowCount + ResetMoviesService.Top5Movies.Count));
		}
	}
}