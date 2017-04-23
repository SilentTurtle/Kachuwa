using System;

namespace Kachuwa.Log
{
    public interface ILoggerService
    {
        DailyLog GetTodaysLogs(int offset = 0, int limit = 20);
        DailyLog GetLogs(int offset , int limit,DateTime day);
    }
}