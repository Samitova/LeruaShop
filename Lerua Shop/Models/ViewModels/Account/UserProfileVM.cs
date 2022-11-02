using Lerua_Shop.Models.Base;
using Lerua_Shop.Models.ModelsDTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Lerua_Shop.Models.ViewModels.Account
{
    public class UserProfileVM : EntityBase, IGetDTO<UserDTO>
    {
        public UserProfileVM()
        { }
        public UserProfileVM(UserDTO user)
        {
            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            UserName = user.UserName;
            Password = user.Password;
            Email = user.Email;
            TelefonNumber = user.TelefonNumber;
            Adress = user.Adress;
            Timestamp = user.Timestamp;
        }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }


        [DataType(DataType.Password)]
        public string Password { get; set; }


        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Passwords are not the same")]
        [DataType(DataType.Password)]

        public string ConfirmPassword { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Telefon Number")]
        public string TelefonNumber { get; set; }
        [Required]
        public string Adress { get; set; }


        public UserDTO GetDTO()
        {
            UserDTO user = new UserDTO();

            user.Id = Id;
            user.FirstName = FirstName;
            user.LastName = LastName;
            user.UserName = UserName;
            user.Password = Password;
            user.Email = Email;
            user.Timestamp = Timestamp;
            user.TelefonNumber = TelefonNumber;
            user.Adress = Adress;

            return user;
        }
    }
}