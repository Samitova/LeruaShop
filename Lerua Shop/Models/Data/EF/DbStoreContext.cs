using Lerua_Shop.Models.ModelsDTO;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Lerua_Shop.Models.Data.EF
{
    public class DbStoreContext : DbContext
    {
        public DbSet<PageDTO> Pages { get; set; }

        public DbStoreContext() : base("name=DbConnection")
        { }
    }
}