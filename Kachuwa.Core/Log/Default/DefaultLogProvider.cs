using System.Diagnostics;

namespace Kachuwa.Log
{
    /// <summary>
    /// Default Implementation of ILogProvider
    /// </summary>
    public class DefaultLogProvider : ILogProvider
    {
        public DefaultLogProvider()
        {
           
        }
        public ILogger GetLogger(string name)
        {
            return new FileBaseLogger();
        }
       
    }
}