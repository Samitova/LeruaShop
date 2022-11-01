using Lerua_Shop.Models.Base;
using Lerua_Shop.Models.ModelsDTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Lerua_Shop.Models.ViewModels.Shop
{   
    public class CategoryVM : EntityBase
    {
        public CategoryVM()
        { }
        public CategoryVM(CategoryDTO category)
        {
            Id = category.Id;
            Name = category.Name;
            Slug = category.Slug;
            Sorting = category.Sorting;
            Timestamp = category.Timestamp;
        }

        [Required]
        public string Name { get; set; }
        public string Slug { get; set; }
        public int Sorting { get; set; }

    }
}
