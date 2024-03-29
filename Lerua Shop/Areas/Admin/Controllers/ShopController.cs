﻿using Lerua_Shop.Models.Data.Repository;
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
using Lerua_Shop.Areas.Admin.Models.ViewModels.Shop;

namespace Lerua_Shop.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
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

        // GET: Admin/Shop/EditProduct/id
        [HttpGet]
        public ActionResult EditProduct(int id)
        {
            ProductDTO product = _repository.ProductsRepository.GetOne(id);

            if (product == null)
            {
                return Content("This product does not exist");
            }

            ProductVM model = new ProductVM(product);

            // initilize categories
            model.Categories = new SelectList(_repository.CategoriesRepository.GetAll(),
                dataValueField: "Id", dataTextField: "Name");
            // get images
            model.GaleryImage = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                .Select(x => Path.GetFileName(x));

            return View(model);
        }

        // POST: Admin/Shop/EditProduct/id
        [HttpPost]
        public ActionResult EditProduct(ProductVM model, HttpPostedFileBase file)
        {
            bool imageExisted = false;
            model.Categories = new SelectList(_repository.CategoriesRepository.GetAll(),
               dataValueField: "Id", dataTextField: "Name"); ;

            model.GaleryImage = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + model.Id + "/Thumbs"))
               .Select(x => Path.GetFileName(x));

            //check model
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //check name
            if (_repository.ProductsRepository.Any(filter: x => x.Id != model.Id && x.Name == model.Name))
            {
                ModelState.AddModelError("", "This product name is already existed");
                return View(model);
            }

            //check image
            if (file != null && file.ContentLength > 0)
            {
                string ext = file.ContentType.ToLower();

                if (ext != "image/jpg" && ext != "image/jpeg" && ext != "image/pjpeg" &&
                    ext != "image/gif" && ext != "image/x-png" && ext != "image/png")
                {
                    ModelState.AddModelError("", "The image was not uploaded - wrong image extention");
                    return View(model);
                }
                imageExisted = true;
            }

            ProductDTO product = _repository.ProductsRepository.GetOne(model.Id);
            product = model.GetDTO();
            product.CategoryName = _repository.CategoriesRepository.GetOne(model.CategoryId)?.Name;
            if (imageExisted)
                product.ImageName = file.FileName;

            try
            {
                _repository.ProductsRepository.Edit(product);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError(string.Empty, $@"Unable to edit record: {ex.Message}");
                return View(model);
            }

            TempData["AM"] = "You have edited the product";

            #region Upload Image

            if (imageExisted)
            {
                // delete existed image
                var originalDirectory = new DirectoryInfo(String.Format($"{Server.MapPath(@"\")}Images\\Uploads\\Products\\"));

                List<string> paths = new List<string>
                {
                    Path.Combine(originalDirectory.ToString(), model.Id.ToString()),
                    Path.Combine(originalDirectory.ToString(), model.Id.ToString() + "\\Thumbs")
                };

                foreach (var path in paths)
                {
                    foreach (var imagefile in new DirectoryInfo(path).GetFiles())
                    {
                        imagefile.Delete();
                    }
                }

                // save image 
                var savePathFull = string.Format($"{paths[0]}\\{file.FileName}");
                var savePathMini = string.Format($"{paths[1]}\\{file.FileName}");

                // !! write try/catch
                file.SaveAs(savePathFull);
                WebImage image = new WebImage(file.InputStream);
                image.Resize(200, 200).Crop(1, 1);
                image.Save(savePathMini);

            }
            #endregion
            return RedirectToAction("EditProduct");
        }

        // Post: Admin/Shop/DeleteProduct/id      
        public ActionResult DeleteProduct(int id)
        {
            ProductDTO product = _repository.ProductsRepository.GetOne(id);
            if (product == null)
            {
                return Content("The product does not exist");
            }
            try
            {
                _repository.ProductsRepository.Delete(product);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                ModelState.AddModelError(string.Empty, $"Unable to delete product. Another admin updated the product. {ex.Message}");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Unable to delete record:{ex.Message}");
            }

            var path = new DirectoryInfo(String.Format($"{Server.MapPath(@"\")}Images\\Uploads\\Products\\{id.ToString()}")).ToString();
            if (Directory.Exists(path))
                Directory.Delete(path, true);

            TempData["AM"] = "You have deleted the product";
            return RedirectToAction("Products");
        }

        /**************************************************************************************************/
        //                                Product gallery image functionality                          
        /**************************************************************************************************/

        // Post: Admin/Shop/SaveGalleryImages/id
        [HttpPost]
        public void SaveGalleryImages(int id)
        {
            foreach (string filename in Request.Files)
            {
                HttpPostedFileBase file = Request.Files[filename];
                if (file != null && file.ContentLength > 0)
                {
                    var savePathFull = new DirectoryInfo(String.Format($"{Server.MapPath(@"\")}Images\\Uploads\\Products" +
                        $"\\{id.ToString()}\\Gallery\\{file.FileName}")).ToString();
                    var savePathMini = new DirectoryInfo(String.Format($"{Server.MapPath(@"\")}Images\\Uploads\\Products" +
                        $"\\{id.ToString()}\\Gallery\\Thumbs\\{file.FileName}")).ToString();

                    file.SaveAs(savePathFull);
                    WebImage image = new WebImage(file.InputStream);
                    image.Resize(200, 200).Crop(1, 1);
                    image.Save(savePathMini);
                }
            }
        }


        // Post: Admin/Shop/DeleteImage/id/imageName      
        public void DeleteImage(int id, string imageName)
        {
            string fullPath = Request.MapPath("~/Images/Uploads/Products/" + id.ToString() + "/Gallery/" + imageName);
            string fullPathnMini = Request.MapPath("~/Images/Uploads/Products/" + id.ToString() + "/Gallery/Thumbs/" + imageName);

            if (System.IO.File.Exists(fullPath))
            {
                try
                {
                    System.IO.File.Delete(fullPath);
                }
                catch (Exception ex)
                {
                    TempData["AM"] = ex.Message;
                }
            }
            if (System.IO.File.Exists(fullPathnMini))
            {
                try
                {
                    System.IO.File.Delete(fullPathnMini);
                }
                catch (Exception ex)
                {
                    TempData["AM"] = ex.Message;
                }
            }

        }

        /**************************************************************************************************/
        //                                Orders for admin functionality                          
        /**************************************************************************************************/

        // GET: Admin/Shop/Orders
        [HttpGet]
        public ActionResult Orders()
        {
            List<OrdersForAdminVM> ordersForAdmin = new List<OrdersForAdminVM>();
            List<OrderVM> orders = _repository.OrdersRepository.GetAll().Select(x => new OrderVM(x)).ToList();
            foreach (var order in orders)
            {

                Dictionary<string, int> productsAndQuantity = new Dictionary<string, int>();
                decimal total = 0m;
                List<OrderDetailsDTO> orderDetailsList = _repository.OrderDetailsRepository
                                                        .GetAll(filter: x => x.OrderId == order.Id);

                UserDTO userDTO = _repository.UsersRepository.GetOne(x => x.Id == order.UserId);
                string userName = userDTO.UserName;

                foreach (var orderDetails in orderDetailsList)
                {
                    ProductDTO productDTO = _repository.ProductsRepository.GetOne(x => x.Id == orderDetails.ProductId);
                    decimal price = productDTO.Price;
                    string productName = productDTO.Name;
                    productsAndQuantity.Add(productName, orderDetails.Quantity);
                    total += orderDetails.Quantity * price;
                }

                ordersForAdmin.Add(new OrdersForAdminVM()
                {
                    OrderNumber = order.Id,
                    UserName = userName,
                    Total = total,
                    ProductsAndQuantity = productsAndQuantity,
                    CreatedAt = order.CreatedAt
                });
            }
            return View(ordersForAdmin);
        }
    }
}