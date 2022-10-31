using Lerua_Shop.Models.Base;
using Lerua_Shop.Models.ModelsDTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace Lerua_Shop.Models.ViewModels.Pages
{
    public class PageVM : EntityBase
    {
        public PageVM()
        { }
        public PageVM(PageDTO pages)
        {
            Id = pages.Id;
            Title = pages.Title;
            Slug = pages.Slug.Replace(" ", "-").ToLower();
            Body = pages.Body;
            Sorting = pages.Sorting;
            HasSidebar = pages.HasSidebar;
            Timestamp = pages.Timestamp;
        }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Title { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Slug { get; set; }
        [Required]
        [StringLength(int.MaxValue, MinimumLength = 3)]
        [AllowHtml]
        public string Body { get; set; }
        public int Sorting { get; set; }
        [Display(Name = "Sidebar")]
        public bool HasSidebar { get; set; }
    }
}