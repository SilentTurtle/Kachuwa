using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kachuwa.Web.Templating
{
    public interface ITemplateDataSourceManager
    {
        void GetAllTemplateDataSource();

        IEnumerable<ITemplateDataSource> GetTemplateDataSource(TemplateTypes type);
        ITemplateDataSource FindByKey(TemplateTypes type, string key);
        Task<object> FetchTemplateData<T>(string key);
        Dictionary<string, object> GetDataMembers(TemplateTypes type, string key);

    }
}