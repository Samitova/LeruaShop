using Lerua_Shop.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Lerua_Shop.Models.ModelsDTO
{
    [Table("tblCategories")]
    public class CategoryDTO : EntityBase
    {
        public string Name { get; set; }
        public string Slug { get; set; }
        public int Sorting { get; set; }
    }
}