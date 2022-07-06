﻿using System.Collections.Generic;

namespace Kachuwa.Web.Layout
{
    public class Column
    {
        public int ColumnId { get; set; }
        public string Name { get; set; } = "";
        public int Order { get; set; }
        public string ClassName { get; set; } = "";
        public string Content { get; set; } = "";
        public int Width { get; set; }
        public List<ColumnModule> Components { get; set; }=new List<ColumnModule>();
    }
}