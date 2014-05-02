using NServiceKit.Common;
using NServiceKit.ServiceClient.Web;
using NServiceKit.Text;

namespace NServiceKit.ServiceInterface.Auth
{
    /// <summary>Interface for authentication HTTP gateway.</summary>
    public interface IAuthHttpGateway
    {
        /// <summary>Downloads the twitter user information described by twitterUserId.</summary>
        ///
        /// <param name="twitterUserId">Identifier for the twitter user.</param>
        ///
        /// <returns>A string.</returns>
        string DownloadTwitterUserInfo(string twitterUserId);

        /// <summary>Downloads the facebook user information described by facebookCode.</summary>
        ///
        /// <param name="facebookCode">The facebook code.</param>
        ///
        /// <returns>A string.</returns>
        string DownloadFacebookUserInfo(string facebookCode);

        /// <summary>Downloads the yammer user information described by yammerUserId.</summary>
        ///
        /// <param name="yammerUserId">Identifier for the yammer user.</param>
        ///
        /// <returns>A string.</returns>
        string DownloadYammerUserInfo(string yammerUserId);
    }

    /// <summary>An authentication HTTP gateway.</summary>
    public class AuthHttpGateway : IAuthHttpGateway
    {
        /// <summary>URL of the twitter user.</summary>
        public const string TwitterUserUrl = "http://api.twitter.com/1/users/lookup.json?user_id={0}";

        /// <summary>URL of the facebook user.</summary>
        public const string FacebookUserUrl = "https://graph.facebook.com/me?access_token={0}";

        /// <summary>URL of the yammer user.</summary>
        public const string YammerUserUrl = "https://www.yammer.com/api/v1/users/{0}.json";

        /// <summary>Downloads the twitter user information described by twitterUserId.</summary>
        ///
        /// <param name="twitterUserId">Identifier for the twitter user.</param>
        ///
        /// <returns>A string.</returns>
        public string DownloadTwitterUserInfo(string twitterUserId)
        {
            twitterUserId.ThrowIfNullOrEmpty("twitterUserId");

            var url = TwitterUserUrl.Fmt(twitterUserId);
            var json = url.GetStringFromUrl();
            return json;
        }

        /// <summary>Downloads the facebook user information described by facebookCode.</summary>
        ///
        /// <param name="facebookCode">The facebook code.</param>
        ///
        /// <returns>A string.</returns>
        public string DownloadFacebookUserInfo(string facebookCode)
        {
            facebookCode.ThrowIfNullOrEmpty("facebookCode");

            var url = FacebookUserUrl.Fmt(facebookCode);
            var json = url.GetStringFromUrl();
            return json;
        }

        /// <summary>
        /// Download Yammer User Info given its ID.
        /// </summary>
        /// <param name="yammerUserId">
        /// The Yammer User ID.
        /// </param>
        /// <returns>
        /// The User info in JSON format.
        /// </returns>
        /// <remarks>
        /// <para>
        /// Yammer provides a method to retrieve current user information via
        /// "https://www.yammer.com/api/v1/users/current.json".
        /// </para>
        /// <para>
        /// However, to ensure consistency with the rest of the Auth codebase,
        /// the explicit URL will be used, where [:id] denotes the User ID: 
        /// "https://www.yammer.com/api/v1/users/[:id].json"
        /// </para>
        /// <para>
        /// Refer to: https://developer.yammer.com/restapi/ for full documentation.
        /// </para>
        /// </remarks>
        public string DownloadYammerUserInfo(string yammerUserId)
        {
            yammerUserId.ThrowIfNullOrEmpty("yammerUserId");

            var url = YammerUserUrl.Fmt(yammerUserId);
            var json = url.GetStringFromUrl();
            return json;
        }
    }
}