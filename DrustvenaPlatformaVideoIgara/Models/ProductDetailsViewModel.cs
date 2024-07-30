namespace DrustvenaPlatformaVideoIgara.Models
{
    public class ProductDetailsViewModel
    {
        public Product Product { get; set; }
        public bool IsInCart { get; set; }
        public bool IsUserLoggedIn { get; set; }
        public bool IsOwned { get; set; }
        public bool IsInWishlist { get; set; }
    }
}
