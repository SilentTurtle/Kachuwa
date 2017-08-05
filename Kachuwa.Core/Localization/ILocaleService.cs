using Kachuwa.Data;

namespace Kachuwa.Localization
{
    public interface ILocaleService
    {
        CrudService<LocaleResource> CrudService { get; set; }
    }
}