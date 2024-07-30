using System;
using System.Collections.Generic;

namespace DrustvenaPlatformaVideoIgara.Models;

public partial class Cart
{
    public int CartId { get; set; }

    public int UserId { get; set; }

    public decimal TotalPrice { get; set; }

    public int? InvoiceId { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual Invoice? Invoice { get; set; }

    public virtual User User { get; set; } = null!;
}
