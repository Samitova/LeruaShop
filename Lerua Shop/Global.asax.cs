using Lerua_Shop.Models.Data.Repository;
using Lerua_Shop.Models.ModelsDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Lerua_Shop
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_AuthenticateRequest()
        {
            GeneralRepository _repository = GeneralRepository.GetInstance();
            if (User == null)
                return;
            string userName = Context.User.Identity.Name;

            string[] roles = null;

            UserDTO userDTO = _repository.UsersRepository.GetOne(x => x.UserName == User.Identity.Name);
            if (userDTO == null)
                return;

            roles = _repository.UserRolesRepository.GetAll(filter: x => x.Id == userDTO.Id).Select(x => x.Role.Name).ToArray();

            IIdentity userIdentity = new GenericIdentity(userName);
            IPrincipal newUserObject = new GenericPrincipal(userIdentity, roles);

            Context.User = newUserObject;

        }
    }
}
