using System.Collections.Generic;

namespace Kachuwa.Web.Layout
{
    public class BootStrapLayoutGridSystem : ILayoutGridSystem
    {
        public string Row { get; set; } = "row";
        public string Container { get; set; } = "container";
        public string ColumnPrefix { get; set; } = "col";
        public string FluidContainer { get; set; } = "container-fluid";
        public int NoOfColumnsPerRow { get; set; } = 12;
        public string ColumnSuffix { get; set; } = "";
        public string ColumnConcatSymbol { get; set; } = "-";
        public string ColumnExtraSmall { get; set; } = "xs";
        public string ColumnSmall { get; set; } = "sm";
        public string ColumnMedium { get; set; } = "md";
        public string ColumngLarge { get; set; } = "ld";
        public string ColumnExtraLarge { get; set; } = "xl";
        public string GetColumnClass(int width)
        {
            return string.Join(" ", GenerateClasses(width));

        }

        private List<string> GenerateClasses(int width)
        {
            var deviceSizez = new string[] { "sm", "md", "ld" }; //new string[] {"xs", "sm", "md", "ld", "xl"};
            var deviceClaseses = new List<string>();
            foreach (var size in deviceSizez)
            {
                var cssclass=this.ColumnPrefix //col
                   + this.ColumnConcatSymbol //-
                   + size//md
                   + this.ColumnConcatSymbol//-
                   + width // TODO:: calculate with for device wise
                   + this.ColumnSuffix;
                deviceClaseses.Add(cssclass);
            }
            return deviceClaseses;
        }
    }
}