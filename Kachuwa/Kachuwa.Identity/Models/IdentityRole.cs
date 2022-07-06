using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kachuwa.Data.Crud.Attribute;

namespace Kachuwa.Identity.Models
{
    [Table("TemporaryRoleDetail")]
    public class TemporaryRoleDetail
    {

        [Key]
        public long Id { get; set; }
        public long RoleId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Narration { get; set; }
    }

    [Table("IdentityRole")]
    public class IdentityRole : KachuwaIdentityRole
    {
        [Key]
        public override int Id { get; set; }

        // public string Name { get; set; }
        public bool IsTemporary { get; set; }
        public bool IsSystem { get; set; } = false;
        public bool IsActive { get; set; } = false;


        [AutoFill(AutoFillProperty.CurrentDate)]
        [IgnoreUpdate]
        public DateTime AddedOn { get; set; }

        [AutoFill(AutoFillProperty.CurrentUserId)]
        [IgnoreUpdate]
        public long AddedBy { get; set; }

        [AutoFill(false)]
        public bool IsDeleted { get; set; }

        [AutoFill(AutoFillProperty.CurrentDate)]

        [IgnoreAll]
        public DateTime DeletedOn { get; set; }

        [AutoFill(AutoFillProperty.CurrentUserId)]
        [IgnoreAll]
        public long DeletedBy { get; set; }
        [IgnoreInsert]
        [AutoFill(AutoFillProperty.CurrentDate)]
        public DateTime UpdatedOn { get; set; }

        [AutoFill(AutoFillProperty.CurrentUserId)]
        [IgnoreInsert]
        public long UpdatedBy { get; set; }

        [IgnoreAll]
        public int RowTotal { get; set; }

    }
}