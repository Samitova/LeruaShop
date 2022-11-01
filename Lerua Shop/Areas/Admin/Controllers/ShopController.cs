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
using System.IO;
using System.Web.Helpers;

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

        // GET: Admin/Shop/AddProduct
        [HttpGet]
        public ActionResult AddProduct()
        {
            ProductVM product = new ProductVM(new ProductDTO());
            // initialize categories
            product.Categories = new SelectList(_repository.CategoriesRepository.GetAll(),
                dataValueField: "Id", dataTextField: "Name");
            return View(product);
        }

        // POST: Admin/Shop/AddProduct
        [HttpPost]
        public ActionResult AddProduct(ProductVM model, HttpPostedFileBase file)
        {
            var categories = new SelectList(_repository.CategoriesRepository.GetAll(),
                dataValueField: "Id", dataTextField: "Name");
            //check model
            if (!ModelState.IsValid)
            {
                model.Categories = categories;
                return View(model);
            }

            if (_repository.ProductsRepository.Any( x => x.Name == model.Name))
            {
                ModelState.AddModelError("", "This product name is already existed");
                return View(model);
            }

            if (file != null && file.ContentLength > 0)
            {
                string ext = file.ContentType.ToLower();

                if (ext != "image/jpg" && ext != "image/jpeg" && ext != "image/pjpeg" &&
                    ext != "image/gif" && ext != "image/x-png" && ext != "image/png")
                {
                    model.Categories = categories;
                    ModelState.AddModelError("", "Bad image extention");
                    return View(model);
                }
            }

            // add product

            ProductDTO product = model.GetDTO();
            product.CategoryName = _repository.CategoriesRepository.GetOne(model.CategoryId)?.Name;
            product.ImageName = file?.FileName;

            try
            {
                _repository.ProductsRepository.Add(product);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $@"Unable to create record: {ex.Message}");
                model.Categories = categories;
                return View(model);
            }

            // create directories for image
            var originalDirectory = new DirectoryInfo(String.Format($"{Server.MapPath(@"\")}Images\\Uploads\\Products\\"));

            List<string> paths = new List<string>
                {
                    originalDirectory.ToString(),
                    Path.Combine(originalDirectory.ToString(), product.Id.ToString()),
                    Path.Combine(originalDirectory.ToString(), product.Id.ToString() + "\\Thumbs"),
                    Path.Combine(originalDirectory.ToString(), product.Id.ToString() + "\\Gallery"),
                    Path.Combine(originalDirectory.ToString(), product.Id.ToString() + "\\Gallery\\Thumbs")
                };

            foreach (var path in paths)
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }

            // save image 

            if (file != null && file.ContentLength > 0)
            {
                var savePathFull = string.Format($"{paths[1]}\\{file.FileName}");
                var savePathMini = string.Format($"{paths[2]}\\{file.FileName}");

                file.SaveAs(savePathFull);
                WebImage image = new WebImage(file.InputStream);
                image.Resize(200, 200).Crop(1, 1);
                image.Save(savePathMini);

            }

            TempData["AM"] = "You have added a new product";

            return RedirectToAction("AddProduct");
        }

    }
}