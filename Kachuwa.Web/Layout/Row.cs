using System.Collections.Generic;

namespace Kachuwa.Web.Layout
{
    public class Row
    {
        public int RowId { get; set; }
        public string ClassName { get; set; } = "";
        public bool IsFluid { get; set; }
        public string RowName { get; set; } = "";
        public int Order { get; set; }
        public List<Column> Columns { get; set; }=new List<Column>();
    }
}