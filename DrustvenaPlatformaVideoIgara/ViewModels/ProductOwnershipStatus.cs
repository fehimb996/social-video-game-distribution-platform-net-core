using DrustvenaPlatformaVideoIgara.Models;

namespace DrustvenaPlatformaVideoIgara.ViewModels
{
    public class ProductOwnershipStatus
    {
        public Product Product { get; set; }
        public bool IsInLibrary { get; set; }
        public bool IsOnWishlist { get; set; }
    }
}
