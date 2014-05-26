using System;
using Funq;
using NServiceKit.Authentication.OpenId;
using NServiceKit.CacheAccess;
using NServiceKit.CacheAccess.Providers;
using NServiceKit.Common;
using NServiceKit.Common.Utils;
using NServiceKit.Configuration;
using NServiceKit.Messaging;
using NServiceKit.MiniProfiler;
using NServiceKit.MiniProfiler.Data;
using NServiceKit.OrmLite;
using NServiceKit.Plugins.ProtoBuf;
using NServiceKit.Redis;
using NServiceKit.Redis.Messaging;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface;
using NServiceKit.ServiceInterface.Admin;
using NServiceKit.ServiceInterface.Auth;
using NServiceKit.Api.Swagger;
using NServiceKit.ServiceInterface.Validation;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints;
using NServiceKit.WebHost.IntegrationTests.Services;
using NServiceKit.WebHost.IntegrationTests.Tests;
using NServiceKit.Razor;

namespace NServiceKit.WebHost.IntegrationTests
{
    /// <summary>A global.</summary>
    public class Global : System.Web.HttpApplication
    {
        /// <summary>An application host.</summary>
        public class AppHost
            : AppHostBase
        {
            /// <summary>Initializes a new instance of the NServiceKit.WebHost.IntegrationTests.Global.AppHost class.</summary>
            public AppHost()
                : base("NServiceKit WebHost IntegrationTests", typeof(Reverse).Assembly) {}

            /// <summary>Configures the given container.</summary>
            ///
            /// <param name="container">The container.</param>
            public override void Configure(Container container)
            {
                JsConfig.EmitCamelCaseNames = true;

				this.PreRequestFilters.Add((req, res) => {
					req.Items["_DataSetAtPreRequestFilters"] = true;
				});

                this.RequestFilters.Add((req, res, dto) => {
                    req.Items["_DataSetAtRequestFilters"] = true;

                    var requestFilter = dto as RequestFilter;
                    if (requestFilter != null)
                    {
                        res.StatusCode = requestFilter.StatusCode;
                        if (!requestFilter.HeaderName.IsNullOrEmpty())
                        {
                            res.AddHeader(requestFilter.HeaderName, requestFilter.HeaderValue);
                        }
                        res.Close();
                    }

                    var secureRequests = dto as IRequiresSession;
                    if (secureRequests != null)
                    {
                        res.ReturnAuthRequired();
                    }
                });

                this.Container.Register<IDbConnectionFactory>(c =>
                    new OrmLiteConnectionFactory(
                        "~/App_Data/db.sqlite".MapHostAbsolutePath(),
                        SqliteDialect.Provider) {
                            ConnectionFilter = x => new ProfiledDbConnection(x, Profiler.Current)
                        });

                this.Container.Register<ICacheClient>(new MemoryCacheClient());
                //this.Container.Register<ICacheClient>(new BasicRedisClientManager());

                ConfigureAuth(container);

                //this.Container.Register<ISessionFactory>(
                //    c => new SessionFactory(c.Resolve<ICacheClient>()));

                var dbFactory = this.Container.Resolve<IDbConnectionFactory>();
                dbFactory.Run(db => db.CreateTable<Movie>(true));
                ModelConfig<Movie>.Id(x => x.Title);
                Routes
                    .Add<Movies>("/custom-movies", "GET, OPTIONS")
                    .Add<Movies>("/custom-movies/genres/{Genre}")
                    .Add<Movie>("/custom-movies", "POST,PUT")
                    .Add<Movie>("/custom-movies/{Id}")
                    .Add<MqHostStats>("/mqstats");


                var resetMovies = this.Container.Resolve<ResetMoviesService>();
                resetMovies.Post(null);

#if true
                var erp = new EmbeddedResourceRazorPlugin();
                erp.AddViewContainingAssemblies(typeof (SwaggerEmbeddedFeature).Assembly);
                Plugins.Add(erp);
#endif
                Plugins.Add(new ValidationFeature());
                Plugins.Add(new SessionFeature());
                Plugins.Add(new ProtoBufFormat());
                Plugins.Add(new SwaggerEmbeddedFeature() { ApiVersion = "1.0", SwaggerUiEnabled = true });
                Plugins.Add(new RequestLogsFeature());

                container.RegisterValidators(typeof(CustomersValidator).Assembly);


                container.Register(c => new FunqSingletonScope()).ReusedWithin(ReuseScope.Default);
                container.Register(c => new FunqRequestScope()).ReusedWithin(ReuseScope.Request);
                container.Register(c => new FunqNoneScope()).ReusedWithin(ReuseScope.None);
                Routes.Add<IocScope>("/iocscope");


                //var onlyEnableFeatures = Feature.All.Remove(Feature.Jsv | Feature.Soap);
                SetConfig(new EndpointHostConfig {
                    GlobalResponseHeaders = {
                        { "Access-Control-Allow-Origin", "*" },
                        { "Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS" },
                        { "Access-Control-Allow-Headers", "Content-Type, X-Requested-With" },
                    },
                    AdminAuthSecret = AuthTestsBase.AuthSecret,
                    //EnableFeatures = onlyEnableFeatures,
                    DebugMode = true, //Show StackTraces for easier debugging
                });
            }

            //Configure NServiceKit Authentication and CustomUserSession
            private void ConfigureAuth(Funq.Container container)
            {
                Routes
                    .Add<Registration>("/register");

                var appSettings = new AppSettings();

                Plugins.Add(new AuthFeature(() => new CustomUserSession(),
                    new IAuthProvider[] {
						new CredentialsAuthProvider(appSettings), 
						new FacebookAuthProvider(appSettings), 
						new TwitterAuthProvider(appSettings), 
                        new GoogleOpenIdOAuthProvider(appSettings), 
                        new OpenIdOAuthProvider(appSettings), 
                        new DigestAuthProvider(appSettings),
						new BasicAuthProvider(appSettings), 
					}));

                Plugins.Add(new RegistrationFeature());

                container.Register<IUserAuthRepository>(c =>
                    new OrmLiteAuthRepository(c.Resolve<IDbConnectionFactory>()));

                var authRepo = (OrmLiteAuthRepository)container.Resolve<IUserAuthRepository>();
                if (new AppSettings().Get("RecreateTables", true))
                    authRepo.DropAndReCreateTables();
                else
                    authRepo.CreateMissingTables();
            }
        }

        /// <summary>Event handler. Called by Application for start events.</summary>
        ///
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">     Event information.</param>
        protected void Application_Start(object sender, EventArgs e)
        {
            var appHost = new AppHost();
            appHost.Init();
        }

        /// <summary>Event handler. Called by Application for begin request events.</summary>
        ///
        /// <param name="src">Source for the.</param>
        /// <param name="e">  Event information.</param>
        protected void Application_BeginRequest(object src, EventArgs e)
        {
            if (Request.IsLocal)
                Profiler.Start();
        }

        /// <summary>Event handler. Called by Application for end request events.</summary>
        ///
        /// <param name="src">Source for the.</param>
        /// <param name="e">  Event information.</param>
        protected void Application_EndRequest(object src, EventArgs e)
        {
            Profiler.Stop();

            var mqHost = AppHostBase.Instance.Container.TryResolve<IMessageService>();
            if (mqHost != null)
                mqHost.Start();
        }

    }
}