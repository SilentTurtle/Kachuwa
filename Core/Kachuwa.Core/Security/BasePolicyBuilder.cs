using System;
using System.Collections.Generic;

namespace Kachuwa.Security
{
    public abstract class BasePolicyBuilder
    {
        private string Key { get; }
        public bool SupportNonce { get; set; } = false;

        protected BasePolicyBuilder(string key)
        {
            Key = key;
        }
        public IList<string> PolicyValues = new List<string>();
        public BasePolicyBuilder AllowSelf()
        {
            PolicyValues.Add("'self'");
            return this;
        }
        public BasePolicyBuilder AllowAll()
        {
            PolicyValues.Add("'*'");
            return this;
        }
        public BasePolicyBuilder AllowfromCdn(string[] cdnUrls)
        {
            PolicyValues.Add("'" + String.Join(",", cdnUrls) + "'");
            return this;
        }

        public BasePolicyBuilder AddNonce()
        {
            SupportNonce = true;
            return this;

        }
        public BasePolicyBuilder None()
        {
            PolicyValues.Add("'none'");
            return this;

        }
        public BasePolicyBuilder AllowInline()
        {
            PolicyValues.Add("'unsafe-inline'");
            return this;
        }
        public BasePolicyBuilder AllowUnsafeEval()
        {
            PolicyValues.Add("'unsafe-inline'");
            return this;
        }

        public BasePolicyBuilder FrameSameOrigin()
        {
            PolicyValues.Add("'sameorigin'");
            return this;
        }
        public BasePolicyBuilder FrameDeny()
        {
            PolicyValues.Add("'deny'");
            return this;
        }


        public string[] Build()
        {
            return new string[]{Key,String.Join(" ", PolicyValues) + ";"};
        }
    }
}