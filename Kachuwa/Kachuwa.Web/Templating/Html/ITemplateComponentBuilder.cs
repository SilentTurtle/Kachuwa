using System.Collections.Generic;

namespace Kachuwa.Web.Templating
{
    public interface ITemplateComponentBuilder<T>
    {
        IEnumerable<T> Templates { get; set; }
        IEnumerable<T> GetTemplateComponents();

    }
}