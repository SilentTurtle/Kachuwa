using System;
using System.Collections.Generic;
using System.Text;

namespace Kachuwa.Web.Rule
{
    public interface IRuleEngine
    {
        string Name { get; set; }
        string Version { get; set; }
        bool Validate(Rule rule);

    }

    public class Rule
    {
        
    }
    
    public class RuleEngineAttribute : Attribute
    {
        public RuleEngineAttribute(string ruleName)
        {
            RuleName = ruleName;
        }

        public string RuleName { get; set; }

        public bool IsDefault { get; set; }

        public bool NoCustomRuleCode { get; set; }
    }

    public class TicketRuleEngine: IRuleEngine
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public bool Validate(Rule rule)
        {
            throw new NotImplementedException();
        }
    }
}
