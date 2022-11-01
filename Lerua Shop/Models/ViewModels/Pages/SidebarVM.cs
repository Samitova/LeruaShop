using Lerua_Shop.Models.Base;
using Lerua_Shop.Models.ModelsDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lerua_Shop.Models.ViewModels.Pages
{
    public class SidebarVM : EntityBase
    {
        public string Body { get; set; }
        public SidebarVM()
        { }

        public SidebarVM(SidebarDTO dto)
        {
            Id = dto.Id;
            Body = dto.Body;
            Timestamp = dto.Timestamp;
        }

    }
}