using System;
using System.Threading.Tasks;
using Kachuwa.Caching;
using Kachuwa.Configuration;
using Kachuwa.Localization;
using Kachuwa.Log;
using Kachuwa.Web.Service;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Kachuwa.Web.TagHelpers
{
    public class SystemVariablesTagHelperComponent : TagHelperComponent
    {
        private readonly ILogger _logger;
        private readonly ILocaleService _localeService;
        private readonly ISettingService _settingService;
        private readonly KachuwaAppConfig _kachuwaAppConfig;

        //order to inject first or last
        public override int Order => 2;
        public SystemVariablesTagHelperComponent(IOptionsSnapshot<KachuwaAppConfig> configSnapShot
            , ILogger logger, ILocaleService localeService,ISettingService settingService)
        {
          
            _logger = logger;
            _localeService = localeService;
            _settingService = settingService;
            _kachuwaAppConfig = configSnapShot.Value;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (_kachuwaAppConfig.IsInstalled)
            {
                if (string.Equals(context.TagName, "head", StringComparison.Ordinal))
                {
                    var setting = await _settingService.GetSetting();
                    var utcDateNow = DateTime.UtcNow;
                    var todaysDate = TimeZoneInfo.ConvertTimeFromUtc(utcDateNow, TimeZoneInfo.FindSystemTimeZoneById(setting.TimeZoneName));
                    var localization = await _localeService.GetDefaultLocaleRegion();
                    var json = JsonConvert.SerializeObject( new
                        { Today= todaysDate.ToString("yyyy-MM-dd"),
                            setting.TimeZoneName,
                            setting.TimeZoneOffset,
                            setting.BaseCulture,
                            setting.BaseCurrency,
                            LocaleRegion = new { localization?.Culture,localization?.Flag,localization?.CountryId }

                        }
                    );
                    string variables = @"<script type='text/javascript'>
                        __kachuwaSettings=" + json + "</script><script type='text/javascript' src='/assets/js/locale/kachuwalocale.js'></script> ";

                    output.PostContent.AppendHtmlLine(variables);

                }

            }

        }
    }
}