using System.Web.Mvc;

namespace cloudSiteMvc.Attributes
{
	public class AuthorizationOrRedirect : AuthorizeAttribute
	{
		public override void OnAuthorization(AuthorizationContext filterContext)
		{
			// If they are authorized, handle accordingly
			if (AuthorizeCore(filterContext.HttpContext))
			{
				base.OnAuthorization(filterContext);
			}
			else
			{
				// Otherwise redirect to your specific authorized area
				filterContext.Result = new RedirectResult("~/Account/Login");
			}
		}
	}
}