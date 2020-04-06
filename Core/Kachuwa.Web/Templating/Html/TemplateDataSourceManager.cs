using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyModel;

namespace Kachuwa.Web.Templating
{
    public class TemplateDataSourceManager : ITemplateDataSourceManager
    {
        public IEnumerable<ITemplateDataSource> TemplateDataSources { get; set; }=new List<ITemplateDataSource>();

        public TemplateDataSourceManager()
        {
            GetAllTemplateDataSource();
        }

        public void GetAllTemplateDataSource()
        {
            var platform = Environment.OSVersion.Platform.ToString();
            var runtimeAssemblyNames = DependencyContext.Default.GetRuntimeAssemblyNames(platform);

            var instances = runtimeAssemblyNames
                .Select(Assembly.Load)
                .SelectMany(a => a.ExportedTypes)
                .Where(t => TypeExtensions.GetInterfaces(t).Contains(typeof(ITemplateDataSource)) &&
                            t.GetConstructor(Type.EmptyTypes) != null)
                .Select(t=>(ITemplateDataSource)Activator.CreateInstance(t));
            var result = instances.ToArray();
            TemplateDataSources = result.ToList();
        }

        public IEnumerable<ITemplateDataSource> GetTemplateDataSource(TemplateTypes type)
        {
            if (TemplateDataSources.Any())
                return TemplateDataSources.Where(x => x.Type == type);
            else
            {
                GetAllTemplateDataSource();
                if (TemplateDataSources.Any())
                    return TemplateDataSources.Where(x => x.Type == type);
                else return null;
            }
            //var platform = Environment.OSVersion.Platform.ToString();
            //var runtimeAssemblyNames = DependencyContext.Default.GetRuntimeAssemblyNames(platform);

            //var instances = runtimeAssemblyNames
            //    .Select(Assembly.Load)
            //    .SelectMany(a => a.ExportedTypes)
            //    .Where(t => TypeExtensions.GetInterfaces(t).Contains(typeof(T)) &&
            //                t.GetConstructor(Type.EmptyTypes) != null)
            //    .Select(y => (T)Activator.CreateInstance(y));
            //var result = instances as T[] ?? instances.ToArray();
            //return result;

        }

        public async Task<object> FetchTemplateData<T>(string key)
        {
            throw new NotImplementedException();
        }

        public ITemplateDataSource FindByKey(TemplateTypes type, string key)
        {
            return TemplateDataSources.SingleOrDefault(x => x.Type == type && x.Key.ToLower() == key.ToLower());
        }
        public Dictionary<string, object> GetDataMembers(TemplateTypes type, string key)
        {
            var templateSource= TemplateDataSources.SingleOrDefault(x => x.Type == type && x.Key.ToLower() == key.ToLower());
            var templateType = templateSource.GetType();
            Dictionary<string, object> properties = new Dictionary<string, object>();
            foreach (PropertyInfo prop in templateType.GetProperties())
                properties.Add(prop.Name, prop.PropertyType);
            return properties;
        }
    }
}