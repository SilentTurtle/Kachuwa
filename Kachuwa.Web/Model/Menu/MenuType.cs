using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Kachuwa.Data.Crud.Attribute;
using Kachuwa.Data.Crud.FormBuilder;

namespace Kachuwa.Web.Model
{
   [Table("MenuType")]
    public class MenuType
    {
        [Data.Crud.Attribute.Key]
        public int MenuTypeId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }
        [AutoFill(false)]
        public bool IsDeleted { get; set; }
        [AutoFill(IsDate = true)]
        [IgnoreUpdate]
        public DateTime AddedOn { get; set; }
        [AutoFill(GetCurrentUser = true)]
        [IgnoreUpdate]
        public string AddedBy { get; set; }
        [IgnoreAll]
        public int RowTotal { get; set; }
    }
}
