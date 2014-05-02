using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using System.Threading;
using Funq;
using NServiceKit.Common.Extensions;
using NServiceKit.Common.Web;
using NServiceKit.Configuration;
using NServiceKit.DataAnnotations;
using NServiceKit.Logging;
using NServiceKit.Logging.Support.Logging;
using NServiceKit.OrmLite;
using NServiceKit.OrmLite.Sqlite;
using NServiceKit.Plugins.ProtoBuf;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface;
using NServiceKit.ServiceInterface.ServiceModel;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints.Tests.IntegrationTests;
using NServiceKit.WebHost.Endpoints.Tests.Support.Operations;

namespace NServiceKit.WebHost.Endpoints.Tests.Support.Host
{

    /// <summary>A get factorial.</summary>
	[Route("/factorial/{ForNumber}")]
	[DataContract]
	public class GetFactorial
	{
        /// <summary>Gets or sets for number.</summary>
        ///
        /// <value>for number.</value>
		[DataMember]
		public long ForNumber { get; set; }
	}

    /// <summary>A get factorial response.</summary>
	[DataContract]
	public class GetFactorialResponse
	{
        /// <summary>Gets or sets the result.</summary>
        ///
        /// <value>The result.</value>
		[DataMember]
		public long Result { get; set; }
	}

    /// <summary>A get factorial service.</summary>
	public class GetFactorialService : IService
	{
        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public object Any(GetFactorial request)
		{
			return new GetFactorialResponse { Result = GetFactorial(request.ForNumber) };
		}

        /// <summary>Gets a factorial.</summary>
        ///
        /// <param name="n">The long to process.</param>
        ///
        /// <returns>The factorial.</returns>
		public static long GetFactorial(long n)
		{
			return n > 1 ? n * GetFactorial(n - 1) : 1;
		}
	}

    /// <summary>The always throws.</summary>
	[DataContract]
	public class AlwaysThrows { }

    /// <summary>The always throws response.</summary>
	[DataContract]
	public class AlwaysThrowsResponse : IHasResponseStatus
	{
        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
		[DataMember]
		public ResponseStatus ResponseStatus { get; set; }
	}

    /// <summary>The always throws service.</summary>
	public class AlwaysThrowsService : ServiceInterface.Service
	{
        /// <summary>Anies the given request.</summary>
        ///
        /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or illegal values.</exception>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
	    public object Any(AlwaysThrows request)
		{
			throw new ArgumentException("This service always throws an error");
		}
	}


    /// <summary>A movie.</summary>
	[Route("/movies", "POST,PUT")]
	[Route("/movies/{Id}")]
	[DataContract]
	public class Movie
	{
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.Support.Host.Movie class.</summary>
		public Movie()
		{
			this.Genres = new List<string>();
		}

        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
        [DataMember(Order = 1)]
		[AutoIncrement]
		public int Id { get; set; }

        /// <summary>Gets or sets the identifier of the imdb.</summary>
        ///
        /// <value>The identifier of the imdb.</value>
        [DataMember(Order = 2)]
		public string ImdbId { get; set; }

        /// <summary>Gets or sets the title.</summary>
        ///
        /// <value>The title.</value>
        [DataMember(Order = 3)]
		public string Title { get; set; }

        /// <summary>Gets or sets the rating.</summary>
        ///
        /// <value>The rating.</value>
        [DataMember(Order = 4)]
		public decimal Rating { get; set; }

        /// <summary>Gets or sets the director.</summary>
        ///
        /// <value>The director.</value>
        [DataMember(Order = 5)]
		public string Director { get; set; }

        /// <summary>Gets or sets the release date.</summary>
        ///
        /// <value>The release date.</value>
        [DataMember(Order = 6)]
		public DateTime ReleaseDate { get; set; }

        /// <summary>Gets or sets the tag line.</summary>
        ///
        /// <value>The tag line.</value>
        [DataMember(Order = 7)]
		public string TagLine { get; set; }

        /// <summary>Gets or sets the genres.</summary>
        ///
        /// <value>The genres.</value>
        [DataMember(Order = 8)]
		public List<string> Genres { get; set; }

		#region AutoGen ReSharper code, only required by tests

