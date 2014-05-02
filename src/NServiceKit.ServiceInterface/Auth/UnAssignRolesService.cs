using System.Collections.Generic;
using NServiceKit.Common;
using NServiceKit.Common.Web;
using NServiceKit.OrmLite;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface.ServiceModel;

namespace NServiceKit.ServiceInterface.Auth
{
    /// <summary>An un assign roles.</summary>
    public class UnAssignRoles : IReturn<UnAssignRolesResponse>
    {
        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.Auth.UnAssignRoles class.</summary>
        public UnAssignRoles()
        {
            this.Roles = new List<string>();
            this.Permissions = new List<string>();
        }

        /// <summary>Gets or sets the name of the user.</summary>
        ///
        /// <value>The name of the user.</value>
        public string UserName { get; set; }

        /// <summary>Gets or sets the permissions.</summary>
        ///
        /// <value>The permissions.</value>
        public List<string> Permissions { get; set; }

        /// <summary>Gets or sets the roles.</summary>
        ///
        /// <value>The roles.</value>
        public List<string> Roles { get; set; }
    }

    /// <summary>An un assign roles response.</summary>
    public class UnAssignRolesResponse : IHasResponseStatus
    {
        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.Auth.UnAssignRolesResponse class.</summary>
        public UnAssignRolesResponse()
        {
            this.AllRoles = new List<string>();
        }

        /// <summary>Gets or sets all roles.</summary>
        ///
        /// <value>all roles.</value>
        public List<string> AllRoles { get; set; }

        /// <summary>Gets or sets all permissions.</summary>
        ///
        /// <value>all permissions.</value>
        public List<string> AllPermissions { get; set; }

        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
        public ResponseStatus ResponseStatus { get; set; }
    }

    /// <summary>An un assign roles service.</summary>
    [RequiredRole(RoleNames.Admin)]
    [DefaultRequest(typeof(UnAssignRoles))]
    public class UnAssignRolesService : Service
    {
        /// <summary>Gets or sets the user authentication repo.</summary>
        ///
        /// <value>The user authentication repo.</value>
        public IUserAuthRepository UserAuthRepo { get; set; }

        /// <summary>Post this message.</summary>
        ///
        /// <exception cref="NotFound">Thrown when a not found error condition occurs.</exception>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Post(UnAssignRoles request)
        {
            request.UserName.ThrowIfNullOrEmpty();

            var userAuth = UserAuthRepo.GetUserAuthByUserName(request.UserName);
            if (userAuth == null)
                throw HttpError.NotFound(request.UserName);

            if (!request.Roles.IsEmpty())
            {
                request.Roles.ForEach(x => userAuth.Roles.Remove(x));
            }
            if (!request.Permissions.IsEmpty())
            {
                request.Permissions.ForEach(x => userAuth.Permissions.Remove(x));
            }

            UserAuthRepo.SaveUserAuth(userAuth);

            return new UnAssignRolesResponse {
                AllRoles = userAuth.Roles,
                AllPermissions = userAuth.Permissions,
            };
        }
    }
}