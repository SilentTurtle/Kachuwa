using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Kachuwa.Data;
using Kachuwa.Extensions;

namespace Kachuwa.Localization
{
    public class LocaleService : ILocaleService
    {
        public CrudService<LocaleRegion> RegionCrudService { get; set; } = new CrudService<LocaleRegion>();
        public CrudService<LocaleResource> CrudService { get; set; } = new CrudService<LocaleResource>();

        public async Task<IEnumerable<LocaleRegionViewModel>> GetLocaleRegions(int page, int limit, string search)
        {
            var dbFactory = DatabaseFactories.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                return await db.QueryAsync<LocaleRegionViewModel>("Select lr.*,c.NiceName as CountryName from " +
                                                            " dbo.LocaleRegion as lr inner join dbo.Country as c " +
                                                            " on lr.CountryId=c.CountryId Where lr.IsDeleted=@IsDeleted and c.NiceName like @Search " +
                                                            "  Order By c.Name OFFSET @RowsPerPage * (@PageNumber-1) ROWS FETCH NEXT @RowsPerPage ROWS ONLY",
                    new { IsDeleted = false, Search = "%" + search + "%", PageNumber = page, RowsPerPage = limit });
            }
        }

        public async Task Save(LocaleResource model)
        {
            var dbFactory = DatabaseFactories.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                await db.ExecuteAsync(
                    "if(Not Exists(select 1 from dbo.LocaleResource where Name=@Name and Culture=@Culture and " +
                    "GroupName=@GroupName)) " +
                    "BEGIN " +
                    " Insert into dbo.LocaleResource(Culture,Name,value,GroupName) values(@Culture,@Name,@Value,@GroupName);" +
                    "END ",
                    new { model.Name, model.Culture, model.GroupName, model.Value });
            }
        }

        public async Task<bool> CheckAlreadyExist(int countryId, string culture)
        {
            var result = await RegionCrudService.GetAsync("Where CountryId=@countryId and Culture=@culture", new { countryId, culture });
            return result != null;
        }

        public async Task<LocaleRegionEditViewModel> GetAllResourcesAsync(int localRegionId, string baseCulture,int page,int limit)
        {
            var result = await RegionCrudService.GetAsync(localRegionId);
            var model = result.To<LocaleRegionEditViewModel>();

            var dbFactory = DatabaseFactories.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                var resources = await db.QueryAsync<EditLocaleResource>(
                    "SELECT COUNT(1) OVER() AS RowTotal, lr.*,lb.Value as BaseValue from dbo.LocaleResource lr left join dbo.LocaleResource lb on lb.Name = lr.Name " +
                     " where lr.culture = @Culture and lb.Culture = @baseCulture " +
                    "Order By lr.Name OFFSET @RowsPerPage * (@PageNumber-1) ROWS FETCH NEXT @RowsPerPage ROWS ONLY",
                    new { model.Culture, baseCulture, RowsPerPage=limit, PageNumber=page });
                model.Resources = resources.ToList();
            }

            return model;

        }

        public async Task<bool> SetDefaultAsync(int localeRegionId)
        {
            var dbFactory = DatabaseFactories.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                await db.ExecuteAsync(
                    "update dbo.LocaleResource set IsDefault=@PrevDefault;" +
                    "Update dbo.LocaleResource set IsDefault=@IsDefault Where LocaleRegionId=@localeRegionId ",
                    new { PrevDefault=false, IsDefault=true, localeRegionId });
                return true;
            }
        }
    }
}