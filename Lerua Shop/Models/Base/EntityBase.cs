using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Lerua_Shop.Models.Base
{
    public abstract class EntityBase
    {
        [Key]
        public virtual int Id { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }
    }
}