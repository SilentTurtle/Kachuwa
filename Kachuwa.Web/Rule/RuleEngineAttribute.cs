using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Kachuwa.Web.Rule
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class RuleEngineAttribute : Attribute,IActionFilter
    {
        public RuleEngineAttribute(string ruleName)
        {
            RuleName = ruleName;
        }

        public string RuleName { get; set; }

        public bool IsDefault { get; set; }

        public bool NoCustomRuleCode { get; set; }
        public void OnActionExecuting(ActionExecutingContext context)
        {
            throw new NotImplementedException();
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
          
            throw new NotImplementedException();
        }
    }
}