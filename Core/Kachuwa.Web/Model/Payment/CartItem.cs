namespace Kachuwa.Web.Payment
{
    public class CartItem
    {
        public string Product { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Weight { get; set; }
        public decimal TaxAmount { get; set; }
    }
}