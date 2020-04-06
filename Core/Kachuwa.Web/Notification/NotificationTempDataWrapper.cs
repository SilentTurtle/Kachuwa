using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;

namespace Kachuwa.Web.Notification
{
    public class NotificationTempDataWrapper : INotificationTempDataWrapper
    {
    
        private readonly IHttpContextAccessor _accessor;
        private readonly ITempDataDictionaryFactory _tempDataDictionaryFactory;

        public NotificationTempDataWrapper(ITempDataDictionaryFactory tempDataDictionaryFactory,
            IHttpContextAccessor httpContextAccessor)
        {
            _tempDataDictionaryFactory = tempDataDictionaryFactory;
            _accessor = httpContextAccessor;
        }
        /// <summary>
        /// Gets or sets <see cref="ITempDataDictionary"/>/>.
        /// </summary>
        private ITempDataDictionary TempData => _tempDataDictionaryFactory?.GetTempData(_accessor.HttpContext);

        public T Get<T>(string key)
        {
            if (TempData.ContainsKey(key))
            {
                string value = TempData[key].ToString();
                TempData.Remove(key);
                return JsonConvert.DeserializeObject<T>(value);
            }
            return default(T);
        }

        public T Peek<T>(string key)
        {
            if (TempData.ContainsKey(key))
            {
                string value = TempData.Peek(key).ToString();
                TempData.Remove(key);
                return JsonConvert.DeserializeObject<T>(value);
            }
            return default(T);
        }

        public void Add(string key, object value)
        {
            TempData[key] = JsonConvert.SerializeObject(value);

        }
    }
}