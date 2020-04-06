using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Kachuwa.Caching;
using Kachuwa.Core;
using Kachuwa.Data;
using Kachuwa.Extensions;
using Kachuwa.Web;
using Microsoft.Extensions.DependencyInjection;

namespace Kachuwa.Localization
{
    public class LocaleService : ILocaleService
    {
        private readonly ICacheService _cacheService;

        public LocaleService(ICacheService cacheService )
        {
            _cacheService = cacheService;
        }
        public CrudService<LocaleRegion> RegionCrudService { get; set; } = new CrudService<LocaleRegion>();
        public async Task<IEnumerable<LocaleRegion>> GetRegions()
        {
            var dbFactory = DatabaseFactories.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                return await db.QueryAsync<LocaleRegion>(@"select l.*,c.NiceName as Country from dbo.LocaleRegion l
                inner join dbo.Country c on c.CountryId = l.CountryId where l.IsActive = 1 and l.IsDeleted = 0",
                    new { IsActive= true, IsDeleted=false });
            }
        }

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
                    "if(Not Exists(select 1 from dbo.LocaleResource where LTRIM(RTRIM(Name))=LTRIM(RTRIM(@Name)) and LTRIM(RTRIM(Culture))=LTRIM(RTRIM(@Culture)) and " +
                    "LTRIM(RTRIM(GroupName))=LTRIM(RTRIM(@GroupName)))) " +
                    "BEGIN " +
                    " Insert into dbo.LocaleResource(Culture,Name,value,GroupName) values(@Culture,@Name,@Value,@GroupName);" +
                    "" +
                    "END ",
                    new { model.Name, model.Culture, model.GroupName, model.Value });
               
            }

          var rb=  ContextResolver.Context.RequestServices.GetService<ResourceBuilder>();
            if (rb != null)
                await rb.Build();
        }

        public async Task<bool> CheckAlreadyExist(int countryId, string culture)
        {
            var result = await RegionCrudService.GetAsync("Where CountryId=@countryId and Culture=@culture", new { countryId, culture });
            return result != null;
        }

        public async Task<LocaleRegionEditViewModel> GetAllResourcesAsync(int localRegionId, string baseCulture, int page, int limit)
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
                    new { model.Culture, baseCulture, RowsPerPage = limit, PageNumber = page });
                model.Resources = resources.ToList();
            }

            return model;

        }

        public async Task<bool> SetDefaultAsync(int localeRegionId)
        {
            _cacheService.Remove(CacheKeys.LocaleRegion);
            var dbFactory = DatabaseFactories.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                await db.ExecuteAsync(
                    "update dbo.LocaleResource set IsDefault=@PrevDefault;" +
                    "Update dbo.LocaleResource set IsDefault=@IsDefault Where LocaleRegionId=@localeRegionId ",
                    new { PrevDefault = false, IsDefault = true, localeRegionId });
                return true;
            }
        }

        public async  Task<bool> AddNewResourceFromBaseCultureAsync(string culture)
        {
            var dbFactory = DatabaseFactories.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                await db.ExecuteAsync(
                    "insert into dbo.LocaleResource Select @Culture,Name,'',GroupName from dbo.LocaleResource where culture=@BaseCulture;",
                    new { Culture = culture,BaseCulture="en-us" });
                return true;
            }
           
        }

        public async Task<LocaleRegion> GetDefaultLocaleRegion()
        {
            return await _cacheService.GetAsync<LocaleRegion>(CacheKeys.LocaleRegion, async () => await this.RegionCrudService.GetAsync("Where IsDefault=@IsDefault and IsActive=@IsActive and IsDeleted=@IsDeleted", new
            {
                IsDefault = true,
                IsActive=true,
                IsDeleted=false
            }));
        }

        public async  Task<IEnumerable<LocaleResourcesExportModel>> GetAllResourcesForExportAsync(int localRegionId, string baseCulture)
        {
            var result = await RegionCrudService.GetAsync(localRegionId);
            var model = result.To<LocaleRegionEditViewModel>();

            var dbFactory = DatabaseFactories.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                var resources = await db.QueryAsync<LocaleResourcesExportModel>(
                    @"SELECT  c.Name as CountryName,
                lb.Name, lb.Value, lb.Culture, lb.GroupName from dbo.LocaleResource lb
                inner
                    join dbo.Localeregion as lg on lg.Culture = lb.Culture
                inner
                    join dbo.Country as c on c.CountryId = lg.CountryId
                where lb.Culture = @Culture
                Order By lb.Name",
                    new { model.Culture, baseCulture});
                return resources;
            }
        }

        public async Task<ImportedStatus> ImportLocaleResources(List<LocaleResourcesImportModel> importedDatas,string addedBy)
        {
            if (importedDatas.Any())
            {
                var dbFactory = DatabaseFactories.GetFactory();
                using (var db = (DbConnection) dbFactory.GetConnection())
                {
                    var first = importedDatas.First();
                    
                    await db.OpenAsync();
                    using (var dbTran = db.BeginTransaction())
                    {
                        try
                        {
                       
                            var localRegionId = await db.ExecuteScalarAsync<int>(
                                @"if(exists(select 1 from  dbo.LocaleRegion where Culture=@Culture))
                                        begin
                                        select LocaleRegionId from dbo.LocaleRegion  where Culture=@Culture
                                        end
                                        else
                                        begin
                                        insert into dbo.LocaleRegion select (select top 1 CountryId from dbo.Country where lower(Name)=lower(@CountryName) ) as CountryId,( (select  top 1 lower(ISO)  from dbo.Country where lower(Name)=lower(@CountryName) )+'.png') as flag,@Culture,0,1,0,getutcdate(),@AddedBy
                                        select SCOPE_IDENTITY()
                                        end ",
                                new { first.Culture, first.CountryName, AddedBy = addedBy }, dbTran);

                            foreach (var resource in importedDatas)
                            {

                                await db.ExecuteAsync(
                                    @"if(exists(select 1 from  dbo.LocaleResource where Culture=@Culture and lower(Name)=lower(@Name)))
                                    begin
                                    update dbo.LocaleResource  set value=@Value  where Culture=@Culture and lower(Name)=lower(@Name)
                                    end
                                    else
                                    begin
                                    insert into dbo.LocaleResource select @Culture,@Name,@Value,@GroupName
                                    end
                                ",
                                    new { resource.Culture, resource.Name, resource.Value, resource.GroupName }, dbTran);
                            }
                            dbTran.Commit();
                            return new ImportedStatus
                            {
                               
                                HasError = false,
                                IsImported = true
                            };
                        }
                        catch (Exception e)
                        {
                            dbTran.Rollback();
                            return new ImportedStatus
                            {
                                Error = e.Message,
                                HasError = true,
                                IsImported = false
                            };
                        }     
                    }


                   
                  
                }
            }
            else
            {
                return new ImportedStatus
                {
                    Error = "No any data found in excel sheets.",
                    HasError = true,
                    IsImported = false
                };
            }
        }
    }
}