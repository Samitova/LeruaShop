using Lerua_Shop.Models.Data.Repository;
using Lerua_Shop.Models.ModelsDTO;
using Lerua_Shop.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lerua_Shop.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PagesController : Controller
    {
        private readonly GeneralRepository _repository = GeneralRepository.GetInstance();

        // GET: Admin/Pages
        public ActionResult Index()
        {
            List<PageVM> pages = _repository.PagesRepository.GetAll(orderBy: q => q.OrderBy(s => s.Sorting))
                                                            .Select(x => new PageVM(x)).ToList();
            return View(pages);           
        }

        // GET: Admin/Pages/AddPage
        [HttpGet]
        public ActionResult AddPage()
        {
            return View();
        }

        // POST: Admin/Pages/AddPage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddPage(PageVM model)
        {
            //Check model
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            List<PageVM> pagesList = _repository.PagesRepository.GetAll(orderBy: q => q.OrderBy(s => s.Sorting))
                                                                .Select(x => new PageVM(x)).ToList();

            string slug = model.Slug.Replace(" ", "-").ToLower();
            //Check title 
            if (pagesList.Any(x => x.Title == model.Title))
            {
                ModelState.AddModelError("", "This title is already existed");
                return View(model);
            }

            //Check slug 
            if (pagesList.Any(x => x.Slug == slug))
            {
                ModelState.AddModelError("", "This slug is already existed");
                return View(model);
            }

            //Create PageDTO            
            PageDTO page = model.GetDTO();
            page.Sorting = (slug == "home") ? 0 : int.MaxValue;

            // Try to add page
            try
            {
                _repository.PagesRepository.Add(page);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $@"Unable to create record: {ex.Message}");
                return View(model);
            }

            TempData["AM"] = "You have added a new page";
            return RedirectToAction("Index");
        }

    }
}