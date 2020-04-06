using System.Collections.Generic;

namespace Kachuwa.Web.Printing
{
    public class PrinterContentProvider : IPrinterContentProvider
    {
        private readonly IEnumerable<IPrinter> _printers;

        public PrinterContentProvider(IEnumerable<IPrinter> printers)
        {
            _printers = printers;
        }
        public IList<string> GetContent(IPrinter printer)
        {
            throw new System.NotImplementedException();
        }
    }
}