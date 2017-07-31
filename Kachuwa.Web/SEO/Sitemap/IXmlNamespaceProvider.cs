using System.Collections.Generic;

namespace Kachuwa.Web
{
    public interface IXmlNamespaceProvider
    {
        IEnumerable<string> GetNamespaces();
    }
}