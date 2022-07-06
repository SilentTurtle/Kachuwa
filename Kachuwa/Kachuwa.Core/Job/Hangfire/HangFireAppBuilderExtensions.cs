using System.Collections.Generic;
using Hangfire;
using Hangfire.Dashboard;
using Kachuwa.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Hangfire.PostgreSql;
using Hangfire.SqlServer;

namespace Kachuwa.Job
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public HangfireAuthorizationFilter()
        {
        }


       

        public bool Authorize(DashboardContext context)
        {
            var httpContext = ContextResolver.Context;
            if (httpContext.User.IsInRole("Admin") || httpContext.User.IsInRole("SuperAdmin") ||
                httpContext.User.IsInRole("ITAdmin"))
            {
                return true;
            }

            return false;
        }
    }
    public static class HangFireBuilderExtensions
    {

        public static IApplicationBuilder UseSchedulingServer(this IApplicationBuilder app)
        {
            app.UseHangfireServer(new BackgroundJobServerOptions
            {
                HeartbeatInterval = new System.TimeSpan(0, 1, 0),
                ServerCheckInterval = new System.TimeSpan(0, 1, 0),
                SchedulePollingInterval = new System.TimeSpan(0, 1, 0),
                //WorkerCount = 6,
                 Queues = new[] { "critical", "default" }
            });
            app.UseHangfireDashboard("/admin/hangfire", new DashboardOptions()
            {
                //Authorization=new List<IDashboardAuthorizationFilter>() { new HangfireAuthorizationFilter()}

            }, JobStorage.Current);
            return app;
        }


        public static IServiceCollection RegisterSchedulingServer(this IServiceCollection services,
            IConfiguration configuration)
        {

            var jobConnectionString = configuration["KachuwaAppConfig:JobConnection"];
            string jCon = configuration[$"ConnectionStrings:{jobConnectionString}"];
            services.AddHangfire(x => x.UseSqlServerStorage(jCon));
            services.TryAddSingleton<IKachuwaJobEngineStarter, HangFireEngineStarter>();
            services.TryAddSingleton<IKachuwaJobRunner, HangFireJobRunner>();
            JobStorage.Current = new SqlServerStorage(jCon);
            return services;
        }
    }
}