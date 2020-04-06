using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Kachuwa.Core.DI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyModel;

namespace Kachuwa.Web
{
    public class WidgetService : IWidgetService
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public List<IWidget> Widgets { get; set; }

        public WidgetService(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            this.Load();
        }
        public Task<bool> Load()
        {
            Widgets = new List<IWidget>();
            var platform = Environment.OSVersion.Platform.ToString();
            var runtimeAssemblyNames = DependencyContext.Default.GetRuntimeAssemblyNames(platform);
            var filteredAssemblies = runtimeAssemblyNames.Where(x => !(x.Name.Contains("Microsoft") || x.Name.Contains("System"))).ToList();
            var instances = filteredAssemblies
                .Select(Assembly.Load)
                .SelectMany(a => a.ExportedTypes)
                .Where(t => TypeExtensions.GetInterfaces(t).Contains(typeof(IWidget)) && t.GetConstructor(Type.EmptyTypes) != null)
                .Select(y => (IWidget)Activator.CreateInstance(y));
            Widgets.AddRange(instances);
            return Task.FromResult(true);
        }

        public async Task<IWidget> Find(string name)
        {
            if (Widgets.Any())
            {
               return Widgets.SingleOrDefault(x => x.SystemName.ToLower() == name.ToLower());
            }

            return null;
        }

        public Task<IEnumerable<IWidget>> GetByConfigSource(string configSourcePath)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<IWidget>> GetAllWidgets()
        {
            if (this.Widgets.Any())
                return this.Widgets;
            else
            {
                await Load();
                return this.Widgets;
            }
        }
    }
}