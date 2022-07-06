using System.Collections.Generic;
using System.Threading.Tasks;
using Kachuwa.Data;

namespace Kachuwa.Localization
{
    public interface ILocaleService
    {
        CrudService<LocaleRegion> RegionCrudService { get; set; }
        Task<IEnumerable<LocaleRegion>> GetRegions();
        CrudService<LocaleResource> CrudService { get; set; }
        Task<IEnumerable<LocaleRegionViewModel>> GetLocaleRegions(int page, int limit, string search);
        Task Save(LocaleResource model);
        Task<bool> CheckAlreadyExist(int countryId, string culture);
        Task<LocaleRegionEditViewModel> GetAllResourcesAsync(int localRegionId, string baseCulture, int page, int limit);
        Task<bool> SetDefaultAsync(int localeRegionId);
        Task<bool> AddNewResourceFromBaseCultureAsync(string culture);
        Task<LocaleRegion> GetDefaultLocaleRegion();
        Task<IEnumerable<LocaleResourcesExportModel>> GetAllResourcesForExportAsync(int localRegionId, string baseCulture);
        Task<ImportedStatus> ImportLocaleResources(List<LocaleResourcesImportModel> importedDatas, string addedBy);
    }
}