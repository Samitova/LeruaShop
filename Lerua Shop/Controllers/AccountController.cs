using Lerua_Shop.Models.Data.Repository;
using Lerua_Shop.Models.ModelsDTO;
using Lerua_Shop.Models.ViewModels.Account;
using Lerua_Shop.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

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

        // GET: Account/Login  
        public ActionResult Login()
        {
            // check authorization
            string userNmae = User.Identity.Name;
            if (!string.IsNullOrEmpty(userNmae))
            {
                return RedirectToAction("user-profile");
            }
            return View();
        }

        // POST: Account/Login    
        [HttpPost]
        public ActionResult Login(LoginUserVM model)
        {
            if (!ModelState.IsValid)
            {
                return View("Login", model);
            }
            // check user
            bool isValidUser = false;

            if (_repository.UsersRepository.Any(filter: x => x.UserName.Equals(model.Username)
                && x.Password.Equals(model.Password)))
            {
                isValidUser = true;
            }
            if (!isValidUser)
            {
                ModelState.AddModelError("", "Invalid username or pasword");
                return View("Login", model);
            }
            else
            {
                FormsAuthentication.SetAuthCookie(model.Username, model.RememberMe);
                return Redirect(FormsAuthentication.GetRedirectUrl(model.Username, model.RememberMe));
            }
        }

        // GET: Account/Logout
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        // GET: Account/UserNavPartial       
        public ActionResult UserNavPartial()
        {
            UserDTO userDTO = _repository.UsersRepository.GetOne(x => x.UserName == User.Identity.Name);

            UserNavPartialVM model = new UserNavPartialVM(userDTO);

            return PartialView("_UserNavPartial", model);
        }

        // GET: Account/user-profile       
        [ActionName("user-profile")]
        [HttpGet]
        public ActionResult UserProfile()
        {
            UserDTO userDTO = _repository.UsersRepository.GetOne(x => x.UserName == User.Identity.Name);
            UserProfileVM model = new UserProfileVM(userDTO);

            return View("UserProfile", model);
        }

        // Post: Account/user-profile       
        [ActionName("user-profile")]
        [HttpPost]
        public ActionResult UserProfile(UserProfileVM model)
        {
            bool userNameIsChanged = false;
            if (!ModelState.IsValid)
            {
                return View("UserProfile", model);
            }

            string userName = User.Identity.Name;
            if (userName != model.UserName)
            {
                userNameIsChanged = true;
                userName = model.UserName;
            }

            if (_repository.UsersRepository.GetAll(x => x.Id != model.Id).Any(y => y.UserName == userName))
            {
                ModelState.AddModelError("", $"Username {User.Identity.Name} is already existed");
                model.UserName = "";
                return View("UserProfile", model);
            }

            UserDTO userDTO = _repository.UsersRepository.GetOne(model.Id);
            userDTO.FirstName = model.FirstName;
            userDTO.LastName = model.LastName;
            userDTO.Email = model.Email;
            userDTO.Adress = model.Adress;
            userDTO.UserName = model.UserName;
            userDTO.TelefonNumber = model.TelefonNumber;
            if (!string.IsNullOrEmpty(model.Password))
            {
                userDTO.Password = model.Password;
            }
            _repository.UsersRepository.SaveChanges();

            TempData["AM"] = "You have edited your profile";

            if (!userNameIsChanged)
            {
                return View("UserProfile", model);
            }
            else
            {
                return RedirectToAction("Logout");
            }
        }


        // GET: Account/Orders       
        [HttpGet]
        public ActionResult Orders(UserProfileVM model)
        {
            List<OrdersForUserVM> ordersForUser = new List<OrdersForUserVM>();
            UserDTO userDTO = _repository.UsersRepository.GetOne(x => x.UserName == User.Identity.Name);

            int userId = userDTO.Id;

            List<OrderVM> orders = _repository.OrdersRepository.GetAll(filter: x => x.UserId == userId)
                                   .Select(x => new OrderVM(x)).ToList();

            foreach (var order in orders)
            {

                Dictionary<string, int> productsAndQuantity = new Dictionary<string, int>();
                decimal total = 0m;
                List<OrderDetailsDTO> orderDetailsList = _repository.OrderDetailsRepository
                                                        .GetAll(filter: x => x.OrderId == order.Id);

                foreach (var orderDetails in orderDetailsList)
                {
                    ProductDTO productDTO = _repository.ProductsRepository.GetOne(x => x.Id == orderDetails.ProductId);
                    decimal price = productDTO.Price;
                    string productName = productDTO.Name;
                    productsAndQuantity.Add(productName, orderDetails.Quantity);
                    total += orderDetails.Quantity * price;
                }

                ordersForUser.Add(new OrdersForUserVM()
                {
                    OrderNumber = order.Id,
                    Total = total,
                    ProductsAndQuantity = productsAndQuantity,
                    CreatedAt = order.CreatedAt
                });
            }
            return View(ordersForUser);
        }
    }
}