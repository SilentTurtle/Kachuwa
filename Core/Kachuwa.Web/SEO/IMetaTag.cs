using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Kachuwa.Web
{
    public interface IMetaTag
    {
        ConcurrentDictionary<string, string> MetaKeyValues { get; set; }

        string Generate();

    }
}