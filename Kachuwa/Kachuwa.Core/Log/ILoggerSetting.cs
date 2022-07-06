using System.Threading.Tasks;

namespace Kachuwa.Log
{
    public interface ILoggerSetting
    {
        bool AllowLogging { get; set; }
    }
}