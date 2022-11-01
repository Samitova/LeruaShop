using Lerua_Shop.Models.Data.Repository;
using Lerua_Shop.Models.ModelsDTO;
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

        // POST: Admin/Shop/AddNewCategory
        [HttpPost]
        public string AddNewCategory(string catName)
        {
            if (_repository.CategoriesRepository.GetAll(filter: x => x.Name == catName).Count > 0)
                return "titletaken";

            CategoryDTO category = new CategoryDTO();
            category.Name = catName;
            category.Slug = catName.Replace(" ", "-").ToLower();
            category.Sorting = int.MaxValue;

            // Try to add category
            try
            {
                _repository.CategoriesRepository.Add(category);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $@"Unable to create the record: {ex.Message}");
                return "titletaken";
            }

            return category.Id.ToString();
        }

    }
}