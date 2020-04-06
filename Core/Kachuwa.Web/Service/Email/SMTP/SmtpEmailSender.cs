using Kachuwa.Configuration;
using Kachuwa.Web.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Kachuwa.Web.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly SmtpEmailSetting _setting;
        private readonly IHostingEnvironment _hostingEnvironment;
        public string Name { get; } = "SMTPSENDER";
        public SmtpEmailSender(IOptionsSnapshot<KachuwaAppConfig> configOption, IHostingEnvironment hostingEnvironment)
        {
            //var configOption = ContextResolver.Context.RequestServices.GetService<IOptionsSnapshot<KachuwaAppConfig>>();
            //_jobRunner = ContextResolver.Context.RequestServices.GetService<IKachuwaJobRunner>();
            // _jobRunner = jobRunner;
            _hostingEnvironment = hostingEnvironment;
            KachuwaAppConfig kachuwaAppConfig = configOption.Value;
            _setting = new SmtpEmailSetting
            {
                Host = kachuwaAppConfig.SMTMConfig.Host,
                Password = kachuwaAppConfig.SMTMConfig.Password,
                Port = kachuwaAppConfig.SMTMConfig.Port,
                UseSSL = kachuwaAppConfig.SMTMConfig.UseSSL,
                UserName = kachuwaAppConfig.SMTMConfig.UserName
            };
        }


        public async Task _sendEmailAsync(
            EmailAddress from,
            IEnumerable<EmailAddress> recipients,
            string subject,
            string text,
            string html)
        {
            try

            {


                var message = new MailMessage { From = new MailAddress(@from.Email, @from.DisplayName ?? "") };

                foreach (var recipient in recipients)
                {
                    message.To.Add(new MailAddress(recipient.Email, recipient.DisplayName ?? ""));
                }

                message.Subject = subject;

                message.Body = string.IsNullOrEmpty(text) ? html : text;
                message.IsBodyHtml = !string.IsNullOrEmpty(html);
                var email = new Email()
                {
                    From = from,
                    MessageHtml = html,
                    MessageText = text,
                    Subject = subject,
                    To = recipients.ToArray()
                };
                var smtpclient = new SmtpClient
                {
                    UseDefaultCredentials = false,
                    Host = _setting.Host,
                    Port = _setting.Port,
                    Credentials = new System.Net.NetworkCredential(_setting.UserName, _setting.Password),
                    EnableSsl = _setting.UseSSL
                };

                smtpclient.SendCompleted += Smtpclient_SendCompleted;
                smtpclient.SendAsync(message, new SendAsyncState(email));


            }
            catch (Exception e)
            {
                throw e;
            }
        }



        public async Task SendEmailAsync(string subject, string message, params EmailAddress[] to)
        {
            var settingService = ContextResolver.Context.RequestServices.GetService<ISettingService>();
            var webSetting = await settingService.GetSetting();
            string senderEmail = webSetting.DefaultEmail;
            if (string.IsNullOrEmpty(senderEmail))
            {
                senderEmail = "info@kachuwaframework.com";
            }
            _sendEmailAsync(
              new EmailAddress() { Email = senderEmail, DisplayName = webSetting.WebsiteName }, to, subject,
              message, "");
            //Task.FromResult(1);


        }

        public Task SendEmailAsync(EmailAddress from, string subject, string message, params EmailAddress[] to)
        {
            return _sendEmailAsync(from, to, subject, message, "");
        }

        public async Task SendTemplatedEmailAsync<T>(string subject, string templateKey,  T context, params EmailAddress[] to)
        {
            string senderEmail = "asdf@asdf.cc";
            var template = File.ReadAllText(Path.Combine(_hostingEnvironment.ContentRootPath, templateKey));
            await _sendEmailAsync(new EmailAddress() { Email = senderEmail }, to, "Subject Here", "", template);
        }

        public Task SendTemplatedEmailAsync<T>(EmailAddress from, string subject, string templateKey, T context, params EmailAddress[] to)
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
