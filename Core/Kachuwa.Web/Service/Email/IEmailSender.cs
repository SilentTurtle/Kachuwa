using System.Threading.Tasks;

namespace Kachuwa.Web.Services
{
    public interface IEmailSender
    {
        string Name { get; }
        Task SendEmailAsync(string subject, string message, params EmailAddress[] to);

        Task SendEmailAsync(EmailAddress from, string subject, string message, params EmailAddress[] to);

        Task SendTemplatedEmailAsync<T>(string subject, string templateKey,  T context, params EmailAddress[] to);

        Task SendTemplatedEmailAsync<T>(EmailAddress from, string subject, string templateKey, T context, params EmailAddress[] to);
    }
}
