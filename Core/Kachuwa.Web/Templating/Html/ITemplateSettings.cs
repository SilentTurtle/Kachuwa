using System.Collections.Generic;

namespace Kachuwa.Web.Templating

{
    public interface ITemplateSettings
    {
        IEnumerable<TemplateSetting> Settings { get; set; }
    }
}