using System.Collections.Generic;

namespace Kachuwa.Security
{
    public class SecurityHeadersPolicy
    {
        public IDictionary<string, string> SetHeaders { get; }
            = new Dictionary<string, string>();

        public ISet<string> RemoveHeaders { get; }
            = new HashSet<string>();

        public bool AddNonce { get; set; } = false;
        public ContentSecurityPolicyBuiilder CspBuiilder { get; set; }
    }
   
}