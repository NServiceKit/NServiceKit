using System;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using NServiceKit.CacheAccess;
using NServiceKit.ServiceInterface;
using NServiceKit.ServiceInterface.Auth;
using NServiceKit.Text;

namespace NServiceKit.Mvc
{
    /// <summary>A controller base.</summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    [Obsolete("To avoid name conflicts with MVC's ControllerBase this has been renamed to NServiceKitController")]
    public abstract class ControllerBase<T> : NServiceKitController<T> where T : class, IAuthSession, new() { }
    /// <summary>A controller base.</summary>
    [Obsolete("To avoid name conflicts with MVC's ControllerBase this has been renamed to NServiceKitController")]
    public abstract class ControllerBase : NServiceKitController { }

    /// <summary>A controller for handling service kits.</summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    public abstract class NServiceKitController<T> : NServiceKitController
        where T : class, IAuthSession, new()
    {
        /// <summary>Gets the user session.</summary>
        ///
        /// <value>The user session.</value>
        protected T UserSession
        {
            get { return SessionAs<T>(); }
        }

        /// <summary>Gets the authentication session.</summary>
        ///
        /// <value>The authentication session.</value>
        public override IAuthSession AuthSession
        {
            get { return UserSession; }
        }
    }


    /// <summary>A controller for handling service kits.</summary>
    [ExecuteNServiceKitFilters]
    public abstract class NServiceKitController : Controller
    {
        /// <summary>The default action.</summary>
        public static string DefaultAction = "Index";
        /// <summary>The catch all controller.</summary>
        public static Func<RequestContext, NServiceKitController> CatchAllController;

        /// <summary>
        /// Default redirct URL if [Authenticate] attribute doesn't permit access.
        /// </summary>
        public virtual string LoginRedirectUrl
        {
            get { return "/login?redirect={0}"; }
        }

        /// <summary>
        /// To change the error result when authentication (<see cref="AuthenticateAttribute"/>) 
        /// fails from redirection to something else, 
        /// override this property and return the appropriate result.
        /// </summary>
        public virtual ActionResult AuthenticationErrorResult
        {
            get
            {
                var returnUrl = HttpContext.Request.Url.PathAndQuery;
                return new RedirectResult(LoginRedirectUrl.Fmt(HttpUtility.UrlEncode(returnUrl)));
            }
        }

        /// <summary>
        /// To change the error result when authorization fails
        /// to something else, override this property and return the appropriate result.
        /// </summary>
        public virtual ActionResult AuthorizationErrorResult
        {
            get
            {
                return new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Error",
                    action = "Unauthorized"
                }));
            }
        }

        /// <summary>Gets or sets the cache.</summary>
        ///
        /// <value>The cache.</value>
        public ICacheClient Cache { get; set; }

        private ISessionFactory sessionFactory;

        /// <summary>Gets or sets the session factory.</summary>
        ///
        /// <value>The session factory.</value>
        public ISessionFactory SessionFactory
        {
            get { return sessionFactory ?? new SessionFactory(Cache); }
            set { sessionFactory = value; }
        }

        /// <summary>
        /// Typed UserSession
        /// </summary>
        private object userSession;

        /// <summary>Session as.</summary>
        ///
        /// <typeparam name="TUserSession">Type of the user session.</typeparam>
        ///
        /// <returns>A TUserSession.</returns>
        protected TUserSession SessionAs<TUserSession>()
        {
            return (TUserSession)(userSession ?? (userSession = Cache.SessionAs<TUserSession>()));
        }

        /// <summary>Clears the session.</summary>
        public virtual void ClearSession()
        {
            userSession = null;
            Cache.ClearSession();
        }

        /// <summary>
        /// Dynamic Session Bag
        /// </summary>
        private ISession session;

        /// <summary>Gets the session.</summary>
        ///
        /// <value>The session.</value>
        public new ISession Session
        {
            get
            {
                return session ?? (session = SessionFactory.GetOrCreateSession());
            }
        }

        /// <summary>Gets the authentication session.</summary>
        ///
        /// <value>The authentication session.</value>
        public virtual IAuthSession AuthSession
        {
            get { return (IAuthSession)userSession; }
        }

        /// <summary>
        /// Creates a <see cref="T:System.Web.Mvc.JsonResult" /> object that serializes the specified object to JavaScript Object Notation (JSON) format using the content type, content encoding, and the
        /// JSON request behavior.
        /// </summary>
        ///
        /// <param name="data">           The JavaScript object graph to serialize.</param>
        /// <param name="contentType">    The content type (MIME type).</param>
        /// <param name="contentEncoding">The content encoding.</param>
        /// <param name="behavior">       The JSON request behavior.</param>
        ///
        /// <returns>The result object that serializes the specified object to JSON format.</returns>
        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new NServiceKitJsonResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding
            };
        }

        /// <summary>Executes the default action on a different thread, and waits for the result.</summary>
        ///
        /// <param name="httpContext">Context for the HTTP.</param>
        ///
        /// <returns>An ActionResult.</returns>
        public virtual ActionResult InvokeDefaultAction(HttpContextBase httpContext)
        {
            try
            {
                this.View(DefaultAction).ExecuteResult(this.ControllerContext);
            }
            catch
            {
                // We failed to execute our own default action, so we'll fall back to
                // the CatchAllController, if one is specified.

                if (CatchAllController != null)
                {
                    var catchAllController = CatchAllController(this.Request.RequestContext);
                    InvokeControllerDefaultAction(catchAllController, httpContext);
                }
            }

            return new EmptyResult();
        }

        /// <summary>Called when a request matches this controller, but no method with the specified action name is found in the controller.</summary>
        ///
        /// <param name="actionName">The name of the attempted action.</param>
        protected override void HandleUnknownAction(string actionName)
        {
            if (CatchAllController == null)
            {
                base.HandleUnknownAction(actionName); // delegate to default MVC behaviour, which will throw 404.
            }
            else
            {
                var catchAllController = CatchAllController(this.Request.RequestContext);
                InvokeControllerDefaultAction(catchAllController, HttpContext);
            }
        }

        private void InvokeControllerDefaultAction(NServiceKitController controller, HttpContextBase httpContext)
        {
            var routeData = new RouteData();
            var controllerName = controller.GetType().Name.Replace("Controller", "");
            routeData.Values.Add("controller", controllerName);
            routeData.Values.Add("action", DefaultAction);
            routeData.Values.Add("url", httpContext.Request.Url.OriginalString);
            controller.Execute(new RequestContext(httpContext, routeData));

        }
    }

    /// <summary>Encapsulates the result of a service kit json.</summary>
    public class NServiceKitJsonResult : JsonResult
    {
        /// <summary>Enables processing of the result of an action method by a custom type that inherits from the <see cref="T:System.Web.Mvc.ActionResult" /> class.</summary>
        ///
        /// <param name="context">The context within which the result is executed.</param>
        public override void ExecuteResult(ControllerContext context)
        {
            var response = context.HttpContext.Response;
            response.ContentType = !string.IsNullOrEmpty(ContentType) ? ContentType : "application/json";

            if (ContentEncoding != null)
            {
                response.ContentEncoding = ContentEncoding;
            }

            if (Data != null)
            {
                response.Write(JsonSerializer.SerializeToString(Data));
            }
        }
    }
}