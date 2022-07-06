using System;
using System.Linq.Expressions;
using Hangfire;

namespace Kachuwa.Job
{
    public class HangFireJobRunner : IKachuwaJobRunner
    {
        public string Queue(KachuwaJob job)
        {
            return BackgroundJob.Enqueue(job.Job);
        }

        public string Queue(Action job)
        {
            return BackgroundJob.Enqueue(() => job());
        }
        public string Queue(Expression<Action> job)
        {
            return BackgroundJob.Enqueue(job);
        }
        public bool Remove(string jobId)
        {
            return BackgroundJob.Delete(jobId);
        }

        public bool Trigger(string jobId)
        {
            return BackgroundJob.Requeue(jobId);
        }

        public string Schedule(KachuwaJob job)
        {
            if (string.IsNullOrEmpty(job.Cron))
                return BackgroundJob.Schedule(job.Job, job.ScheduledTimeSpan);
            else
            {
                //TODO:: cron to timespan
                return BackgroundJob.Schedule(job.Job, job.ScheduledTimeSpan);
            }
        }

        public void Recurring(KachuwaJob job)
        {
            RecurringJob.AddOrUpdate(job.JobName, job.Job, job.Cron);
        }
    }
}