using Lerua_Shop.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lerua_Shop.Models.ModelsDTO
{
    [Table("tblSidebar")]
    public class SidebarDTO : EntityBase
    {
        [AllowHtml]
        public string Body { get; set; }
    }
}