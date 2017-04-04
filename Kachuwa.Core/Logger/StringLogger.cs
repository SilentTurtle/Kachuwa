using System;
using System.Text;

namespace Kachuwa.Logger
{
    /// <summary>
    /// Represents Simple String Logger
    /// </summary>
    public static class StringLogger
    {
        private static readonly StringBuilder Buffer = new StringBuilder();
        public static void Log(string value)
        {
            lock (Buffer)
            {
                Buffer.AppendLine($"{DateTime.Now} {value}");
            }
        }

        public new static string ToString()
        {
            lock (Buffer)
            {
                return Buffer.ToString();
            }
        }
    }
}