using Lerua_Shop.Models.Data.Repository;
using Lerua_Shop.Models.ModelsDTO;
using Lerua_Shop.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace Lerua_Shop.Areas.Admin.Controllers
{
    public class ShopController : Controller
    {
        private readonly GeneralRepository _repository = GeneralRepository.GetInstance();

        /**************************************************************************************************/
        //                                Category functionality                          
        /**************************************************************************************************/

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
            if (_repository.CategoriesRepository.Any(filter: x => x.Name == catName))
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

        // POST: Admin/Shop/ReorderCategories
        [HttpPost]
        public void ReorderCategories(int[] id)
        {
            int count = 1;
            CategoryDTO category;
            foreach (var idCat in id)
            {
                category = _repository.CategoriesRepository.GetOne(idCat);
                category.Sorting = count;
                _repository.PagesRepository.SaveChanges();
                count++;
            }
        }

        // GET: Admin/Shop/DeleteCategory/id        
        [HttpGet]
        public ActionResult DeleteCategory(int id)
        {
            CategoryDTO category = _repository.CategoriesRepository.GetOne(id);
            try
            {
                _repository.CategoriesRepository.Delete(category);
            }           
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Unable to delete record:{ex.Message}");
            }

            TempData["AM"] = $"{category.Name} was successfuly deleted";
            return RedirectToAction("Categories");
        }

        // Post: Admin/Shop/RenameCategory/id
        [HttpPost]
        public string RenameCategory(string newCatName, int id)
        {            
            if (_repository.CategoriesRepository.Any(x => x.Name == newCatName && x.Id == id))
                return "noaction";

            if (_repository.CategoriesRepository.Any(x => x.Name == newCatName))
                return "titletaken";

            CategoryDTO category = _repository.CategoriesRepository.GetOne(id);
            category.Name = newCatName;
            category.Slug = newCatName.Replace(" ", "-").ToLower();

            try
            {
                _repository.CategoriesRepository.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                ModelState.AddModelError(string.Empty, $@"Unable to update. Another admin updated the record. {ex.Message}");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $@"Unable to updete the record:{ex.Message}");
            }

            TempData["AM"] = $"Category was successfuly renamed";
            return "renamed";
        }


        /**************************************************************************************************/
        //                                Product functionality                          
        /**************************************************************************************************/

        // GET: Admin/Shop/Products
        [HttpGet]
        public ActionResult Products(int? page, int? catId)
        {
            List<ProductVM> products = _repository.ProductsRepository.GetAll(filter: x => catId == null
                                        || catId == 0 || x.CategoryId == catId).Select(x => new ProductVM(x)).ToList();

            ViewBag.Categories = new SelectList(_repository.CategoriesRepository.GetAll(), "Id", "Name");

            ViewBag.SelectedCategory = catId.ToString();

            var pageNumber = page ?? 1;
            var onePageOfProducts = products.ToPagedList(pageNumber, 3);
            ViewBag.onePageOfProducts = onePageOfProducts;


            return View(products);
        }



    }
}