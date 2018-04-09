using System.Collections.Generic;

namespace Kachuwa.Web
{
    public interface IMetaTag
    {
        Dictionary<string, string> MetaKeyValues { get; set; }

        string Generate();

    }
}