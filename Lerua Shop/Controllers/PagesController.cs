using Lerua_Shop.Models.Data.Repository;
using Lerua_Shop.Models.ModelsDTO;
using Lerua_Shop.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lerua_Shop.Controllers
{
    public class PagesController : Controller
    {
        private readonly GeneralRepository _repository = GeneralRepository.GetInstance();

        // GET: Index/{page}
        public ActionResult Index(string page = "")
        {
            if (page == "")
            {
                page = "home";
            }

            PageDTO pageDTO = _repository.PagesRepository.GetOne(x => x.Slug == page.Replace(" ", "-").ToLower());

            if (pageDTO == null)
            {
                return RedirectToAction("Index", new { page = "" });
            }

            ViewBag.PageTitle = pageDTO.Title;
            if (pageDTO.HasSidebar)
            {
                ViewBag.Sidebar = "Yes";
            }
            else
            {
                ViewBag.Sidebar = "No";
            }

            PageVM model = new PageVM(pageDTO);

            return View(model);
        }

        // GET: Index/PagesMenuPartial
        public ActionResult PagesMenuPartial()
        {
            List<PageVM> pageList = _repository.PagesRepository.GetAll(filter: x => x.Slug != "home",
                orderBy: q => q.OrderBy(s => s.Sorting)).Select(x => new PageVM(x)).ToList();

            return PartialView("_PagesMenuPartial", pageList);
        }
    }
}