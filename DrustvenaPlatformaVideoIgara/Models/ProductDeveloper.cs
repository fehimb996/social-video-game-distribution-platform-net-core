using System;
using System.Collections.Generic;

namespace DrustvenaPlatformaVideoIgara.Models;

public partial class ProductDeveloper
{
    public int ProductDeveloperId { get; set; }

    public int DeveloperId { get; set; }

    public int ProductId { get; set; }

    public virtual Developer Developer { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
