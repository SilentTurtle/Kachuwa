using System;
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
        public LocaleResourceProvider(ResourceBuilder resourceBuilder, ILocaleService localeService)
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
            await _resourceBuilder.Build();
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
            if (_resources.Any())
            {
                string culture = GetCurrentCulture();
                var localeResources =
                    _resources.Where(e => e.Culture.ToLowerInvariant() == culture && e.GroupName == groupName);
                _groupResources = localeResources;
            }
            else
            {

            }
        }
        public void ResetLookUpGroup()
        {
            _groupName = "";
        }
        public string Get(string key)
        {
            try
            {


                //if (!_resources.Any())
                //    return key;
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
                    if (!_resources.Any())
                    {
                        Task.Run(async () => await Store(key, key, _groupName, culture));
                        return key;
                    }
                    else
                    {
                        var localeResource =
                            _groupResources.FirstOrDefault(
                                e => e.Culture.ToLowerInvariant() == culture && e.Name == key);
                        if (localeResource != null)
                            return localeResource.Value;
                        else
                        {
                            var _localeResource =
                                _resources.FirstOrDefault(e =>
                                    e.Culture.ToLowerInvariant() == culture && e.Name == key);

                            if (_localeResource != null)
                                return _localeResource.Value;
                            else
                            {
                                Task.Run(async () => await Store(key, key, _groupName, culture));
                                return key;
                            }
                        }
                    }

                }
            }
            catch (Exception e)
            {
                return key;
            }
        }
        public string Get(string key, string culture)
        {
            try
            {


                //if (!_resources.Any())
                //    return key;
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
                    if (!_resources.Any())
                    {
                        Task.Run(async () => await Store(key, key, _groupName, culture));
                        return key;
                    }
                    else
                    {
                        var localeResource =
                            _groupResources.FirstOrDefault(
                                e => e.Culture.ToLowerInvariant() == culture && e.Name == key);
                        if (localeResource != null)
                            return localeResource.Value;
                        else
                        {
                            Task.Run(async () => await Store(key, key, _groupName, culture));
                            return key;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return key;
            }
        }
        public IEnumerable<LocaleResource> GetByGroup(string groupName)
        {
            if (_resources.Any())
            {
                string culture = GetCurrentCulture();
                var localeResources =
                    _resources.Where(e => e.Culture.ToLowerInvariant() == culture && e.GroupName == groupName);
                return localeResources;
            }
            else
            {
                return _resources;
            }

        }
        public IEnumerable<LocaleResource> GetByGroup(string groupName, string culture)
        {
            if (_resources.Any())
            {
                var localeResources =
                    _resources.Where(e => e.Culture.ToLowerInvariant() == culture && e.GroupName == groupName);
                return localeResources;
            }
            else
            {
                return _resources;
            }
        }


    }
}