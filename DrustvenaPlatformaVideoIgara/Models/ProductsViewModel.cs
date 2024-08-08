namespace DrustvenaPlatformaVideoIgara.Models
{
    public class ProductsViewModel
    {
        public IEnumerable<Product> RandomProducts { get; set; }
        public IEnumerable<TopSellingProduct> TopSellingProducts { get; set; }
        public IEnumerable<Product> ProductsUnder10Bucks { get; set; }
        public IEnumerable<Product> ProductsUnder5Bucks { get; set; }
        public IEnumerable<Product> FreeProducts { get; set; }
    }
}
