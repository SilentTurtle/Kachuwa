using System.Collections.Generic;
using System.Text;

namespace Kachuwa.Web
{
    public class TwitterMetaTag : IMetaTag
    {
        public Dictionary<string, string> MetaKeyValues { get; set; }

        public TwitterMetaTag(Dictionary<string, string> metaKeyValues)
        {
            MetaKeyValues = metaKeyValues;
        }

        public string Generate()
        {
            StringBuilder tagBuilder = new StringBuilder();

            foreach (var keyvalue in MetaKeyValues)
            {
                tagBuilder.AppendFormat("<meta  name=\"twitter:{0}\" content=\"{1}\"  />", keyvalue.Key, keyvalue.Value);

            }
            return tagBuilder.ToString();
        }
    }
}