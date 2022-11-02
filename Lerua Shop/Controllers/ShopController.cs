using Lerua_Shop.Models.Data.Repository;
using Lerua_Shop.Models.ModelsDTO;
using Lerua_Shop.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.IO;
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

        // GET: Shop/Category/name
        public ActionResult Category(string name)
        {
            CategoryDTO category = _repository.CategoriesRepository.GetOne(x => x.Slug == name);
            if (category == null)
            {
                return Content("This category does not exist");
            }

            List<ProductVM> productList = _repository.ProductsRepository.GetAll(filter: x => x.CategoryId == category.Id)
                            .Select(x => new ProductVM(x)).ToList();

            ViewBag.CategoryName = category.Name;           

            return View(productList);
        }

        // GET: Shop/product-details/name
        [ActionName("product-details")]
        public ActionResult ProductDetails(string name)
        {           
            if (!_repository.ProductsRepository.Any( x => x.Slug == name))
            {
                return RedirectToAction("Index", "Shop");
            }

            ProductDTO productDTO = _repository.ProductsRepository.GetOne(x => x.Slug == name);
            ProductVM model = new ProductVM(productDTO);

            model.GaleryImage = model.GaleryImage = Directory
                .EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + productDTO.Id + "/Gallery/Thumbs"))
                .Select(x => Path.GetFileName(x));

            return View("ProductDetails", model);
        }
    }
}