        /// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />.</summary>
        ///
        /// <param name="other">The movie to compare to this object.</param>
        ///
        /// <returns>true if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />; otherwise, false.</returns>
		public bool Equals(Movie other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.ImdbId, ImdbId)
				&& Equals(other.Title, Title)
				&& other.Rating == Rating
				&& Equals(other.Director, Director)
				&& other.ReleaseDate.Equals(ReleaseDate)
				&& Equals(other.TagLine, TagLine)
				&& Genres.EquivalentTo(other.Genres);
		}

        /// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />.</summary>
        ///
        /// <param name="obj">The object to compare with the current object.</param>
        ///
        /// <returns>true if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />; otherwise, false.</returns>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(Movie)) return false;
			return Equals((Movie)obj);
		}

        /// <summary>Serves as a hash function for a particular type.</summary>
        ///
        /// <returns>A hash code for the current <see cref="T:System.Object" />.</returns>
		public override int GetHashCode()
		{
			return ImdbId != null ? ImdbId.GetHashCode() : 0;
		}
		#endregion
	}

    /// <summary>A movie response.</summary>
	[DataContract]
	public class MovieResponse
	{
        /// <summary>Gets or sets the movie.</summary>
        ///
        /// <value>The movie.</value>
		[DataMember]
		public Movie Movie { get; set; }
	}


    /// <summary>A movie service.</summary>
    public class MovieService : ServiceInterface.Service
	{
        /// <summary>Gets or sets the database factory.</summary>
        ///
        /// <value>The database factory.</value>
		public IDbConnectionFactory DbFactory { get; set; }

		/// <summary>
		/// GET /movies/{Id} 
		/// </summary>
		public object Get(Movie movie)
		{
			return new MovieResponse {
				Movie = DbFactory.Run(db => db.GetById<Movie>(movie.Id))
			};
		}

		/// <summary>
		/// POST /movies
		/// </summary>
		public object Post(Movie movie)
		{
			var newMovieId = DbFactory.Run(db => {
				db.Insert(movie);
				return db.GetLastInsertId();
			});

			var newMovie = new MovieResponse {
				Movie = DbFactory.Run(db => db.GetById<Movie>(newMovieId))
			};
			return new HttpResult(newMovie) {
				StatusCode = HttpStatusCode.Created,
				Headers = {
					{ HttpHeaders.Location, this.RequestContext.AbsoluteUri.WithTrailingSlash() + newMovieId }
				}
			};
		}

		/// <summary>
		/// PUT /movies
		/// </summary>
		public object Put(Movie movie)
		{
			DbFactory.Run(db => db.Save(movie));
			return new MovieResponse();
		}

		/// <summary>
		/// DELETE /movies/{Id}
		/// </summary>
		public object Delete(Movie request)
		{
			DbFactory.Run(db => db.DeleteById<Movie>(request.Id));
			return new MovieResponse();
		}
	}


    /// <summary>A movies.</summary>
	[DataContract]
	[Route("/movies", "GET")]
    [Route("/movies/genres/{Genre}")]
	public class Movies
	{
        /// <summary>Gets or sets the genre.</summary>
        ///
        /// <value>The genre.</value>
		[DataMember]
		public string Genre { get; set; }

        /// <summary>Gets or sets the movie.</summary>
        ///
        /// <value>The movie.</value>
		[DataMember]
		public Movie Movie { get; set; }
	}

    /// <summary>The movies response.</summary>
	[DataContract]
	public class MoviesResponse
	{
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.Support.Host.MoviesResponse class.</summary>
		public MoviesResponse()
		{
			Movies = new List<Movie>();
		}

        /// <summary>Gets or sets the movies.</summary>
        ///
        /// <value>The movies.</value>
		[DataMember(Order = 1)]
		public List<Movie> Movies { get; set; }
	}

    /// <summary>The movies service.</summary>
    public class MoviesService : ServiceInterface.Service
	{
		/// <summary>
		/// GET /movies 
		/// GET /movies/genres/{Genre}
		/// </summary>
		public object Get(Movies request)
		{
			var response = new MoviesResponse {
				Movies = request.Genre.IsNullOrEmpty()
					? Db.Select<Movie>()
					: Db.Select<Movie>("Genres LIKE {0}", "%" + request.Genre + "%")
			};

			return response;
		}
	}

    /// <summary>The movies zip.</summary>
	public class MoviesZip
	{
        /// <summary>Gets or sets the genre.</summary>
        ///
        /// <value>The genre.</value>
		public string Genre { get; set; }

        /// <summary>Gets or sets the movie.</summary>
        ///
        /// <value>The movie.</value>
		public Movie Movie { get; set; }
	}

    /// <summary>The movies zip response.</summary>
	public class MoviesZipResponse
	{
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.Support.Host.MoviesZipResponse class.</summary>
		public MoviesZipResponse()
		{
			Movies = new List<Movie>();
		}

        /// <summary>Gets or sets the movies.</summary>
        ///
        /// <value>The movies.</value>
		public List<Movie> Movies { get; set; }
	}

    /// <summary>The movies zip service.</summary>
    public class MoviesZipService : ServiceInterface.Service
	{
        /// <summary>Gets or sets the database factory.</summary>
        ///
        /// <value>The database factory.</value>
		public IDbConnectionFactory DbFactory { get; set; }

        /// <summary>Gets the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public object Get(MoviesZip request)
		{
			return Post(request);
		}

        /// <summary>Post this message.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public object Post(MoviesZip request)
		{
			var response = new MoviesZipResponse {
				Movies = request.Genre.IsNullOrEmpty()
					? DbFactory.Run(db => db.Select<Movie>())
					: DbFactory.Run(db => db.Select<Movie>("Genres LIKE {0}", "%" + request.Genre + "%"))
			};

			return RequestContext.ToOptimizedResult(response);
		}
	}


    /// <summary>A reset movies.</summary>
	[DataContract]
	[Route("/reset-movies")]
	public class ResetMovies { }

    /// <summary>A reset movies response.</summary>
	[DataContract]
	public class ResetMoviesResponse
		: IHasResponseStatus
	{
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.Support.Host.ResetMoviesResponse class.</summary>
		public ResetMoviesResponse()
		{
			this.ResponseStatus = new ResponseStatus();
		}

        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
		[DataMember]
		public ResponseStatus ResponseStatus { get; set; }
	}

    /// <summary>A reset movies service.</summary>
	public class ResetMoviesService : ServiceInterface.Service
	{
        /// <summary>The top 5 movies.</summary>
		public static List<Movie> Top5Movies = new List<Movie>
		{
			new Movie { ImdbId = "tt0111161", Title = "The Shawshank Redemption", Rating = 9.2m, Director = "Frank Darabont", ReleaseDate = new DateTime(1995,2,17), TagLine = "Fear can hold you prisoner. Hope can set you free.", Genres = new List<string>{"Crime","Drama"}, },
			new Movie { ImdbId = "tt0068646", Title = "The Godfather", Rating = 9.2m, Director = "Francis Ford Coppola", ReleaseDate = new DateTime(1972,3,24), TagLine = "An offer you can't refuse.", Genres = new List<string> {"Crime","Drama", "Thriller"}, },
			new Movie { ImdbId = "tt1375666", Title = "Inception", Rating = 9.2m, Director = "Christopher Nolan", ReleaseDate = new DateTime(2010,7,16), TagLine = "Your mind is the scene of the crime", Genres = new List<string>{"Action", "Mystery", "Sci-Fi", "Thriller"}, },
			new Movie { ImdbId = "tt0071562", Title = "The Godfather: Part II", Rating = 9.0m, Director = "Francis Ford Coppola", ReleaseDate = new DateTime(1974,12,20), Genres = new List<string> {"Crime","Drama", "Thriller"}, },
			new Movie { ImdbId = "tt0060196", Title = "The Good, the Bad and the Ugly", Rating = 9.0m, Director = "Sergio Leone", ReleaseDate = new DateTime(1967,12,29), TagLine = "They formed an alliance of hate to steal a fortune in dead man's gold", Genres = new List<string>{"Adventure","Western"}, },
		};

        /// <summary>Post this message.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public object Post(ResetMovies request)
		{
            const bool overwriteTable = true;
            Db.CreateTable<Movie>(overwriteTable);
            Db.SaveAll(Top5Movies);

			return new ResetMoviesResponse();
		}
	}

    /// <summary>Encapsulates the result of a get http.</summary>
	[DataContract]
	public class GetHttpResult { }

    /// <summary>A get HTTP result response.</summary>
	[DataContract]
	public class GetHttpResultResponse
	{
        /// <summary>Gets or sets the result.</summary>
        ///
        /// <value>The result.</value>
		[DataMember]
		public string Result { get; set; }
	}

    /// <summary>A HTTP result service.</summary>
	public class HttpResultService : IService
	{
        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public object Any(GetHttpResult request)
		{
			var getHttpResultResponse = new GetHttpResultResponse { Result = "result" };
			return new HttpResult(getHttpResultResponse);
		}
	}


    /// <summary>An inbox post response request.</summary>
    [Route("/inbox/{Id}/responses", "GET, PUT, OPTIONS")]
    public class InboxPostResponseRequest
    {
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
        public int Id { get; set; }

        /// <summary>Gets or sets the responses.</summary>
        ///
        /// <value>The responses.</value>
        public List<PageElementResponseDTO> Responses { get; set; }
    }

    /// <summary>An inbox post response request response.</summary>
    public class InboxPostResponseRequestResponse
    {
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
        public int Id { get; set; }

        /// <summary>Gets or sets the responses.</summary>
        ///
        /// <value>The responses.</value>
        public List<PageElementResponseDTO> Responses { get; set; }
    }

    /// <summary>A page element response dto.</summary>
    public class PageElementResponseDTO
    {
        /// <summary>Gets or sets the identifier of the page element.</summary>
        ///
        /// <value>The identifier of the page element.</value>
        public int PageElementId { get; set; }

        /// <summary>Gets or sets the type of the page element.</summary>
        ///
        /// <value>The type of the page element.</value>
        public string PageElementType { get; set; }

        /// <summary>Gets or sets the page element response.</summary>
        ///
        /// <value>The page element response.</value>
        public string PageElementResponse { get; set; }
    }

    /// <summary>An inbox post response request service.</summary>
    public class InboxPostResponseRequestService : ServiceInterface.Service
    {
        /// <summary>Anies the given request.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Any(InboxPostResponseRequest request)
        {
            if (request.Responses == null || request.Responses.Count == 0)
            {
                throw new ArgumentNullException("Responses");
            }
            return new InboxPostResponseRequestResponse {
                Id = request.Id,
                Responses = request.Responses
            };
        }
    }

    /// <summary>An inbox post.</summary>
    [Route("/inbox/{Id}/responses", "GET, PUT, OPTIONS")]
    public class InboxPost
    {
        /// <summary>Gets or sets a value indicating whether the throw.</summary>
        ///
        /// <value>true if throw, false if not.</value>
        public bool Throw { get; set; }

        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
        public int Id { get; set; }
    }

    /// <summary>An inbox post service.</summary>
    public class InboxPostService : ServiceInterface.Service
    {
        /// <summary>Anies the given request.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Any(InboxPost request)
        {
            if (request.Throw)
                throw new ArgumentNullException("Throw");
            
            return null;
        }
    }

    /// <summary>A long running.</summary>
    [DataContract]
    [Route("/long_running")]
    public class LongRunning { }

    /// <summary>A long running service.</summary>
    public class LongRunningService : ServiceInterface.Service
    {
        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Any(LongRunning request)
        {
            Thread.Sleep(5000);

            return "LongRunning done.";
        }
    }

    /// <summary>An example application host HTTP listener.</summary>
    public class ExampleAppHostHttpListener
		: AppHostHttpListenerBase
	{
		//private static ILog log;

        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.Support.Host.ExampleAppHostHttpListener class.</summary>
		public ExampleAppHostHttpListener()
			: base("NServiceKit Examples", typeof(GetFactorialService).Assembly)
		{
			LogManager.LogFactory = new DebugLogFactory();
			//log = LogManager.GetLogger(typeof(ExampleAppHostHttpListener));
		}

        /// <summary>Gets or sets the configure filter.</summary>
        ///
        /// <value>The configure filter.</value>
		public Action<Container> ConfigureFilter { get; set; }

        /// <summary>Configures the given container.</summary>
        ///
        /// <param name="container">The container.</param>
		public override void Configure(Container container)
		{
			EndpointHostConfig.Instance.GlobalResponseHeaders.Clear();

			//Signal advanced web browsers what HTTP Methods you accept
			base.SetConfig(new EndpointHostConfig {
				GlobalResponseHeaders =
				{
					{ "Access-Control-Allow-Origin", "*" },
					{ "Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS" },
				},
				WsdlServiceNamespace = "http://www.NServiceKit.net/types",
				LogFactory = new ConsoleLogFactory(),
				DebugMode = true,
			});

			this.RegisterRequestBinder<CustomRequestBinder>(
				httpReq => new CustomRequestBinder { IsFromBinder = true });

			Routes
				.Add<Movies>("/custom-movies", "GET")
				.Add<Movies>("/custom-movies/genres/{Genre}")
				.Add<Movie>("/custom-movies", "POST,PUT")
				.Add<Movie>("/custom-movies/{Id}")
				.Add<GetFactorial>("/fact/{ForNumber}")
				.Add<MoviesZip>("/movies.zip")
				.Add<GetHttpResult>("/gethttpresult")
			;

			container.Register<IResourceManager>(new ConfigurationResourceManager());

			//var appSettings = container.Resolve<IResourceManager>();

			container.Register(c => new ExampleConfig(c.Resolve<IResourceManager>()));
			//var appConfig = container.Resolve<ExampleConfig>();

			container.Register<IDbConnectionFactory>(c =>
				new OrmLiteConnectionFactory(
					":memory:", false,
					SqliteOrmLiteDialectProvider.Instance));

			var resetMovies = container.Resolve<ResetMoviesService>();
			resetMovies.Post(null);

			//var movies = container.Resolve<IDbConnectionFactory>().Exec(x => x.Select<Movie>());
			//Console.WriteLine(movies.Dump());

			if (ConfigureFilter != null)
			{
				ConfigureFilter(container);
			}

            Plugins.Add(new ProtoBufFormat());
            Plugins.Add(new RequestInfoFeature());
		}
	}

    /// <summary>An example application host HTTP listener long running.</summary>
    public class ExampleAppHostHttpListenerLongRunning
    : AppHostHttpListenerLongRunningBase
    {
        //private static ILog log;

        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.Support.Host.ExampleAppHostHttpListenerLongRunning class.</summary>
        public ExampleAppHostHttpListenerLongRunning()
            : base("NServiceKit Examples", 500, typeof(GetFactorialService).Assembly)
        {
            LogManager.LogFactory = new DebugLogFactory();
            //log = LogManager.GetLogger(typeof(ExampleAppHostHttpListener));
        }

        /// <summary>Gets or sets the configure filter.</summary>
        ///
        /// <value>The configure filter.</value>
        public Action<Container> ConfigureFilter { get; set; }

        /// <summary>Configures the given container.</summary>
        ///
        /// <param name="container">The container.</param>
        public override void Configure(Container container)
        {
            EndpointHostConfig.Instance.GlobalResponseHeaders.Clear();

            //Signal advanced web browsers what HTTP Methods you accept
            base.SetConfig(new EndpointHostConfig
            {
                GlobalResponseHeaders =
				{
					{ "Access-Control-Allow-Origin", "*" },
					{ "Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS" },
				},
                WsdlServiceNamespace = "http://www.NServiceKit.net/types",
                LogFactory = new ConsoleLogFactory(),
                DebugMode = true,
            });

            this.RegisterRequestBinder<CustomRequestBinder>(
                httpReq => new CustomRequestBinder { IsFromBinder = true });

            Routes
                .Add<Movies>("/custom-movies", "GET")
                .Add<Movies>("/custom-movies/genres/{Genre}")
                .Add<Movie>("/custom-movies", "POST,PUT")
                .Add<Movie>("/custom-movies/{Id}")
                .Add<GetFactorial>("/fact/{ForNumber}")
                .Add<MoviesZip>("/movies.zip")
                .Add<GetHttpResult>("/gethttpresult")
            ;

            container.Register<IResourceManager>(new ConfigurationResourceManager());

            //var appSettings = container.Resolve<IResourceManager>();

            container.Register(c => new ExampleConfig(c.Resolve<IResourceManager>()));
            //var appConfig = container.Resolve<ExampleConfig>();

            container.Register<IDbConnectionFactory>(c =>
                new OrmLiteConnectionFactory(
                    ":memory:", false,
                    SqliteOrmLiteDialectProvider.Instance));

            var resetMovies = container.Resolve<ResetMoviesService>();
            resetMovies.Post(null);

            //var movies = container.Resolve<IDbConnectionFactory>().Exec(x => x.Select<Movie>());
            //Console.WriteLine(movies.Dump());

            if (ConfigureFilter != null)
            {
                ConfigureFilter(container);
            }

            Plugins.Add(new RequestInfoFeature());
        }
    }

}