using System;
using System.Reflection;
using System.Threading.Tasks;
using Kachuwa.Core.DI;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace Kachuwa.Web
{
    public class WebServiceRegistrar : IServiceRegistrar
    {
        private bool _isInstalled = false;

        public void Register(IServiceCollection services, IConfiguration configuration)
        {
            var str_isInstalled = configuration["KachuwaAppConfig:IsInstalled"].ToLower();
            _isInstalled = str_isInstalled != "false";

            services.RegisterKachuwaWebServices(_isInstalled, configuration);
            var embeddedAssembly = new EmbeddedFileProvider(typeof(WebServiceRegistrar).GetTypeInfo().Assembly);
            services.Configure<MvcRazorRuntimeCompilationOptions>(opts => { opts.FileProviders.Add(embeddedAssembly); });
            


        }

        public void Update(IServiceCollection serviceCollection)
        {
            if (_isInstalled)
            {

                // var builder = serviceCollection.BuildServiceProvider();
                //  var settingService = builder.GetService<ISettingService>();
                // serviceCollection.AddSingleton(settingService.CrudService.Get(1));
            }

        }
    }

    public class MemoryCacheTicketStore : ITicketStore
    {
        private const string KeyPrefix = "AuthSessionStore-";
        private IMemoryCache _cache;

        public MemoryCacheTicketStore()
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
        }

        public async Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            var guid = Guid.NewGuid();
            var key = KeyPrefix + guid.ToString();
            await RenewAsync(key, ticket);
            return key;
        }

        public Task RenewAsync(string key, AuthenticationTicket ticket)
        {
            var options = new MemoryCacheEntryOptions();
            var expiresUtc = ticket.Properties.ExpiresUtc;
            if (expiresUtc.HasValue)
            {
                options.SetAbsoluteExpiration(expiresUtc.Value);
            }

            options.SetSlidingExpiration(TimeSpan.FromHours(1)); // TODO: configurable.

            _cache.Set(key, ticket, options);

            return Task.FromResult(0);
        }

        public Task<AuthenticationTicket> RetrieveAsync(string key)
        {
            AuthenticationTicket ticket;
            _cache.TryGetValue(key, out ticket);
            return Task.FromResult(ticket);
        }

        public Task RemoveAsync(string key)
        {
            _cache.Remove(key);
            return Task.FromResult(0);
        }
    }
}