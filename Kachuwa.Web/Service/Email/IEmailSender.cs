using System.Threading.Tasks;

namespace Kachuwa.Web.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
