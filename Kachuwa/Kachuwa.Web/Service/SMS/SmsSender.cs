using System.Threading.Tasks;
using Kachuwa.Log;

namespace Kachuwa.Web.Service
{
    public class SmsSender : ISmsSender
    {
        private readonly ILogger _logger;


        public SmsSender(ILogger logger)
        {
            _logger = logger;
        }


        public Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            _logger.Log(LogType.Info, () => $"SMS: {number}, Message: {message}");
            return Task.FromResult(0);
        }
    }
}