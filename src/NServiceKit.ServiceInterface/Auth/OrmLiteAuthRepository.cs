using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using NServiceKit.Common;
using NServiceKit.OrmLite;
using NServiceKit.Text;

namespace NServiceKit.ServiceInterface.Auth
{
    /// <summary>An ORM lite authentication repository.</summary>
    public class OrmLiteAuthRepository : IUserAuthRepository, IClearable
    {
        //http://stackoverflow.com/questions/3588623/c-sharp-regex-for-a-username-with-a-few-restrictions
        public Regex ValidUserNameRegEx = new Regex(@"^(?=.{3,15}$)([A-Za-z0-9][._-]?)*$", RegexOptions.Compiled);

        private readonly IDbConnectionFactory dbFactory;
        private readonly IHashProvider passwordHasher;

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.Auth.OrmLiteAuthRepository class.</summary>
        ///
        /// <param name="dbFactory">The database factory.</param>
        public OrmLiteAuthRepository(IDbConnectionFactory dbFactory)
            : this(dbFactory, new SaltedHash())
        { }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.Auth.OrmLiteAuthRepository class.</summary>
        ///
        /// <param name="dbFactory">     The database factory.</param>
        /// <param name="passwordHasher">The password hasher.</param>
        public OrmLiteAuthRepository(IDbConnectionFactory dbFactory, IHashProvider passwordHasher)
        {
            this.dbFactory = dbFactory;
            this.passwordHasher = passwordHasher;
        }

        /// <summary>Creates missing tables.</summary>
        public void CreateMissingTables()
        {
            dbFactory.Run(db => {
                db.CreateTable<UserAuth>(false);
                db.CreateTable<UserOAuthProvider>(false);
            });
        }

        /// <summary>Drop and re create tables.</summary>
        public void DropAndReCreateTables()
        {
            dbFactory.Run(db =>
            {
                db.CreateTable<UserAuth>(true);
                db.CreateTable<UserOAuthProvider>(true);
            });
        }

        private void ValidateNewUser(UserAuth newUser, string password)
        {
            newUser.ThrowIfNull("newUser");
            password.ThrowIfNullOrEmpty("password");

            if (newUser.UserName.IsNullOrEmpty() && newUser.Email.IsNullOrEmpty())
                throw new ArgumentNullException("UserName or Email is required");

            if (!newUser.UserName.IsNullOrEmpty())
            {
                if (!ValidUserNameRegEx.IsMatch(newUser.UserName))
                    throw new ArgumentException("UserName contains invalid characters", "UserName");
            }
        }

        /// <summary>Creates user authentication.</summary>
        ///
        /// <param name="newUser"> The new user.</param>
        /// <param name="password">The password.</param>
        ///
        /// <returns>The new user authentication.</returns>
        public UserAuth CreateUserAuth(UserAuth newUser, string password)
        {
            ValidateNewUser(newUser, password);

            return dbFactory.Run(db => {
                AssertNoExistingUser(db, newUser);

                string salt;
                string hash;
                passwordHasher.GetHashAndSaltString(password, out hash, out salt);
                var digestHelper = new DigestAuthFunctions();
                newUser.DigestHa1Hash = digestHelper.CreateHa1(newUser.UserName, DigestAuthProvider.Realm, password);
                newUser.PasswordHash = hash;
                newUser.Salt = salt;
                newUser.CreatedDate = DateTime.UtcNow;
                newUser.ModifiedDate = newUser.CreatedDate;

                db.Insert(newUser);

                newUser = db.GetById<UserAuth>(db.GetLastInsertId());
                return newUser;
            });
        }

        private static void AssertNoExistingUser(IDbConnection db, UserAuth newUser, UserAuth exceptForExistingUser = null)
        {
            if (newUser.UserName != null)
            {
                var existingUser = GetUserAuthByUserName(db, newUser.UserName);
                if (existingUser != null
                    && (exceptForExistingUser == null || existingUser.Id != exceptForExistingUser.Id))
                    throw new ArgumentException("User {0} already exists".Fmt(newUser.UserName));
            }
            if (newUser.Email != null)
            {
                var existingUser = GetUserAuthByUserName(db, newUser.Email);
                if (existingUser != null
                    && (exceptForExistingUser == null || existingUser.Id != exceptForExistingUser.Id))
                    throw new ArgumentException("Email {0} already exists".Fmt(newUser.Email));
            }
        }

