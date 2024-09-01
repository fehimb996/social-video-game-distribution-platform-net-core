namespace DrustvenaPlatformaVideoIgara.Models
{
    public class ProductDetailsViewModel
    {
        public Product Product { get; set; }
        public bool IsInCart { get; set; }
        public bool IsUserLoggedIn { get; set; }
        public bool IsOwned { get; set; }
        public bool IsInWishlist { get; set; }
        public List<Review> Reviews { get; set; }
        public Review UserReview { get; set; }
        public string NewComment { get; set; }
        public bool? NewRating { get; set; }
        public int? UserId { get; set; }
        public List<Genre> Genres { get; set; }
        public List<Platform> Platforms { get; set; }
        public List<Publisher> Publishers { get; set; }
        public List<Developer> Developers { get; set; }
        public List<ProductImage> ProductImages { get; set; }
    }
}
