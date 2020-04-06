using System;
using System.Collections.Generic;
using System.Text;

namespace Kachuwa.Web.Printing
{
    public class DefaultPrinter : IPrinter
    {
        public string Name { get; set; }
        public void Print(string ipAddress, int port, IList<string> linesToPrint)
        {
            throw new NotImplementedException();
        }
    }
}
