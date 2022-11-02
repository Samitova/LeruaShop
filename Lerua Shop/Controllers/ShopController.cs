using Lerua_Shop.Models.Data.Repository;
using Lerua_Shop.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lerua_Shop.Controllers
{
    public class ShopController : Controller
    {
        private readonly GeneralRepository _repository = GeneralRepository.GetInstance();

        // GET: Shop
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Pages");   
        }

        // GET: Shop
        public ActionResult CategoryMenuPartial()
        {
            List<CategoryVM> categoriesList = _repository.CategoriesRepository.GetAll(
                             orderBy: q => q.OrderBy(s => s.Sorting)).Select(x => new CategoryVM(x)).ToList();

            return PartialView("_CategoryMenuPartial", categoriesList);
        }
    }
}