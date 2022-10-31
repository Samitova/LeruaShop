using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lerua_Shop.Models.ViewModels
{
    public  interface IGetDTO<T> where T : class
    {
        T GetDTO();
    }
}
