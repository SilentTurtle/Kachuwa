using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Storage;
using Kachuwa.Data.Extension;
using Kachuwa.Web.Model;

namespace Kachuwa.Web.Service
{
    public class AAKASHSMSSender : ISmsSender
    {
        private readonly ISMSLogService _smsLogService;
        public string Name { get; } = "AakashSMS";
        public string baseUrl { get; } = "https://sms.aakashsms.com/sms/v3/send";

        private readonly IStorageConnection _jobStorageConnection;
        public AAKASHSMSSender(IBackgroundJobClient backgroundJobClient, ISMSLogService smsLogService)
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
                    From = this.Name,
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

           
                HttpResponseMessage smsResponse = client.GetAsync($"{baseUrl}?auth_token=34949ec46a9695d9e66377c58adff00b4be94b7056ea50c383725fc668266103&to={number}&text={message}").Result;
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