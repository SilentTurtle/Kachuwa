using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kachuwa.Data.Crud.Attribute;
using Kachuwa.Data.Crud.FormBuilder;

namespace Kachuwa.Web.Rule
{
    [Table("Rule")]
    public class Rule
    {
        [Key]
        public int RuleId { get; set; }
        [System.ComponentModel.DataAnnotations.Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public int ActivateRuleOn { get; set; }
        [System.ComponentModel.DataAnnotations.Required]
        public string RuleAction { get; set; }
        [System.ComponentModel.DataAnnotations.Required]
        public string Value { get; set; }

        public bool AllowNextRule { get; set; }

        public DateTime ActiveFrom { get; set; }

        public DateTime ActiveTill { get; set; }

        public bool IsActive { get; set; }

        [AutoFill(false)]
        [IgnoreUpdate]
        public bool IsDeleted { get; set; }

        [AutoFill(AutoFillProperty.CurrentDate)]
        [IgnoreUpdate]
        public DateTime AddedOn { get; set; }

        [AutoFill(AutoFillProperty.CurrentUser)]
        [IgnoreUpdate]
        public string AddedBy { get; set; }

        [IgnoreAll]
        public int RowTotal { get; set; }
    }
}