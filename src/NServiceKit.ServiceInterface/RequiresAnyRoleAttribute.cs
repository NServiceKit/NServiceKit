using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using NServiceKit.Common;
using NServiceKit.Common.Web;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface.Auth;
using NServiceKit.WebHost.Endpoints;

namespace NServiceKit.ServiceInterface
{
    /// <summary>
    /// Indicates that the request dto, which is associated with this attribute,
    /// can only execute, if the user has any of the specified roles.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class RequiresAnyRoleAttribute : AuthenticateAttribute
    {
        /// <summary>Gets or sets the required roles.</summary>
        ///
        /// <value>The required roles.</value>
        public List<string> RequiredRoles { get; set; }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.RequiresAnyRoleAttribute class.</summary>
        ///
        /// <param name="applyTo">The apply to.</param>
        /// <param name="roles">  A variable-length parameters list containing roles.</param>
        public RequiresAnyRoleAttribute(ApplyTo applyTo, params string[] roles)
        {
            this.RequiredRoles = roles.ToList();
            this.ApplyTo = applyTo;
            this.Priority = (int)RequestFilterPriority.RequiredRole;
        }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.RequiresAnyRoleAttribute class.</summary>
        ///
        /// <param name="roles">A variable-length parameters list containing roles.</param>
        public RequiresAnyRoleAttribute(params string[] roles)
            : this(ApplyTo.All, roles) { }

        /// <summary>This method is only executed if the HTTP method matches the <see cref="ApplyTo"/> property.</summary>
        ///
        /// <param name="req">       The http request wrapper.</param>
        /// <param name="res">       The http response wrapper.</param>
        /// <param name="requestDto">The request DTO.</param>
        public override void Execute(IHttpRequest req, IHttpResponse res, object requestDto)
        {
            if (EndpointHost.Config.HasValidAuthSecret(req))
                return;

            base.Execute(req, res, requestDto); //first check if session is authenticated
            if (res.IsClosed) return; //AuthenticateAttribute already closed the request (ie auth failed)

            var session = req.GetSession();
            if (HasAnyRoles(req, session)) return;

            if (DoHtmlRedirectIfConfigured(req, res)) return;

            res.StatusCode = (int)HttpStatusCode.Forbidden;
            res.StatusDescription = "Invalid Role";
            res.EndRequest();
        }

        /// <summary>Query if 'session' has any roles.</summary>
        ///
        /// <param name="req">         The request.</param>
        /// <param name="session">     The session.</param>
        /// <param name="userAuthRepo">The user authentication repo.</param>
        ///
        /// <returns>true if any roles, false if not.</returns>
        public bool HasAnyRoles(IHttpRequest req, IAuthSession session, IUserAuthRepository userAuthRepo = null)
        {
            if (HasAnyRoles(session)) return true;

            session.UpdateFromUserAuthRepo(req, userAuthRepo);

            if (HasAnyRoles(session))
            {
                req.SaveSession(session);
                return true;
            }
            return false;
        }

        /// <summary>Query if 'session' has any roles.</summary>
        ///
        /// <param name="session">The session.</param>
        ///
        /// <returns>true if any roles, false if not.</returns>
        public bool HasAnyRoles(IAuthSession session)
        {
            return this.RequiredRoles
                .Any(requiredRole => session != null
                    && session.HasRole(requiredRole));
        }

        /// <summary>
        /// Check all session is in any supplied roles otherwise a 401 HttpError is thrown
        /// </summary>
        /// <param name="requestContext"></param>
        /// <param name="requiredRoles"></param>
        public static void AssertRequiredRoles(IRequestContext requestContext, params string[] requiredRoles)
        {
            if (requiredRoles.IsEmpty()) return;

            var req = requestContext.Get<IHttpRequest>();
            if (EndpointHost.Config.HasValidAuthSecret(req))
                return;

            var session = req.GetSession();

            if (session != null && requiredRoles.Any(session.HasRole))
                return;

            session.UpdateFromUserAuthRepo(req);

            if (session != null && requiredRoles.Any(session.HasRole))
                return;

            var statusCode = session != null && session.IsAuthenticated
                ? (int)HttpStatusCode.Forbidden
                : (int)HttpStatusCode.Unauthorized;

            throw new HttpError(statusCode, "Invalid Role");
        }
    }
}
