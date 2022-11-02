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
    }
}