using Lerua_Shop.Models.Base;
using Lerua_Shop.Models.ModelsDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lerua_Shop.Models.ViewModels.Shop
{
    public class OrderVM : EntityBase
    {
        public OrderVM()
        { }

        public OrderVM(OrderDTO order)
        {
            Id = order.Id;
            UserId = order.UserId;
            CreatedAt = order.CreatedAt;
            Timestamp = order.Timestamp;
        }

        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}