using Kachuwa.Data;

namespace Kachuwa.Web.Rule
{
    public class RuleService: IRuleService
    {
        public CrudService<Rule> RuleCrudService { get; set; }=new CrudService<Rule>();
        public CrudService<RuleAction> ActionService { get; set; }=new CrudService<RuleAction>();
        public CrudService<RuleCondition> ConditionService { get; set; }=new CrudService<RuleCondition>();
    }
}