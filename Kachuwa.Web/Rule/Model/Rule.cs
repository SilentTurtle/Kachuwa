using System;
using Kachuwa.Data.Crud.Attribute;
using Kachuwa.Data.Crud.FormBuilder;

namespace Kachuwa.Web.Rule
{
    [Table("Rule")]
    public class Rule
    {
        [Key]
        public int RuleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ActivateRuleOn { get; set; }
        public string Value { get; set; }
        public bool AllowNextRule { get; set; }
        public DateTime ActiveFrom { get; set; }
        public DateTime ActiveTill { get; set; }
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