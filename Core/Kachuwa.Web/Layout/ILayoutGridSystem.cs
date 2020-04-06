namespace Kachuwa.Web.Layout
{
    public interface ILayoutGridSystem
    {
        string FluidContainer { get; set; }
        string Container { get; set; }
        string Row { get; set; }
        int NoOfColumnsPerRow { get; set; }
        string ColumnPrefix { get; set; }
        string ColumnSuffix { get; set; }
        string ColumnConcatSymbol { get; set; }
        string ColumnExtraSmall { get; set; }
        string ColumnSmall { get; set; }
        string ColumnMedium { get; set; }
        string ColumngLarge { get; set; }
        string ColumnExtraLarge { get; set; }

        string GetColumnClass(int width);
    }
}