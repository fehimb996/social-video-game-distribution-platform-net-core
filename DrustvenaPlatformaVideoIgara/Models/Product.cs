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

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();

    public virtual ICollection<Developer> Developers { get; set; } = new List<Developer>();

    public virtual ICollection<Genre> Genres { get; set; } = new List<Genre>();

    public virtual ICollection<Platform> Platforms { get; set; } = new List<Platform>();

    public virtual ICollection<Publisher> Publishers { get; set; } = new List<Publisher>();
}
