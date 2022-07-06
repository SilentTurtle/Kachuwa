using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kachuwa.Data.Crud.Attribute;

namespace Kachuwa.Web.Model
{
    [Table("AdministrativeIPAccess")]
    public class AdministrativeIPAccess
    {
        [Key]
        public int AdministrativeIPAccessId { get; set; }
        public long RoleId { get; set; }
        public bool AllowIPV4 { get; set; }
        public bool AllowIPV6 { get; set; }
        public bool ActivateIPV6 { get; set; }

        public bool IsRange { get; set; }
        public string IPV4Range { get; set; }
        public string IPV6Range { get; set; }
        public bool IsActive { get; set; }

        [AutoFill(false)]
        public bool IsDeleted { get; set; }

        [AutoFill(AutoFillProperty.CurrentDate)]
        [IgnoreUpdate]
        public DateTime AddedOn { get; set; }

        [AutoFill(AutoFillProperty.CurrentUserId)]
        [IgnoreUpdate]
        public long AddedBy { get; set; }


        [AutoFill(AutoFillProperty.CurrentUserId)]
        [IgnoreUpdate]
        public long DeletedBy { get; set; }
        [AutoFill(AutoFillProperty.CurrentDate)]
        [IgnoreInsert]
        public DateTime UpdatedOn { get; set; }
        [AutoFill(AutoFillProperty.CurrentDate)]
        [IgnoreInsert]
        public DateTime DeletedOn { get; set; }

        [AutoFill(AutoFillProperty.CurrentUserId)]
        [IgnoreInsert]
        public long UpdatedBy { get; set; }

        [IgnoreAll]
        public int RowTotal { get; set; }
    }
}
