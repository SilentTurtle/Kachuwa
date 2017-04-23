using System.Collections.Generic;

namespace Kachuwa.Web.Layout
{
    public interface ILayoutContent
    {
        string Name { get; set; }
        List<LayoutContentResource> Resources { get; set; }
        List<Row> Rows { get; set; }
    }
}