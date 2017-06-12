using System.Collections.Generic;

namespace Kachuwa.Web.Rule
{
    public class KachuwaRule
    {
        public KachuwaRule()
        {
            Inputs = new List<object>();
        }
        public string MemberName { get; set; }
        public string Operator { get; set; }
        public string TargetValue { get; set; }
        public List<KachuwaRule> Rules { get; set; }
        public List<object> Inputs { get; set; }
    }
}