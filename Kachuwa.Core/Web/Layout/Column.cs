using System.Collections.Generic;

namespace Kachuwa.Web.Layout
{
    public class Column
    {
        public int ColumnId { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public string ClassName { get; set; }
        public string Content { get; set; }
        public int Width { get; set; }
        public List<ColumnModule> Components { get; set; }
    }

    public class ColumnModule
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string FullName { get; set; }
        public string Params { get; set; }
    }
}