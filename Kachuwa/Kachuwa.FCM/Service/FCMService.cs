using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Mime;
using System.Net.Http.Headers;
using Kachuwa.FCM;

namespace Kachuwa.FCM
{
    public class FCMService : IFCMService
    {
        private readonly FCMSetting _settings;
        private readonly IHttpClientFactory _httpClientFactory;

        public FCMService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _settings = new FCMSetting
            {
                SenderId = configuration.GetSection("FCMSetting:SenderId").ToString(),
                ApplicationId = configuration.GetSection("FCMSetting:ApplicationId").ToString()
            };

            _httpClientFactory = httpClientFactory;
        }


        public void SubscribeTopic(string token, string topic)
        {

            if (String.IsNullOrEmpty(token) || String.IsNullOrEmpty(topic))
                throw new Exception($"Empty Token");

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://iid.googleapis.com");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("key", "=" + _settings.ApplicationId);

            var data = new
            {
                to = String.Format("/topics/{0}", topic),
                registration_tokens = new string[] { token }
            };

            var JsonData = JsonConvert.SerializeObject(data);

            HttpContent contentPost = new StringContent(JsonData, Encoding.UTF8, MediaTypeNames.Application.Json);
            client.PostAsync("/iid/v1:batchAdd", contentPost);
        }

        public void FcmSend(string token, string title, string message, string Clicek_Url, string Image_Uri)
        {
            if (String.IsNullOrEmpty(token))
                throw new Exception($"Empty Token");

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://fcm.googleapis.com/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("key", "=" + _settings.ApplicationId);
            client.DefaultRequestHeaders.Add("Sender", $"id={_settings.SenderId}");

            var data = new
            {
                to = token,
                notification = new
                {
                    body = message,
                    title = title,
                    sound = "Enabled"
                }
            };
            var data2 = new
            {
                to = "/topics/" + token,
                priority = "high",
                collapse_key = "demo",
                notification = new
                {
                    body = message,
                    title = title,
                    icon = String.IsNullOrEmpty(Image_Uri) ? null : Image_Uri,
                    click_action = String.IsNullOrEmpty(Clicek_Url) ? null : Clicek_Url,
                    sound = "Enabled",
                }
            };
            var JsonData = JsonConvert.SerializeObject(data);

            HttpContent contentPost = new StringContent(JsonData, Encoding.UTF8, MediaTypeNames.Application.Json);
            var Re = client.PostAsync("/fcm/send", contentPost);

        }

        public void FcmSend(string token, string title, string message)
        {
            FcmSend(token, title, message, string.Empty, string.Empty);
        }

        public void FcmSend(string token, string title, string message, string Clicek_Url)
        {
            FcmSend(token, title, message, Clicek_Url, string.Empty);
        }

        public void FcmSendTopic(string token, string title, string message, string Clicek_Url, string Image_Uri)
        {
            if (String.IsNullOrEmpty(token))
                throw new Exception($"Empty Token");

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://fcm.googleapis.com/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("key", "=" + _settings.ApplicationId);
            client.DefaultRequestHeaders.Add("Sender", $"id={_settings.SenderId}");

            var data = new
            {
                to = "/topics/" + token,
                priority = "high",
                collapse_key = "demo",
                notification = new
                {
                    body = message,
                    title = title,
                    icon = String.IsNullOrEmpty(Image_Uri) ? null : Image_Uri,
                    click_action = String.IsNullOrEmpty(Clicek_Url) ? null : Clicek_Url,
                    sound = "Enabled",
                }
            };
            var JsonData = JsonConvert.SerializeObject(data);

            HttpContent contentPost = new StringContent(JsonData, Encoding.UTF8, MediaTypeNames.Application.Json);
            var Re = client.PostAsync("/fcm/send", contentPost);
        }

        public void FcmSendTopic(string token, string title, string message, string Clicek_Url)
        {
            FcmSendTopic(token, title, message, Clicek_Url, string.Empty);
        }

        public void FcmSendTopic(string token, string title, string message)
        {
            FcmSendTopic(token, title, message, string.Empty, string.Empty);
        }
    }
}
