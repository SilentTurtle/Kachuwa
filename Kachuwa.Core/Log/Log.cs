using System;

namespace Kachuwa.Log
{
    public class Log
    {
        public string LogId { get; set; } = Guid.NewGuid().ToString();
        public string Status { get; set; }
        public string DateTime { get; set; }
        public int LogType { get; set; }
        public string Error { get; set; }

    }
}