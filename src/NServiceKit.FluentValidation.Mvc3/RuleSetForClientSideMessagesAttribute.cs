namespace FluentValidation.Mvc {
	using System.Web;
	using System.Web.Mvc;

	/// <summary>
	/// Specifies which ruleset should be used when deciding which validators should be used to generate client-side messages.
	/// </summary>
	public class RuleSetForClientSideMessagesAttribute : ActionFilterAttribute {
		private const string key = "_FV_ClientSideRuleSet";
		string[] ruleSets;

        /// <summary>Initializes a new instance of the FluentValidation.Mvc.RuleSetForClientSideMessagesAttribute class.</summary>
        ///
        /// <param name="ruleSets">Sets the rule belongs to.</param>
		public RuleSetForClientSideMessagesAttribute(params string[] ruleSets) {
			this.ruleSets = ruleSets;
		}

        /// <summary>Called by the ASP.NET MVC framework before the action method executes.</summary>
        ///
        /// <param name="filterContext">The filter context.</param>
		public override void OnActionExecuting(ActionExecutingContext filterContext) {
			filterContext.HttpContext.Items[key] = ruleSets;
		}

        /// <summary>Gets rule sets for client validation.</summary>
        ///
        /// <param name="context">The context.</param>
        ///
        /// <returns>An array of string.</returns>
		public static string[] GetRuleSetsForClientValidation(HttpContextBase context) {
			return context.Items[key] as string[] ?? new string[] { null };
		}
	}
}