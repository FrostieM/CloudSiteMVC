﻿using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using cloudSiteMvc.Attributes;
using cloudSiteMvc.Models;

namespace cloudSiteMvc.Controllers
{
	[AuthorizationOrRedirect]
	public class LoggerController : Controller
	{
		private readonly ApplicationDbContext _db = new ApplicationDbContext();


		[OutputCache(Duration = 1600, Location = OutputCacheLocation.Client)]
		public async Task<ActionResult> LogInfo()
		{
			var loggers = await _db.Loggers.Include(l => l.User).OrderByDescending(i => i.DateTime).ToListAsync();
			return View(loggers);
		}
	}
}