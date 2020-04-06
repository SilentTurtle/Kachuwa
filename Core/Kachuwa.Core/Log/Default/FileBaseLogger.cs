using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;

namespace Kachuwa.Log
{
    public class FileBaseLogger : ILogger
    {
        private readonly ILoggerSetting _loggerSetting;
        private static object _ratesFileLock = new object();
        private string Basedir { get; set; }
        private string _todaysDate { get; set; }
        public FileBaseLogger(IHostingEnvironment hostingEnvironment, ILoggerSetting loggerSetting)
        {
            _loggerSetting = loggerSetting;
            Basedir = hostingEnvironment.ContentRootPath + "\\Logs\\";
            _todaysDate = DateTime.Now.ToString("yyyy_MM_dd");

            if (!Directory.Exists(Basedir))
            {
                Directory.CreateDirectory(Basedir);
            }
            LogFilePath = Basedir + CustomFileName+_todaysDate + ".log";
            _init();
        }
        public bool CreateFile<T>()
        {
            var type = typeof(T);
            CustomFileName = type.FullName;
            LogFilePath = Basedir + CustomFileName+_todaysDate + ".log";
            return true;
        }

        private string CustomFileName { get; set; } = "";
        public bool CreateFile(string name)
        {
            CustomFileName = name;
            LogFilePath = Basedir + CustomFileName + _todaysDate + ".log";
            return true;
        }
        private string LogFilePath { get; set; }
        private List<Log> Logs { get; set; }
        private void _init()
        {
            _todaysDate = DateTime.Now.ToString("yyyy_MM_dd");
            if (!File.Exists(LogFilePath))
            {
                using (var stream = File.Create(LogFilePath, 1024, FileOptions.None))
                {
                }
            }
            else
            {
                long b = new FileInfo(LogFilePath).Length;
                long kb = b / 1024;
                long mb = kb / 1024;
                // long gb = mb / 1024;
                if (mb >= 5)
                {
                    LogFilePath = Basedir + CustomFileName+ _todaysDate + "-" + DateTime.Now.ToString("h.mm") + ".log";
                    using (var stream = File.Create(LogFilePath, 1024, FileOptions.None))
                    {
                    }
                }
            }

        }

        public bool Log(LogType logtype, Func<string> messageFunc, object obj = null)
        {
            try
            {
                if (!_loggerSetting.AllowLogging)
                {
                    return false;
                }
                _init();
                var log = new Log
                {
                    LogType = (int)logtype,
                    DateTime = DateTime.Now.ToString(),
                    Status = messageFunc(),
                    Error = obj == null
                        ? ""
                        : JsonConvert.SerializeObject(new {
                            s =((Exception)obj).Source ,
                            st = ((Exception)obj).StackTrace,
                                ie = ((Exception)obj).InnerException,
                                data = ((Exception)obj).Data,
                                ((Exception)obj).HelpLink
                        }
                        )
                };
                string json = JsonConvert.SerializeObject(log);

                AddLog($"{json}{Environment.NewLine}");
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

       

        public void AddLog(string log)
        {
            Attempt(TryToUpdateRates, log, maximumNumberOfAttempts: 50, timeToWaitBetweenRetriesInMs: 100);
        }

        private void TryToUpdateRates(string log)
        {
            lock (_ratesFileLock)
            {
                using (var stream = GetRatesFileStream())
                {
                    WriteLog(log, stream);
                }
            }
        }

        private Stream GetRatesFileStream()
        {
            return File.Open(LogFilePath, FileMode.Append, FileAccess.Write);
        }

        private void WriteLog(string log, Stream stream)
        {
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.WriteLine(log);
            }
        }

        private static void Attempt(Action<string> work, string log, int maximumNumberOfAttempts, int timeToWaitBetweenRetriesInMs)
        {
            var numberOfFailedAttempts = 0;
            while (true)
            {
                try
                {
                    work(log);
                    return;
                }
                catch
                {
                    numberOfFailedAttempts++;
                    if (numberOfFailedAttempts >= maximumNumberOfAttempts)
                        throw;
                    Thread.Sleep(timeToWaitBetweenRetriesInMs);
                }
            }
        }
    }
}