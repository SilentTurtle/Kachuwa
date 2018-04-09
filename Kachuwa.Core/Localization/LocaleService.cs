using Kachuwa.Data;

namespace Kachuwa.Localization
{
    public class LocaleService: ILocaleService
    {
        public CrudService<LocaleResource> CrudService { get; set; }=new CrudService<LocaleResource>();
    }
}