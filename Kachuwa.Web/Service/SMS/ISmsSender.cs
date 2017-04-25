using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Kachuwa.Web.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
    public class SmsSender : ISmsSender
    {
        private readonly ILogger<SmsSender> _logger;

        public SmsSender(ILogger<SmsSender> logger)
        {
            _logger = logger;
        }


        public Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            _logger.LogInformation("SMS: {number}, Message: {message}", number, message);
            return Task.FromResult(0);
        }
    }
}
