using Lerua_Shop.Models.ModelsDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lerua_Shop.Models.ViewModels.Account
{
    public class UserNavPartialVM
    {
        public UserNavPartialVM()
        { }
        public UserNavPartialVM(UserDTO name)
        {
            FirstName = name.FirstName;
            LastName = name.LastName;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}