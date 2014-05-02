using System.Collections.Generic;

namespace NServiceKit.ServiceInterface.Auth
{
    /// <summary>Interface for user authentication repository.</summary>
    public interface IUserAuthRepository
    {
        /// <summary>Creates user authentication.</summary>
        ///
        /// <param name="newUser"> The new user.</param>
        /// <param name="password">The password.</param>
        ///
        /// <returns>The new user authentication.</returns>
        UserAuth CreateUserAuth(UserAuth newUser, string password);

        /// <summary>Updates the user authentication.</summary>
        ///
        /// <param name="existingUser">The existing user.</param>
        /// <param name="newUser">     The new user.</param>
        /// <param name="password">    The password.</param>
        ///
        /// <returns>An UserAuth.</returns>
        UserAuth UpdateUserAuth(UserAuth existingUser, UserAuth newUser, string password);

        /// <summary>Gets user authentication by user name.</summary>
        ///
        /// <param name="userNameOrEmail">The user name or email.</param>
        ///
        /// <returns>The user authentication by user name.</returns>
        UserAuth GetUserAuthByUserName(string userNameOrEmail);

        /// <summary>Attempts to authenticate from the given data.</summary>
        ///
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <param name="userAuth">The user authentication.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        bool TryAuthenticate(string userName, string password, out UserAuth userAuth);

        /// <summary>Attempts to authenticate from the given data.</summary>
        ///
        /// <param name="digestHeaders">The digest headers.</param>
        /// <param name="PrivateKey">   The private key.</param>
        /// <param name="NonceTimeOut"> The nonce time out.</param>
        /// <param name="sequence">     The sequence.</param>
        /// <param name="userAuth">     The user authentication.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        bool TryAuthenticate(Dictionary<string, string> digestHeaders, string PrivateKey, int NonceTimeOut, string sequence, out UserAuth userAuth);

        /// <summary>Loads user authentication.</summary>
        ///
        /// <param name="session">The session.</param>
        /// <param name="tokens"> The tokens.</param>
        void LoadUserAuth(IAuthSession session, IOAuthTokens tokens);

        /// <summary>Gets user authentication.</summary>
        ///
        /// <param name="userAuthId">Identifier for the user authentication.</param>
        ///
        /// <returns>The user authentication.</returns>
        UserAuth GetUserAuth(string userAuthId);

        /// <summary>Saves a user authentication.</summary>
        ///
        /// <param name="authSession">The authentication session.</param>
        void SaveUserAuth(IAuthSession authSession);

        /// <summary>Saves a user authentication.</summary>
        ///
        /// <param name="userAuth">The user authentication.</param>
        void SaveUserAuth(UserAuth userAuth);

        /// <summary>Gets user o authentication providers.</summary>
        ///
        /// <param name="userAuthId">Identifier for the user authentication.</param>
        ///
        /// <returns>The user o authentication providers.</returns>
        List<UserOAuthProvider> GetUserOAuthProviders(string userAuthId);

        /// <summary>Gets user authentication.</summary>
        ///
        /// <param name="authSession">The authentication session.</param>
        /// <param name="tokens">     The tokens.</param>
        ///
        /// <returns>The user authentication.</returns>
        UserAuth GetUserAuth(IAuthSession authSession, IOAuthTokens tokens);

        /// <summary>Creates or merge authentication session.</summary>
        ///
        /// <param name="authSession">The authentication session.</param>
        /// <param name="tokens">     The tokens.</param>
        ///
        /// <returns>The new or merge authentication session.</returns>
        string CreateOrMergeAuthSession(IAuthSession authSession, IOAuthTokens tokens);
    }
}
