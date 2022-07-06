using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kachuwa.Web
{
    public class MetaTag : IMetaTag
    {
        public ConcurrentDictionary<string, string> MetaKeyValues { get; set; }

        public MetaTag(ConcurrentDictionary<string, string> metaKeyValues)
        {
            MetaKeyValues = metaKeyValues;
        }

        public string Generate()
        {
            StringBuilder tagBuilder = new StringBuilder();

            foreach (var keyvalue in MetaKeyValues)
            {
                tagBuilder.AppendFormat("<meta name=\"{0}\" content=\"{1}\" />", keyvalue.Key, keyvalue.Value);

            }
            return tagBuilder.ToString();
        }
    }
}
