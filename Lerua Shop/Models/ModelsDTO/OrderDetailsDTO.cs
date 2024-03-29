﻿using Lerua_Shop.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Lerua_Shop.Models.ModelsDTO
{
    [Table("tblOrderDetails")]
    public class OrderDetailsDTO : EntityBase
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        [ForeignKey("OrderId")]
        public virtual OrderDTO Orders { get; set; }

        [ForeignKey("UserId")]
        public virtual UserDTO Users { get; set; }

        [ForeignKey("ProductId")]
        public virtual ProductDTO Products { get; set; }
    }
}