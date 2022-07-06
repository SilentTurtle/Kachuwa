using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kachuwa.Data.Crud.Attribute;

namespace Kachuwa.Web
{
    [Table("EmailServiceProvider")]
    public class EmailServiceProvider
    {
        [Key]
        public int EmailServiceProviderId { get; set; }
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public string Image { get; set; }

        public bool IsDefault { get; set; }

        public bool IsActive { get; set; }
        [AutoFill(AutoFillProperty.CurrentDate)]
        public DateTime AddedOn { get; set; }
        [AutoFill(AutoFillProperty.CurrentUser)]
        public string AddedBy { get; set; }
        [IgnoreAll]
        public int RowTotal { get; set; }
    }
}