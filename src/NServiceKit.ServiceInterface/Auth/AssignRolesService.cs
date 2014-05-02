using System.Collections.Generic;
using System.Linq;
using NServiceKit.Common;
using NServiceKit.Common.Web;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface.ServiceModel;

namespace NServiceKit.ServiceInterface.Auth
{
    /// <summary>An assign roles.</summary>
    public class AssignRoles : IReturn<AssignRolesResponse>
    {
        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.Auth.AssignRoles class.</summary>
        public AssignRoles()
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

    /// <summary>An assign roles response.</summary>
    public class AssignRolesResponse : IHasResponseStatus
    {
        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.Auth.AssignRolesResponse class.</summary>
        public AssignRolesResponse()
        {
            this.AllRoles = new List<string>();
            this.AllPermissions = new List<string>();
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

    /// <summary>An assign roles service.</summary>
    [RequiredRole(RoleNames.Admin)]
    [DefaultRequest(typeof(AssignRoles))]
    public class AssignRolesService : Service
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
        public object Post(AssignRoles request)
        {
            request.UserName.ThrowIfNullOrEmpty();

            var userAuth = UserAuthRepo.GetUserAuthByUserName(request.UserName);
            if (userAuth == null)
                throw HttpError.NotFound(request.UserName);

            if (!request.Roles.IsEmpty())
            {
                foreach (var missingRole in request.Roles.Where(x => !userAuth.Roles.Contains(x)))
                {
                    userAuth.Roles.Add(missingRole);
                }
            }
            if (!request.Permissions.IsEmpty())
            {
                foreach (var missingPermission in request.Permissions.Where(x => !userAuth.Permissions.Contains(x)))
                {
                    userAuth.Permissions.Add(missingPermission);
                }
            }

            UserAuthRepo.SaveUserAuth(userAuth);

            return new AssignRolesResponse {
                AllRoles = userAuth.Roles,
                AllPermissions = userAuth.Permissions,
            };
        }
    }
}