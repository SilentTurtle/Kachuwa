using System.Collections.Generic;
using System.Text;

namespace Kachuwa.Web
{
    public class OgMetaTag : IMetaTag
    {
        public Dictionary<string, string> MetaKeyValues { get; set; }
        private string FbAppId { get; set; }

        public OgMetaTag(Dictionary<string, string> metaKeyValues,string fbId=null)
        {
            MetaKeyValues = metaKeyValues;
            FbAppId = fbId;
        }

        public string Generate()
        {
            StringBuilder tagBuilder = new StringBuilder();
            if (!string.IsNullOrEmpty(FbAppId))
            {
                tagBuilder.AppendFormat("<meta  property=\"fb:app_id\"content=\"{0}\"  />", FbAppId);

            }
            foreach (var keyvalue in MetaKeyValues)
            {
                tagBuilder.AppendFormat("<meta  property=\"og:{0}\" content=\"{1}\"  />", keyvalue.Key, keyvalue.Value);

            }
            return tagBuilder.ToString();
        }
    }
}