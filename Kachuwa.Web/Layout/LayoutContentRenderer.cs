using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Kachuwa.Web.Layout
{
    public class LayoutContentRenderer : ILayoutRenderer
    {
        private LayoutContent _layout;
        private ILayoutGridSystem _layoutGridSys;

        private string _container = "<div class=\"{0}\" >{1}</div>";
        private string _row = "<div rid=\"{0}\" rname={1} class={2} >{3}</div>";
        private string _column = "<div cid={0} cname={1} class={2} >{3}</div>";
        private string _component = "<component name={0} param='new{ }' ></component>";



        public string Render(LayoutContent layoutContent, LayoutGridSystem gridSystem)
        {
            try
            {


                _layout = layoutContent;

                switch (gridSystem)
                {
                    case LayoutGridSystem.BootStrap:
                        _layoutGridSys = new BootStrapLayoutGridSystem();
                        break;


                }

                StringBuilder layout = new StringBuilder();

                foreach (var resource in _layout.Resources)
                {
                    switch (resource.Type)
                    {
                        case "css":
                            break;
                        case "script":
                            break;
                    }
                }

                var rows = _layout.Rows.OrderBy(e => e.Order);
                foreach (var row in rows)
                {
                    var columns = row.Columns.OrderBy(x => x.Order);
                    // var rowColums = "";
                    StringBuilder rowColums = new StringBuilder();

                    foreach (var column in columns)
                    {
                        StringBuilder modules = new StringBuilder();
                        if (column.Components != null)
                        {
                            foreach (var component in column.Components)
                            {
                                if (string.IsNullOrEmpty(component.Params))
                                {
                                    modules.Append(
                                "<component name=\"" + component.Name + "\" }' ></component>");
                                    modules.Append("<div class=\"clearfix\"></div>");
                                }
                                else
                                {
                                    modules.Append(
                                "<component name=\"" + component.Name + "\" params='new { " + component.Params + "}' ></component>");
                                    modules.Append("<div class=\"clearfix\"></div>");
                                }

                            }
                        }

                        rowColums.AppendFormat("<div cid=\"{0}\" cname=\"{1}\" class=\"{2}\" >{3}</div>", column.ColumnId, column.Name, _layoutGridSys.GetColumnClass(column.Width), modules);
                    }
                    var finalRow = string.Format("<div rid=\"{0}\" rname=\"{1}\" class=\"{2}\" >{3}</div>", row.RowId, row.RowName, _layoutGridSys.Row, rowColums);
                    string outerWrapper = row.IsFluid == true ? _layoutGridSys.Row : _layoutGridSys.Container;
                    outerWrapper += " " + row.ClassName;
                    var container = string.Format("<div class=\"{0}\" >{1}</div>", outerWrapper, finalRow);
                    layout.Append(container);


                }
                return layout.ToString();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}