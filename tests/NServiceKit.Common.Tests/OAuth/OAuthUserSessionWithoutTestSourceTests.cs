using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using NServiceKit.Common.Utils;
using NServiceKit.Configuration;
using NServiceKit.OrmLite;
using NServiceKit.OrmLite.SqlServer;
using NServiceKit.OrmLite.Sqlite;
using NServiceKit.Redis;
using NServiceKit.ServiceInterface.Auth;
using NServiceKit.Text;

namespace NServiceKit.Common.Tests.OAuth
{
    /// <summary>An authentication user session without test source tests.</summary>
    [TestFixture, Explicit("Manual OAuth Test with iteration over data stores")]
    [Ignore]
    public class OAuthUserSessionWithoutTestSourceTests
    {
        private OAuthUserSessionTests tests;
        private readonly List<IUserAuthRepository> userAuthRepositorys = new List<IUserAuthRepository>();

        OrmLiteConnectionFactory dbFactory = new OrmLiteConnectionFactory(
            ":memory:", false, SqliteOrmLiteDialectProvider.Instance);

        /// <summary>Sets the up.</summary>
        ///
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        [SetUp]
        public void SetUp()
        {
			try
			{
                tests = new OAuthUserSessionTests();
				var inMemoryRepo = new InMemoryAuthRepository();
				inMemoryRepo.Clear();
				userAuthRepositorys.Add(inMemoryRepo);

                var appSettings = new AppSettings();
				var redisRepo = new RedisAuthRepository(new BasicRedisClientManager(new string[] { appSettings.GetString("Redis.Host") ?? "localhost" }));
				redisRepo.Clear();
				userAuthRepositorys.Add(redisRepo);

				if (OAuthUserSessionTestsBase.UseSqlServer)
				{
					var connStr = @"Data Source=.\SQLEXPRESS;AttachDbFilename=|DataDirectory|\App_Data\auth.mdf;Integrated Security=True;Connect Timeout=30;User Instance=True";
					var sqlServerFactory = new OrmLiteConnectionFactory(connStr, SqlServerOrmLiteDialectProvider.Instance);
					var sqlServerRepo = new OrmLiteAuthRepository(sqlServerFactory);
					sqlServerRepo.DropAndReCreateTables();
				}
				else
				{
					var sqliteInMemoryRepo = new OrmLiteAuthRepository(dbFactory);
					dbFactory.Run(db => {
						db.CreateTable<UserAuth>(true);
						db.CreateTable<UserOAuthProvider>(true);
					});
					sqliteInMemoryRepo.Clear();
					userAuthRepositorys.Add(sqliteInMemoryRepo);

					var sqliteDbFactory = new OrmLiteConnectionFactory(
						"~/App_Data/auth.sqlite".MapProjectPath());
					var sqliteDbRepo = new OrmLiteAuthRepository(sqliteDbFactory);
					sqliteDbRepo.CreateMissingTables();
					userAuthRepositorys.Add(sqliteDbRepo);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				throw;
			}
		}
		
        /// <summary>Does persist twitter o authentication.</summary>
        [Test]
        public void Does_persist_TwitterOAuth()
        {
            userAuthRepositorys.ForEach(x => tests.Does_persist_TwitterOAuth(x));
        }

        /// <summary>Does persist facebook o authentication.</summary>
        [Test]
        public void Does_persist_FacebookOAuth()
        {
            userAuthRepositorys.ForEach(x => tests.Does_persist_FacebookOAuth(x));
        }

        /// <summary>Does merge facebook o authentication twitter o authentication.</summary>
        [Test]
        public void Does_merge_FacebookOAuth_TwitterOAuth()
        {
            userAuthRepositorys.ForEach(x => tests.Does_merge_FacebookOAuth_TwitterOAuth(x));
        }

        /// <summary>Can login with user created create user authentication.</summary>
        [Test]
        public void Can_login_with_user_created_CreateUserAuth()
        {
            userAuthRepositorys.ForEach(x => tests.Can_login_with_user_created_CreateUserAuth(x));
        }


    }
}