        /// <summary>Updates the user authentication.</summary>
        ///
        /// <param name="existingUser">The existing user.</param>
        /// <param name="newUser">     The new user.</param>
        /// <param name="password">    The password.</param>
        ///
        /// <returns>An UserAuth.</returns>
        public UserAuth UpdateUserAuth(UserAuth existingUser, UserAuth newUser, string password)
        {
            ValidateNewUser(newUser, password);

            return dbFactory.Run(db => {
                AssertNoExistingUser(db, newUser, existingUser);

                var hash = existingUser.PasswordHash;
                var salt = existingUser.Salt;
                if (password != null)
                {
                    passwordHasher.GetHashAndSaltString(password, out hash, out salt);
                }
                // If either one changes the digest hash has to be recalculated
                var digestHash = existingUser.DigestHa1Hash;
                if (password != null || existingUser.UserName != newUser.UserName)
                {
                    var digestHelper = new DigestAuthFunctions();
                    digestHash = digestHelper.CreateHa1(newUser.UserName, DigestAuthProvider.Realm, password);
                }
                newUser.Id = existingUser.Id;
                newUser.PasswordHash = hash;
                newUser.Salt = salt;
                newUser.DigestHa1Hash = digestHash;
                newUser.CreatedDate = existingUser.CreatedDate;
                newUser.ModifiedDate = DateTime.UtcNow;

                db.Save(newUser);

                return newUser;
            });
        }

        /// <summary>Gets user authentication by user name.</summary>
        ///
        /// <param name="userNameOrEmail">The user name or email.</param>
        ///
        /// <returns>The user authentication by user name.</returns>
        public UserAuth GetUserAuthByUserName(string userNameOrEmail)
        {
            return dbFactory.Run(db => GetUserAuthByUserName(db, userNameOrEmail));
        }

        private static UserAuth GetUserAuthByUserName(IDbConnection db, string userNameOrEmail)
        {
            var isEmail = userNameOrEmail.Contains("@");
            var userAuth = isEmail
                ? db.Select<UserAuth>(q => q.Email == userNameOrEmail).FirstOrDefault()
                : db.Select<UserAuth>(q => q.UserName == userNameOrEmail).FirstOrDefault();

            return userAuth;
        }

        /// <summary>Attempts to authenticate from the given data.</summary>
        ///
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <param name="userAuth">The user authentication.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        public bool TryAuthenticate(string userName, string password, out UserAuth userAuth)
        {
            //userId = null;
            userAuth = GetUserAuthByUserName(userName);
            if (userAuth == null) return false;

            if (passwordHasher.VerifyHashString(password, userAuth.PasswordHash, userAuth.Salt))
            {
                //userId = userAuth.Id.ToString(CultureInfo.InvariantCulture);
                return true;
            }

            userAuth = null;
            return false;
        }

        /// <summary>Attempts to authenticate from the given data.</summary>
        ///
        /// <param name="digestHeaders">The digest headers.</param>
        /// <param name="PrivateKey">   The private key.</param>
        /// <param name="NonceTimeOut"> The nonce time out.</param>
        /// <param name="sequence">     The sequence.</param>
        /// <param name="userAuth">     The user authentication.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        public bool TryAuthenticate(Dictionary<string,string> digestHeaders, string PrivateKey, int NonceTimeOut, string sequence, out UserAuth userAuth)
        {
            //userId = null;
            userAuth = GetUserAuthByUserName(digestHeaders["username"]);
            if (userAuth == null) return false;

            var digestHelper = new DigestAuthFunctions();
            if (digestHelper.ValidateResponse(digestHeaders,PrivateKey,NonceTimeOut,userAuth.DigestHa1Hash,sequence))
            {
                //userId = userAuth.Id.ToString(CultureInfo.InvariantCulture);
                return true;
            }
            userAuth = null;
            return false;
        }

        /// <summary>Loads user authentication.</summary>
        ///
        /// <param name="session">The session.</param>
        /// <param name="tokens"> The tokens.</param>
        public void LoadUserAuth(IAuthSession session, IOAuthTokens tokens)
        {
            session.ThrowIfNull("session");

            var userAuth = GetUserAuth(session, tokens);
            LoadUserAuth(session, userAuth);
        }

        private void LoadUserAuth(IAuthSession session, UserAuth userAuth)
        {
            if (userAuth == null) return;

            var idSesije = session.Id;  //first record session Id (original session Id)
            session.PopulateWith(userAuth); //here, original sessionId is overwritten with facebook user Id
            session.Id = idSesije;  //we return Id of original session here

            session.UserAuthId = userAuth.Id.ToString(CultureInfo.InvariantCulture);
            session.ProviderOAuthAccess = GetUserOAuthProviders(session.UserAuthId)
                .ConvertAll(x => (IOAuthTokens)x);
            
        }

        /// <summary>Gets user authentication.</summary>
        ///
        /// <param name="userAuthId">Identifier for the user authentication.</param>
        ///
        /// <returns>The user authentication.</returns>
        public UserAuth GetUserAuth(string userAuthId)
        {
            return dbFactory.Run(db => db.GetByIdOrDefault<UserAuth>(userAuthId));
        }

