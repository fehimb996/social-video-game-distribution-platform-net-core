using DrustvenaPlatformaVideoIgara.Models;

namespace DrustvenaPlatformaVideoIgara.ViewModels
{
    public class ProductsViewModel
    {
        public IEnumerable<Product> RandomProducts { get; set; }
        public IEnumerable<TopSellingProducts> TopSellingProducts { get; set; }
        public IEnumerable<Product> ProductsUnder10Bucks { get; set; }
        public IEnumerable<Product> ProductsUnder5Bucks { get; set; }
        public IEnumerable<Product> FreeProducts { get; set; }
        public IEnumerable<int> OwnedProductIds { get; set; }
        public IEnumerable<int> WishlistProductIds { get; set; }
        public IEnumerable<ProductOwnershipStatus> UserProducts { get; set; }
    }
}
