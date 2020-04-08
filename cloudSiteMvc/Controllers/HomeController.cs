using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using cloudSiteMvc.Attributes;
using cloudSiteMvc.ViewData;

namespace cloudSiteMvc.Controllers
{
	
	[AuthorizationOrRedirect]
	public class HomeController : Controller
	{
		private class ComputerAccess
		{
			private List<ComputerInfo> Computers { get; set; } = new List<ComputerInfo>();
			private DateTime? Date { get; set; }

			public IEnumerable<ComputerInfo> GetComputers(SpreadsheetReader reader)
			{
				var now = DateTime.Now;
				if (Date != null && Date.Value.AddMinutes(2) >= now) return Computers;

				Computers.Clear();
				Date = now;
				Computers = reader.GetComputers().ToList();

				return Computers;
			}
		}

		private static readonly SpreadsheetReader Reader = new SpreadsheetReader();
		private static readonly ComputerAccess Computer = new ComputerAccess();

		private const int MaxPageSize = 10;


		
		public ActionResult Index()
		{
			return View(Computer.GetComputers(Reader));
		}

		
		public ActionResult GetComputersByProgram(string programName)
		{
			if (programName == null) return PartialView(Computer.GetComputers(Reader));
			var computers = Computer.GetComputers(Reader)
				.Where(comp
					=> comp.Apps.Any(app => app.Name.ToLower().Contains(programName.ToLower())));

			return PartialView(computers);
		}

		[Route("{computerName}/{page:int}")]
		public ActionResult ComputerPrograms(string computerName, int page = 1)
		{
			var computerInfo = Computer.GetComputers(Reader)
				.Single(comp => comp.Name == computerName);

			var computer = new ComputerViewData
			{
				ComputerInfo = new ComputerInfo
				{
					Date = computerInfo.Date,
					Name = computerInfo.Name,
					Apps = computerInfo.Apps.Skip(MaxPageSize * (page - 1)).Take(MaxPageSize)
				},
				Pagination = new PaginationViewData
				{
					CurrentPage = page,
					PageSize = MaxPageSize,
					TotalItems = computerInfo.Apps.Count()
				}
			};

			return View(computer);
		}

		public ActionResult About()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}

		public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}
	}
}