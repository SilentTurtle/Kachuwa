using Kachuwa.Data.Crud.Attribute;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kachuwa.Web.Rule
{
    [Table("RuleCondition")]
    public class RuleCondition
    {
        public RuleCondition()
        {
            Inputs = new List<object>();
        }
        [Key]
        public int RuleConditionId { get; set; }

        public int RuleId { get; set; }
        public int ParentRuleConditionId { get; set; }
        public string RuleOperator { get; set; }
        public string MemberValue { get; set; }
        public string TargetValue { get; set; }
        public string Operator { get; set; }

        /// <summary>
        /// nested Rules
        /// </summary>
        [IgnoreAll]
        public List<RuleCondition> Conditions { get; set; }

        /// <summary>
        /// used for dynamic object for reflections
        /// </summary>
        [IgnoreAll]
        public List<object> Inputs { get; set; }
    }
}