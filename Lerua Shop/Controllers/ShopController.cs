using Lerua_Shop.Models.Data.Repository;
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
    }
}