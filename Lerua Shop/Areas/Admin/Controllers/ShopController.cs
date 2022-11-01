using Lerua_Shop.Models.Data.Repository;
using Lerua_Shop.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lerua_Shop.Areas.Admin.Controllers
{
    public class ShopController : Controller
    {
        private readonly GeneralRepository _repository = GeneralRepository.GetInstance();

        // GET: Admin/Shop
        [HttpGet]
        public ActionResult Categories()
        {
            List<CategoryVM> categories = _repository.CategoriesRepository.GetAll(orderBy: q => q.OrderBy(s => s.Sorting))
                                                                    .Select(x => new CategoryVM(x)).ToList();
            return View(categories);
        }
    }
}