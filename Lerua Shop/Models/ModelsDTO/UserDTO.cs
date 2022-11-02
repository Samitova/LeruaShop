using Lerua_Shop.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Lerua_Shop.Models.ModelsDTO
{
    [Table("tblUsers")]
    public class UserDTO : EntityBase
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string TelefonNumber { get; set; }
        public string Adress { get; set; }

    }
}