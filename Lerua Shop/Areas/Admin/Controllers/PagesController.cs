using Lerua_Shop.Models.Data.Repository;
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
    }
}