using System.Collections.Generic;
using System.Threading.Tasks;
using Kachuwa.Data;

namespace Kachuwa.Localization
{
    public interface ILocaleService
    {
        CrudService<LocaleRegion> RegionCrudService { get; set; }
        CrudService<LocaleResource> CrudService { get; set; }
        Task<IEnumerable<LocaleRegionViewModel>> GetLocaleRegions(int page, int limit, string search);
        Task Save(LocaleResource model);
        Task<bool> CheckAlreadyExist(int countryId, string culture);
        Task<LocaleRegionEditViewModel> GetAllResourcesAsync(int localRegionId, string baseCulture, int page, int limit);
        Task<bool> SetDefaultAsync(int localeRegionId);
    }
}