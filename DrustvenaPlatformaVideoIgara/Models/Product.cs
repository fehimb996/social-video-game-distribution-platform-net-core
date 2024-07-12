using System;
using System.Collections.Generic;

namespace DrustvenaPlatformaVideoIgara.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public string ProductDetails { get; set; } = null!;

    public DateOnly ReleaseDate { get; set; }

    public decimal Price { get; set; }

    public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();

    public virtual ICollection<Picture> Pictures { get; set; } = new List<Picture>();

    public virtual ICollection<ProductDeveloper> ProductDevelopers { get; set; } = new List<ProductDeveloper>();

    public virtual ICollection<ProductGenre> ProductGenres { get; set; } = new List<ProductGenre>();

    public virtual ICollection<ProductPlatform> ProductPlatforms { get; set; } = new List<ProductPlatform>();

    public virtual ICollection<ProductPublisher> ProductPublishers { get; set; } = new List<ProductPublisher>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
}
