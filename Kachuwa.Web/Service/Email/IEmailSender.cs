using System.Threading.Tasks;

namespace Kachuwa.Web.Services
{
    public interface IEmailSender
    {

        Task SendEmailAsync(string subject, string message, params EmailAddress[] to);

        Task SendEmailAsync(EmailAddress from, string subject, string message, params EmailAddress[] to);

        Task SendTemplatedEmailAsync<T>(string templateKey, T context, params EmailAddress[] to);

        Task SendTemplatedEmailAsync<T>(EmailAddress from, string templateKey, T context, params EmailAddress[] to);
    }
}
