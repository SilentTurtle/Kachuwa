using System.Collections.Generic;
using Kachuwa.Web;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;

namespace Kachuwa.Localization
{
    public static class LocaleHtmlHelper
    {


        public static string LocaleValue(this HtmlHelper helper, string key)
        {
            var service=(ILocaleResourceProvider)ContextResolver.Context.RequestServices.GetService<ILocaleResourceProvider>();
            return service.Get(key);
        }
        public static string LocaleValue(this HtmlHelper helper, string key,string culture)
        {
            var service = (ILocaleResourceProvider)ContextResolver.Context.RequestServices.GetService<ILocaleResourceProvider>();
            return service.Get(key, culture);
        }
        public static IEnumerable<LocaleResource> LocaleValuesByGroup(this HtmlHelper helper, string groupName)
        {
            var service = (ILocaleResourceProvider)ContextResolver.Context.RequestServices.GetService<ILocaleResourceProvider>();
            return service.GetByGroup(groupName);
        }
        public static IEnumerable<LocaleResource> LocaleValuesByGroup(this HtmlHelper helper, string groupName, string culture)
        {
            var service = (ILocaleResourceProvider)ContextResolver.Context.RequestServices.GetService<ILocaleResourceProvider>();
            return service.GetByGroup(groupName, culture);
        }
    }
}