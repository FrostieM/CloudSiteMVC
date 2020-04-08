using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using cloudSiteMvc.Models;

namespace cloudSiteMvc.Controllers
{
	public class LoggerController : Controller
	{
		private readonly ApplicationDbContext _db = new ApplicationDbContext();


		[OutputCache(Duration = 3600, Location = OutputCacheLocation.Client)]
		public async Task<ActionResult> LogInfo()
		{
			var loggers = await _db.Loggers.Include(l => l.User).OrderByDescending(i => i.DateTime).ToListAsync();
			return View(loggers);
		}
	}
}