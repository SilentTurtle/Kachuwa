using System.Collections.Generic;

namespace Kachuwa.Web.Printing
{
    public interface IPrinterContentProvider
    {
        IList<string> GetContent(IPrinter printer);
    }
}