using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Hangfire;
using Hangfire.Storage;
using Kachuwa.Data.Extension;
using Kachuwa.Web.Model;

namespace Kachuwa.Web.Service
{
    public class SamayaSMSSender : ISmsSender
    {
        private readonly ISMSLogService _smsLogService;
        public string Name { get; } = "SamayaSMS";
        private readonly IStorageConnection _jobStorageConnection;
        public SamayaSMSSender(IBackgroundJobClient backgroundJobClient, ISMSLogService smsLogService)
        {
            _smsLogService = smsLogService;
            _jobStorageConnection = JobStorage.Current.GetConnection();
        }
       
        [Queue("critical")]
        public async Task SendSms(string number, string message)
        {
            try
            {
                var log = new SMSLog()
                {
                    From =this.Name,
                    To = number,
                    Body = message,
                    SentDate = DateTime.Now,
                    DeliveredDate = DateTime.Now


                };
                log.AutoFill();

                log.SMSLogId = await _smsLogService.LogCrudService.InsertAsync<long>(log);
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Clear();
                // client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header
                //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "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");

                //'{"personalizations": [{"to": [{"email": "test@example.com"}]}],"from": {"email": "test@example.com"},"subject": "Sending with SendGrid is Fun","content": [{"type": "text/plain", "value": "and easy to do anywhere, even with cURL"}]}'



                var baseUrl = @"http://sms.graycode.com.np/base/smsapi/index.php";
                HttpResponseMessage smsResponse = client.GetAsync($"{baseUrl}?key=55DC8D9C8E90AE&campaign=4455&routeid=22&type=text&contacts={number}&senderid={this.Name}&msg={HttpUtility.HtmlEncode(message)}").Result;
                if (smsResponse.IsSuccessStatusCode)
                {
                    log.IsSent = true;
                    log.DeliveredDate = DateTime.Now;
                    await _smsLogService.LogCrudService.UpdateAsync(log);
                }
                else
                {
                    log.IsSent = false;
                    log.DeliveredDate = DateTime.Now;
                    log.GatewayResponse = await smsResponse.Content.ReadAsStringAsync();
                    await _smsLogService.LogCrudService.UpdateAsync(log);
                    throw new Exception($"Unable to send with {smsResponse.StatusCode} status code");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }
        }
        public async Task SendSmsAsync(string number, string message)
        {
            BackgroundJob.Enqueue(() => SendSms(number, message));
        }

      
    }
}