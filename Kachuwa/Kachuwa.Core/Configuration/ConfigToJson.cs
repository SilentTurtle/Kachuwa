using System;
using System.IO;
using System.Threading;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kachuwa.Configuration
{
    public class ConfigToJson : IConfigToJson
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private const string AppSettingFile = "appsettings.json";
        private const string KachuwaConfigFile = "config\\kachuwaconfig.json";
        private static object _ratesFileLock = new object();

        public ConfigToJson(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public bool SaveConnectionString(KachuwaConnectionStrings connectionString)
        {
            Attempt<KachuwaConnectionStrings>(TryToUpdateRates<KachuwaConnectionStrings>, connectionString, maximumNumberOfAttempts: 50, timeToWaitBetweenRetriesInMs: 100);

            return true;
        }

        public bool SaveKachuwaConfig(KachuwaAppConfig config)
        {
            Attempt<KachuwaAppConfig>(TryToUpdateRates<KachuwaAppConfig>, config, maximumNumberOfAttempts: 50, timeToWaitBetweenRetriesInMs: 100);
            return true;
        }

        private void TryToUpdateRates<T>(object config)
        {
            lock (_ratesFileLock)
            {
                var instance = typeof(T);
                if (instance == typeof(KachuwaAppConfig))
                {
                    string oriFileJson =
                        File.ReadAllText(Path.Combine(_hostingEnvironment.ContentRootPath, KachuwaConfigFile));
                    JObject jsonObj = JObject.Parse(oriFileJson);
                    jsonObj["KachuwaAppConfig"] = JObject.FromObject(config); 
                    var configJson1 = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
                    File.WriteAllText(Path.Combine(_hostingEnvironment.ContentRootPath, KachuwaConfigFile),
                        configJson1);
                    //using (var stream1 = GetRatesFileStream<T>())
                    //{
                    //    SaveFile(configJson1, stream1);
                    //}
                }
                else
                {
                    string oriFileJson =
                         File.ReadAllText(Path.Combine(_hostingEnvironment.ContentRootPath, AppSettingFile));
                    JObject jsonObj = JObject.Parse(oriFileJson);
                    jsonObj["ConnectionStrings"]  = JObject.FromObject(config); 
                    var configJson = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
                    File.WriteAllText(Path.Combine(_hostingEnvironment.ContentRootPath, AppSettingFile),configJson);
                    //using (var stream = GetRatesFileStream<T>())
                    //{
                    //    SaveFile(configJson, stream);
                    //}
                }
            }
        }

        private Stream GetRatesFileStream<T>()
        {
            var instance = typeof(T);
            if (instance == typeof(KachuwaAppConfig))
            {
                return File.Open(Path.Combine(_hostingEnvironment.ContentRootPath, KachuwaConfigFile), FileMode.OpenOrCreate, FileAccess.Write);
            }
            else
            {
                return File.Open(Path.Combine(_hostingEnvironment.ContentRootPath, AppSettingFile), FileMode.OpenOrCreate, FileAccess.Write);
            }

        }

        private void SaveFile(string json, Stream stream)
        {
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.WriteAsync(json);
            }
        }

        private static void Attempt<T>(Action<T> work, object config, int maximumNumberOfAttempts, int timeToWaitBetweenRetriesInMs) where T : new()
        {
            var numberOfFailedAttempts = 0;
            while (true)
            {
                try
                {
                    var x = (T)config;
                    work(x);
                    return;
                }
                catch
                {
                    numberOfFailedAttempts++;
                    if (numberOfFailedAttempts >= maximumNumberOfAttempts)
                        throw;
                    Thread.Sleep(timeToWaitBetweenRetriesInMs);
                }
            }
        }

    }
}