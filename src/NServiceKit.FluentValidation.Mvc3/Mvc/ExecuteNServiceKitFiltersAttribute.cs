using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using NServiceKit.ServiceInterface;
using NServiceKit.ServiceInterface.Auth;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints.Extensions;

namespace NServiceKit.Mvc
{
    /// <summary>Attribute for execute n service kit filters.</summary>
	public class ExecuteNServiceKitFiltersAttribute : ActionFilterAttribute
	{
        /// <summary>Called by the ASP.NET MVC framework before the action method executes.</summary>
        ///
        /// <param name="filterContext">The filter context.</param>
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var ssController = filterContext.Controller as NServiceKitController;
			if (ssController == null) return;

			var authAttrs = GetActionAndControllerAttributes<AuthenticateAttribute>(filterContext);
			if (authAttrs.Count > 0 && ( ssController.AuthSession==null || !ssController.AuthSession.IsAuthenticated))
			{
				filterContext.Result = ssController.AuthenticationErrorResult;
				return;
			}

			var roleAttrs = GetActionAndControllerAttributes<RequiredRoleAttribute>(filterContext);
			var anyRoleAttrs = GetActionAndControllerAttributes<RequiresAnyRoleAttribute>(filterContext);
			var permAttrs = GetActionAndControllerAttributes<RequiredPermissionAttribute>(filterContext);
			var anyPermAttrs = GetActionAndControllerAttributes<RequiresAnyPermissionAttribute>(filterContext);

			if (roleAttrs.Count + anyRoleAttrs.Count + permAttrs.Count + anyPermAttrs.Count == 0) return;

			var httpReq = HttpContext.Current.Request.ToRequest();
			var userAuthRepo = httpReq.TryResolve<IUserAuthRepository>();

			var hasRoles = roleAttrs.All(x => x.HasAllRoles(httpReq, ssController.AuthSession, userAuthRepo));
			if (!hasRoles)
			{
				filterContext.Result = ssController.AuthorizationErrorResult;
				return;
			}

			var hasAnyRole = anyRoleAttrs.All(x => x.HasAnyRoles(httpReq, ssController.AuthSession, userAuthRepo));
			if (!hasAnyRole)
			{
				filterContext.Result = ssController.AuthorizationErrorResult;
				return;
			}

			var hasPermssions = permAttrs.All(x => x.HasAllPermissions(httpReq, ssController.AuthSession, userAuthRepo));
			if (!hasPermssions)
			{
				filterContext.Result = ssController.AuthorizationErrorResult;
				return;
			}

			var hasAnyPermission = anyPermAttrs.All(x => x.HasAnyPermissions(httpReq, ssController.AuthSession, userAuthRepo));
			if (!hasAnyPermission)
			{
				filterContext.Result = ssController.AuthorizationErrorResult;
				return;
			}
		}

		private static List<T> GetActionAndControllerAttributes<T>(ActionExecutingContext filterContext)
			where T : Attribute
		{
			var attrs = new List<T>();

			var attr = filterContext.ActionDescriptor
				.GetCustomAttributes(typeof(T), true)
				.FirstOrDefault() as T;

			if (attr != null)
				attrs.Add(attr);

			attr = filterContext.Controller.GetType()
				.GetCustomAttributes(typeof(T), true)
				.FirstOrDefault() as T;

			if (attr != null)
				attrs.Add(attr);

			return attrs;
		}
	}
}