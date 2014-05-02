using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using NServiceKit.Common;
using NServiceKit.Redis;
using NServiceKit.Text;

namespace NServiceKit.ServiceInterface.Auth
{
    /// <summary>The redis authentication repository.</summary>
    public class RedisAuthRepository : IUserAuthRepository, IClearable
    {
        //http://stackoverflow.com/questions/3588623/c-sharp-regex-for-a-username-with-a-few-restrictions
        public Regex ValidUserNameRegEx = new Regex(@"^(?=.{3,15}$)([A-Za-z0-9][._-]?)*$", RegexOptions.Compiled);

        private readonly IRedisClientManagerFacade factory;

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.Auth.RedisAuthRepository class.</summary>
        ///
        /// <param name="factory">The factory.</param>
        public RedisAuthRepository(IRedisClientsManager factory)
            : this(new RedisClientManagerFacade(factory)) {}

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.Auth.RedisAuthRepository class.</summary>
        ///
        /// <param name="factory">The factory.</param>
        public RedisAuthRepository(IRedisClientManagerFacade factory)
        {
            this.factory = factory;
        }

        /// <summary>Gets or sets the namespace prefix.</summary>
        ///
        /// <value>The namespace prefix.</value>
        public string NamespacePrefix { get; set; }

        private string UsePrefix
        {
            get { return NamespacePrefix ?? ""; }
        }

        private string IndexUserAuthAndProviderIdsSet(long userAuthId)
        {
            return UsePrefix + "urn:UserAuth>UserOAuthProvider:" + userAuthId;
        }

        private string IndexProviderToUserIdHash(string provider)
        {
            return UsePrefix + "hash:ProviderUserId>OAuthProviderId:" + provider;
        }

        private string IndexUserNameToUserId
        {
            get
            {
                return UsePrefix + "hash:UserAuth:UserName>UserId";
            }
        }

