using Kachuwa.Data;

namespace Kachuwa.Web.Rule
{
    public interface IRuleService
    {
        CrudService<Rule> RuleCrudService { get; set; }
        CrudService<RuleAction> ActionService { get; set; }
        CrudService<RuleCondition> ConditionService { get; set; }
    }
}