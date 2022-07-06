using System.Collections.Generic;

namespace Kachuwa.Localization
{
    public interface ILocaleResourceProvider
    {
        void LookUpGroupAt(string groupName);
        void ResetLookUpGroup();
        string Get(string key);
        string Get(string key, string culture);
        IEnumerable<LocaleResource> GetByGroup(string groupName);
        IEnumerable<LocaleResource> GetByGroup(string groupName, string culture);
    }
}