        private string IndexEmailToUserId
        {
            get
            {
                return UsePrefix + "hash:UserAuth:Email>UserId";
            }
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

        private void AssertNoExistingUser(IRedisClientFacade redis, UserAuth newUser, UserAuth exceptForExistingUser = null)
        {
            if (newUser.UserName != null)
            {
                var existingUser = GetUserAuthByUserName(redis, newUser.UserName);
                if (existingUser != null
                    && (exceptForExistingUser == null || existingUser.Id != exceptForExistingUser.Id))
                        throw new ArgumentException("User {0} already exists".Fmt(newUser.UserName));
            }
            if (newUser.Email != null)
            {
                var existingUser = GetUserAuthByUserName(redis, newUser.Email);
                if (existingUser != null
                    && (exceptForExistingUser == null || existingUser.Id != exceptForExistingUser.Id))
                        throw new ArgumentException("Email {0} already exists".Fmt(newUser.Email));
            }
        }

        /// <summary>Creates user authentication.</summary>
        ///
        /// <param name="newUser"> The new user.</param>
        /// <param name="password">The password.</param>
        ///
        /// <returns>The new user authentication.</returns>
        public virtual UserAuth CreateUserAuth(UserAuth newUser, string password)
        {
            ValidateNewUser(newUser, password);

            using (var redis = factory.GetClient())
            {
                AssertNoExistingUser(redis, newUser);

                var saltedHash = new SaltedHash();
                string salt;
                string hash;
                saltedHash.GetHashAndSaltString(password, out hash, out salt);

                newUser.Id = redis.As<UserAuth>().GetNextSequence();
                newUser.PasswordHash = hash;
                newUser.Salt = salt;
                var digestHelper = new DigestAuthFunctions();
                newUser.DigestHa1Hash = digestHelper.CreateHa1(newUser.UserName, DigestAuthProvider.Realm, password);
                newUser.CreatedDate = DateTime.UtcNow;
                newUser.ModifiedDate = newUser.CreatedDate;

                var userId = newUser.Id.ToString(CultureInfo.InvariantCulture);
                if (!newUser.UserName.IsNullOrEmpty())
                    redis.SetEntryInHash(IndexUserNameToUserId, newUser.UserName, userId);
                if (!newUser.Email.IsNullOrEmpty())
                    redis.SetEntryInHash(IndexEmailToUserId, newUser.Email, userId);

                redis.Store(newUser);

                return newUser;
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

            using (var redis = factory.GetClient())
            {
                AssertNoExistingUser(redis, newUser, existingUser);

                if (existingUser.UserName != newUser.UserName && existingUser.UserName != null)
                {
                    redis.RemoveEntryFromHash(IndexUserNameToUserId, existingUser.UserName);
                }
                if (existingUser.Email != newUser.Email && existingUser.Email != null)
                {
                    redis.RemoveEntryFromHash(IndexEmailToUserId, existingUser.Email);
                }

                var hash = existingUser.PasswordHash;
                var salt = existingUser.Salt;
                if (password != null)
                {
                    var saltedHash = new SaltedHash();
                    saltedHash.GetHashAndSaltString(password, out hash, out salt);
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
                newUser.CreatedDate = existingUser.CreatedDate;
                newUser.ModifiedDate = DateTime.UtcNow;

                var userId = newUser.Id.ToString(CultureInfo.InvariantCulture);
                if (!newUser.UserName.IsNullOrEmpty())
                    redis.SetEntryInHash(IndexUserNameToUserId, newUser.UserName, userId);
                if (!newUser.Email.IsNullOrEmpty())
                    redis.SetEntryInHash(IndexEmailToUserId, newUser.Email, userId);

                redis.Store(newUser);

                return newUser;
            }
        }

        /// <summary>Gets user authentication by user name.</summary>
        ///
        /// <param name="userNameOrEmail">The user name or email.</param>
        ///
        /// <returns>The user authentication by user name.</returns>
        public virtual UserAuth GetUserAuthByUserName(string userNameOrEmail)
        {
            using (var redis = factory.GetClient())
            {
                return GetUserAuthByUserName(redis, userNameOrEmail);
            }
        } 

        private UserAuth GetUserAuthByUserName(IRedisClientFacade redis, string userNameOrEmail)
        {
            var isEmail = userNameOrEmail.Contains("@");
            var userId = isEmail
                ? redis.GetValueFromHash(IndexEmailToUserId, userNameOrEmail)
                : redis.GetValueFromHash(IndexUserNameToUserId, userNameOrEmail);

            return userId == null ? null : redis.As<UserAuth>().GetById(userId);
        }

        /// <summary>Attempts to authenticate from the given data.</summary>
        ///
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <param name="userAuth">The user authentication.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        public virtual bool TryAuthenticate(string userName, string password, out UserAuth userAuth)
        {
            //userId = null;
            userAuth = GetUserAuthByUserName(userName);
            if (userAuth == null) return false;

            var saltedHash = new SaltedHash();
            if (saltedHash.VerifyHashString(password, userAuth.PasswordHash, userAuth.Salt))
            {
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
        public bool TryAuthenticate (Dictionary<string, string> digestHeaders, string PrivateKey, int NonceTimeOut, string sequence, out UserAuth userAuth)
        {
            userAuth = GetUserAuthByUserName(digestHeaders["username"]);
            if (userAuth == null) return false;

            var digestHelper = new DigestAuthFunctions();
            if (digestHelper.ValidateResponse(digestHeaders, PrivateKey, NonceTimeOut, userAuth.DigestHa1Hash,sequence))
            {
                return true;
            }
            userAuth = null;
            return false;
        }

        /// <summary>Loads user authentication.</summary>
        ///
        /// <param name="session">The session.</param>
        /// <param name="tokens"> The tokens.</param>
        public virtual void LoadUserAuth(IAuthSession session, IOAuthTokens tokens)
        {
            session.ThrowIfNull("session");

            var userAuth = GetUserAuth(session, tokens);
            LoadUserAuth(session, userAuth);
        }

        private void LoadUserAuth(IAuthSession session, UserAuth userAuth)
        {
            if (userAuth == null) return;

            session.PopulateWith(userAuth);
            session.UserAuthId = userAuth.Id.ToString(CultureInfo.InvariantCulture);
            session.ProviderOAuthAccess = GetUserOAuthProviders(session.UserAuthId)
                .ConvertAll(x => (IOAuthTokens)x);
        }

        private UserAuth GetUserAuth(IRedisClientFacade redis, string userAuthId)
        {
            long longId;
            if (userAuthId == null || !long.TryParse(userAuthId, out longId)) return null;

            return redis.As<UserAuth>().GetById(longId);
        }

        /// <summary>Gets user authentication.</summary>
        ///
        /// <param name="userAuthId">Identifier for the user authentication.</param>
        ///
        /// <returns>The user authentication.</returns>
        public UserAuth GetUserAuth(string userAuthId)
        {
            using (var redis = factory.GetClient())
                return GetUserAuth(redis, userAuthId);
        }

        /// <summary>Saves a user authentication.</summary>
        ///
        /// <param name="authSession">The authentication session.</param>
        public void SaveUserAuth(IAuthSession authSession)
        {
            using (var redis = factory.GetClient())
            {
                var userAuth = !authSession.UserAuthId.IsNullOrEmpty()
                    ? GetUserAuth(redis, authSession.UserAuthId)
                    : authSession.TranslateTo<UserAuth>();

                if (userAuth.Id == default(int) && !authSession.UserAuthId.IsNullOrEmpty())
                    userAuth.Id = int.Parse(authSession.UserAuthId);

                userAuth.ModifiedDate = DateTime.UtcNow;
                if (userAuth.CreatedDate == default(DateTime))
                    userAuth.CreatedDate = userAuth.ModifiedDate;

                redis.Store(userAuth);
            }
        }

        /// <summary>Saves a user authentication.</summary>
        ///
        /// <param name="userAuth">The user authentication.</param>
        public void SaveUserAuth(UserAuth userAuth)
        {
            userAuth.ModifiedDate = DateTime.UtcNow;
            if (userAuth.CreatedDate == default(DateTime))
                userAuth.CreatedDate = userAuth.ModifiedDate;

            using (var redis = factory.GetClient())
            {
                redis.Store(userAuth);

                var userId = userAuth.Id.ToString(CultureInfo.InvariantCulture);
                if (!userAuth.UserName.IsNullOrEmpty())
                    redis.SetEntryInHash(IndexUserNameToUserId, userAuth.UserName, userId);
                if (!userAuth.Email.IsNullOrEmpty())
                    redis.SetEntryInHash(IndexEmailToUserId, userAuth.Email, userId);
            }
        }

        /// <summary>Gets user o authentication providers.</summary>
        ///
        /// <param name="userAuthId">Identifier for the user authentication.</param>
        ///
        /// <returns>The user o authentication providers.</returns>
        public List<UserOAuthProvider> GetUserOAuthProviders(string userAuthId)
        {
            userAuthId.ThrowIfNullOrEmpty("userAuthId");

            using (var redis = factory.GetClient())
            {
                var idx = IndexUserAuthAndProviderIdsSet(long.Parse(userAuthId));
                var authProiverIds = redis.GetAllItemsFromSet(idx);
                return redis.As<UserOAuthProvider>().GetByIds(authProiverIds).OrderBy(x => x.ModifiedDate).ToList();
            }
        }

        /// <summary>Gets user authentication.</summary>
        ///
        /// <param name="authSession">The authentication session.</param>
        /// <param name="tokens">     The tokens.</param>
        ///
        /// <returns>The user authentication.</returns>
        public virtual UserAuth GetUserAuth(IAuthSession authSession, IOAuthTokens tokens)
        {
            using (var redis = factory.GetClient())
                return GetUserAuth(redis, authSession, tokens);
        }

        private UserAuth GetUserAuth(IRedisClientFacade redis, IAuthSession authSession, IOAuthTokens tokens)
        {
            if (!authSession.UserAuthId.IsNullOrEmpty())
            {
                var userAuth = GetUserAuth(redis, authSession.UserAuthId);
                if (userAuth != null) return userAuth;
            }
            if (!authSession.UserAuthName.IsNullOrEmpty())
            {
                var userAuth = GetUserAuthByUserName(authSession.UserAuthName);
                if (userAuth != null) return userAuth;
            }

            if (tokens == null || tokens.Provider.IsNullOrEmpty() || tokens.UserId.IsNullOrEmpty()) return null;

            var oAuthProviderId = GetAuthProviderByUserId(redis, tokens.Provider, tokens.UserId);
            if (!oAuthProviderId.IsNullOrEmpty())
            {
                var oauthProvider = redis.As<UserOAuthProvider>().GetById(oAuthProviderId);
                if (oauthProvider != null)
                    return redis.As<UserAuth>().GetById(oauthProvider.UserAuthId);
            }
            return null;
        } 

        /// <summary>Creates or merge authentication session.</summary>
        ///
        /// <param name="authSession">The authentication session.</param>
        /// <param name="tokens">     The tokens.</param>
        ///
        /// <returns>The new or merge authentication session.</returns>
        public string CreateOrMergeAuthSession(IAuthSession authSession, IOAuthTokens tokens)
        {
            using (var redis = factory.GetClient())
            {
                UserOAuthProvider oAuthProvider = null;

                var oAuthProviderId = GetAuthProviderByUserId(redis, tokens.Provider, tokens.UserId);
                if (!oAuthProviderId.IsNullOrEmpty())
                    oAuthProvider = redis.As<UserOAuthProvider>().GetById(oAuthProviderId);

                var userAuth = GetUserAuth(redis, authSession, tokens) 
                    ?? new UserAuth { Id = redis.As<UserAuth>().GetNextSequence(), };
                 
                if (oAuthProvider == null)
                {
                    oAuthProvider = new UserOAuthProvider {
                        Id = redis.As<UserOAuthProvider>().GetNextSequence(),
                        UserAuthId = userAuth.Id,
                        Provider = tokens.Provider,
                        UserId = tokens.UserId,
                    };
                    var idx = IndexProviderToUserIdHash(tokens.Provider);
                    redis.SetEntryInHash(idx, tokens.UserId, oAuthProvider.Id.ToString(CultureInfo.InvariantCulture));
                }

                oAuthProvider.PopulateMissing(tokens);
                userAuth.PopulateMissing(oAuthProvider);

                userAuth.ModifiedDate = DateTime.UtcNow;
                if (oAuthProvider.CreatedDate == default(DateTime))
                    oAuthProvider.CreatedDate = userAuth.ModifiedDate;
                oAuthProvider.ModifiedDate = userAuth.ModifiedDate;

                redis.Store(userAuth);
                redis.Store(oAuthProvider);
                redis.AddItemToSet(IndexUserAuthAndProviderIdsSet(userAuth.Id), oAuthProvider.Id.ToString(CultureInfo.InvariantCulture));

                return userAuth.Id.ToString(CultureInfo.InvariantCulture);
            }
        }

        private string GetAuthProviderByUserId(IRedisClientFacade redis, string provider, string userId)
        {
            var idx = IndexProviderToUserIdHash(provider);
            var oAuthProviderId = redis.GetValueFromHash(idx, userId);
            return oAuthProviderId;
        }

        /// <summary>Clears this object to its blank/initial state.</summary>
        public void Clear()
        {
            this.factory.Clear();
        }


    }

}
