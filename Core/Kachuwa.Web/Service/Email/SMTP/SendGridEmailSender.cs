using Hangfire;
using Hangfire.Storage;
using Kachuwa.Data.Extension;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Kachuwa.Web.Services
{
    public class SendGridEmailSender : IEmailSender
    {
        private readonly IEmailLogService _emailLogService;
        private readonly ITemplateEngine _templateEngine;
        public string Name { get; } = "SendGrid";
        private readonly IStorageConnection _jobStorageConnection;
        public SendGridEmailSender(IBackgroundJobClient backgroundJobClient, IEmailLogService emailLogService, ITemplateEngine templateEngine)
        {
            _emailLogService = emailLogService;
            _templateEngine = templateEngine;
            _jobStorageConnection = JobStorage.Current.GetConnection();
        }
        public async Task SendEmailAsync(string subject, string message, params EmailAddress[] to)
        {
            var from = new EmailAddress() { Email = "noreply@onlinekachhya.com", DisplayName = "OK" };
            await SendEmailAsync(from, subject, message, to);
        }

        [Queue("critical")]
        public async Task SendEmail(EmailAddress @from, string subject, string message, params EmailAddress[] to)
        {
            try
            {
                var log = new EmailLog()
                {
                    From = from.Email,
                    To = String.Join(",", to.Select(x => x.Email)),
                    Body = message,
                    Subject = subject,
                    SentDate = DateTime.Now,
                    DeliveredDate = DateTime.Now


                };
                log.AutoFill();

                log.EmailLogId = await _emailLogService.LogCrudService.InsertAsync<long>(log);
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Clear();
                // client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "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");

                //'{"personalizations": [{"to": [{"email": "test@example.com"}]}],"from": {"email": "test@example.com"},"subject": "Sending with SendGrid is Fun","content": [{"type": "text/plain", "value": "and easy to do anywhere, even with cURL"}]}'

                var _contents = new List<Object>();
                var _to = new List<Object>();
                _contents.Add(new { type = "text/html", value = message });
                _to.Add(new { to = to.Select(x => new { email = x.Email }) });


                var payload2 = new
                {
                    personalizations = _to,
                    from = new { email = @from.Email },
                    subject = subject,
                    content = _contents

                };
                var json = JsonConvert.SerializeObject(payload2);
                HttpContent c = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage emailResponse = client.PostAsync($"https://api.sendgrid.com/v3/mail/send", c).Result;
                if (emailResponse.IsSuccessStatusCode)
                {
                    log.IsSent = true;
                    log.DeliveredDate = DateTime.Now;
                    await _emailLogService.LogCrudService.UpdateAsync(log);
                }
                else
                {
                    log.IsSent = false;
                    log.DeliveredDate = DateTime.Now;
                    log.GatewayResponse = await emailResponse.Content.ReadAsStringAsync();
                    await _emailLogService.LogCrudService.UpdateAsync(log);
                    throw new Exception($"Unable to send with {emailResponse.StatusCode} status code");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }
        }
        public async Task SendEmailAsync(EmailAddress @from, string subject, string message, params EmailAddress[] to)
        {
            BackgroundJob.Enqueue(() => SendEmail(@from, subject, message, to));
        }

        public Task SendTemplatedEmailAsync<T>(string subject, string templateKey, T context, params EmailAddress[] to)
        {
            var from = new EmailAddress() { Email = "noreply@onlinekachhya.com", DisplayName = "OK" };

            string template = _templateEngine.RenderFromFile(templateKey, context, true);

            BackgroundJob.Enqueue(() => SendEmail(@from, subject, template, to));
            return Task.CompletedTask;
        }

        public Task SendTemplatedEmailAsync<T>(EmailAddress @from, string subject, string templateKey, T context, params EmailAddress[] to)
        {
            // var from = new EmailAddress() { Email = "noreply@onlinekachhya.com", DisplayName = "OK" };

            string template = _templateEngine.RenderFromFile(templateKey, context, true);

            BackgroundJob.Enqueue(() => SendEmail(@from, subject, template, to));
            return Task.CompletedTask;
        }
    }
}