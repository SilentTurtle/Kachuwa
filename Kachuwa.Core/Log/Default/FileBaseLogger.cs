using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Kachuwa.Log
{
    public class FileBaseLogger : ILogger, ILoggerService
    {
        private readonly string _basedir ="\\wwwroot" + "\\Logs\\";
        private string LogFilePath { get; set; }
        private List<Log> Logs { get; set; }
        private void _init()
        {
            var TodayDate = DateTime.Now.ToString("yyyy_MM_dd");

            if (!Directory.Exists(_basedir))
            {
                Directory.CreateDirectory(_basedir);
            }
            LogFilePath = _basedir + TodayDate + ".json";
            if (!File.Exists(LogFilePath))
            {
                //File.Create(LogFilePath);
                using (var stream = File.Create(LogFilePath, 1024, FileOptions.None))
                {
                    //stream.Close();
                    // stream.Dispose();
                }
            }
            string json = "";
            using (var stream = File.Open(LogFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                StreamReader sr = new StreamReader(stream);
                json = sr.ReadToEnd();
                // sr.Close();
                // Use stream
            }
            try
            {
                // string json = File.ReadAllText(LogFilePath);
                Logs = JsonConvert.DeserializeObject<List<Log>>(json);
                if (Logs == null)
                    Logs = new List<Log>();
            }
            catch (Exception ex)
            {
                if (Logs == null)
                    Logs = new List<Log>();
            }


        }

        private List<Log> _init(DateTime day )
        {
            List<Log> logs=new List<Log>();
            var TodayDate = day.ToString("yyyy_MM_dd");
           
            LogFilePath = _basedir + TodayDate + ".json";
            if (!File.Exists(LogFilePath))
            {
                throw  new Exception("Please use previos dates.");
            }
            string json = "";
            using (var stream = File.Open(LogFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                StreamReader sr = new StreamReader(stream);
                json = sr.ReadToEnd();
                //sr.Close();
                // Use stream
            }
            try
            {
                // string json = File.ReadAllText(LogFilePath);
                logs = JsonConvert.DeserializeObject<List<Log>>(json);
                return logs;
               
            }
            catch (Exception ex)
            {
                return logs;
            }


        }
        
        public bool Log(LogType logtype, Func<string> messageFunc, object obj = null)
        {
            try
            {
                _init();
                var log = new Log
                {
                    LogType = (int)logtype,
                    DateTime = DateTime.Now.ToString(),
                    Status = messageFunc(),
                    Error = obj == null ? "" : JsonConvert.SerializeObject(obj)
                };
                Logs.Add(log);
                string json = JsonConvert.SerializeObject(Logs);

                File.WriteAllText(LogFilePath, json);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public DailyLog GetTodaysLogs(int offset = 0, int limit = 20)
        {
            _init();
            Logs.Reverse();
            var logs = new DailyLog();
            logs.TotalCount = Logs.Count();
            logs.Logs = Logs.Skip((offset - 1) * limit).Take(limit).ToList();
            return logs;
        }

        public DailyLog GetLogs(int offset, int limit, DateTime day)
        {
            var _logs= _init(day);
            _logs.Reverse();
            var logs = new DailyLog();
            logs.TotalCount = _logs.Count();
            logs.Logs = _logs.Skip((offset - 1) * limit).Take(limit).ToList();
            return logs;
        }
    }
}