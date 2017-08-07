using System;
using System.Collections.Generic;
using System.Linq;

namespace Kachuwa.Security
{
    public class ContentSecurityPolicyBuiilder
    {
        private IDictionary<string,string> CspPolicies=new Dictionary<string, string>();
        public bool SupportNonce { get; set; } = false;


        public ContentSecurityPolicyBuiilder AddScriptPolicy()
        {
            return this;
        }
        public ContentSecurityPolicyBuiilder AddScriptPolicy(Action<ScriptPolicyBuilder> builder)
        {
            var scripPolicyBuilder = new ScriptPolicyBuilder();
            builder(scripPolicyBuilder);
            SupportNonce = scripPolicyBuilder.SupportNonce;
            var values= scripPolicyBuilder.Build();
            CspPolicies.Add(values[0], values[1]);
            return this;
        }
        public ContentSecurityPolicyBuiilder AddStylePolicy(Action<StylePolicyBuilder> builder)
        {
            var policyBuilder = new StylePolicyBuilder();
            builder(policyBuilder);
            SupportNonce = policyBuilder.SupportNonce;
            var values = policyBuilder.Build();
            CspPolicies.Add(values[0], values[1]);
            return this;
        }
        public ContentSecurityPolicyBuiilder AddObjectPolicy(Action<ObjectPolicyBuilder> builder)
        {
            var policyBuilder = new ObjectPolicyBuilder();
            builder(policyBuilder);
            var values = policyBuilder.Build();
            CspPolicies.Add(values[0], values[1]);
            return this;
        }
        public ContentSecurityPolicyBuiilder AddDefaultPolicy(Action<DefaultPolicyBuilder> builder)
        {
            var policyBuilder = new DefaultPolicyBuilder();
            builder(policyBuilder);
            var values = policyBuilder.Build();
            CspPolicies.Add(values[0], values[1]);
            return this;
        }
        public ContentSecurityPolicyBuiilder AddImagePolicy(Action<ImagePolicyBuilder> builder)
        {
            var policyBuilder = new ImagePolicyBuilder();
            builder(policyBuilder);
            var values = policyBuilder.Build();
            CspPolicies.Add(values[0], values[1]);
            return this;
        }
        public ContentSecurityPolicyBuiilder AddMediaPolicy(Action<MediaPolicyBuilder> builder)
        {
            var policyBuilder = new MediaPolicyBuilder();
            builder(policyBuilder);
            var values = policyBuilder.Build();
            CspPolicies.Add(values[0], values[1]);
            return this;
        }
        public ContentSecurityPolicyBuiilder AddFramePolicy(Action<FramePolicyBuilder> builder)
        {
            var policyBuilder = new FramePolicyBuilder();
            builder(policyBuilder);
            var values = policyBuilder.Build();
            CspPolicies.Add(values[0], values[1]);
            return this;
        }
        public ContentSecurityPolicyBuiilder AddConnectPolicy(Action<ConnectPolicyBuilder> builder)
        {
            var policyBuilder = new ConnectPolicyBuilder();
            builder(policyBuilder);
            var values = policyBuilder.Build();
            CspPolicies.Add(values[0], values[1]);
            return this;
        }
        public ContentSecurityPolicyBuiilder AddBaseUriPolicy(Action<BaseUriolicyBuilder> builder)
        {
            var policyBuilder = new BaseUriolicyBuilder();
            builder(policyBuilder);
            var values = policyBuilder.Build();
            CspPolicies.Add(values[0], values[1]);
            return this;
        }
        public ContentSecurityPolicyBuiilder AddFormActionPolicy(Action<FormActionPolicyBuilder> builder)
        {
            var policyBuilder = new FormActionPolicyBuilder();
            builder(policyBuilder);
            var values = policyBuilder.Build();
            CspPolicies.Add(values[0], values[1]);
            return this;
        }

        public string Build()
        {
            string cspContent = "";
            List<string> keyvalues = new List<string>();

          
            foreach (var key in CspPolicies.Keys)
            {
               
                    keyvalues.Add(key + " " + CspPolicies[key] + ";");
                
            }
            cspContent = String.Join(" ", keyvalues);
            return cspContent;
        }
        public string Build(ICspNonceService cspNonceService)
        {
            string cspContent = "";
            List<string> keyvalues = new List<string>();

            cspNonceService.GenerateNew();
            foreach (var key in CspPolicies.Keys)
            {
                if (key == "script-scr" || key == "style-src")
                {
                    keyvalues.Add(key + " "+ CspPolicies[key] +" 'nonce-" + cspNonceService.GetNonce() + "'" + ";");
                }
                else
                {
                    keyvalues.Add(key + " " + CspPolicies[key] + ";");
                }
            }
            cspContent = String.Join(" ", keyvalues);
            return cspContent;
        }

    }
}