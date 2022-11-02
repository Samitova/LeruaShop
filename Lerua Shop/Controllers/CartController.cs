using Lerua_Shop.Models.Data.Repository;
using Lerua_Shop.Models.ModelsDTO;
using Lerua_Shop.Models.ViewModels.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lerua_Shop.Controllers
{
    public class CartController : Controller
    {
        private readonly GeneralRepository _repository = GeneralRepository.GetInstance();

        // GET: Cart
        public ActionResult Index()
        {
            var cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();
            if (cart.Count == 0 || Session["cart"] == null)
            {
                ViewBag.Message = "Your cart is empty";
                return View();
            }

            decimal total = 0m;
            foreach (var item in cart)
            {
                total += item.Total;
            }

            ViewBag.GrandTotal = total;

            return View(cart);
        }

        // GET Cart/CartPartial
        public ActionResult CartPartial()
        {
            CartVM model = new CartVM();
            decimal price = 0m;
            int quantity = 0;

            if (Session["cart"] != null)
            {
                var cart = (List<CartVM>)Session["cart"];
                foreach (var item in cart)
                {
                    quantity += item.Quantity;
                    price += item.Price * item.Quantity;
                }

                model.Price = price;
                model.Quantity = quantity;
            }
            else
            {
                model.Price = 0m;
                model.Quantity = 0;
            }

            return PartialView("_CartPartial", model);
        }

        // GET Cart/AddToCartPartial/id
        public ActionResult AddToCartPartial(int id)
        {
            var cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();
            ProductDTO productDTO = _repository.ProductsRepository.GetOne(id);
            CartVM model = new CartVM();

            var productInCart = cart.FirstOrDefault(x => x.ProductId == id);

            if (productInCart == null)
            {
                cart.Add(new CartVM
                {
                    ProductId = productDTO.Id,
                    ProductName = productDTO.Name,
                    Quantity = 1,
                    Price = productDTO.Price,
                    Image = productDTO.ImageName
                });
            }
            else
            {
                productInCart.Quantity++;
            }

            decimal price = 0m;
            int quantity = 0;

            foreach (var item in cart)
            {
                quantity += item.Quantity;
                price += item.Price * item.Quantity;
            }

            model.Quantity = quantity;
            model.Price = price;

            Session["cart"] = cart;

            return PartialView("_AddToCartPartial", model);
        }

        // GET: Cart/IncrementProduct       
        public JsonResult IncrementProduct(int productId)
        {
            var cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

            model.Quantity++;

            var result = new { qty = model.Quantity, price = model.Price };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // GET: Cart/DecrementProduct       
        public JsonResult DecrementProduct(int productId)
        {
            var cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

            if (model.Quantity > 1)
            {
                model.Quantity--;
            }
            else
            {
                model.Quantity = 0;
                cart.Remove(model);
            }

            var result = new { qty = model.Quantity, price = model.Price };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // GET: Cart/RemoveProduct       
        public void RemoveProduct(int productId)
        {
            var cart = Session["cart"] as List<CartVM>;

            CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

            cart.Remove(model);
        }

    }
}