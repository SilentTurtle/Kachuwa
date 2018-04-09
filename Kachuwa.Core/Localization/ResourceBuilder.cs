using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;

namespace Kachuwa.Localization
{
    public class ResourceBuilder
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly LocaleSetting _setting;
        private readonly ILocaleService _localeService;
        public List<LocaleResource> AllLocaleResources;

        private const string LocaleResourceFileName = "locale_resources-{0}.json";
        public ResourceBuilder( LocaleSetting setting, IHostingEnvironment hostingEnvironment, ILocaleService localeService)
        {
            _hostingEnvironment = hostingEnvironment;
            _setting = setting;
            _localeService = localeService;
            AllLocaleResources = new List<LocaleResource>();
           
        }



        public async Task Build()
        {
            if (_setting.UseDbResources)
            {
                var localeResources = await _localeService.CrudService.GetListAsync();
                AllLocaleResources.AddRange(localeResources);
               
            }
            if (_setting.UseJsonResources)
            {
                
            }
            if (_setting.UseXmlResources)
            {
                
            }
            var folderPath = Path.Combine(_hostingEnvironment.ContentRootPath, "Locale");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var _allLocaleResourcesByGroup= AllLocaleResources.Select(x=>new
               {
                   x.Name,
                   x.Value,
                   x.Culture,
                   x.GroupName
               }).GroupBy(g=>g.Culture);

            foreach (var group in _allLocaleResourcesByGroup)
            {
                var filePath = Path.Combine(folderPath, String.Format(LocaleResourceFileName,group.Key));

                var list = group.ToList().GroupBy(e => e.GroupName)
                    .Select(x => new
                    {
                        Group = x.Key,
                        Locales = x.ToList().Select(z => new
                        {
                            z.Name,
                            z.Value,
                            z.Culture
                        })
                    });

                var jsonResources = JsonConvert.SerializeObject(list);
                using (var writer = File.CreateText(filePath))
                {
                    await writer.WriteLineAsync(jsonResources);

                    await writer.FlushAsync();
                }
            }

        

          

           
        }





    }
}