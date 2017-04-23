using System.Linq;
using System.Text;

namespace Kachuwa.Web.Layout
{
    public class LayoutContentRenderer
    {
        private ILayoutContent _layout;
        public LayoutContentRenderer(ILayoutContent layout)
        {
            _layout = layout;
        }

        private string _row = "<div rid={0} rname={1} class={2} >{3}</div>";
        private string _column = "<div cid={0} cname={1} class={2} >{3}</div>";
        public string Render()
        {
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
                    rowColums.AppendFormat(_column, column.ColumnId, column.Name, column.ClassName, column.Content);
                }
                layout.AppendFormat(_row, row.RowId, row.RowName, row.ClassName, rowColums.ToString());


            }
            return layout.ToString();

        }
    }
}