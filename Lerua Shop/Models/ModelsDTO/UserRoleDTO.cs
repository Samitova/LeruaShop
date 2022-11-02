using Lerua_Shop.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Lerua_Shop.Models.ModelsDTO
{
    [Table("tblUserRoles")]
    public class UserRoleDTO : EntityBase
    {
        [Key, Column(name: "UserId", Order = 0)]
        public override int Id { get; set; }

        [Key, Column(Order = 1)]
        public int RoleId { get; set; }

        [ForeignKey(nameof(Id))]
        public virtual UserDTO User { get; set; }
        [ForeignKey("RoleId")]
        public virtual RoleDTO Role { get; set; }
    }
}