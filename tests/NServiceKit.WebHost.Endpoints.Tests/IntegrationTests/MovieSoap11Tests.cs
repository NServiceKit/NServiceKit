using System;
using System.Collections.Generic;
using NUnit.Framework;
using NServiceKit.Common.Web;
using NServiceKit.ServiceClient.Web;
using NServiceKit.Text;

namespace NServiceKit.WebHost.Endpoints.Tests.IntegrationTests
{
    /// <summary>A movie SOAP 11 tests.</summary>
    [TestFixture]
    public class MovieSoap11Tests : IntegrationTestBase
    {
        private Soap11ServiceClient soapClient;

        /// <summary>Executes the before each test action.</summary>
        [SetUp]
        public void OnBeforeEachTest(){
            soapClient = new Soap11ServiceClient(BaseUrl);
            soapClient.Send<ResetMoviesResponse>(new ResetMovies());
        }

        /// <summary>Can list all movies.</summary>
        [Test]
        public void Can_list_all_movies(){
            var response = soapClient.Send<RestMoviesResponse>(new RestMovies());
            Assert.That(response.Movies, Has.Count.EqualTo(ConfigureDatabase.Top5Movies.Count));
        }

        /// <summary>Can reset movie database.</summary>
        [Test]
        public void Can_ResetMovieDatabase()
        {
            var response = soapClient.Send<ResetMoviesResponse>(new ResetMovies());
            Assert.That(response.ResponseStatus.ErrorCode, Is.Null);
        }
    }
}
