using System;

namespace Kachuwa.Log
{
    /// <summary>
    /// Simple interface that represent a logger.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Log a message the specified log level.
        /// </summary>
        /// <param name="logtype"></param>
        /// <param name="messageFunc">The message function.</param>
        /// <param name="obj">An optional exception.</param>
        /// <returns>true if the message was logged. Otherwise false.</returns>
        /// <remarks>
        /// Note to implementers: the message func should not be called if the loglevel is not enabled
        /// so as not to incur performance penalties.
        /// 
        /// To check IsEnabled call Log with only LogLevel and check the return value, no event will be written
        /// </remarks>
        bool Log(LogType logtype, Func<string> messageFunc, object obj = null);
    }
}
