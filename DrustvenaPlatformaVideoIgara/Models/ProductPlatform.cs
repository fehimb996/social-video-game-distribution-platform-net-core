using System;
using System.Collections.Generic;

namespace DrustvenaPlatformaVideoIgara.Models;

public partial class ProductPlatform
{
    public int ProductPlatformId { get; set; }

    public int PlatformId { get; set; }

    public int ProductId { get; set; }

    public virtual Platform Platform { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
