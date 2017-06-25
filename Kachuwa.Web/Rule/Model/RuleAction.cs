using System;
using Kachuwa.Data.Crud.Attribute;
using Kachuwa.Data.Crud.FormBuilder;

namespace Kachuwa.Web.Rule
{
    [Table("RuleAction")]
    public class RuleAction
    {
        [Kachuwa.Data.Crud.Attribute.Key]
        public int RuleActionId { get; set; }
        [System.ComponentModel.DataAnnotations.Required]
        public string Name { get; set; }

        public string Description { get; set; }
        [System.ComponentModel.DataAnnotations.Required]
        public string ApplyOn { get; set; }

        public bool IsActive { get; set; }

        [IgnoreUpdate]
        [AutoFill(false)]
        public bool IsDeleted { get; set; }
        [IgnoreUpdate]
        [AutoFill(IsDate = true)]
        public DateTime AddedOn { get; set; }
        [IgnoreUpdate]
        [AutoFill(GetCurrentUser = true)]
        public string AddedBy { get; set; }
        [IgnoreAll]
        public int RowTotal { get; set; }
    }
}