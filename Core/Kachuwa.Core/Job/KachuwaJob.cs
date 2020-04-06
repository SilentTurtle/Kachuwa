using System;
using System.Linq.Expressions;

namespace Kachuwa.Job
{
    public class KachuwaJob
    {
        public string JobName { get; set; }
        public Expression<Action> Job { get; set; }
        public string Cron { get; set; }
        public TimeSpan ScheduledTimeSpan { get; set; }
    }
}