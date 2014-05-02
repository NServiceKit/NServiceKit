using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface.Auth;
using NServiceKit.WebHost.Endpoints.Extensions;

namespace NServiceKit.ServiceInterface
{
    /// <summary>
    /// Indicates that the request dto, which is associated with this attribute,
    /// can only execute, if the user has specific permissions.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class RequiredPermissionAttribute : AuthenticateAttribute
    {
        /// <summary>Gets or sets the required permissions.</summary>
        ///
        /// <value>The required permissions.</value>
        public List<string> RequiredPermissions { get; set; }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.RequiredPermissionAttribute class.</summary>
        ///
        /// <param name="applyTo">    The apply to.</param>
        /// <param name="permissions">A variable-length parameters list containing permissions.</param>
        public RequiredPermissionAttribute(ApplyTo applyTo, params string[] permissions)
        {
            this.RequiredPermissions = permissions.ToList();
            this.ApplyTo = applyTo;
            this.Priority = (int) RequestFilterPriority.RequiredPermission;
        }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.RequiredPermissionAttribute class.</summary>
        ///
        /// <param name="permissions">A variable-length parameters list containing permissions.</param>
        public RequiredPermissionAttribute(params string[] permissions)
            : this(ApplyTo.All, permissions) {}

        /// <summary>This method is only executed if the HTTP method matches the <see cref="ApplyTo"/> property.</summary>
        ///
        /// <param name="req">       The http request wrapper.</param>
        /// <param name="res">       The http response wrapper.</param>
        /// <param name="requestDto">The request DTO.</param>
        public override void Execute(IHttpRequest req, IHttpResponse res, object requestDto)
        {
            base.Execute(req, res, requestDto); //first check if session is authenticated
            if (res.IsClosed) return; //AuthenticateAttribute already closed the request (ie auth failed)

            var session = req.GetSession();
            if (HasAllPermissions(req, session)) return;

            if (DoHtmlRedirectIfConfigured(req, res)) return;

            res.StatusCode = (int)HttpStatusCode.Forbidden;
            res.StatusDescription = "Invalid Permission";
            res.EndRequest();
        }

        /// <summary>Query if 'session' has all permissions.</summary>
        ///
        /// <param name="req">         The request.</param>
        /// <param name="session">     The session.</param>
        /// <param name="userAuthRepo">The user authentication repo.</param>
        ///
        /// <returns>true if all permissions, false if not.</returns>
        public bool HasAllPermissions(IHttpRequest req, IAuthSession session, IUserAuthRepository userAuthRepo=null)
        {
            if (HasAllPermissions(session)) return true;

            if (userAuthRepo == null) 
                userAuthRepo = req.TryResolve<IUserAuthRepository>();

            if (userAuthRepo == null) return false;

            var userAuth = userAuthRepo.GetUserAuth(session, null);
            session.UpdateSession(userAuth);

            if (HasAllPermissions(session))
            {
                req.SaveSession(session);
                return true;
            }
            return false;
        }

        /// <summary>Query if 'session' has all permissions.</summary>
        ///
        /// <param name="session">The session.</param>
        ///
        /// <returns>true if all permissions, false if not.</returns>
        public bool HasAllPermissions(IAuthSession session)
        {
            return this.RequiredPermissions
                .All(requiredPermission => session != null 
                    && session.HasPermission(requiredPermission));
        }
    }

}
