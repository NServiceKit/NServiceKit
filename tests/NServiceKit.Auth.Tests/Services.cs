#define HTTP_LISTENER
using System.Collections.Generic;
using System.Runtime.Serialization;
using NServiceKit.Common;
using NServiceKit.Common.Web;
using NServiceKit.DataAnnotations;
using NServiceKit.OrmLite;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface;
using NServiceKit.ServiceInterface.Auth;
using NServiceKit.ServiceInterface.ServiceModel;

#if HTTP_LISTENER
namespace NServiceKit.Auth.Tests
#else
namespace NServiceKit.AuthWeb.Tests
#endif
{
    /// <summary>A get user profile.</summary>
    [Route("/profile")]
    public class GetUserProfile { }

    /// <summary>A user profile.</summary>
    public class UserProfile
    {
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
        public int Id { get; set; }

        /// <summary>Gets or sets the user authentication.</summary>
        ///
        /// <value>The user authentication.</value>
        public UserAuth UserAuth { get; set; }

        /// <summary>Gets or sets the session.</summary>
        ///
        /// <value>The session.</value>
        public AuthUserSession Session { get; set; }

        /// <summary>Gets or sets the user authentication providers.</summary>
        ///
        /// <value>The user authentication providers.</value>
        public List<UserOAuthProvider> UserAuthProviders { get; set; }
    }

    /// <summary>A user profile response.</summary>
    public class UserProfileResponse
    {
        /// <summary>Gets or sets the result.</summary>
        ///
        /// <value>The result.</value>
        public UserProfile Result { get; set; }

        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
        public ResponseStatus ResponseStatus { get; set; }
    }

    /// <summary>A user profile service.</summary>
    [Authenticate]
    public class UserProfileService : ServiceInterface.Service
    {
        /// <summary>Gets the given request.</summary>
        ///
        /// <param name="request">The request to get.</param>
        ///
        /// <returns>An UserProfile.</returns>
        public UserProfile Get(GetUserProfile request)
        {
            var session = base.SessionAs<CustomUserSession>();

            var userAuthId = session.UserAuthId.ToInt();
            var userProfile = new UserProfile
            {
                Id = userAuthId,
                Session = session,
                UserAuth = Db.QueryById<UserAuth>(userAuthId),
                UserAuthProviders = Db.Select<UserOAuthProvider>(x => x.UserAuthId == userAuthId),
            };

            return userProfile;
        }
    }

    /// <summary>A reset user authentication.</summary>
    [Route("/reset-userauth")]
    public class ResetUserAuth { }
    /// <summary>A reset user authentication service.</summary>
    public class ResetUserAuthService : ServiceInterface.Service
    {
        /// <summary>Gets the given request.</summary>
        ///
        /// <param name="request">The request to get.</param>
        ///
        /// <returns>An object.</returns>
        public object Get(ResetUserAuth request)
        {
            this.Cache.Remove(SessionFeature.GetSessionKey(Request));

            Db.DeleteAll<UserAuth>();
            Db.DeleteAll<UserOAuthProvider>();

            return HttpResult.Redirect(Request.UrlReferrer.AbsoluteUri);
        }
    }


    /// <summary>A rockstars.</summary>
    [Route("/rockstars")]
    [Route("/rockstars/aged/{Age}")]
    [Route("/rockstars/delete/{Delete}")]
    [Route("/rockstars/{Id}")]
    public class Rockstars
    {
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
        public int Id { get; set; }

        /// <summary>Gets or sets the person's first name.</summary>
        ///
        /// <value>The name of the first.</value>
        public string FirstName { get; set; }

        /// <summary>Gets or sets the person's last name.</summary>
        ///
        /// <value>The name of the last.</value>
        public string LastName { get; set; }

        /// <summary>Gets or sets the age.</summary>
        ///
        /// <value>The age.</value>
        public int? Age { get; set; }

        /// <summary>Gets or sets the delete.</summary>
        ///
        /// <value>The delete.</value>
        public string Delete { get; set; }
    }

    /// <summary>The rockstars response.</summary>
    [DataContract]
    public class RockstarsResponse
    {
        /// <summary>Gets or sets the number of. </summary>
        ///
        /// <value>The total.</value>
        [DataMember]
        public int Total { get; set; }

        /// <summary>Gets or sets the aged.</summary>
        ///
        /// <value>The aged.</value>
        [DataMember]
        public int? Aged { get; set; }

        /// <summary>Gets or sets the results.</summary>
        ///
        /// <value>The results.</value>
        [DataMember]
        public List<Rockstar> Results { get; set; }
    }

    /// <summary>A rockstar.</summary>
    public class Rockstar
    {
        /// <summary>Information describing the seed.</summary>
        public static Rockstar[] SeedData = new[] {
            new Rockstar(1, "Jimi", "Hendrix", 27), 
            new Rockstar(2, "Janis", "Joplin", 27), 
            new Rockstar(3, "Jim", "Morrisson", 27), 
            new Rockstar(4, "Kurt", "Cobain", 27),              
            new Rockstar(5, "Elvis", "Presley", 42), 
            new Rockstar(6, "Michael", "Jackson", 50), 
        };

        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
        [AutoIncrement]
        public int Id { get; set; }

        /// <summary>Gets or sets the person's first name.</summary>
        ///
        /// <value>The name of the first.</value>
        public string FirstName { get; set; }

        /// <summary>Gets or sets the person's last name.</summary>
        ///
        /// <value>The name of the last.</value>
        public string LastName { get; set; }

        /// <summary>Gets or sets the age.</summary>
        ///
        /// <value>The age.</value>
        public int? Age { get; set; }

        /// <summary>Gets or sets a value indicating whether the alive.</summary>
        ///
        /// <value>true if alive, false if not.</value>
        public bool Alive { get; set; }

        /// <summary>Initializes a new instance of the NServiceKit.Auth.Tests.Rockstar class.</summary>
        public Rockstar() { }

        /// <summary>Initializes a new instance of the NServiceKit.Auth.Tests.Rockstar class.</summary>
        ///
        /// <param name="id">       The identifier.</param>
        /// <param name="firstName">The person's first name.</param>
        /// <param name="lastName"> The person's last name.</param>
        /// <param name="age">      The age.</param>
        public Rockstar(int id, string firstName, string lastName, int age)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Age = age;
        }
    }

    /// <summary>The rockstars service.</summary>
    [DefaultRequest(typeof(Rockstars))]
    public class RockstarsService : ServiceInterface.Service
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
        public object Get(Rockstars request)
        {
            if (request.Delete == "reset")
            {
                Db.DeleteAll<Rockstar>();
                Db.Insert(Rockstar.SeedData);
            }
            else if (request.Delete.IsInt())
            {
                Db.DeleteById<Rockstar>(request.Delete.ToInt());
            }

            return new RockstarsResponse
            {
                Aged = request.Age,
                Total = Db.GetScalar<int>("select count(*) from Rockstar"),
                Results = request.Id != default(int) ?
                    Db.Select<Rockstar>(q => q.Id == request.Id)
                      : request.Age.HasValue ?
                    Db.Select<Rockstar>(q => q.Age == request.Age.Value)
                      : Db.Select<Rockstar>()
            };
        }

        /// <summary>Post this message.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Post(Rockstars request)
        {
            Db.Insert(request.TranslateTo<Rockstar>());
            return Get(new Rockstars());
        }
    }

    /// <summary>A data Model for the view that uses layout and.</summary>
    [Route("/viewmodel/{Id}")]
    public class ViewThatUsesLayoutAndModel
    {
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
        public string Id { get; set; }
    }

    /// <summary>A view that uses layout and model response.</summary>
    public class ViewThatUsesLayoutAndModelResponse
    {
        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>Gets or sets the results.</summary>
        ///
        /// <value>The results.</value>
        public List<string> Results { get; set; }
    }

    /// <summary>A view service.</summary>
    public class ViewService : ServiceInterface.Service
    {
        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Any(ViewThatUsesLayoutAndModel request)
        {
            return new ViewThatUsesLayoutAndModelResponse
            {
                Name = request.Id ?? "Foo",
                Results = new List<string> { "Tom", "Dick", "Harry" }
            };
        }
    }
}
