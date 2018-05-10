using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kachuwa.Web;
using Microsoft.AspNetCore.Localization;

namespace Kachuwa.Localization
{
    public class LocaleResourceProvider : ILocaleResourceProvider
    {
        private readonly IEnumerable<LocaleResource> _resources;
        private IEnumerable<LocaleResource> _groupResources;
        private readonly ResourceBuilder _resourceBuilder;
        private readonly ILocaleService _localeService;
        private string _groupName = "";
        public LocaleResourceProvider(ResourceBuilder resourceBuilder, ILocaleService localeService )
        {
          
            _resourceBuilder = resourceBuilder;
            _localeService = localeService;
            _resources = _resourceBuilder.AllLocaleResources;
        }
        /// <summary>
        /// this is temporary to store all keys to db
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="group"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        private async Task Store(string key, string value, string group
            , string culture)
        {
            var model = new LocaleResource
            {
                Name = key,
                Value = value,
                Culture = culture,
                GroupName = group
            };
            await _localeService.Save(model);
        }
        public string GetCurrentCulture()
        {
            var rqf = ContextResolver.Context.Features.Get<IRequestCultureFeature>();
            // Culture contains the information of the requested culture
            var culture = rqf.RequestCulture.Culture;
            return culture.ToString().ToLowerInvariant();
        }

        public void LookUpGroupAt(string groupName)
        {
            _groupName = groupName;
            string culture = GetCurrentCulture();
            var localeResources = _resources.Where(e => e.Culture.ToLowerInvariant() == culture && e.GroupName == groupName);
            _groupResources = localeResources;
        }
        public void ResetLookUpGroup()
        {
            _groupName = "";
        }
        public string Get(string key)
        {
            if (string.IsNullOrEmpty(_groupName))
            {
                string culture = GetCurrentCulture();
                var localeResource =
                    _resources.FirstOrDefault(e => e.Culture.ToLowerInvariant() == culture && e.Name == key);
                if (localeResource != null)
                    return localeResource.Value;
                else
                {
                    Task.Run(async () => await Store(key, key, _groupName, culture));
                    return key;
                }
            }
            else
            {
                string culture = GetCurrentCulture();
                var localeResource =
                    _groupResources.FirstOrDefault(e => e.Culture.ToLowerInvariant() == culture && e.Name == key);
                if (localeResource != null)
                    return localeResource.Value;
                else
                {
                    Task.Run(async () => await Store(key, key, _groupName, culture));
                    return key;
                }
            }
        }
        public string Get(string key,string culture)
        {
            if (string.IsNullOrEmpty(_groupName))
            {
                var localeResource =
                    _resources.FirstOrDefault(e => e.Culture.ToLowerInvariant() == culture && e.Name == key);
                if (localeResource != null)
                    return localeResource.Value;
                else
                {
                    Task.Run(async () => await Store(key, key, _groupName, culture));
                    return key;
                }
            }
            else
            {
                var localeResource =
                    _groupResources.FirstOrDefault(e => e.Culture.ToLowerInvariant() == culture && e.Name == key);
                if (localeResource != null)
                    return localeResource.Value;
                else
                {
                    Task.Run(async () => await Store(key, key, _groupName, culture));
                    return key;
                }
            }
        }
        public IEnumerable<LocaleResource> GetByGroup(string groupName)
        {
            string culture = GetCurrentCulture();
            var localeResources = _resources.Where(e => e.Culture.ToLowerInvariant() == culture && e.GroupName == groupName);
            return localeResources;
        }
        public IEnumerable<LocaleResource> GetByGroup(string groupName, string culture)
        {
            var localeResources = _resources.Where(e => e.Culture.ToLowerInvariant() == culture && e.GroupName == groupName);
            return localeResources;
        }


    }
}