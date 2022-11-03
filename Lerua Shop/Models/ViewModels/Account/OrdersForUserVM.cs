using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Lerua_Shop.Models.ViewModels.Account
{
    public class OrdersForUserVM
    {
        [DisplayName("Order ID")]
        public int OrderNumber { get; set; }

        public decimal Total { get; set; }
        public Dictionary<string, int> ProductsAndQuantity { get; set; }

        [DisplayName("Created At")]
        public DateTime CreatedAt { get; set; }
    }
}