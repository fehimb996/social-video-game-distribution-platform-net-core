namespace DrustvenaPlatformaVideoIgara.Models
{
    public class WishlistViewModel
    {
        public Wishlist Wishlist { get; set; }
        public IEnumerable<CartItem> CartItems { get; set; }
        public int UserId { get; set; }
    }
}
