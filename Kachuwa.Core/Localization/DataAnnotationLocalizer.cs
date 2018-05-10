using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Kachuwa.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Kachuwa.Localization
{
    public class DataAnnotationLocalizer : IStringLocalizer
    {
        private readonly ILocaleResourceProvider _localeResourceProvider;
        private readonly string _cultureName;



        public DataAnnotationLocalizer() : this(CultureInfo.CurrentUICulture)
        {

        }

        public DataAnnotationLocalizer(CultureInfo cultureInfo)
        {
            _localeResourceProvider = ContextResolver.Context.RequestServices.GetService<ILocaleResourceProvider>();

            _cultureName = cultureInfo.Name;
        }
       
        public LocalizedString this[string name]
        {
            get
            {
                var value = GetString(name);
                return new LocalizedString(name, value ?? name, resourceNotFound: value == null);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var format = GetString(name);
                var value = string.Format(format ?? name, arguments);
                return new LocalizedString(name, value, resourceNotFound: format == null);
            }
        }

        private string GetString(string name)
        {
           // var query = _localeResourceProvider.GetByGroup("dataannotation");
           return _localeResourceProvider.Get(name);// query.FirstOrDefault(l => l.Name.ToLower().Trim() == name.ToLower().Trim());
         
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return new DataAnnotationLocalizer(culture);
        }
        public IEnumerable<LocalizedString> GetAllStrings(bool includeAncestorCultures)
        {
            return _localeResourceProvider.GetByGroup("dataannotation")
                .Select(l => new LocalizedString(l.Name, l.Value, true));
        }
    }
}