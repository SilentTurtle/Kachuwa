using System.Collections.Generic;
using System.Linq;
using Kachuwa.Web;
using Microsoft.AspNetCore.Localization;

namespace Kachuwa.Localization
{
    public class LocaleResourceProvider : ILocaleResourceProvider
    {
        private readonly IEnumerable<LocaleResource> _resources;
        private readonly ResourceBuilder _resourceBuilder;
        private readonly ILocaleService _localeService;

        public LocaleResourceProvider(ResourceBuilder resourceBuilder, ILocaleService localeService )
        {
          
            _resourceBuilder = resourceBuilder;
            _localeService = localeService;
            _resources = _resourceBuilder.AllLocaleResources;
        }

        public string GetCurrentCulture()
        {
            var rqf = ContextResolver.Context.Features.Get<IRequestCultureFeature>();
            // Culture contains the information of the requested culture
            var culture = rqf.RequestCulture.Culture;
            return culture.ToString().ToLowerInvariant();
        }

        public string Get(string key)
        {
            string culture = GetCurrentCulture();
            var localeResource = _resources.FirstOrDefault(e => e.Culture.ToLowerInvariant() == culture && e.Name == key);
            if (localeResource != null)
                return localeResource.Value;
            else return key;
        }
        public string Get(string key,string culture)
        {
            var localeResource = _resources.FirstOrDefault(e => e.Culture.ToLowerInvariant() == culture && e.Name == key);
            if (localeResource != null)
                return localeResource.Value;
            else return key;
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