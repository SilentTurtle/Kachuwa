using Kachuwa.Plugin;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Kachuwa.Web.Services
{
    public class SmtpEmailSender :  IEmailSender
    {
        private SmtpEmailSetting _setting;
       

        private Task _sendEmailAsync(
            EmailAddress from,
            IEnumerable<EmailAddress> recipients,
            string subject,
            string text,
            string html)
        {
            var message = new MailMessage();
            message.From = new MailAddress(from.Email, from.DisplayName);

            foreach (var recipient in recipients)
            {
                message.To.Add(new MailAddress(recipient.Email, recipient.DisplayName));
            }

            message.Subject = subject;

            message.Body = string.IsNullOrEmpty(text) ? html : text;

            using (var smtpclient = new SmtpClient())
            {

                smtpclient.Port = _setting.Port;
                smtpclient.Credentials = new System.Net.NetworkCredential(_setting.UserName, _setting.Password);
                smtpclient.EnableSsl = _setting.UseSSL;
                smtpclient.SendCompleted += Smtpclient_SendCompleted;

                smtpclient.SendAsync(message, new SendAsyncState(new Email()));
                return Task.CompletedTask;
            }
        }



        public Task SendEmailAsync(string subject, string message, params EmailAddress[] to)
        {
            string senderEmail = "";
            return _sendEmailAsync(new EmailAddress() { Email = senderEmail }, to, subject, message, "");
        }

        public Task SendEmailAsync(EmailAddress from, string subject, string message, params EmailAddress[] to)
        {
            return _sendEmailAsync(from, to, subject, message, "");
        }

        public Task SendTemplatedEmailAsync<T>(string templateKey, T context, params EmailAddress[] to)
        {
            string senderEmail = "";
            throw new NotImplementedException();
            //return _sendEmailAsync(new EmailAddress() { Email = senderEmail }, to, subject, message, "");
        }

        public Task SendTemplatedEmailAsync<T>(EmailAddress from, string templateKey, T context, params EmailAddress[] to)
        {
            throw new NotImplementedException();
        }

       

        private void Smtpclient_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            var smtpClient = (SmtpClient)sender;
            var userAsyncState = (SendAsyncState)e.UserState;
            if (e.Error != null)
                Console.WriteLine("Error sending email.");
            else if (e.Cancelled)
                Console.WriteLine("Sending of email cancelled.");
            smtpClient.SendCompleted -= Smtpclient_SendCompleted;
        }
    }
}
