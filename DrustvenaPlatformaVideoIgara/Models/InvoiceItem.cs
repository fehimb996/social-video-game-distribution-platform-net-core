using System;
using System.Collections.Generic;

namespace DrustvenaPlatformaVideoIgara.Models;

public partial class InvoiceItem
{
    public int InvoiceItemId { get; set; }

    public int InvoiceId { get; set; }

    public int ProductId { get; set; }

    public decimal Price { get; set; }

    public virtual Invoice Invoice { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
