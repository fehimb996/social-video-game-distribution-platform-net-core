using System;
using System.Collections.Generic;

namespace DrustvenaPlatformaVideoIgara.Models;

public partial class Invoice
{
    public int InvoiceId { get; set; }

    public int UserId { get; set; }

    public DateTime DateIssued { get; set; }

    public int PaymentMethodId { get; set; }

    public decimal TotalPrice { get; set; }

    public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();

    public virtual PaymentMethod PaymentMethod { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
