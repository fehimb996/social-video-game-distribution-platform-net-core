using System;
using System.Collections.Generic;

namespace DrustvenaPlatformaVideoIgara.Models;

public partial class Platform
{
    public int PlatformId { get; set; }

    public string PlatformName { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
