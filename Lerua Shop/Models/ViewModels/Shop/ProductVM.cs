using Lerua_Shop.Models.Base;
using Lerua_Shop.Models.ModelsDTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lerua_Shop.Models.ViewModels.Shop
{
    public class ProductVM : EntityBase, IGetDTO<ProductDTO>
    {
        public ProductVM()
        { }
        public ProductVM(ProductDTO product)
        {
            Id = product.Id;
            Name = product.Name;
            Slug = product.Slug;
            Description = product.Description;
            Brand = product.Brand;
            CategoryName = product.CategoryName;
            CategoryId = product.CategoryId;
            Price = product.Price;
            Avaliability = product.Avaliability;
            Amount = product.Amount;
            ImageName = product.ImageName;
            Timestamp = product.Timestamp;
        }

        [Required]
        public string Name { get; set; }
        public string Slug { get; set; }

        [Required]
        public string Description { get; set; }
        public string Brand { get; set; }
        public string CategoryName { get; set; }

        [Required]
        [DisplayName("Category")]
        public int CategoryId { get; set; }
        public decimal Price { get; set; }
        public bool Avaliability { get; set; }
        public int Amount { get; set; }

        [DisplayName("Image")]
        public string ImageName { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }
        public IEnumerable<string> GaleryImage { get; set; }

        public ProductDTO GetDTO()
        {
            ProductDTO product = new ProductDTO();

            product.Id = this.Id;
            product.Name = this.Name;
            product.Slug = this.Name.Replace(" ", "-").ToLower();
            product.Description = this.Description;
            product.Brand = this.Brand;
            product.CategoryName = this.CategoryName;
            product.CategoryId = this.CategoryId;
            product.Price = this.Price;
            product.Avaliability = this.Avaliability;
            product.Amount = this.Amount;
            product.ImageName = this.ImageName;
            product.Timestamp = this.Timestamp;

            return product;
        }
    }
}