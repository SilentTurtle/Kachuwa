using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4.Services;
using Kachuwa.Caching;
using Kachuwa.Data;
using Kachuwa.Web.Model;

namespace Kachuwa.Web.Service
{
    public class SettingService : ISettingService
    {
        private readonly ICacheService _cacheService;

        public SettingService(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }
        public CrudService<Setting> CrudService { get; set; } = new CrudService<Setting>();
        private const string Key = "Kachuwa.Setting";
        public async Task<Setting> GetSetting()
        {

            return await _cacheService.GetAsync<Setting>(Key, async () => await this.CrudService.GetAsync(1));

        }

        public async Task<Setting> SaveSetting(Setting setting)
        {
            _cacheService.Remove(Key);
            setting.SettingId = 1;
            await this.CrudService.UpdateAsync(setting);
            return await GetSetting();
        }
    }
}
