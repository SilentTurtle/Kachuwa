using System;
using System.Collections.Generic;
using System.Text;

namespace Kachuwa.Web.Rule
{
    public interface IRuleActions<T>
    {
        string Name { get; }
        T OnModel { get; set; }

    }

    public class OrderRuleActions
    {
        
    }
    public interface IRuleEngine
    {
        string Name { get; set; }
        string Version { get; set; }
        Func<T, bool> CompileRule<T>(RuleCondition r);
        Func<T, bool> CompileRules<T>(IList<RuleCondition> rules);

    }
}
