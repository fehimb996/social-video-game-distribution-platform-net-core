using System;
using System.Collections.Generic;

namespace DrustvenaPlatformaVideoIgara.Models;

public partial class Platform
{
    public int PlatformId { get; set; }

    public string PlatformName { get; set; } = null!;

    public virtual ICollection<ProductPlatform> ProductPlatforms { get; set; } = new List<ProductPlatform>();
}
