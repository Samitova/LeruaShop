using Lerua_Shop.Models.Data.Repository;
using Lerua_Shop.Models.ModelsDTO;
using Lerua_Shop.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
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

        // GET: Admin/Pages/Details/id
        [HttpGet]
        public ActionResult PageDetails(int id)
        {
            PageDTO page = _repository.PagesRepository.GetOne(id);
            if (page == null)
            {
                return Content("The page does not exist");
            }

            PageVM model = new PageVM(page);
            return View(model);
        }

        // GET: Admin/Pages/EditPage/id
        [HttpGet]
        public ActionResult EditPage(int id)
        {
            PageDTO page = _repository.PagesRepository.GetOne(id);
            if (page == null)
            {
                return Content("The page does not exist");
            }

            PageVM model = new PageVM(page);
            return View(model);
        }

        // Post: Admin/Pages/EditPage/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPage(PageVM model)
        {
            //Check model
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            string slug = model.Slug.Replace(" ", "-").ToLower();

            List<PageDTO> pagesList = _repository.PagesRepository.GetAll(filter: x => x.Title == model.Title
                || x.Slug == slug);

            //Check title 
            if (pagesList.Where(x => x.Id != model.Id).Any(x => x.Title == model.Title))
            {
                ModelState.AddModelError("", "This title is already existed");
                return View(model);
            }

            //Check slug 
            if (pagesList.Where(x => x.Id != model.Id).Any(x => x.Slug == slug))
            {
                ModelState.AddModelError("", "This slug is already existed");
                return View(model);
            }

            //Create PageDTO       
            PageDTO page = model.GetDTO();

            try
            {
                _repository.PagesRepository.Edit(page);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                ModelState.AddModelError(string.Empty,
                $@"Unable to save the record. Another admin has updated it.{ex.Message}");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $@"Unable to save the record.{ex.Message}");
                return View(model);
            }

            TempData["AM"] = "You have edited page";
            return RedirectToAction("EditPage");
        }

        // GET: Admin/Pages/DeletePage/id
        [HttpGet]
        public ActionResult DeletePage(int id)
        {
            PageDTO page = _repository.PagesRepository.GetOne(id);
            if (page == null)
            {
                return Content("The page does not exist");
            }

            PageVM model = new PageVM(page);
            return View(model);
        }

        // Post: Admin/Pages/DeletePage/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePage(PageVM model)
        {
            try
            {
                _repository.PagesRepository.Delete(model.GetDTO());
            }
            catch (DbUpdateConcurrencyException ex)
            {
                ModelState.AddModelError(string.Empty, $@"Unable to delete record. Another admin updated the record. {ex.Message}");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $@"Unable to delete record:{ex.Message}");
            }

            TempData["AM"] = $"Page {model.Title} was successfuly deleted";
            return RedirectToAction("Index");
        }

        // Post: Admin/Pages/ReorderPages
        [HttpPost]
        public void ReorderPages(int[] id)
        {
            int count = 1;
            PageDTO page;
            foreach (var idPage in id)
            {
                page = _repository.PagesRepository.GetOne(idPage);
                page.Sorting = count;
                _repository.PagesRepository.SaveChanges();
                count++;
            }
        }

        // GET: Admin/Pages/EditSidebar/id
        [HttpGet]
        public ActionResult EditSidebar(int id)
        {
            SidebarDTO sidebar = _repository.SidebarsRepository.GetOne(id);
            if (sidebar == null)
            {
                return Content("The page does not exist");
            }

            SidebarVM model = new SidebarVM(sidebar);
            return View(model);
        }

        // Post: Admin/Pages/EditSidebar/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSidebar(SidebarVM model)
        {
            //Check model
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //Create SidebarDTO       
            SidebarDTO sidebar = _repository.SidebarsRepository.GetOne(model.Id);

            try
            {
                _repository.SidebarsRepository.Edit(sidebar);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                ModelState.AddModelError(string.Empty,
                $@"Unable to save the record. Another admin has updated it.{ex.Message}");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $@"Unable to save the record.{ex.Message}");
                return View(model);
            }
            TempData["AM"] = "You have edited the sidebar";
            return RedirectToAction("EditPage");

        }

    }
}