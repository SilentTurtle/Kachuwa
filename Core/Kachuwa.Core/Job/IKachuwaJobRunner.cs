using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Kachuwa.Job
{
    public interface IKachuwaJobRunner
    {
        string Queue(Action job);
        string Queue(Expression<Action> job);
        string Queue(KachuwaJob job);
        void Recurring(KachuwaJob job);
        bool Remove(string jobId);
        string Schedule(KachuwaJob job);
        bool Trigger(string jobId);
    }
}