        /// <summary>Saves a user authentication.</summary>
        ///
        /// <param name="authSession">The authentication session.</param>
        public void SaveUserAuth(IAuthSession authSession)
        {
            dbFactory.Run(db => {

                var userAuth = !authSession.UserAuthId.IsNullOrEmpty()
                    ? db.GetByIdOrDefault<UserAuth>(authSession.UserAuthId)
                    : authSession.TranslateTo<UserAuth>();

                if (userAuth.Id == default(int) && !authSession.UserAuthId.IsNullOrEmpty())
                    userAuth.Id = int.Parse(authSession.UserAuthId);

                userAuth.ModifiedDate = DateTime.UtcNow;
                if (userAuth.CreatedDate == default(DateTime))
                    userAuth.CreatedDate = userAuth.ModifiedDate;

                db.Save(userAuth);
            });
        }

        /// <summary>Saves a user authentication.</summary>
        ///
        /// <param name="userAuth">The user authentication.</param>
        public void SaveUserAuth(UserAuth userAuth)
        {
            userAuth.ModifiedDate = DateTime.UtcNow;
            if (userAuth.CreatedDate == default(DateTime))
                userAuth.CreatedDate = userAuth.ModifiedDate;

            dbFactory.Run(db => db.Save(userAuth));
        }

        /// <summary>Gets user o authentication providers.</summary>
        ///
        /// <param name="userAuthId">Identifier for the user authentication.</param>
        ///
        /// <returns>The user o authentication providers.</returns>
        public List<UserOAuthProvider> GetUserOAuthProviders(string userAuthId)
        {
            var id = int.Parse(userAuthId);
            return dbFactory.Run(db =>
                db.Select<UserOAuthProvider>(q => q.UserAuthId == id)).OrderBy(x => x.ModifiedDate).ToList();
        }

        /// <summary>Gets user authentication.</summary>
        ///
        /// <param name="authSession">The authentication session.</param>
        /// <param name="tokens">     The tokens.</param>
        ///
        /// <returns>The user authentication.</returns>
        public UserAuth GetUserAuth(IAuthSession authSession, IOAuthTokens tokens)
        {
            if (!authSession.UserAuthId.IsNullOrEmpty())
            {
                var userAuth = GetUserAuth(authSession.UserAuthId);
                if (userAuth != null) return userAuth;
            }
            if (!authSession.UserAuthName.IsNullOrEmpty())
            {
                var userAuth = GetUserAuthByUserName(authSession.UserAuthName);
                if (userAuth != null) return userAuth;
            }

            if (tokens == null || tokens.Provider.IsNullOrEmpty() || tokens.UserId.IsNullOrEmpty())
                return null;

            return dbFactory.Run(db => {
                var oAuthProvider = db.Select<UserOAuthProvider>(q => 
                    q.Provider == tokens.Provider && q.UserId == tokens.UserId).FirstOrDefault();

                if (oAuthProvider != null)
                {
                    var userAuth = db.GetByIdOrDefault<UserAuth>(oAuthProvider.UserAuthId);
                    return userAuth;
                }
                return null;
            });
        }

        /// <summary>Creates or merge authentication session.</summary>
        ///
        /// <param name="authSession">The authentication session.</param>
        /// <param name="tokens">     The tokens.</param>
        ///
        /// <returns>The new or merge authentication session.</returns>
        public string CreateOrMergeAuthSession(IAuthSession authSession, IOAuthTokens tokens)
        {
            var userAuth = GetUserAuth(authSession, tokens) ?? new UserAuth();

            return dbFactory.Run(db => {

                var oAuthProvider = db.Select<UserOAuthProvider>(q =>
                    q.Provider == tokens.Provider && q.UserId == tokens.UserId).FirstOrDefault();

                if (oAuthProvider == null)
                {
                    oAuthProvider = new UserOAuthProvider {
                        Provider = tokens.Provider,
                        UserId = tokens.UserId,
                    };
                }

                oAuthProvider.PopulateMissing(tokens);
                userAuth.PopulateMissing(oAuthProvider);

                userAuth.ModifiedDate = DateTime.UtcNow;
                if (userAuth.CreatedDate == default(DateTime))
                    userAuth.CreatedDate = userAuth.ModifiedDate;

                db.Save(userAuth);

                oAuthProvider.UserAuthId = userAuth.Id != default(int)
                    ? userAuth.Id
                    : (int)db.GetLastInsertId();

                if (oAuthProvider.CreatedDate == default(DateTime))
                    oAuthProvider.CreatedDate = userAuth.ModifiedDate;
                oAuthProvider.ModifiedDate = userAuth.ModifiedDate;

                db.Save(oAuthProvider);

                return oAuthProvider.UserAuthId.ToString(CultureInfo.InvariantCulture);
            });
        }

        /// <summary>Clears this object to its blank/initial state.</summary>
        public void Clear()
        {
            dbFactory.Run(db => {
                db.DeleteAll<UserAuth>();
                db.DeleteAll<UserOAuthProvider>();
            });
        }
    }
}
