using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Kachuwa.Web.Services;

namespace Kachuwa.Web.Extensions
{
    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link)
        {
            var to= new List<EmailAddress>() {
                new EmailAddress{
                    Email=email }
            };
            return emailSender.SendEmailAsync("Confirm your email",
                $"Please confirm your account by clicking this link: <a href='{HtmlEncoder.Default.Encode(link)}'>link</a>", to.ToArray() );
        }
    }
}