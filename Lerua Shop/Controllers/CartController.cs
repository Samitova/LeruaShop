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
            return View();
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
    }
}