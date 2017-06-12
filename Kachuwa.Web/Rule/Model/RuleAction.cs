using System;
using Kachuwa.Data.Crud.Attribute;
using Kachuwa.Data.Crud.FormBuilder;

namespace Kachuwa.Web.Rule
{
    [Table("RuleAction")]
    public class RuleAction
    {
        [Key]
        public int RuleActionId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ApplyOn { get; set; }

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