using System.Diagnostics;

namespace Kachuwa.Logger
{
    /// <summary>
    /// Default Implementation of ILogProvider
    /// </summary>
    public class DefaultLogProvider : ILogProvider
    {
        public DefaultLogProvider()
        {
           
        }
        public ILog GetLogger(string name)
        {
            return new FileBaseLogger();
        }

        //private class DefaultLogger : ILog
        //{

        //    public bool Log(LogType logtype, Func<string> messageFunc, Exception exception)
        //    {
        //        if (messageFunc == null)
        //        {
        //            return logtype >= LogType.Info;
        //        }

        //        StringLogger.Log(messageFunc());
        //        return true;
        //    }
        //}
    }
}