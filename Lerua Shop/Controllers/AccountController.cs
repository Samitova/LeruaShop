using Lerua_Shop.Models.Data.Repository;
using Lerua_Shop.Models.ModelsDTO;
using Lerua_Shop.Models.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lerua_Shop.Controllers
{
    public class AccountController : Controller
    {
        private readonly GeneralRepository _repository = GeneralRepository.GetInstance();

        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        // GET: account/create-account
        [ActionName("create-account")]
        public ActionResult CreateAccount()
        {
            return View("CreateAccount");
        }

        // POST: account/create-account
        [ActionName("create-account")]
        [HttpPost]
        public ActionResult CreateAccount(UserVM model)
        {
            if (!ModelState.IsValid)
            {
                return View("CreateAccount", model);
            }

            if (_repository.UsersRepository.Any(filter: x => x.UserName.Equals(model.UserName)))
            {
                ModelState.AddModelError("", $"Username {model.UserName} is already taken");
                model.UserName = "";
                return View("CreateAccount", model);
            }

            UserDTO userDTO = model.GetDTO();
            try
            {
                _repository.UsersRepository.Add(userDTO);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("CreateAccount", model);
            }

            UserRoleDTO userRoleDTO = new UserRoleDTO()
            {
                Id = userDTO.Id,
                RoleId = 2
            };

            try
            {
                _repository.UserRolesRepository.Add(userRoleDTO);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("CreateAccount", model);
            }

            TempData["AM"] = "You are now registered and can login";

            return RedirectToAction("Login");
        }

    }
}