using Lerua_Shop.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Lerua_Shop.Models.ModelsDTO
{
    [Table("tblProducts")]
    public class ProductDTO : EntityBase
    {
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public string Brand { get; set; }
        public string CategoryName { get; set; }
        public int CategoryId { get; set; } 
        public decimal Price { get; set; }
        public int Amount { get; set; } = 1;
        public string ImageName { get; set; }

        [ForeignKey("CategoryId")]
        public virtual CategoryDTO Category { get; set; }

    }
}