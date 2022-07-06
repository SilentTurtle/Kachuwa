using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kachuwa.Web.Templating
{
    public interface ITemplateDataSource
    {
        TemplateTypes Type { get; }
        string Key { get; }
        Task RenderSource();
        Task RenderSource(Dictionary<string,object> parameters);
    }
}