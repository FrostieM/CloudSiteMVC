using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace cloudSiteMvc
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}"); 
			
			routes.MapRoute(
				name: "Log",
				url: "Log/{id}",
				defaults: new { controller = "Logger", action = "LogInfo", id = UrlParameter.Optional }
			);
			
			routes.MapRoute(
				name: "ComputerPrograms",
				url: "ComputerPrograms/{id}",
				defaults: new { controller = "Home", action = "ComputerPrograms", id = UrlParameter.Optional }
			);

			routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
			);

			
		}
	}
}
